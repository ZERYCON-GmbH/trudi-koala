namespace IVU.Common.Tls.HandshakeLayer.Protocol
{
    // Types as defined in: https://tools.ietf.org/html/rfc4492#section-5.1.1
    public enum EccNamedCurve : ushort
    {
        Ect163K1 = 1,
        Sect163R1 = 2,
        Sect163R2 = 3,
        Sect193R1 = 4,
        Sect193R2 = 5,
        Sect233K1 = 6,
        Sect233R1 = 7,
        Sect239K1 = 8,
        Sect283K1 = 9,
        Sect283R1 = 10,
        Sect409K1 = 11,
        Sect409R1 = 12,
        Sect571K1 = 13,
        Sect571R1 = 14,
        Secp160K1 = 15,
        Secp160R1 = 16,
        Secp160R2 = 17,
        Secp192K1 = 18,
        Secp192R1 = 19,
        Secp224K1 = 20,
        Secp224R1 = 21,
        Secp256K1 = 22,

        /// <summary>
        /// Used for LMN (old FNN spec, replaced by brainpool curve 26)
        /// </summary>
        Secp256R1 = 23,

        Secp384R1 = 24,
        Secp521R1 = 25,

        /// <summary>
        /// Used for LMN
        /// </summary>
        BrainpoolP256R1 = 26,
        BrainpoolP384R1 = 27,
        BrainpoolP512R1 = 28,

        //reserved (0xFE00..0xFEFF),
        ArbitraryExplicitPrimeCurves = 0xFF01,
        ArbitraryExplicitChar2Curves = 0xFF02,
    }
}