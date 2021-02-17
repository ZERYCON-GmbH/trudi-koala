//
// AaltoTLS.PluginInterface.CipherSuite
//

using System;

namespace IVU.Common.Tls.Plugin.CipherSuitePluginInterface
{
    public class CipherSuite
    {
        public readonly ProtocolVersion ProtocolVersion;
        public readonly UInt16 CipherSuiteID;
        public readonly string CipherSuiteName;

        private KeyExchangeAlgorithm _keyExchangeAlgorithm;
        private SignatureAlgorithm _signatureAlgorithm;
        private PseudoRandomFunction _pseudoRandomFunction;
        private BulkCipherAlgorithm _bulkCipherAlgorithm;
        private MACAlgorithm _macAlgorithm;

        public bool IsAnonymous
        {
            get { return (KeyExchangeAlgorithm.CertificateKeyAlgorithm == null && SignatureAlgorithm.CertificateKeyAlgorithm == null); }
        }

        public KeyExchangeAlgorithm KeyExchangeAlgorithm
        {
            get { return _keyExchangeAlgorithm; }
            internal set { _keyExchangeAlgorithm = value; }
        }

        public SignatureAlgorithm SignatureAlgorithm
        {
            get { return _signatureAlgorithm; }
            internal set { _signatureAlgorithm = value; }
        }

        public PseudoRandomFunction PseudoRandomFunction
        {
            get { return _pseudoRandomFunction; }
            internal set { _pseudoRandomFunction = value; }
        }

        public BulkCipherAlgorithm BulkCipherAlgorithm
        {
            get { return _bulkCipherAlgorithm; }
            internal set { _bulkCipherAlgorithm = value; }
        }

        public MACAlgorithm MACAlgorithm
        {
            get { return _macAlgorithm; }
            internal set { _macAlgorithm = value; }
        }

        public CipherSuite(ProtocolVersion version)
        {
            ProtocolVersion = version;
            CipherSuiteID = 0x0000;
            CipherSuiteName = "TLS_NULL_WITH_NULL_NULL";

            _keyExchangeAlgorithm = new KeyExchangeAlgorithmNull();
            _signatureAlgorithm = new SignatureAlgorithmNull();
            _pseudoRandomFunction = null;
            _bulkCipherAlgorithm = new BulkCipherAlgorithmNull();
            _macAlgorithm = new MACAlgorithmNull();
        }

        public CipherSuite(ProtocolVersion version, UInt16 id, string csName)
        {
            ProtocolVersion = version;
            CipherSuiteID = id;
            CipherSuiteName = csName;
        }
    }
}
