namespace TRuDI.HanAdapter.Interface
{
    using System.Diagnostics;

    /// <summary>
    /// Represents the version of a SMGW firmware component.
    /// </summary>
    [DebuggerDisplay("{Component}, {Version}")]
    public class FirmwareVersion
    {
        /// <summary>
        /// Gets or sets the name or a description of the component.
        /// </summary>
        public string Component { get; set; }

        /// <summary>
        /// Gets or sets the version number.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the hash value if present.
        /// </summary>
        public string Hash { get; set; }
    }
}
