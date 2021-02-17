namespace TRuDI.HanAdapter.Repository
{
    using System;

    /// <summary>
    /// Unknown or unsupported FLAG identifier.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class UnknownManufacturerException : Exception
    {
        /// <summary>
        /// Gets or sets the FLAG identifier that isn't known.
        /// </summary>
        public string FlagId { get; set; }
    }
}
