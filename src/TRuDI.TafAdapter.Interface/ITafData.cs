namespace TRuDI.TafAdapter.Interface
{
    using System;

    using TRuDI.HanAdapter.Interface;

    /// <summary>
    /// Common interface used by all TAF adapters for the resulting data object.
    /// </summary>
    public interface ITafData
    {
        /// <summary>
        /// TAF to that the data belongs to.
        /// </summary>
        TafId TafId { get; }

        /// <summary>
        /// Start of the billing period.
        /// </summary>
        DateTime Begin { get; }

        /// <summary>
        /// End of the billing period.
        /// </summary>
        DateTime End { get; }

    }
}