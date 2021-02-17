namespace TRuDI.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using TRuDI.Models.BasicData;

    /// <summary>
    /// The class provides access to to original value lists.
    /// </summary>
    public class OriginalValueList
    {
        /// <summary>
        /// The meter reading which belongs to the original value list.
        /// </summary>
        public MeterReading MeterReading { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OriginalValueList" /> class.
        /// </summary>
        /// <param name="meterReading">The meter reading.</param>
        /// <param name="serviceCategory">The service category.</param>
        public OriginalValueList(MeterReading meterReading, Kind serviceCategory)
        {
            this.MeterReading = meterReading;
            this.ServiceCategory = serviceCategory;

            this.Obis = new ObisId(this.MeterReading.ReadingType.ObisCode);
            this.DisplayUnit = this.MeterReading.ReadingType.Uom.GetDisplayUnit(this.MeterReading.ReadingType.PowerOfTenMultiplier ?? PowerOfTenMultiplier.None);

            this.MeasurementPeriod = meterReading.GetMeasurementPeriod();
            foreach (var block in this.MeterReading.IntervalBlocks)
            {
                this.GapCount += block.GetGapCount(this.MeasurementPeriod);
                this.ValueCount += block.IntervalReadings.Count;

                var statusCount = block.GetStatusCount();
                this.OkCount += statusCount.Ok;
                this.WarningCount += statusCount.Warning;
                this.TempErrorCount += statusCount.TempError;
                this.CriticalTempErrorCount += statusCount.CriticalTempError;
                this.FatalErrorCount += statusCount.FatalError;
            }

            if (this.MeterReading.IntervalBlocks.Count != 0)
            {
                this.Start = this.MeterReading.IntervalBlocks.First().IntervalReadings.First().TargetTime.Value;
                this.End = this.MeterReading.IntervalBlocks.Last().IntervalReadings.Last().TargetTime.Value;
            }
            else
            {
                this.Start = null;
                this.End = null;
            }

            this.Meter = this.MeterReading.Meters.FirstOrDefault()?.MeterId;

            if (this.Start.HasValue && this.End.HasValue)
            {
                this.HistoricValues = this.CalculateHistoricConsumption();
            }
        }

        public Kind ServiceCategory { get; set; }

        public List<HistoricConsumption> HistoricValues { get; }

        public int GapCount { get; }
        public int ValueCount { get; }
        public int FatalErrorCount { get; set; }
        public int CriticalTempErrorCount { get; set; }
        public int TempErrorCount { get; set; }
        public int WarningCount { get; set; }
        public int OkCount { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance uses the TargetTime property.
        /// </summary>
        public bool IsTargetTimeUsed => this.MeterReading.IsTargetTimeUsed;

        public bool HasErrors => this.FatalErrorCount > 0 || this.WarningCount > 0 || this.TempErrorCount > 0
                                 || this.CriticalTempErrorCount > 0 || this.WarningCount > 0;

        public TimeSpan MeasurementPeriod { get; }

        public ObisId Obis { get; }

        public string DisplayUnit { get; }

        public Uom Uom => this.MeterReading.ReadingType.Uom ?? Uom.Not_Applicable;

        public PowerOfTenMultiplier PowerOfTenMultiplier => this.MeterReading.ReadingType.PowerOfTenMultiplier ?? PowerOfTenMultiplier.None;

        public short Scaler => this.MeterReading.ReadingType.Scaler;

        public string Meter { get; }

        public DateTime? Start { get; }
        public DateTime? End { get; }

        public bool HasData => this.MeterReading.IntervalBlocks.Count != 0;

        public IEnumerable<IntervalReading> GetReadings(DateTime start, DateTime end)
        {
            if (this.MeterReading.IntervalBlocks.Count == 0)
            {
                yield break;
            }

            start = start.ToUniversalTime();
            end = end.ToUniversalTime();

            var lastTimestamp = this.Start.Value.ToUniversalTime();

            if (this.MeasurementPeriod != TimeSpan.Zero)
            {
                var currentTimestamp = this.Start?.GetPrevMeasurementPeriod(this.MeasurementPeriod).ToUniversalTime();
                if (currentTimestamp < this.Start?.ToUniversalTime())
                {
                    currentTimestamp += this.MeasurementPeriod;
                }

                foreach (var block in this.MeterReading.IntervalBlocks)
                {
                    foreach (var reading in block.IntervalReadings)
                    {
                        if (reading.TargetTime?.ToUniversalTime() > currentTimestamp)
                        {
                            var alignedTargetTime = ModelExtensions.GetAlignedTimestamp(
                                reading.TargetTime.Value,
                                (int)this.MeasurementPeriod.TotalSeconds).ToUniversalTime();

                            var lastTargetTimeAligned = ModelExtensions.GetAlignedTimestamp(
                                lastTimestamp,
                                (int)this.MeasurementPeriod.TotalSeconds);

                            if (lastTargetTimeAligned != currentTimestamp && (alignedTargetTime > currentTimestamp || (alignedTargetTime > currentTimestamp && lastTargetTimeAligned > currentTimestamp)))
                            {
                                while (reading.TargetTime?.ToUniversalTime() > currentTimestamp)
                                {
                                    if (currentTimestamp >= start && currentTimestamp <= end)
                                    {
                                        // found a gap: create the missing element with only the timestamp.
                                        yield return new IntervalReading
                                        {
                                            TimePeriod =
                                                new Interval
                                                {
                                                    Start = currentTimestamp
                                                            .Value.ToLocalTime()
                                                },
                                            TargetTime = currentTimestamp?.ToLocalTime()
                                        };

                                        lastTimestamp = currentTimestamp.Value;
                                    }

                                    currentTimestamp += this.MeasurementPeriod;
                                    if (this.MeasurementPeriod == TimeSpan.Zero)
                                    {
                                        yield break;
                                    }
                                }
                            }
                        }

                        if (reading.TargetTime?.ToUniversalTime() > end)
                        {
                            yield break;
                        }

                        if (reading.TargetTime?.ToUniversalTime() >= start)
                        {
                            yield return reading;

                            lastTimestamp = reading.TargetTime.Value.ToUniversalTime();
                            if (reading.TargetTime?.ToUniversalTime() >= currentTimestamp)
                            {
                                currentTimestamp += this.MeasurementPeriod;
                            }
                        }
                        else
                        {
                            currentTimestamp = reading.TargetTime?.GetPrevMeasurementPeriod(this.MeasurementPeriod).ToUniversalTime();
                            if (currentTimestamp < reading.TargetTime?.ToUniversalTime())
                            {
                                currentTimestamp += this.MeasurementPeriod;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var block in this.MeterReading.IntervalBlocks)
                {
                    foreach (var reading in block.IntervalReadings)
                    {
                        if (reading.TargetTime?.ToUniversalTime() < start)
                        {
                            continue;
                        }

                        if (reading.TargetTime?.ToUniversalTime() <= end)
                        {
                            yield return reading;
                        }
                        else
                        {
                            yield break;
                        }
                    }
                }
            }
        }

        public List<DailyOvlErrorStatus> GetErrorsList()
        {
            var statusList = new List<DailyOvlErrorStatus>();

            if (this.MeterReading.IntervalBlocks.Count == 0)
            {
                return statusList;
            }

            var currentDay = new DailyOvlErrorStatus { Timestamp = DateTime.MinValue.Date };

            foreach (var reading in this.GetReadings(this.Start.Value, this.End.Value))
            {
                if (currentDay.Timestamp != reading.TargetTime?.Date)
                {
                    currentDay = new DailyOvlErrorStatus { Timestamp = reading.TargetTime.Value.Date };
                    statusList.Add(currentDay);
                }

                currentDay.ValueCount++;

                if (reading.Value == null)
                {
                    currentDay.GapCount++;
                    continue;
                }

                switch (reading.StatusPTB ?? reading.StatusFNN.MapToStatusPtb())
                {
                    case StatusPTB.NoError:
                        currentDay.OkCount++;
                        break;

                    case StatusPTB.Warning:
                        currentDay.WarningCount++;
                        break;

                    case StatusPTB.TemporaryError:
                        currentDay.TempErrorCount++;
                        break;

                    case StatusPTB.CriticalTemporaryError:
                        currentDay.CriticalTempErrorCount++;
                        break;

                    case StatusPTB.FatalError:
                        currentDay.FatalErrorCount++;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return statusList;
        }

        public IntervalReading GetReading(DateTime timestamp)
        {
            foreach (var block in this.MeterReading.IntervalBlocks)
            {
                var reading = block.IntervalReadings.FirstOrDefault(ir => ir.TargetTime == timestamp);
                if (reading != null)
                {
                    return reading;
                }
            }

            return null;
        }

        /// <summary>
        /// Historic consumption is needed for following time periods, (if enough data available):
        /// %daysGoingBack% most recent days, day-by-day
        /// %weeksGoingBack% most recent calendar weeks, week-by-week
        /// %monthsGoingBack% most recent calendar months, month-by-month
        /// the most recent calender year
        /// </summary>
        /// <returns></returns>
        private List<HistoricConsumption> CalculateHistoricConsumption()
        {
            var retList = new List<HistoricConsumption>();

            if (this.MeterReading.IntervalBlocks.Count == 0)
            {
                return retList;
            }

            var dayStartHour = 0;
            if (this.ServiceCategory == Kind.Gas)
            {
                dayStartHour = 6;
            }


            // Dayly Historic Reads
            int daysGoingBack = 7;
            var lastDayEnd = new DateTime(this.End.Value.Year, this.End.Value.Month, this.End.Value.Day, dayStartHour, 0, 0, DateTimeKind.Local);

            for (int i = 0; i < daysGoingBack; i++)
            {
                if (lastDayEnd < this.Start)
                {
                    break;
                }

                var startTime = lastDayEnd.AddDays(-1);
                var startReading = this.GetReading(startTime);
                var endReading = this.GetReading(lastDayEnd);

                var val = new HistoricConsumption(startReading, endReading, startTime, lastDayEnd, TimeUnit.Day);
                if (val.Value != null)
                {
                    retList.Add(val);
                }

                lastDayEnd = lastDayEnd.AddDays(-1);
            }

            // Weekly
            int weeksGoingBack = 4;
            var lastSundayEnd = new DateTime(this.End.Value.Year, this.End.Value.Month, this.End.Value.Day, dayStartHour, 0, 0, DateTimeKind.Local);

            while (lastSundayEnd.DayOfWeek != DayOfWeek.Monday)
            {
                lastSundayEnd = lastSundayEnd.AddDays(-1);
            }

            for (int i = 0; i < weeksGoingBack; i++)
            {
                if (lastSundayEnd < this.Start)
                {
                    break;
                }

                var startTime = lastSundayEnd.AddDays(-7);
                var startReading = this.GetReading(startTime);
                var endReading = this.GetReading(lastSundayEnd);

                var val = new HistoricConsumption(startReading, endReading, startTime, lastSundayEnd, TimeUnit.Week);
                if (val.Value != null)
                {
                    retList.Add(val);
                }

                lastSundayEnd = lastSundayEnd.AddDays(-7);
            }

            // Monthly
            int monthsGoingBack = 36;
            var lastMonthEnd = new DateTime(this.End.Value.Year, this.End.Value.Month, 1, dayStartHour, 0, 0, DateTimeKind.Local);

            for (int i = 0; i < monthsGoingBack; i++)
            {
                if (lastMonthEnd < this.Start)
                {
                    break;
                }

                var startTime = lastMonthEnd.AddMonths(-1);
                var startReading = this.GetReading(startTime);
                var endReading = this.GetReading(lastMonthEnd);

                var val = new HistoricConsumption(startReading, endReading, startTime, lastMonthEnd, TimeUnit.Month);
                if (val.Value != null)
                {
                    retList.Add(val);
                }

                lastMonthEnd = lastMonthEnd.AddMonths(-1);
            }

            // Yearly
            int yearsGoingBack = 3;
            var lastYearEnd = new DateTime(this.End.Value.Year, 1, 1, dayStartHour, 0, 0, DateTimeKind.Local);

            for (int i = 0; i < yearsGoingBack; i++)
            {
                if (lastYearEnd < this.Start)
                {
                    break;
                }

                var startTime = lastYearEnd.AddYears(-1);
                var startReading = this.GetReading(startTime);
                var endReading = this.GetReading(lastYearEnd);

                var val = new HistoricConsumption(startReading, endReading, startTime, lastYearEnd, TimeUnit.Year);
                if (val.Value != null)
                {
                    retList.Add(val);
                }

                lastYearEnd = lastYearEnd.AddYears(-1);
            }

            return retList;
        }
    }
}
