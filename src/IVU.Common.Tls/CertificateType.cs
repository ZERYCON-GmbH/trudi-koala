namespace IVU.Common.Tls
{
    public enum CertificateType : byte
    {
        RsaSign = 1,
        DssSign = 2,
        RsaFixedDh = 3,
        DssFixedDh = 4,
        RsaEphemeralDhReserved = 5,
        DssEphemeralDhReserved = 6,
        FortezzaDmsReserved = 20,
        EcdsaSign = 64,
        RsaFixedEcdh = 65,
        EcdsaFixedEcdh = 66,
    }
}