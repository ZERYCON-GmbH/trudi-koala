namespace TRuDI.Backend.Models
{
    /// <summary>
    /// The possible authentication modes.
    /// </summary>
    public enum AuthMode
    {
        /// <summary>
        /// The username/password mode.
        /// </summary>
        UserPassword = 0,

        /// <summary>
        /// The client certificate mode.
        /// </summary>
        ClientCertificate = 1,
    }
}