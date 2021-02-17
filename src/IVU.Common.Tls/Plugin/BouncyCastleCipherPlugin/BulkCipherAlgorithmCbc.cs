using System;
using System.Security.Cryptography;
using IVU.Common.Tls.Plugin.CipherSuitePluginInterface;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;

namespace IVU.Common.Tls.Plugin.BouncyCastleCipherPlugin
{
    using IVU.Common.Tls.Plugin.CipherSuitePluginInterface;

    public class BulkCipherAlgorithmCbc<BlockCipherType> : BulkCipherAlgorithm where BlockCipherType : IBlockCipher, new()
    {
        private int _keySize;

        public BulkCipherAlgorithmCbc(int keySize)
        {
            if (keySize % 8 != 0)
                throw new ArgumentException("keySize");
            _keySize = keySize / 8;

            // Make sure the key size is valid for this cipher and get block size
            BlockCipherType cipher = new BlockCipherType();
            cipher.Init(true, new KeyParameter(new byte[_keySize]));
            BlockSize = cipher.GetBlockSize();
        }

        public override int KeySize => _keySize;

        public override int BlockSize { get; }

        public override int Strength => _keySize * 8;

        public override BulkCipherAlgorithmType Type => BulkCipherAlgorithmType.Block;

        public override bool SupportsProtocolVersion(ProtocolVersion version)
        {
            return true;
        }

        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV, byte[] additional)
        {
            if (rgbKey == null)
            {
                throw new ArgumentNullException(nameof(rgbKey));
            }
            if (rgbKey.Length != _keySize)
            {
                throw new CryptographicException("rgbKey");
            }
            if (rgbIV == null)
            {
                throw new ArgumentNullException(nameof(rgbIV));
            }
            if (rgbIV.Length != BlockSize)
            {
                throw new CryptographicException("rgbIV");
            }

            var cipher = new BouncyCastleBlockCipher<BlockCipherType>(rgbKey);
            return new GenericCbcModeCryptoTransform(cipher, true, rgbIV);
        }

        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV, byte[] additional)
        {
            if (rgbKey == null)
                throw new ArgumentNullException(nameof(rgbKey));
            if (rgbKey.Length != _keySize)
                throw new CryptographicException("rgbKey");
            if (rgbIV == null)
                throw new ArgumentNullException(nameof(rgbIV));
            if (rgbIV.Length != BlockSize)
                throw new CryptographicException("rgbIV");

            var cipher = new BouncyCastleBlockCipher<BlockCipherType>(rgbKey);
            return new GenericCbcModeCryptoTransform(cipher, false, rgbIV);
        }
    }
}

