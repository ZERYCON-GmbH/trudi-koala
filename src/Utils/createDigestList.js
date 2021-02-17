const crypto = require('crypto');
const fs = require('fs');
const path = require('path');


const checksumFile = process.argv[3];
const directory = process.argv[2];

// Generates a RIPEMD-160 digest value for the specified file
function generateRipeMd160(filename, callback) {
    var hash = crypto.createHash('RIPEMD160');
    var fileStream = fs.ReadStream(filename);

    fileStream.on('data', function (data) {
        hash.update(data);
    });
    fileStream.on('end', function () {
        var digest = hash.digest('hex');
        callback(null, digest);
    });
}

// Generates a list of all files with RIPEMD-160 digest values for the specified directory
function generateChecksumsForTree(dir, done) {
    var results = [];

    fs.readdir(dir, function (err, list) {
        if (err) {
            return done(err);
        }

        var pending = list.length;

        if (!pending) {
            return done(null, results);
        }

        list.forEach(function (file) {
            fs.stat(path.join(dir, file), function (err, stat) {

                if (stat && stat.isDirectory()) {
                    generateChecksumsForTree(path.join(dir, file), function (err, res) {
                        res.forEach(function (r) { results.push(r); });

                        if (!--pending) {
                            done(null, results);
                        }
                    });
                } else {
                    if (stat.size > 0) {
                        var fname = path.join(dir, file);
                        generateRipeMd160(fname, function (e, hash) {
                            if (e) {
                                done(e);
                            }

                            var item = { "path": fname.substring(directory.length), "hash": hash };

                            results.push(item);
                            console.log(item.path);

                            if (!--pending) {
                                done(null, results);
                            }
                        });
                    }
                    else {
                        if (!--pending) {
                            done(null, results);
                        }
                    }
                }
            });
        });
    });
};


generateChecksumsForTree(directory,
    function (err, results) {
        if (err) {
            throw err;
        }

        results.sort(function (a, b) {
            return a.path.localeCompare(b.path);
        });

        fs.writeFile(checksumFile,
            JSON.stringify(results),
            'utf8',
            function (err) {
                if (err) {
                    return console.log(err);
                }

                console.log("The file was saved!");
            });
        
    });