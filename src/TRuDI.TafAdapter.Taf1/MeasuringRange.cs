using System;
using System.Collections.Generic;
using System.Text;
using TRuDI.Models;
using TRuDI.Models.BasicData;
using TRuDI.TafAdapter.Interface;

namespace TRuDI.TafAdapter.Taf1
{
    using TRuDI.TafAdapter.Interface.Taf2;

    public class MeasuringRange : IMeasuringRange
    {
        public MeasuringRange()
        {
        }

        public MeasuringRange(DateTime Start, DateTime End, ushort TariffId, long Amount)
        {
            this.Start = Start;
            this.End = End;
            this.TariffId = TariffId;
            this.Amount = Amount;
        }

        public DateTime Start
        {
            get; set;
        }

        public DateTime End
        {
            get; set;
        }

        public ushort TariffId
        {
            get; set;
        }

        public long Amount
        {
            get; set;
        }
    }
}
