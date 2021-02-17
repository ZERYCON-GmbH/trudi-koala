using System.Security.Cryptography;
using IVU.Common.Tls.Plugin.CipherSuitePluginInterface;

namespace IVU.Common.Tls
{
    using IVU.Common.Tls.Plugin.CipherSuitePluginInterface;

    public struct KeyBlock
	{
		public readonly byte[] ClientWriteMACKey;
		public readonly byte[] ServerWriteMACKey;
		public readonly byte[] ClientWriteKey;
		public readonly byte[] ServerWriteKey;
		public readonly byte[] ClientWriteIV;
		public readonly byte[] ServerWriteIV;
	
		public KeyBlock(CipherSuite cipherSuite, byte[] masterSecret, byte[] seed)
		{
			DeriveBytes deriveBytes = cipherSuite.PseudoRandomFunction.CreateDeriveBytes(masterSecret, "key expansion", seed);
			ClientWriteMACKey = deriveBytes.GetBytes(cipherSuite.MACAlgorithm.HashSize);
			ServerWriteMACKey = deriveBytes.GetBytes(cipherSuite.MACAlgorithm.HashSize);
			ClientWriteKey = deriveBytes.GetBytes(cipherSuite.BulkCipherAlgorithm.KeySize);
			ServerWriteKey = deriveBytes.GetBytes(cipherSuite.BulkCipherAlgorithm.KeySize);
			ClientWriteIV = deriveBytes.GetBytes(cipherSuite.BulkCipherAlgorithm.FixedIVLength);
			ServerWriteIV = deriveBytes.GetBytes(cipherSuite.BulkCipherAlgorithm.FixedIVLength);
		}
	}
}
