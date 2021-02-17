namespace TRuDI.TafAdapter.Taf1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TRuDI.HanAdapter.Interface;
    using TRuDI.Models.CheckData;
    using TRuDI.TafAdapter.Interface.Taf2;

    public class Taf1Data : ITaf2Data
    {
        private readonly List<AccountingMonth> accountingMonths = new List<AccountingMonth>();
        private readonly List<Register> summaryRegister;
        private readonly List<Reading> initialReadings = new List<Reading>();

        public Taf1Data(IList<Register> summaryRegister, IReadOnlyList<TariffStage> tariffStages)
        {
            this.summaryRegister = new List<Register>(summaryRegister);
            this.TariffStages = tariffStages;
        }

        public TafId TafId => TafId.Taf1;

        public DateTime Begin { get; private set; }

        public DateTime End { get; private set; }

        public IReadOnlyList<TariffStage> TariffStages { get; }

        public void SetDate(DateTime begin, DateTime end)
        {
            this.Begin = begin;
            this.End = end;
        }

        public void AddInitialReading(Reading reading)
        {
            if (this.initialReadings.Count < 1)
            {
                this.initialReadings.Add(reading);
            }
            else
            {
                if (this.initialReadings.FirstOrDefault(ir => ir.ObisCode == reading.ObisCode) == null)
                {
                    this.initialReadings.Add(reading);
                }
            }
        }

        public void Add(AccountingMonth month)
        {
            foreach (Register reg in this.summaryRegister)
            {
                reg.Amount = reg.Amount + month.SummaryRegister.FirstOrDefault(r => r.TariffId == reg.TariffId).Amount;
            }

            this.accountingMonths.Add(month);
        }


        public IReadOnlyList<Reading> InitialReadings => this.initialReadings;

        public IReadOnlyList<IAccountingSection> AccountingSections => this.accountingMonths;

        public IReadOnlyList<Register> SummaryRegister => this.summaryRegister;
    }
}
