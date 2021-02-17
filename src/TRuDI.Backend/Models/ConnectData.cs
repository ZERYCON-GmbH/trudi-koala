namespace TRuDI.Backend.Models
{
    /// <summary>
    /// This class stores the data needed to connect to the SMGW device.s
    /// </summary>
    public class ConnectData
    {
        /// <summary>
        /// Gets or sets the DIN device identifier.
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Gets or sets the authentication mode.
        /// </summary>
        public AuthMode AuthMode { get; set; } = AuthMode.UserPassword;

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the IP address of SMGW.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the TCP port used to connect to the SMGW.
        /// </summary>
        public ushort Port { get; set; } = 443;
    }
}
