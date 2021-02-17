namespace TRuDI.HanAdapter.Interface
{
    using System.Collections.Generic;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    /// Instance of this class is returned by the <see cref="IHanAdapter.Connect(string, System.Net.IPEndPoint, byte[], string, System.Collections.Generic.Dictionary{string, string}, System.TimeSpan, System.Threading.CancellationToken, System.Action{ProgressInfo})"/>.
    /// </summary>
    public class ConnectResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectResult"/> class.
        /// </summary>
        /// <param name="certificate">The TLS server certificate of the gateway.</param>
        /// <param name="firmwareVersions">The list of the firmware component versions.</param>
        public ConnectResult(X509Certificate2 certificate, IReadOnlyList<FirmwareVersion> firmwareVersions)
        {
            this.Certificate = certificate;
            this.FirmwareVersions = firmwareVersions;
        }

        /// <summary>
        /// Gets the TLS server certificate of the gateway.
        /// </summary>
        public X509Certificate2 Certificate { get; }

        /// <summary>
        /// Gets the list of the gateway firmware component versions.
        /// </summary>
        public IReadOnlyList<FirmwareVersion> FirmwareVersions { get; }
    }
}