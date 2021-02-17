namespace TRuDI.TafAdapter.Interface.Taf2
{
    using System.Collections.Generic;

    using TRuDI.Models.CheckData;

    /// <inheritdoc />
    /// <summary>
    /// Interface used for the TAF-2 (Zeitvariabler Tarif) and TAF-1 data.
    /// </summary>
    public interface ITaf2Data : ITafData
    {
        /// <summary>
        /// Contians the first reading from the original value list.
        /// </summary>
        IReadOnlyList<Reading> InitialReadings { get; }

        /// <summary>
        /// A list of days or months with tariff changes.
        /// </summary>
        IReadOnlyList<IAccountingSection> AccountingSections { get; }

        /// <summary>
        /// The sums of the total period.
        /// </summary>
        IReadOnlyList<Register> SummaryRegister { get; }

        /// <summary>
        /// List of the used tariff stages.
        /// </summary>
        IReadOnlyList<TariffStage> TariffStages { get; }
    }
}
