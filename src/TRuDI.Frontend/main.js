const electron = require('electron');

// Module to control application life.
const app = electron.app;
app.commandLine.appendSwitch('no-proxy-server');

// Module to create native browser window.
const BrowserWindow = electron.BrowserWindow;

const path = require('path');
const url = require('url');
const crypto = require('crypto');
const os = require('os');
const fs = require('fs');

// Set platform-specific parameters of the backend application
const backendPathWindows32 = '../TRuDI.Backend/bin/dist/win7-x86';
const backendPathWindows64 = '../TRuDI.Backend/bin/dist/win7-x64';
const backendPathLinux = '../TRuDI.Backend/bin/dist/linux-x64';

let backendCheckFailed = false;

const backendConfig = {
    // The executable used to start the backend process.
    executablePath: "",

    // Path where the backend is located
    workPath: "",

    // The main assembly name
    mainAssembly: "",

    // Command line options forwarded to the backend.
    commandLineOptions: [],

    // List of digest values for the backend file
    checksums: null,

    // Expected subject of the TLS certificate used by the backend
    // The commonName is set to the RIPEMD-160 digest value of the main assembly.
    certCommonName: "",
};

writeLog(`Running on plattform ${os.platform()}, arch ${os.arch()}`);

if (os.platform() === 'linux') {
    backendConfig.workPath = path.join(__dirname, backendPathLinux);
    backendConfig.executablePath = path.join(__dirname, backendPathLinux, 'TRuDI.Backend');
    backendConfig.mainAssembly = path.join(__dirname, backendPathLinux, 'TRuDI.Backend.dll');
    backendConfig.checksums = JSON.parse(fs.readFileSync(path.join(__dirname, "checksums-linux.json"), 'utf-8'));
    backendConfig.certCommonName = backendConfig.checksums.find(function (f) { return f.path === "/TRuDI.Backend.dll" }).hash;
}
else if (os.platform() === 'win32' && os.arch() === 'x64') {
    backendConfig.workPath = path.join(__dirname, backendPathWindows64);
    backendConfig.executablePath = path.join(__dirname, backendPathWindows64, 'TRuDI.Backend.exe');
    backendConfig.mainAssembly = path.join(__dirname, backendPathWindows64, 'TRuDI.Backend.dll');
    backendConfig.checksums = JSON.parse(fs.readFileSync(path.join(__dirname, "checksums-win32-x64.json"), 'utf-8'));
    backendConfig.certCommonName = backendConfig.checksums.find(function (f) { return f.path === "\\TRuDI.Backend.dll" }).hash;
}
else if (os.platform() === 'win32' && os.arch() === 'ia32') {
    backendConfig.workPath = path.join(__dirname, backendPathWindows32);
    backendConfig.executablePath = path.join(__dirname, backendPathWindows32, 'TRuDI.Backend.exe');
    backendConfig.mainAssembly = path.join(__dirname, backendPathWindows32, 'TRuDI.Backend.dll');
    backendConfig.checksums = JSON.parse(fs.readFileSync(path.join(__dirname, "checksums-win32-x86.json"), 'utf-8'));
    backendConfig.certCommonName = backendConfig.checksums.find(function (f) { return f.path === "\\TRuDI.Backend.dll" }).hash;
}

// Parse command line options.
const argv = process.argv.slice(1);
for (let i = 0; i < argv.length; i++) {

    writeLog(`arg ${i}: ${argv[i]}`);
    if (argv[i].match(/^--test=/)) {
        backendConfig.commandLineOptions.push(argv[i]);
        continue;
    }

    if (argv[i].match(/^--log=/)) {
        backendConfig.commandLineOptions.push(argv[i]);
        continue;
    }

    if (argv[i].match(/^--loglevel=/)) {
        backendConfig.commandLineOptions.push(argv[i]);
        continue;
    }
}

// Generates a RIPEMD-160 digest value for the specified file
function generateRipeMd160(filename, callback) {
        var hash = crypto.createHash('RIPEMD160');
        var fileStream = fs.ReadStream(filename);

        fileStream.on('error', function(err) {
            writeLog("Error on file '" + filename + "', error message: " + err);
            var digest = hash.digest('hex');
            callback(null, digest);
        });

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
                if (err) {
                    return done(err);
                }

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
                                return done(e);
                            }

                            results.push({ "path": fname.substring(backendConfig.workPath.length), "hash": hash });

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


// Check if the backend has the expected digest values
function checkIntegrity() {
    writeLog("Checking application integrity...");

    generateChecksumsForTree(backendConfig.workPath,
        function (err, results) {
            if (err) {
                throw err;
            }

            results.sort(function (a, b) {
                return a.path.localeCompare(b.path);
            });

            if (results.length !== backendConfig.checksums.length) {
                backendCheckFailed = true;
                writeLog("Integrity check failed: length of digest arrays differ");

                var minLength = Math.min(results.length, backendConfig.checksums.length);
                for (var i = 0; i < minLength; i++) {
                    if (results[i].path !== backendConfig.checksums[i].path) {
                        writeLog("Integrity check failed: index %i: found file \"%s\", expected file \"%s\"",
                            i,
                            results[i].path,
                            backendConfig.checksums[i].path);

                        break;
                    }
                }

                mainWindow.loadURL(url.format({
                    pathname: path.join(__dirname, 'integrity_check_failed.html'),
                    protocol: 'file:',
                    slashes: true
                }));

                return;
            }

            for (var i = 0; i < results.length; i++) {
                if (results[i].path !== backendConfig.checksums[i].path) {
                    backendCheckFailed = true;
                    writeLog("Integrity check failed: index %i: found file \"%s\", expected file \"%s\"",
                        i,
                        results[i].path,
                        backendConfig.checksums[i].path);

                    mainWindow.loadURL(url.format({
                        pathname: path.join(__dirname, 'integrity_check_failed.html'),
                        protocol: 'file:',
                        slashes: true
                    }));

                    return;
                }

                if (results[i].hash !== backendConfig.checksums[i].hash) {
                    backendCheckFailed = true;
                    writeLog("Integrity check failed: different digest value: file \"%s\", calculated: %s expected %s",
                        i,
                        results[i].path,
                        results[i].hash,
                        backendConfig.checksums[i].hash);

                    mainWindow.loadURL(url.format({
                        pathname: path.join(__dirname, 'integrity_check_failed.html'),
                        protocol: 'file:',
                        slashes: true
                    }));

                    return;
                }
            }

            writeLog("Integrity check passed!");
        });
}

// Check the server certificate
app.on('certificate-error',
    (event, webContents, requestUrl, error, certificate, callback) => {
        if (certificate.subject.commonName === backendConfig.certCommonName) {
            event.preventDefault();
            callback(true);
        }
        else {
            callback(false);
            writeLog("Integrity check failed: invalid CommonName used in TLS certificate");

            mainWindow.loadURL(url.format({
                pathname: path.join(__dirname, 'integrity_check_failed.html'),
                protocol: 'file:',
                slashes: true
            }));
        }
    });


// Keep a global reference of the window object, if you don't, the window will
// be closed automatically when the JavaScript object is garbage collected.
let mainWindow;

// Create the browser window.
function createWindow() {
   mainWindow = new BrowserWindow({
      width: 1240, height: 740, webPreferences: {
         nodeIntegration: true,
         enableRemoteModule: true
      } });
    mainWindow.removeMenu();

    mainWindow.loadURL(url.format({
        pathname: path.join(__dirname, 'startup.html'),
        protocol: 'file:',
        slashes: true
    }));

    // Emitted when the window is closed.
    mainWindow.on('closed', function () {
        // Dereference the window object, usually you would store windows
        // in an array if your app supports multi windows, this is the time
        // when you should delete the corresponding element.
        mainWindow = null;
    });

    mainWindow.webContents.on("did-fail-load", function () {
        writeLog("did-fail-load: %d, %s", arguments[1], arguments[3]);
        if (!backendCheckFailed) {
            mainWindow.loadURL(url.format({
                pathname: path.join(__dirname, 'backend_connect_failed.html'),
                protocol: 'file:',
                slashes: true
            }));
        }
    });
}

// Loads the backend URL
function connectToBackend(port) {
    if (backendCheckFailed) {
        return;
    }

    writeLog("Connecting to backend on port %s", port);
    mainWindow.loadURL("https://127.0.0.1:" + port);
}

// This method will be called when Electron has finished
// initialization and is ready to create browser windows.
// Some APIs can only be used after this event occurs.
app.on('ready', startBackendService);

// Quit when all windows are closed.
app.on('window-all-closed', function () {
    // On OS X it is common for applications and their menu bar
    // to stay active until the user quits explicitly with Cmd + Q
    if (process.platform !== 'darwin') {
        app.quit();
    }
});

app.on('activate', function () {
    // On OS X it's common to re-create a window in the app when the
    // dock icon is clicked and there are no other windows open.
    if (mainWindow === null) {
        createWindow();
    }
});

var backendServiceProcess = null;

// run the backend server
function startBackendService() {
    createWindow();
    checkIntegrity();

    try {
        backendServiceProcess = require('child_process').spawn(backendConfig.executablePath,
            backendConfig.commandLineOptions,
            { cwd: backendConfig.workPath });
        backendServiceProcess.stdout.on('data',
            (data) => {
                writeLog(`stdout: ${data}`);
                var s = data.toString();

                var match = s.match("##### TRUDI\-BACKEND\-PORT: ([0-9]+) #####");
                if (match) {
                    var backendPort = match[1];
                    writeLog("Backend-Port: %s", backendPort);
                    backendStarted = true;
                    connectToBackend(backendPort);
                }
            });

        backendServiceProcess.on('exit',
            function(exitCode) {
                writeLog('Backend process exited: ' + exitCode);
                mainWindow.loadURL(url.format({
                    pathname: path.join(__dirname, 'backend_connect_failed.html'),
                    protocol: 'file:',
                    slashes: true
                }));
            });
    } catch (error) {
        mainWindow.loadURL(url.format({
            pathname: path.join(__dirname, 'backend_connect_failed.html'),
            protocol: 'file:',
            slashes: true
        }));
    }
}

// Kill process when electron exits
process.on('exit', function () {
    writeLog('exit');
    backendServiceProcess.kill();
});

function writeLog(msg, ...args) {
    console.log((new Date()).toISOString() + " - " + msg, ...args);
}
