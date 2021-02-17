namespace TRuDI.Models
{
    using System;

    using TRuDI.Models.BasicData;

    public class HistoricConsumption
    {
        public TimeUnit UnitOfTime { get; set; }

        public DateTime Begin { get; set; }
        public DateTime End { get; set; }

        public HistoricConsumption(IntervalReading startReading, IntervalReading endReading, DateTime begin, DateTime end, TimeUnit unitOfTime)
        {
            if (startReading?.Value != null && endReading?.Value != null)
            {
                this.Value = endReading.Value - startReading.Value;
            }
            
            this.UnitOfTime = unitOfTime;
            this.Begin = begin;
            this.End = end;
        }

        /// <summary>
        /// return null if the consumption cannot be calculated (missing end value or start value)
        /// </summary>
        /// <returns></returns>
        public long? Value { get; }
    }
}