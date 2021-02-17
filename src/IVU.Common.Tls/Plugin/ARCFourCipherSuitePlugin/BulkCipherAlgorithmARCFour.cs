using System;
using System.Security.Cryptography;
using IVU.Common.Tls.Plugin.CipherSuitePluginInterface;

namespace IVU.Common.Tls.Plugin.ARCFourCipherSuitePlugin
{
    using IVU.Common.Tls.Plugin.CipherSuitePluginInterface;

    public class BulkCipherAlgorithmARCFour : BulkCipherAlgorithm
	{
		private int _keySize;

		public BulkCipherAlgorithmARCFour(int keySize)
		{
			if (keySize <= 0)
				throw new ArgumentOutOfRangeException("Invalid keysize (" + keySize + "), must be larger than 0");
			if ((keySize % 8) != 0)
				throw new ArgumentException("Invalid key size: " + keySize);
			
			_keySize = keySize/8;
		}

		public override int KeySize
		{
			get { return _keySize; }
		}

		public override int BlockSize
		{
			get { return 1; }
		}
		
		public override int Strength
		{
			get { return _keySize*8; }
		}

		public override BulkCipherAlgorithmType Type
		{
			get { return BulkCipherAlgorithmType.Stream; }
		}

		public override bool SupportsProtocolVersion(ProtocolVersion version)
		{
			if (version.IsUsingDatagrams) {
				return false;
			}
			return true;
		}

		public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV, byte[] additional)
		{
			if (rgbKey == null)
				throw new ArgumentNullException("rgbKey");
			if (rgbKey.Length != _keySize)
				throw new CryptographicException("rgbKey");
			
			return new ARCFourCryptoTransform(rgbKey);
		}

		public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV, byte[] additional)
		{
			if (rgbKey == null)
				throw new ArgumentNullException("rgbKey");
			if (rgbKey.Length != _keySize)
				throw new CryptographicException("rgbKey");
			
			return new ARCFourCryptoTransform(rgbKey);
		}
	}
}
