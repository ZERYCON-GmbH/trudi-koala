namespace IVU.Http.Http
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Security.Cryptography.X509Certificates;

    using Org.BouncyCastle.Pkcs;
    using Org.BouncyCastle.Crypto.Parameters;
    using System.Security.Authentication;

    public class ClientCertificateWithKey
    {
        public ClientCertificateWithKey(byte[] pkcs12data, string password)
        {
            if(pkcs12data == null || pkcs12data.Length == 0)
            {
                throw new ArgumentException("No PKCS#12 data specified", nameof(pkcs12data));
            }

            var inputKeyStore = new Pkcs12Store();
            try
            {
                using (var ms = new MemoryStream(pkcs12data))
                {
                    inputKeyStore.Load(ms, string.IsNullOrEmpty(password) ? new char[0] : password.ToCharArray());
                }
            }
            catch(IOException ex)
            {
                throw new AuthenticationException("Parsing of the PKCS#12 data failed", ex);
            }
            catch(Exception)
            {
                throw;
            }

            var keyAlias = inputKeyStore.Aliases.Cast<string>().FirstOrDefault(n => inputKeyStore.IsKeyEntry(n));
            if(keyAlias == null)
            {
                throw new InvalidDataException("No private key found in PKCS12 data");
            }

            var bcert = inputKeyStore.GetCertificate(keyAlias);

            this.Certificate = new X509Certificate2(bcert.Certificate.GetEncoded());

            var ck = inputKeyStore.GetKey(keyAlias);
            var ecpk = ck.Key as ECPrivateKeyParameters;

            this.Key = ecpk.D.ToByteArrayUnsigned();

            var sb = new StringBuilder();
            TextWriter tw = new StringWriter(sb);

            var pw = new Org.BouncyCastle.OpenSsl.PemWriter(tw);
            pw.WriteObject(ecpk);

            this.Key = Encoding.ASCII.GetBytes(sb.ToString());
        }

        public ClientCertificateWithKey(X509Certificate2 certificate, byte[] key)
        {
            this.Certificate = certificate;
            this.Key = key;
        }

        public X509Certificate2 Certificate { get; }

        /// <summary>
        /// The private key in PEM format.
        /// </summary>
        public byte[] Key { get; }
    }
}
