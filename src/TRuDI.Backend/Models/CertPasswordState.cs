namespace TRuDI.Backend.Models
{
    public enum CertPasswordState
    {
        NoCertSelected = 0,
        WithoutPassword,
        InvalidPassword,
        PasswordValid,
        InvalidCertFile,
    }
}