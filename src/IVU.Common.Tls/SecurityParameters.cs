using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using IVU.Common.Tls.Plugin;
using IVU.Common.Tls.Plugin.CipherSuitePluginInterface;

namespace IVU.Common.Tls
{
    using IVU.Common.Tls.HandshakeLayer.Protocol;
    using IVU.Common.Tls.Plugin;
    using IVU.Common.Tls.Plugin.CipherSuitePluginInterface;

    // TODO: Should include client cipher suites and extensions to help in selection
    public delegate int ServerCertificateSelectionCallback(CipherSuite cipherSuite, X509CertificateCollection[] serverCertificates);

    public delegate bool ServerCertificateValidationCallback(X509CertificateCollection certificates);

    // TODO: Should include supported hash algorithms and possibly other information, what?
    public delegate int ClientCertificateSelectionCallback(X509CertificateCollection[] clientCertificates, X509CertificateCollection serverCertificate);

    public class SecurityParameters
    {
        private List<X509CertificateCollection> _availableCertificates;
        private List<CertificatePrivateKey> _availablePrivateKeys;

        public ProtocolVersion MinimumVersion;
        public ProtocolVersion MaximumVersion;
        public readonly List<CipherSuiteId> CipherSuiteIDs;
        public readonly List<byte> CompressionIDs;

        // Callbacks used by clients, null means default
        public ServerCertificateValidationCallback ServerCertificateValidationCallback;
        public ClientCertificateSelectionCallback ClientCertificateSelectionCallback;

        // Callbacks used by servers, null means default
        public ServerCertificateSelectionCallback ServerCertificateSelectionCallback;

        // Client certificate parameters, ignored if client
        public readonly List<CertificateType> ClientCertificateTypes;
        public readonly List<string> ClientCertificateAuthorities;


        public X509CertificateCollection[] AvailableCertificates
        {
            get { return _availableCertificates.ToArray(); }
        }

        public CertificatePrivateKey[] AvailablePrivateKeys
        {
            get { return _availablePrivateKeys.ToArray(); }
        }

        public SecurityParameters()
        {
            _availableCertificates = new List<X509CertificateCollection>();
            _availablePrivateKeys = new List<CertificatePrivateKey>();

            MinimumVersion = ProtocolVersion.SSL3_0;
            MaximumVersion = ProtocolVersion.TLS1_2;

            CipherSuiteIDs = new List<CipherSuiteId>();
            //CipherSuiteIDs.Add(0x002F); // TLS_RSA_WITH_AES_128_CBC_SHA
            //CipherSuiteIDs.Add(0x0033); // TLS_DHE_RSA_WITH_AES_128_CBC_SHA

            CompressionIDs = new List<byte>();
            CompressionIDs.Add(0x00);

            ClientCertificateTypes = new List<CertificateType>();
            ClientCertificateAuthorities = new List<string>();
        }

        public void AddCertificate(X509CertificateCollection certificate, CertificatePrivateKey privateKey)
        {
            if (certificate == null)
                throw new ArgumentNullException("certificate");
            if (privateKey == null)
                throw new ArgumentNullException("privateKey");
            if (certificate.Count == 0)
                throw new ArgumentException("certificate");

            _availableCertificates.Add(certificate);
            _availablePrivateKeys.Add(privateKey);
        }
    }
}

