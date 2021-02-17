namespace TRuDI.TafAdapter.Interface.Taf2
{
    using System;
    using System.Collections.Generic;

    public interface IAccountingSection
    {
        DateTime Start { get; }
        Reading Reading { get; }

        IReadOnlyList<IMeasuringRange> MeasuringRanges { get; }
        IReadOnlyList<Register> SummaryRegister { get; }
    }
}
