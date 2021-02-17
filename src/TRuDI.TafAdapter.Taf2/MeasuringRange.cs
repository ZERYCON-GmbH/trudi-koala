using TRuDI.Models;

namespace TRuDI.TafAdapter.Taf2
{
    using System;
    using TRuDI.Models.BasicData;
    using TRuDI.TafAdapter.Interface;
    using TRuDI.TafAdapter.Interface.Taf2;

    public class MeasuringRange : IMeasuringRange
    {
        public MeasuringRange() { }

        public MeasuringRange(DateTime start, DateTime end, ushort tariffId, long amount)
        {
            this.Start = start;
            this.End = end;
            this.TariffId = tariffId;
            this.Amount = amount;
        }

        public MeasuringRange(DateTime start, DateTime end, long amount)
        {
            this.Start = start;
            this.End = end;
            this.TariffId = 63;
            this.Amount = amount;
        }

        public MeasuringRange(DateTime start, DateTime end, MeterReading mr, int amount)
        {
            this.Start = start;
            this.End = end;
            var obis = new ObisId(mr.ReadingType.ObisCode);
            this.TariffId = obis.E;
            this.Amount = amount;
        }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public ushort TariffId { get; set; }

        public long Amount { get; set; }
    }
}
