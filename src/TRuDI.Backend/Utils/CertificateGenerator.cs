namespace TRuDI.Backend.Utils
{
    using System;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;

    using Org.BouncyCastle.Asn1.X509;
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Generators;
    using Org.BouncyCastle.Crypto.Operators;
    using Org.BouncyCastle.Crypto.Prng;
    using Org.BouncyCastle.Math;
    using Org.BouncyCastle.Pkcs;
    using Org.BouncyCastle.Security;
    using Org.BouncyCastle.Utilities;
    using Org.BouncyCastle.X509;

    /// <summary>
    /// The certificate generator is used to generate a certificate to secure the TLS connection between the Electron frontend and the ASP.Net backend.
    /// </summary>
    public class CertificateGenerator
    {
        /// <summary>
        /// Generates the certificate.
        /// </summary>
        /// <param name="subjectName">Name of the subject.</param>
        /// <returns>The generated certificate.</returns>
        public static X509Certificate2 GenerateCertificate(string subjectName)
        {
            var randomGenerator = new CryptoApiRandomGenerator();
            var random = new SecureRandom(randomGenerator);

            var certificateGenerator = new X509V3CertificateGenerator();

            var serialNumber =
                BigIntegers.CreateRandomInRange(
                    BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), random);

            certificateGenerator.SetSerialNumber(serialNumber);

            var subjectDN = new X509Name(subjectName);
            var issuerDN = subjectDN;
            certificateGenerator.SetIssuerDN(issuerDN);
            certificateGenerator.SetSubjectDN(subjectDN);

            var notBefore = DateTime.UtcNow.Date;
            var notAfter = notBefore.AddDays(31);

            certificateGenerator.SetNotBefore(notBefore);
            certificateGenerator.SetNotAfter(notAfter);

            var keyGenerationParameters = new KeyGenerationParameters(random, 2048);

            var keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            var subjectKeyPair = keyPairGenerator.GenerateKeyPair();

            certificateGenerator.SetPublicKey(subjectKeyPair.Public);

            var issuerKeyPair = subjectKeyPair;

            ISignatureFactory signatureFactory = new Asn1SignatureFactory("SHA256WithRSA", issuerKeyPair.Private, random);
            var certificate = certificateGenerator.Generate(signatureFactory);

            var store = new Pkcs12Store();
            string friendlyName = certificate.SubjectDN.ToString();
            var certificateEntry = new X509CertificateEntry(certificate);
            store.SetCertificateEntry(friendlyName, certificateEntry);
            store.SetKeyEntry(friendlyName, new AsymmetricKeyEntry(subjectKeyPair.Private), new[] { certificateEntry });

            var stream = new MemoryStream();
            store.Save(stream, new char[0], random);

            return new X509Certificate2(stream.ToArray());
        }
    }
}
