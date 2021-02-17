namespace TRuDI.HanAdapter.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Represents the data of a tariff contract (corresponds to a TAF).
    /// </summary>
    [DebuggerDisplay("{TafId}: {TafName}, Begin: {Begin}")]
    public class ContractInfo
    {
        /// <summary>
        /// BSI TAF ID (TAF-1, TAF-2, TAF-6, TAF-7, ...)
        /// </summary>
        public TafId TafId { get; set; }

        /// <summary>
        /// Logical name of the TAF profile (uniqe identification of the TAF profile).
        /// </summary>
        public string TafName { get; set; }

        /// <summary>
        /// Contract description if available (tag TariffName).
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a list with the meters associated with this contract.
        /// The IDs must have the format of "Herstellerübergreifende Identifikationsnummer" (DIN 43863-5, e.g. "1XXX0012345678")
        /// </summary>
        public IReadOnlyList<string> Meters { get; set; }

        /// <summary>
        /// Gets or sets the metering point identifier (Zählpunktbezeichnung).
        /// </summary>
        public string MeteringPointId { get; set; }

        /// <summary>
        /// Gets or sets the supplier identifier.
        /// </summary>
        public string SupplierId { get; set; }

        /// <summary>
        /// Gets or sets the consumer identifier.
        /// </summary>
        public string ConsumerId { get; set; }

        /// <summary>
        /// Begin of contract.
        /// </summary>
        public DateTime Begin { get; set; }

        /// <summary>
        /// End of contract. <c>null</c> if still active and there is no end configured.
        /// </summary>
        public DateTime? End { get; set; }

        /// <summary>
        /// Available billing periods for this contract. 
        /// </summary>
        public IReadOnlyList<BillingPeriod> BillingPeriods { get; set; }

        /// <summary>
        /// The medium of the TAF.
        /// </summary>
        public ObisMedium? Medium { get; set; }
    }
}