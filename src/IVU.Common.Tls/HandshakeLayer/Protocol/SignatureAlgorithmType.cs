namespace IVU.Common.Tls.HandshakeLayer.Protocol
{
    public enum SignatureAlgorithmType : byte
    {
        //https://tools.ietf.org/html/rfc5246#section-7.4.1.4.1
        Anonymous = 0,
        Rsa = 1,
        Dsa = 2,
        Ecdsa = 3,
    }
}