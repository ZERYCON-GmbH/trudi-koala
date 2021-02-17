namespace TRuDI.HanAdapter.Interface
{
    using System;

    /// <summary>
    /// This class defines what the HAN adapter has to read out.
    /// </summary>
    public class AdapterContext
    {
        /// <summary>
        /// The selected contract for readout.
        /// </summary>
        public ContractInfo Contract { get; set; }

        /// <summary>
        /// Gets or sets the billing period to read the derived register values.
        /// </summary>
        public BillingPeriod BillingPeriod { get; set; }

        /// <summary>
        /// Begin timestamp for readout of the original reading list and log data.
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// End timestamp for readout of the original reading list and log data.
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// Only meaningful when TAF-7 is applicable.
        /// The AnalysisProfile may contain data that is neccessary for SMGW readout.
        /// </summary>
        public AnalysisProfile AnalysisProfile { get; set; }

        /// <summary>
        /// Gets or sets a flag to enable or disable the loading of the log data. 
        /// Log data can be huge and are probably not always required by the user.
        /// </summary>
        public bool WithLogdata { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"Contract: {this.Contract.TafId}: {this.Contract.TafName}, begin: {this.Contract.Begin}, end: {this.Contract.End.ToString() ?? "-"}, " +
                $"BillingPeriod: begin: {this.BillingPeriod?.Begin}, end: {this.BillingPeriod?.End?.ToString() ?? "-"}, " + 
                $"Query: begin: {this.Start}, end: {this.End}";
        }
    }
}