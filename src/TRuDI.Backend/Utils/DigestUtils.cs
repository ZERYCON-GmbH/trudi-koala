namespace TRuDI.Backend.Utils
{
    using System;
    using System.IO;
    using System.Reflection;

    using Org.BouncyCastle.Crypto.Digests;

    /// <summary>
    /// Provides methods to create digest values.
    /// </summary>
    public static class DigestUtils
    {
        /// <summary>
        /// Gets the digest for the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly to calculate the digest from.</param>
        /// <returns>The digest string.</returns>
        public static string GetDigestFromAssembly(Assembly assembly)
        {
            var digest = new RipeMD160Digest();
            var data = File.ReadAllBytes(assembly.Location);

            digest.BlockUpdate(data, 0, data.Length);

            var result = new byte[digest.GetDigestSize()];
            digest.DoFinal(result, 0);

            return BitConverter.ToString(result).Replace("-", "");
        }

        /// <summary>
        /// Gets the SHA3 digest from the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The digest string.</returns>
        public static string GetSha3(MemoryStream data)
        {
            var digest = new KeccakDigest();
            digest.BlockUpdate(data.ToArray(), 0, (int)data.Length);

            var result = new byte[digest.GetDigestSize()];
            digest.DoFinal(result, 0);

            return BitConverter.ToString(result).Replace("-", "");
        }

        /// <summary>
        /// Gets the RIPEMD160 digest for the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The digest string.</returns>
        public static string GetRipemd160(MemoryStream data)
        {
            var digest = new RipeMD160Digest();
            digest.BlockUpdate(data.ToArray(), 0, (int)data.Length);

            var result = new byte[digest.GetDigestSize()];
            digest.DoFinal(result, 0);

            return BitConverter.ToString(result).Replace("-", "");
        }

        /// <summary>
        /// Gets the RIPEMD160 digest for the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>The digest string.</returns>
        public static string GetRipemd160(string filename)
        {
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Delete | FileShare.ReadWrite))
            {
                byte[] buffer = new byte[4092];
                int bytesRead;

                var digest = new RipeMD160Digest();
                while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    digest.BlockUpdate(buffer, 0, bytesRead);
                }

                var result = new byte[digest.GetDigestSize()];
                digest.DoFinal(result, 0);

                return BitConverter.ToString(result).Replace("-", "");
            }
        }
    }
}
