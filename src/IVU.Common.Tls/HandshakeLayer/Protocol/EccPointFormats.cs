namespace IVU.Common.Tls.HandshakeLayer.Protocol
{
    // Types as defined in: https://tools.ietf.org/html/rfc4492#section-5.1.2
    public enum EccPointFormats : byte
    {
        Uncompressed = 0,
        AnsiX962CompressedPrime = 1,
        AnsiX962CompressedChar2 = 2,
    }
}