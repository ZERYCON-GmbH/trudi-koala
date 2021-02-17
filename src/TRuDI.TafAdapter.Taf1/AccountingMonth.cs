namespace TRuDI.TafAdapter.Taf1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TRuDI.TafAdapter.Interface;

    using TRuDI.TafAdapter.Interface.Taf2;

    public class AccountingMonth : IAccountingSection
    {
        private readonly List<MeasuringRange> measuringRanges = new List<MeasuringRange>();
        private readonly List<Register> summaryRegister;

        public AccountingMonth(IList<Register> register)
        {
            this.summaryRegister = new List<Register>(register);
        }

        public DateTime Start
        {
            get; set;
        }

        public Reading Reading
        {
            get; set;
        }

        public void Add(MeasuringRange range)
        {
            this.measuringRanges.Add(range);

            this.summaryRegister.FirstOrDefault(r => r.TariffId == range.TariffId).Amount =
                       this.summaryRegister.FirstOrDefault(r => r.TariffId == range.TariffId).Amount + range.Amount;
        }

        public IReadOnlyList<IMeasuringRange> MeasuringRanges => this.measuringRanges;

        public IReadOnlyList<Register> SummaryRegister => this.summaryRegister;
    }
}
