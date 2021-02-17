namespace TRuDI.Backend.Application
{
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    using TRuDI.Backend.Utils;
    using TRuDI.HanAdapter.Repository;
    using TRuDI.TafAdapter.Repository;

    /// <summary>
    /// Helper class used to calculate checksums of important application files.
    /// </summary>
    public class ApplicationChecksums
    {
        /// <summary>
        /// Calculates the digest values.
        /// </summary>
        public void Calculate()
        {
            var items = new List<DigestItem>();
            this.Items = items;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                items.Add(new DigestItem("../../../../../TRuDI.exe"));
                items.Add(new DigestItem("../../../../app.asar"));
            }
            else
            {
                items.Add(new DigestItem("../../../../../trudi"));
                items.Add(new DigestItem("../../../../app.asar"));
            }

            Task.Run(() =>
                {
                    foreach (var item in items)
                    {
                        this.GetDigest(item);
                    }
                });

            Task.Run(() =>
                {
                    foreach (var hanAdapter in HanAdapterRepository.AvailableAdapters)
                    {
                        hanAdapter.Hash = DigestUtils.GetDigestFromAssembly(hanAdapter.Assembly).AddSpace(4).Value;
                    }

                    foreach (var tafAdapter in TafAdapterRepository.AvailableAdapters)
                    {
                        tafAdapter.Hash = DigestUtils.GetDigestFromAssembly(tafAdapter.Assembly).AddSpace(4).Value;
                    }
                });
        }

        /// <summary>
        /// Gets the list of files with calculated digest values.
        /// </summary>
        public IReadOnlyList<DigestItem> Items { get; private set; }

        /// <summary>
        /// Gets the digest of the specified file.
        /// </summary>
        /// <param name="item">The item to calculate the digest for.</param>
        private void GetDigest(DigestItem item)
        {
            var directory = Path.GetDirectoryName(this.GetType().Assembly.Location);
            var filepath = Path.Combine(directory, item.Filepath);

            if (File.Exists(filepath))
            {
                item.Digest = DigestUtils.GetRipemd160(filepath).AddSpace(4).Value;
            }
            else
            {
                item.Digest = "nicht gefunden";
            }
        }

        /// <summary>
        /// Contains the filename and the digest value of the file.
        /// </summary>
        public class DigestItem
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DigestItem"/> class.
            /// </summary>
            /// <param name="adapter">The adapter.</param>
            public DigestItem(TafAdapterInfo adapter)
            {
                this.Digest = adapter.Hash;
                this.Filename = adapter.Assembly.GetName().Name;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="DigestItem"/> class.
            /// </summary>
            /// <param name="adapter">The adapter.</param>
            public DigestItem(HanAdapterInfo adapter)
            {
                this.Digest = adapter.Hash;
                this.Filename = adapter.Assembly.GetName().Name + $"_{adapter.ManufacturerName}";
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="DigestItem"/> class.
            /// </summary>
            /// <param name="filepath">The filepath.</param>
            public DigestItem(string filepath)
            {
                this.Filepath = filepath;
                this.Filename = Path.GetFileName(filepath);
            }

            /// <summary>
            /// Gets or sets the filename (without path).
            /// </summary>
            public string Filename { get; set; }

            /// <summary>
            /// Gets or sets the file path.
            /// </summary>
            public string Filepath { get; set; }

            /// <summary>
            /// Gets or sets the digest value as hex formatted string.
            /// </summary>
            public string Digest { get; set; }
        }
    }
}
