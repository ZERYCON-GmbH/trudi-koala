namespace TRuDI.TafAdapter.Taf2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TRuDI.HanAdapter.Interface;
    using TRuDI.Models.CheckData;
    using TRuDI.TafAdapter.Interface.Taf2;

    public class Taf2Data : ITaf2Data
    {
        private List<AccountingDay> accountingDays = new List<AccountingDay>();
        private readonly List<Register> summaryRegister;
        private readonly List<Reading> initialReadings = new List<Reading>();

        public Taf2Data(IList<Register> summaryRegister, IReadOnlyList<TariffStage> tariffStages)
        {
            this.summaryRegister = new List<Register>(summaryRegister);
            this.TariffStages = tariffStages;
        }

        public TafId TafId => TafId.Taf2;

        public DateTime Begin { get; set; }

        public DateTime End { get; set; }

        public IReadOnlyList<TariffStage> TariffStages { get; }

        public void Add(AccountingDay day)
        {
            foreach (Register reg in this.summaryRegister)
            {
                reg.Amount = reg.Amount + day.SummaryRegister.FirstOrDefault(r => r.TariffId == reg.TariffId && reg.ObisCode.C == r.ObisCode.C).Amount;
            }

            this.accountingDays.Add(day);
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

        public void OrderSections()
        {
            if (this.accountingDays.Count > 1)
            {
                this.accountingDays = this.accountingDays.OrderBy(day => day.Start).ToList();
            }
        }

        public void SetDates(DateTime begin, DateTime end)
        {
            this.Begin = begin;
            this.End = end;
        }

        public IReadOnlyList<Reading> InitialReadings => this.initialReadings;

        public IReadOnlyList<IAccountingSection> AccountingSections => this.accountingDays;

        public IReadOnlyList<Register> SummaryRegister => this.summaryRegister;
    }
}
