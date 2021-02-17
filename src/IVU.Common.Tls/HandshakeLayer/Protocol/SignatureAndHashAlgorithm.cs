namespace IVU.Common.Tls.HandshakeLayer.Protocol
{
    public class SignatureAndHashAlgorithm
    {
        public SignatureAndHashAlgorithm()
        {
        }

        public SignatureAndHashAlgorithm(ushort value)
        {
            this.HashAlgorithm = (HashAlgorithmType)(value >> 8);
            this.SignatureAlgorithm = (SignatureAlgorithmType)(value & 0xFF);
        }

        public HashAlgorithmType HashAlgorithm { get; set; }
        public SignatureAlgorithmType SignatureAlgorithm { get; set; }
    }
}