namespace TRuDI.TafAdapter.Taf2
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using TRuDI.Models;
    using TRuDI.TafAdapter.Interface.Taf2;

    [DebuggerDisplay("{Start} - {Reading.ObisCode} - {Reading.Amount}")]
    public class AccountingDay : IAccountingSection
    {
        private readonly List<MeasuringRange> measuringRanges = new List<MeasuringRange>();
        private readonly List<Register> summaryRegister;

        public AccountingDay(IList<Register> register)
        {
            this.summaryRegister = new List<Register>(register);
        }

        public DateTime Start { get; set; }

        public Reading Reading { get; set; }

        public void Add(MeasuringRange range, ObisId obisId)
        {
            this.measuringRanges.Add(range);
            this.summaryRegister.First(r => r.TariffId == range.TariffId && obisId.C == r.ObisCode.C).Amount =
                this.summaryRegister.First(r => r.TariffId == range.TariffId && obisId.C == r.ObisCode.C).Amount + range.Amount;  
        }

        public IReadOnlyList<IMeasuringRange> MeasuringRanges => this.measuringRanges;

        public IReadOnlyList<Register> SummaryRegister => this.summaryRegister;
    }
}
