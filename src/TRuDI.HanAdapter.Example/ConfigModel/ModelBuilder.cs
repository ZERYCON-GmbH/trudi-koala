namespace TRuDI.HanAdapter.Example.ConfigModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TRuDI.HanAdapter.Interface;
    using TRuDI.Models;
    using TRuDI.Models.BasicData;
    using TRuDI.Models.CheckData;

    /// <summary>
    /// The class provides methods to create the model data for an XmlBuilder instance.
    /// </summary>
    public static class ModelBuilder
    {

        /// <summary>
        /// Creates the LogEntries for the xml data file.
        /// </summary>
        /// <param name="builder">The calling XmlBuilder Object </param>
        /// <param name="hanConfiguration"></param>
        /// <returns></returns>
        public static List<LogEntry> CreateLogs(this XmlFactory builder, HanAdapterExampleConfig hanConfiguration)
        {
            var logs = new List<LogEntry>();
            var messages = hanConfiguration.XmlConfig.PossibleLogMessages;
            var logCount = GetRandomNumber(hanConfiguration.XmlConfig.MinLogCount, hanConfiguration.XmlConfig.MaxLogCount);
            var timestamp = hanConfiguration.Start;

            for (int index = 0; index < logCount; index++)
            {
                logs.Add(new LogEntry()
                {
                    RecordNumber = (uint)index + 1,
                    LogEvent = new LogEvent()
                    {
                        Level = (Level)GetRandomNumber(1, 4),
                        Outcome = (Outcome)GetRandomNumber(0, 1),
                        Text = messages[GetRandomNumber(0, messages.Count - 1)],
                        Timestamp = (DateTime)timestamp?.SetRandomTimestamp(new TimeSpan(0, 15, 0))
                    }
                });

                timestamp = logs.LastOrDefault().LogEvent.Timestamp;
            }

            return logs;
        }

        public static List<MeterReading> CreateMeterReadings(this XmlFactory builder, HanAdapterExampleConfig hanConfiguration)
        {
            var meterReadings = new List<MeterReading>();
            var billingPeriod = hanConfiguration.BillingPeriod.SetBillingPeriod((DateTime)hanConfiguration.Start, (DateTime)hanConfiguration.End);
            var originalValueList = new List<MeterReading>();
            var taf2Data = new List<Taf2Data>();

            foreach (MeterReadingConfig meterReadingConfig in hanConfiguration.XmlConfig.MeterReadingConfigs)
            {
                var mr = new MeterReading();


                mr.InitMeterReading(meterReadingConfig);

                if (meterReadingConfig.IsOML)
                {
                    mr.ReadingType.QualifiedLogicalName = BuildQualifiedLogicalName(mr.Meters[0].MeterId, mr.ReadingType.ObisCode);
                    mr.OriginalValueList(hanConfiguration, meterReadingConfig, taf2Data);
                    originalValueList.Add(mr);
                }
                else
                {
                    mr.ReadingType.QualifiedLogicalName = BuildQualifiedLogicalName(hanConfiguration.XmlConfig.TariffId, mr.ReadingType.ObisCode);
                    switch (hanConfiguration.Contract.TafId)
                    {
                        case TafId.Taf1:
                            mr.Taf1RegisterMeterReading(mr.GetAssociatedOriginalValueList(originalValueList), billingPeriod, meterReadingConfig);
                            break;
                        case TafId.Taf2:
                            mr.Taf2RegisterMeterReading(mr.GetAssociatedOriginalValueList(originalValueList), billingPeriod, meterReadingConfig, taf2Data);
                            break;
                        default:
                            throw new InvalidOperationException($"for the specified Tafid {hanConfiguration.Contract.TafId} a derived register should not be created.");
                    }
                }

                meterReadings.Add(mr);
            }
            return meterReadings;
        }

        public static List<DayProfile> CreateDayProfiles(this XmlFactory builder, HanAdapterExampleConfig hanConfiguration)
        {
            var dayProfiles = new List<DayProfile>();

            foreach (DayProfileConfig config in hanConfiguration.XmlConfig.DayProfiles)
            {
                var dayProfile = new DayProfile();
                var dayTimeProfiles = new List<DayTimeProfile>();

                dayProfile.DayId = config.DayId;

                foreach (DayTimeProfileConfig dayTimeConfig in config.DayTimeProfiles)
                {
                    var time = dayTimeConfig.Start;

                    while (time <= dayTimeConfig.End)
                    {
                        var dayTimeProfile = new DayTimeProfile();
                        dayTimeProfile.StartTime.Hour = (byte)time.Hour;
                        dayTimeProfile.StartTime.Minute = (byte)time.Minute;
                        dayTimeProfile.TariffNumber = dayTimeConfig.TariffNumber;
                        time = time.AddSeconds(900);
                        dayTimeProfiles.Add(dayTimeProfile);
                    }

                }

                dayProfile.DayTimeProfiles = dayTimeProfiles;
                dayProfiles.Add(dayProfile);
            }

            return dayProfiles;
        }

        public static List<SpecialDayProfile> CreateSpecialDayProfiles(this XmlFactory builder, HanAdapterExampleConfig hanConfiguration)
        {
            var specialDayProfiles = new List<SpecialDayProfile>();

            var specialDayProfileSets = GetSpecialDayProfileSets(hanConfiguration);

            var billingPeriod = hanConfiguration.BillingPeriod.SetBillingPeriod((DateTime)hanConfiguration.Start, (DateTime)hanConfiguration.End);

            foreach ((List<int> allowedDayIds, List<int> tariffNumbers, ObisId obisCode) set in specialDayProfileSets)
            {
                var dayIds = set.allowedDayIds;
                var date = billingPeriod.Begin;

                while (date < billingPeriod.End)
                {
                    var specialDayProfile = new SpecialDayProfile
                    {
                        SpecialDayDate = new DayVarType
                        {
                            Year = (ushort)date.Year,
                            Month = (byte)date.Month,
                            DayOfMonth = (byte)date.Day
                        }
                    };

                    var index = GetRandomNumber(0, dayIds.Count - 1);
                    specialDayProfile.DayId = (ushort)dayIds[index];
                    date = date.AddDays(1);
                    specialDayProfiles.Add(specialDayProfile);
                }
            }

            specialDayProfiles.OrderBy(spd => spd.SpecialDayDate.GetDate());
            return specialDayProfiles;
        }

        private static List<(List<int> allowedDayIds, List<int> tariffNumbers, ObisId obisCode)> GetSpecialDayProfileSets(HanAdapterExampleConfig hanConfiguration)
        {
            var specialDayProfileSets = new List<(List<int> allowedDayIds, List<int> tariffNumbers, ObisId obisCode)>();
            
            foreach(MeterReadingConfig reading in hanConfiguration.XmlConfig.MeterReadingConfigs)
            {
                if (reading.IsOML)
                {
                    specialDayProfileSets.Add((new List<int>(), new List<int>(), new ObisId(reading.ObisCode)));
                }
            }

            foreach(TariffStageConfig config in hanConfiguration.XmlConfig.TariffStageConfigs)
            {
                foreach ((List<int> allowedDayIds, List < int > tariffNumbers, ObisId obisCode) specialDayProfileSet in specialDayProfileSets)
                {
                    if(specialDayProfileSet.obisCode.C == new ObisId(config.ObisCode).C)
                    {
                        specialDayProfileSet.tariffNumbers.Add((int)config.TariffNumber);
                    }
                }
            }

            foreach(DayProfileConfig config in hanConfiguration.XmlConfig.DayProfiles)
            {
                foreach ((List<int> allowedDayIds, List<int> tariffNumbers, ObisId obisCode) specialDayProfileSet in specialDayProfileSets)
                {
                    var isValidDayTimeProfile = true;
                    foreach (DayTimeProfileConfig dtConfig in config.DayTimeProfiles)
                    {
                        if (specialDayProfileSet.tariffNumbers.Contains((int)dtConfig.TariffNumber))
                        {
                            continue;
                        }
                        else
                        {
                            isValidDayTimeProfile = false;
                        }
                    }

                    if (isValidDayTimeProfile)
                    {
                        specialDayProfileSet.allowedDayIds.Add((int)config.DayId);
                    }
                }
            }

            return specialDayProfileSets;
        }

        private static void OriginalValueList(this MeterReading meterReading, HanAdapterExampleConfig hanConfiguration, MeterReadingConfig meterReadingConfig, List<Taf2Data> taf2Data)
        {
            var intervalBlock = InitIntervalBlockWithInterval((DateTime)hanConfiguration.Start, (DateTime)hanConfiguration.End);
            var taf2 = new Taf2Data(new ObisId(meterReading.ReadingType.ObisCode));

            var value = meterReadingConfig.OMLInitValue;
            var preValue = value;
            var timestamp = intervalBlock.Interval.Start.ToUniversalTime();

            var end = intervalBlock.Interval.GetEnd().GetDateWithoutSeconds().ToUniversalTime();
            while (timestamp <= end)
            {
                var ir = new IntervalReading()
                {
                    TimePeriod = new Interval()
                    {
                        Duration = 0,
                        Start = timestamp.ToLocalTime()
                    },
                    Value = value
                };

                if (preValue != value)
                {
                    taf2.Data.Add((timestamp.ToLocalTime(), GetRandomNumber(0, meterReadingConfig.Taf2TariffStages), value - preValue));
                }

                preValue = value;
                value = value + GetRandomNumber(1, hanConfiguration.XmlConfig.MaxConsumptionValue);
                timestamp = timestamp.AddSeconds(meterReadingConfig.PeriodSeconds.Value).GetDateWithoutSeconds();
                SetStatusWord(ir, meterReadingConfig);
                ir.IntervalBlock = intervalBlock;
                intervalBlock.IntervalReadings.Add(ir);
            }

            taf2Data.Add(taf2);
            intervalBlock.MeterReading = meterReading;
            meterReading.IntervalBlocks.Add(intervalBlock);
        }

        private static void Taf1RegisterMeterReading(this MeterReading meterReading, MeterReading oml, BillingPeriod billingPeriod, MeterReadingConfig meterReadingConfig)
        {
            var intervalBlock = InitIntervalBlockWithInterval(billingPeriod.Begin, (DateTime)billingPeriod.End);
            var period = meterReadingConfig.Taf1PeriodInMonth;
            var requiredValues = GetRequiredIntervalReadingsFromOML(oml, billingPeriod.Begin, (DateTime)billingPeriod.End);

            var timestamp = billingPeriod.Begin.AddMonths(period);
            var initValue = requiredValues.FirstOrDefault(val => val.TimePeriod.Start == billingPeriod.Begin).Value;
            var value = GetNextValue(initValue, requiredValues, timestamp, (DateTime)billingPeriod.End);

            while (timestamp <= billingPeriod.End)
            {
                var ir = new IntervalReading()
                {
                    TimePeriod = new Interval()
                    {
                        Duration = 0,
                        Start = timestamp
                    },
                    Value = value
                };

                SetStatusWord(ir, meterReadingConfig);
                ir.IntervalBlock = intervalBlock;
                intervalBlock.IntervalReadings.Add(ir);
                timestamp = timestamp.AddMonths(period);
                value = GetNextValue(initValue, requiredValues, timestamp, (DateTime)billingPeriod.End);
            }

            intervalBlock.MeterReading = meterReading;
            meterReading.IntervalBlocks.Add(intervalBlock);
        }

        private static void Taf2RegisterMeterReading(this MeterReading meterReading, MeterReading oml, BillingPeriod billingPeriod, MeterReadingConfig meterReadingConfig, List<Taf2Data> taf2Data)
        {
            var intervalBlock = InitIntervalBlockWithInterval(billingPeriod.Begin, (DateTime)billingPeriod.End);
            var ObisCode = new ObisId(oml.ReadingType.ObisCode);
            var taf2 = taf2Data.FirstOrDefault(t => t.ObisID.ToHexString() == ObisCode.ToHexString());
            var requiredValues = GetRequiredIntervalReadingsFromOML(oml, billingPeriod.Begin, (DateTime)billingPeriod.End);

            int value = 0;
            if (new ObisId(meterReading.ReadingType.ObisCode).E == 0)
            {
                foreach ((DateTime timestamp, int tariff, int value) item in taf2.Data)
                {
                    if (item.timestamp >= billingPeriod.Begin && item.timestamp <= (DateTime)billingPeriod.End)
                    {
                        value = value + item.value;
                    }
                }
            }
            else
            {
                foreach ((DateTime timestamp, int tariff, int value) item in taf2.Data)
                {
                    if (item.timestamp >= billingPeriod.Begin && item.timestamp <= (DateTime)billingPeriod.End && item.tariff == meterReadingConfig.Taf2TariffStage)
                    {
                        value = value + item.value;
                    }
                }
            }

            var ir = new IntervalReading()
            {
                TimePeriod = new Interval()
                {
                    Duration = (uint)(billingPeriod.End - billingPeriod.Begin)?.TotalSeconds,
                    Start = billingPeriod.Begin
                },
                Value = value
            };

            SetStatusWord(ir, meterReadingConfig);
            ir.IntervalBlock = intervalBlock;
            intervalBlock.IntervalReadings.Add(ir);
            intervalBlock.MeterReading = meterReading;
            meterReading.IntervalBlocks.Add(intervalBlock);
        }

        private static MeterReading GetAssociatedOriginalValueList(this MeterReading meterReading, List<MeterReading> meterReadings)
        {
            return meterReadings.FirstOrDefault(mr => new ObisId(mr.ReadingType.ObisCode).C == new ObisId(meterReading.ReadingType.ObisCode).C);
        }

        private static void InitMeterReading(this MeterReading meterReading, MeterReadingConfig meterReadingConfig)
        {
            meterReading.Meters.Add(new Meter() { MeterId = meterReadingConfig.MeterId });
            meterReading.MeterReadingId = meterReadingConfig.MeterReadingId;

            meterReading.ReadingType = new ReadingType()
            {
                MeterReading = meterReading,
                PowerOfTenMultiplier = (PowerOfTenMultiplier)meterReadingConfig.PowerOfTenMultiplier,
                Uom = (Uom)meterReadingConfig.Uom,
                Scaler = (short)meterReadingConfig.Scaler,
                ObisCode = meterReadingConfig.ObisCode
            };
        }

        private static List<IntervalReading> GetRequiredIntervalReadingsFromOML(MeterReading oml, DateTime start, DateTime end)
        {
            return oml.IntervalBlocks.FirstOrDefault(ib => ib.Interval.IsPeriodInIntervalBlock(start, end))
                     .IntervalReadings.Where(ir => ir.TimePeriod.Start >= start && ir.TimePeriod.GetEnd() <= end)
                     .OrderBy(ir => ir.TimePeriod.Start).ToList();
        }

        private static long? GetNextValue(long? initValue, List<IntervalReading> readings, DateTime next, DateTime end)
        {
            long? result = 0;
            if (next <= end)
            {
                result = readings.FirstOrDefault(val => val.TimePeriod.Start == next).Value - initValue;
            }

            return result;
        }

        private static BillingPeriod SetBillingPeriod(this BillingPeriod billingPeriod, DateTime start, DateTime end)
        {
            if (billingPeriod == null)
            {
                billingPeriod = new BillingPeriod()
                {
                    Begin = start,
                    End = end
                };
            }

            return billingPeriod;
        }

        private static IntervalBlock InitIntervalBlockWithInterval(DateTime start, DateTime end)
        {
            return new IntervalBlock() { Interval = new Interval() { Duration = (uint)(end - start).TotalSeconds, Start = start } };
        }

        private static void SetStatusWord(IntervalReading ir, MeterReadingConfig config)
        {
            if (config.UsedStatus == "FNN")
            {
                ir.StatusFNN = new StatusFNN(GetStatusFNN());
            }
            else if (config.UsedStatus == "PTB")
            {
                ir.StatusPTB = (StatusPTB)GetRandomNumber(0, 4);
            }
        }

        private static string BuildQualifiedLogicalName(string iD, string obisCode)
        {
            return $"{obisCode}.{iD}.sm";
        }

        /// <summary>
        /// Im Durchschnitt, wird alle 6 Tage ein fataler Fehler, und alle 2 Tage eine Warnung vorkommen
        /// </summary>
        /// <returns></returns>
        private static string GetStatusFNN()
        {
            var rand = GetRandomNumber(0, 6 * 96 + 1); //6-tage-lang von Intervallen

            if (rand % (6 * 96) == 0)
                return "210500000004"; //Fataler Fehler
            else if (rand % (2 * 96) == 0)
                return "200500000004"; //Warnung
            else
                return "0500000004";   //Kein Fehler
        }

        private static int GetRandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max + 1);
        }

        private static DateTime SetRandomTimestamp(this DateTime start, TimeSpan max)
        {
            var result = GetRandomNumber((int)(max.TotalSeconds / 2), (int)max.TotalSeconds);

            return start.AddSeconds(result);
        }
    }
}
