using IVU.Common.Tls.Plugin.CipherSuitePluginInterface;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;

namespace IVU.Common.Tls.Plugin.BouncyCastleCipherPlugin
{
    using IVU.Common.Tls.Plugin.CipherSuitePluginInterface;

    public class BouncyCastleBlockCipher<BlockCipherType> : IGenericBlockCipher where BlockCipherType : IBlockCipher, new()
    {
        private int _blockSize;
        private IBlockCipher _encryptor;
        private IBlockCipher _decryptor;

        public int BlockSize
        {
            get { return _blockSize; }
        }

        public BouncyCastleBlockCipher(byte[] key)
        {
            _blockSize = new BlockCipherType().GetBlockSize();

            ICipherParameters param = new KeyParameter(key);

            _encryptor = new BlockCipherType();
            _encryptor.Init(true, param);

            _decryptor = new BlockCipherType();
            _decryptor.Init(false, param);
        }

        public void Encrypt(byte[] inputBuffer, int inputOffset, byte[] outputBuffer, int outputOffset)
        {
            _encryptor.ProcessBlock(inputBuffer, inputOffset, outputBuffer, outputOffset);
        }

        public void Decrypt(byte[] inputBuffer, int inputOffset, byte[] outputBuffer, int outputOffset)
        {
            _decryptor.ProcessBlock(inputBuffer, inputOffset, outputBuffer, outputOffset);
        }
    }
}

