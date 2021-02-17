namespace IVU.Common.Tls.HandshakeLayer.Protocol
{
    public enum HashAlgorithmType : byte
    {
        //https://tools.ietf.org/html/rfc5246#section-7.4.1.4.1
        None = 0,
        Md5 = 1,
        Sha1 = 2,
        Sha224 = 3,
        Sha256 = 4,
        Sha384 = 5,
        Sha512 = 6,
    }
}