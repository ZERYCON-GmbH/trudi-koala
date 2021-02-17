using System.Security.Cryptography.X509Certificates;

namespace IVU.Common.Tls.Plugin.CipherSuitePluginInterface
{
	public abstract class KeyExchangeAlgorithm
	{
		// Returns the OID for compatible certificate or null
		public abstract string CertificateKeyAlgorithm { get; }
		
		// Returns true if this version is supported, otherwise false
		public abstract bool SupportsProtocolVersion(ProtocolVersion version);
		
		// Returns the server key exchange message as a byte array
		public abstract byte[] GetServerKeys(ProtocolVersion version, CertificatePrivateKey certPrivateKey);
		// Returns the server key exchange signature as a byte array
		public abstract byte[] ProcessServerKeys(ProtocolVersion version, byte[] data, X509Certificate serverCertificate);

		// Returns the client key exchange message as a byte array
		public abstract byte[] GetClientKeys(ProtocolVersion version, ProtocolVersion clientVersion, CertificatePublicKey publicKey);
		public abstract void ProcessClientKeys(ProtocolVersion version, ProtocolVersion clientVersion, CertificatePrivateKey privateKey, byte[] data);
		
		// Returns the resulting master secret, either GetClientKeys or
		// ProcessClientKeys needs to be called before calling this method
		public abstract byte[] GetMasterSecret(PseudoRandomFunction prf, byte[] seed);
	}
}
