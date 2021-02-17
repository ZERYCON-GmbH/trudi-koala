namespace TRuDI.TafAdapter.Taf2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TRuDI.Models;
    using TRuDI.Models.BasicData;
    using TRuDI.Models.CheckData;
    using TRuDI.TafAdapter.Interface;
    using TRuDI.TafAdapter.Interface.Taf2;
    using TRuDI.TafAdapter.Taf2.Components;

    /// <inheritdoc />
    /// <summary>
    /// Default TAF-2 implementation.
    /// </summary>
    public class TafAdapterTaf2 : ITafAdapter
    {
        private List<OriginalValueList> originalValueLists;
        private DateTime billingPeriodStart;
        private DateTime billingPeriodEnd;

        /// <inheritdoc />
        /// <summary>
        /// Calculates the derived registers for Taf2.
        /// </summary>
        /// <param name="device">Data from the SMGW. There should be just original value lists.</param>
        /// <param name="supplier">The calculation data from the supplier.</param>
        /// <returns>An ITaf2Data instance. The object contains the calculated data.</returns>
        public TafAdapterData Calculate(UsagePointAdapterTRuDI device, UsagePointLieferant supplier)
        {
            this.originalValueLists =
                device.MeterReadings.Where(mr => mr.IsOriginalValueList()).Select(mr => new OriginalValueList(mr, device.ServiceCategory.Kind ?? Kind.Electricity)).ToList();

            if (!this.originalValueLists.Any())
            {
                throw new InvalidOperationException("Es ist keine originäre Messwertliste verfügbar.");
            }

            var registers = supplier.GetRegister();
            this.UpdateReadingTypeFromOriginalValueList(registers);

            var accountingPeriod = new Taf2Data(registers, supplier.AnalysisProfile.TariffStages);
            var dayProfiles = supplier.AnalysisProfile.TariffChangeTrigger.TimeTrigger.DayProfiles;

            foreach (var ovl in this.originalValueLists)
            {
                var bpStart = ovl.MeterReading.GetFirstReadingTimestamp(supplier.AnalysisProfile.BillingPeriod.Start, supplier.AnalysisProfile.BillingPeriod.GetEnd());
                var bpEnd = ovl.MeterReading.GetLastReadingTimestamp(supplier.AnalysisProfile.BillingPeriod.Start, supplier.AnalysisProfile.BillingPeriod.GetEnd());
                if (bpEnd == null || bpStart == null || bpStart == bpEnd)
                {
                    throw new InvalidOperationException("Keine Messwerte innerhalb des Abbrechnungszeitraums gefunden.");
                }
                
                accountingPeriod.SetDates(bpStart.Value, bpEnd.Value);
                this.SetTotalBillingPeriod(accountingPeriod);

                var validDayProfiles = dayProfiles.GetValidDayProfilesForMeterReading(ovl.Obis, supplier.AnalysisProfile.TariffStages);
                var specialDayProfiles = this.GetSpecialDayProfiles(supplier, validDayProfiles, accountingPeriod.Begin, accountingPeriod.End);
                long latestReading = 0;
                ushort latestTariffId = 63;

                // Check if the lists validDayProfiles and special DayProfiles are null or empty
                if (!this.CheckUsedLists(validDayProfiles, specialDayProfiles)) { continue; }

                // Calculation of the single days (latestReading and latestTariffId are needed for days across gap detection)
                foreach (SpecialDayProfile profile in specialDayProfiles)
                {
                    var currentDayData = this.GetDayData(profile, dayProfiles, ovl.MeterReading, supplier, latestReading, latestTariffId);
                    if (!(currentDayData.day.Start == DateTime.MinValue) || currentDayData.day.MeasuringRanges.Count != 0)
                    {
                        accountingPeriod.Add(currentDayData.day);
                    }
                    latestReading = currentDayData.latestReading;
                    latestTariffId = currentDayData.tariffId;
                }

                // Set the initial overall meter reading
                accountingPeriod.AddInitialReading(new Reading()
                {
                    Amount = accountingPeriod.AccountingSections.First().Reading.Amount,
                    ObisCode = ovl.Obis
                });

            }

            accountingPeriod.OrderSections();
            return new TafAdapterData(typeof(Taf2SummaryView), typeof(Taf2DetailView), accountingPeriod);
        }

        /// <summary>
        /// The main calculation method for every day in the billing period.
        /// </summary>
        /// <param name="profile">The current SpecialDayProfile</param>
        /// <param name="dayProfiles">A List of all DayProfiles</param>
        /// <param name="meterReading">The MeterReading instance with the raw data.</param>
        /// <param name="supplier">Contains the calculation data.</param>
        /// <param name="latestReading">The last valid value of an IntervalReading.</param>
        /// <param name="latestTariffId">The last vaild tariff.</param>
        /// <returns>The calculated AccountingSection</returns>
        public (AccountingDay day, long latestReading, ushort tariffId) GetDayData(SpecialDayProfile profile, List<DayProfile> dayProfiles, 
            MeterReading meterReading, UsagePointLieferant supplier, long latestReading, ushort latestTariffId)
        {
            var registers = supplier.GetRegister();
            this.UpdateReadingTypeFromOriginalValueList(registers);

            var currentDay = new AccountingDay(registers);
            
            // Every SpecialDayProfile is linked to a dayProfile which contains dayTimeProfiles.
            // This dayTimeProfiles are needed because they contain the tariff change information of the day.
            var dayTimeProfiles = GetValidDayTimeProfiles(dayProfiles, profile);

            var start = ModelExtensions.GetDateTimeFromSpecialDayProfile(profile, dayTimeProfiles[0]);
            var index = 1;
            var startReading = meterReading.GetIntervalReadingFromDate(start);
            
            // If the current day has a gap at 00:00
            if (startReading == null || !IsStatusValid(startReading))
            {
                var startData = this.GetStartReading(start, dayTimeProfiles, profile, 
                    meterReading, currentDay, index, latestReading, latestTariffId);

                index = startData.index;
                startReading = startData.startReading;
                start = startData.startReading != null && IsStatusValid(startData.startReading) ? startData.startReading.TargetTime.Value : start;
                latestTariffId = startData.tariffId;
                latestReading = startData.latestReading;
            }

            // Check whether dayTimeProfiles is null or empty
            this.CheckInitSettings(dayTimeProfiles);

            if(startReading != null && IsStatusValid(startReading))
            {
                currentDay.Reading = new Reading() { Amount = startReading.Value, ObisCode = new ObisId(meterReading.ReadingType.ObisCode) };
                currentDay.Start = profile.SpecialDayDate.GetDate();
            }
                        
            var endReading = SetConcreteIntervalReading(null, DateTime.MinValue);
            for (var i = index; i < dayTimeProfiles.Count; i++)
            {
                DateTime end;
                // Check if the tariff number changes
                if (dayTimeProfiles[i-1].TariffNumber != dayTimeProfiles[i].TariffNumber)
                {
                    // Set the end of the current range 
                    end = ModelExtensions.GetDateTimeFromSpecialDayProfile(profile, dayTimeProfiles[i]);
                    endReading = SetIntervalReading(meterReading, end, i, dayTimeProfiles.Count);

                    var rangeData = GetNextRange(endReading.reading, endReading.end,
                                                 startReading, start, end,
                                                 dayTimeProfiles, meterReading, profile, currentDay, i, latestReading, latestTariffId);
                  

                    latestReading = rangeData.latestReading;
                    start = rangeData.reading.TargetTime.Value;
                    startReading = rangeData.reading;
                    i = rangeData.index;
                    latestTariffId = rangeData.range.TariffId;
                    latestReading = this.GetLatestReading(latestReading, rangeData.reading);

                    if (!this.IsRangeEmpty(rangeData.range))
                    {
                        currentDay.Add(rangeData.range, new ObisId(meterReading.ReadingType.ObisCode));
                    }   
                }
                // If there is no tariff change at the current timestamp
                else
                {
                    //  Check if it is the last value of the current day
                    if (i == dayTimeProfiles.Count - 1)
                    {
                        end = ModelExtensions.GetDateTimeFromSpecialDayProfile(profile, dayTimeProfiles[i]);
                        endReading = SetIntervalReading(meterReading, end, i, dayTimeProfiles.Count);

                        var rangeData = GetNextRange(endReading.reading, endReading.end, 
                                                     startReading, start, end, 
                                                     dayTimeProfiles, meterReading, profile, currentDay, i, latestReading, latestTariffId);

                    
                        latestReading = rangeData.latestReading;
                        start = end;
                        startReading = rangeData.reading;
                        i = rangeData.index;
                        latestTariffId = rangeData.range.TariffId;
                        latestReading = this.GetLatestReading(latestReading, rangeData.reading);

                        if (!this.IsRangeEmpty(rangeData.range)){
                            currentDay.Add(rangeData.range, new ObisId(meterReading.ReadingType.ObisCode));
                        }
                    }
                }
            }
       
            return (currentDay, latestReading, latestTariffId);
        }

        /// <summary>
        /// If no coresonding value to the tarffif change time is found this method will be called.
        /// </summary>
        /// <param name="start">The beginning of the tariff stage range.</param>
        /// <param name="end">The current endpoint of the tariff stage range.</param>
        /// <param name="profile">The used SpecialDayProfile instance.</param>
        /// <param name="dayTimeProfiles">The used DayTime profile.</param>
        /// <param name="meterReading">The raw data.</param>
        /// <param name="index">Start index for iterating over the measurement list.</param>
        /// <returns>The matching DateTime and the new index for the parent loop.</returns>
        public (DateTime end, int index) FindLastValidTime(DateTime start, DateTime end, SpecialDayProfile profile,
            List<DayTimeProfile> dayTimeProfiles, MeterReading meterReading, int index)
        {
            var result = end;
            var match = false;
            var helpindex = index;

            while (!match)
            {
                // Get the next lower value 
                // Example: If there is no value at 5:00 then it is set to 4:45 and so on until a value 
                //          was found or start is reached. If start is reached the next upper value is searched (FindNextValidTime)
                result = ModelExtensions.GetDateTimeFromSpecialDayProfile(profile, dayTimeProfiles[helpindex]);

                if (result == start)
                {
                    (result, helpindex) = this.FindNextValidTime(end, profile, dayTimeProfiles, meterReading, index);

                    if (helpindex + 1 == dayTimeProfiles.Count)
                    {
                        break;
                    }
                }

                var reading = meterReading.GetIntervalReadingFromDate(result);
           
                if (reading != null && IsStatusValid(reading))
                {
                    match = true;
                }
                else
                {
                    helpindex--;
                    if (helpindex < 0)
                    {
                        throw new InvalidOperationException(
                            "Die PTB oder FNN Stati aller Messwerte sind kritisch oder fatal.");
                    }
                }
            }

            return (result, helpindex);
        }

        /// <summary>
        /// Is called from FindLastValidTime when no matching vaule was found.
        /// </summary>
        /// <param name="end">The current endpoint of the tariff stage range.</param>
        /// <param name="profile">The used SpecialDayProfile instance.</param>
        /// <param name="dayTimeProfiles">The used DayTime profile.</param>
        /// <param name="meterReading">The raw data.</param>
        /// <param name="index">For marking the parent loop index.</param>
        /// <returns>The matching DateTime and the new index for the parent loop.</returns>
        public (DateTime end, int index) FindNextValidTime(DateTime end, SpecialDayProfile profile,
            List<DayTimeProfile> dayTimeProfiles, MeterReading meterReading, int index)
        {
            DateTime result = end;
            var match = false;
            var helpindex = index;

            while (!match)
            {
                if (helpindex >= dayTimeProfiles.Count)
                {
                    helpindex = helpindex - 1;
                    break;
                }

                result = ModelExtensions.GetDateTimeFromSpecialDayProfile(profile, dayTimeProfiles[helpindex]);
                var reading = meterReading.GetIntervalReadingFromDate(result);
               
                if (reading != null && IsStatusValid(reading))
                {
                    match = true;
                }
                else
                {
                    helpindex++;
                }
            }
            return (result, helpindex);
        }

        /// <summary>
        /// If a intervalReading to the tariff change timestamp is needed, this method will be called.
        /// </summary>
        /// <param name="meterReading">The raw data.</param>
        /// <param name="end">The date for the meterReading</param>
        /// <param name="index">the index of the parent loop</param>
        /// <returns>An intervalReading or null</returns>
        public (IntervalReading reading, DateTime end) SetIntervalReading(MeterReading meterReading, DateTime end, int index, int dayTimeProfilesCount)
        {
            var date = LocalSetLastReading(end, index, dayTimeProfilesCount);
            var reading = meterReading.GetIntervalReadingFromDate(date);
            
            // If there is a gap at 0:00 o'clock the next day
            if (date > end && (reading == null || !IsStatusValid(reading)))
            {
                date = end;
                reading = meterReading.GetIntervalReadingFromDate(date);
            }

            return (reading, date);
        }

        private static DateTime LocalSetLastReading(DateTime time, int idx, int dtpCount)
        {
            // Checked the TimeOfDay Value. In every case a value from DayTimeProfiles is used. 
            if (idx == dtpCount - 1 && time.TimeOfDay == new TimeSpan(23, 45, 00))
            {
                return time.AddSeconds(900);
            }

            return time;
        }


        /// <summary>
        /// This method is used to create an empty endReading object
        /// </summary>
        /// <returns></returns>
        public static (IntervalReading reading, DateTime end) SetConcreteIntervalReading(IntervalReading reading, DateTime end)
        {
            return (reading, end);
        }

        /// <summary>
        /// Set billingPeriodStart and -End. accountingPeriod.Begin and accountingPeriod.End can not be null 
        /// due to the validation process. There must be a billing period.
        /// </summary>
        /// <param name="accountingPeriod"></param>
        public void SetTotalBillingPeriod(Taf2Data accountingPeriod)
        {
            this.billingPeriodStart = accountingPeriod.Begin;
            this.billingPeriodEnd = accountingPeriod.End;
        }

        /// <summary>
        /// Check which dayTimeProfiles of the current dayProfile are needed for the calculation
        /// </summary>
        /// <param name="dayProfiles">A list of dayProfiles.</param>
        /// <param name="profile">The current SpecialDayProfile object.</param>
        /// <returns>The needed dayTimeProfiles for the calculation.</returns>
        public List<DayTimeProfile> GetValidDayTimeProfiles(List<DayProfile> dayProfiles, SpecialDayProfile profile)
        {

            // NullReferenceException due to Parsing not possible. See Test: TestWrongDayIdSupplierFileParsingException()
            var dayTimeProfiles = dayProfiles.FirstOrDefault(p => p.DayId == profile.DayId).DayTimeProfiles;

            // Is the start of the billing period in this SpecialDayProfile
            if(profile.SpecialDayDate.GetDate() == billingPeriodStart.Date)
            {
                // Is the billing period less than a day
                if (profile.SpecialDayDate.GetDate() == billingPeriodEnd.Date)
                {
                    dayTimeProfiles = dayTimeProfiles
                        .Where(p => p.GetTime() >= billingPeriodStart.TimeOfDay && p.GetTime() <= billingPeriodEnd.TimeOfDay).ToList();
                }
                else
                {
                    dayTimeProfiles = dayTimeProfiles
                        .Where(p => p.GetTime() >= billingPeriodStart.TimeOfDay).ToList();
                }
            }
            // Is the end of the billing period in this SpecialDayProfile
            else if(profile.SpecialDayDate.GetDate() == billingPeriodEnd.Date)
            {
                dayTimeProfiles = dayTimeProfiles
                    .Where(p => p.GetTime() <= billingPeriodEnd.TimeOfDay).ToList();
            }

            return dayTimeProfiles.OrderBy(p => ModelExtensions.GetDateTimeFromSpecialDayProfile(profile, p)).ToList();
        }

        /// <summary>
        /// This value is needed if there is a gap between 2 days.
        /// </summary>
        /// <param name="latestReading">The current latest reading</param>
        /// <param name="reading">The current reading with a possible new value for latestReading</param>
        /// <returns>The latest reading value</returns>
        public long GetLatestReading(long latestReading, IntervalReading reading)
        {
            var result = latestReading;

            if(reading != null && IsStatusValid(reading))
            {
                if (reading.Value.HasValue)
                {
                    result = reading.Value > latestReading ? (long)reading.Value : latestReading;
                }
            }
            
            return result;
        }

        /// <summary>
        /// Calculates the next valid range for the current day
        /// </summary>
        /// <param name="reading">The IntervalReading at the end of the new range.</param>
        /// <param name="endCurrentReading">The end date of the current range.</param>
        /// <param name="startReading">The IntervalReading at the start of the new range.</param>
        /// <param name="start">The start date of the current range.</param>
        /// <param name="end">The global end date of the current day.</param>
        /// <param name="dayTimeProfiles">The dayTimeProfiles for the current day.</param>
        /// <param name="meterReading">The meterReading object of the current original value list.</param>
        /// <param name="profile">The current SpecialDayProfile.</param>
        /// <param name="currentDay">The current AccountingDay object.</param>
        /// <param name="i">The current index value of the parent loop.</param>
        /// <param name="latestReading">The current meterReading value.</param>
        /// <param name="latestTariffId">The last valid tariff id.</param>
        /// <returns>The rangeData of the found range</returns>
        public (IntervalReading reading, MeasuringRange range, int index, long latestReading) GetNextRange(IntervalReading reading, 
            DateTime endCurrentReading, IntervalReading startReading, DateTime start, DateTime end, List<DayTimeProfile> dayTimeProfiles, 
            MeterReading meterReading, SpecialDayProfile profile, AccountingDay currentDay, int i, long latestReading, ushort latestTariffId)
        {
            var endReading = SetConcreteIntervalReading(reading, endCurrentReading);
            var range = new MeasuringRange();
           
            // Normal situation: reading is valid.
            if (endReading.reading != null && IsStatusValid(endReading.reading))
            {
                range = new MeasuringRange(start, endReading.end, (ushort)dayTimeProfiles[i - 1].TariffNumber, 
                    (endReading.reading.Value.Value - startReading.Value.Value));
            }
            else // A gap was found
            {
                // Looking for the next valid IntervalReading 
                var result = FindLastValidTime(start, end, profile, dayTimeProfiles, meterReading, i - 1);
                endReading = SetIntervalReading(meterReading, result.end, result.index, dayTimeProfiles.Count);

                if (result.end < end && endReading.reading != null) // An IntervalReading was found which is bigger than startReading but smaller than the detected gap.
                {
                    range = new MeasuringRange(start, endReading.end, (ushort)dayTimeProfiles[result.index].TariffNumber, 
                        (endReading.reading.Value.Value - startReading.Value.Value));
                }
                else
                {
                    // Count in error register
                    if (endReading.reading != null && IsStatusValid(endReading.reading))
                    {
                        range = new MeasuringRange(start, endReading.end,(endReading.reading.Value.Value - startReading.Value.Value));
                    }
                    else
                    {
                            // No closing valid IntervalReading for the current day was found (There was a gap at the last tariff change)
                            latestTariffId = 63;
                            endReading.reading = startReading; // No valid IntervalReading was found. Reset range to mark it as invalid.
                    }
                    
                }

                // 1 Measurement period at tariff change is missing
                if ((i - 1) == result.index)
                {
                 
                    latestReading = this.GetLatestReading(latestReading, endReading.reading);
                    latestTariffId = range.TariffId;

                    currentDay.Add(range, new ObisId(meterReading.ReadingType.ObisCode));
                    var j = result.index;
                    var nextDate = ModelExtensions.GetDateTimeFromSpecialDayProfile(profile, dayTimeProfiles[j + 1]);
                    var nextReading = SetIntervalReading(meterReading, nextDate, j + 1, dayTimeProfiles.Count);
                    while (nextReading.reading == null || !IsStatusValid(nextReading.reading))
                    {
                        j++;
                        if (j < dayTimeProfiles.Count - 1)
                        {
                            nextDate = ModelExtensions.GetDateTimeFromSpecialDayProfile(profile,
                                dayTimeProfiles[j + 1]);
                            nextReading = SetIntervalReading(meterReading, nextDate, j + 1, dayTimeProfiles.Count);
                        }
                        else
                        {   
                                nextDate = ModelExtensions.GetDateTimeFromSpecialDayProfile(profile,
                                    dayTimeProfiles[j]);
                                nextReading = SetIntervalReading(meterReading, nextDate, j, dayTimeProfiles.Count);
                            
                                if (nextReading.reading == null || !IsStatusValid(endReading.reading))
                                {
                                    nextReading = endReading;
                                    break;
                                }
                        }
                    }

                    // Count in error register
                    range = new MeasuringRange(endReading.end, nextReading.end, (nextReading.reading.Value.Value - endReading.reading.Value.Value));
                    endReading = nextReading;
                    result.index = j + 1;
                }

                // Protect for the special case i == dayTimeProfiles.Count -1 because it could lead to an endless loop.
                if (i != dayTimeProfiles.Count - 1) { i = result.index; }     
            }

            // Check if the range object is empty 
            if(this.IsRangeEmpty(range)) { range.TariffId = latestTariffId; }

            return (endReading.reading, range, i, latestReading);
        }

        /// <summary>
        /// Searches the next valid startReading (In case the first value or values of the day are gaps)
        /// </summary>
        /// <param name="start">the current timestamp.</param>
        /// <param name="dayTimeProfiles">The dayTimeProfiles for the current day.</param>
        /// <param name="profile">The specialDayProfile of the current day.</param>
        /// <param name="meterReading">The meterReading object of the current original value list.</param>
        /// <param name="currentDay">The current AccountingDay object.</param>
        /// <param name="dtpIndex">The current dayTimeProfiles index.</param>
        /// <param name="latestReading">The current meterReading value.</param>
        /// <param name="lastTariffId">The last valid tariff id.</param>
        /// <returns>The next valid start value.</returns>
        public (IntervalReading startReading, int index, long latestReading, ushort tariffId) GetStartReading(DateTime start, List<DayTimeProfile> dayTimeProfiles,
            SpecialDayProfile profile, MeterReading meterReading, AccountingDay currentDay, int dtpIndex, long latestReading, ushort lastTariffId)
        {
            var dayStart = start;
            var startReading = new IntervalReading();
            var index = dtpIndex;

            for (int i = 1; i < dayTimeProfiles.Count; i++)
            {
                start = ModelExtensions.GetDateTimeFromSpecialDayProfile(profile, dayTimeProfiles[i]);
                startReading = meterReading.GetIntervalReadingFromDate(start);

                // If the gap reaches the next tariff change 
                if (dayTimeProfiles[i].TariffNumber != lastTariffId && lastTariffId != 63)
                {
                    lastTariffId = 63;
                }

                // Checks if a startReading is found
                if (startReading != null && IsStatusValid(startReading))
                {
                    currentDay.Add(new MeasuringRange()
                    {
                        Start = dayStart,
                        End = start,
                        Amount = (startReading.Value.Value - latestReading),
                        TariffId = lastTariffId
                    }, new ObisId(meterReading.ReadingType.ObisCode));

                    index = i;
                    latestReading = GetLatestReading(latestReading, startReading);
                    lastTariffId = (ushort)dayTimeProfiles[i].TariffNumber;
                    break;
                }
                else 
                {
                    index = i;
                }
            }

            // if no startReading for the whole day was found, set index to 96 
            if((startReading == null || !IsStatusValid(startReading)) && index == dayTimeProfiles.Count - 1)
            {
                var nextDay0OClock = start.AddSeconds(900);
                startReading = meterReading.GetIntervalReadingFromDate(nextDay0OClock);

                // Check if a value for the next day at 0:00 o Clock exists
                if (startReading != null && IsStatusValid(startReading))
                {
                    currentDay.Add(new MeasuringRange()
                    {
                        Start = dayStart,
                        End = nextDay0OClock,
                        Amount = startReading.Value.Value - latestReading,
                        TariffId = lastTariffId
                    }, new ObisId(meterReading.ReadingType.ObisCode));
                }
                index = dayTimeProfiles.Count;
            }

            return (startReading, index, latestReading, lastTariffId);
        }

        /// <summary>
        /// Check if the MeasuringRange object is empty
        /// </summary>
        /// <param name="range">The object to be checked.</param>
        /// <returns>True if the MeasuringRange object is empty.</returns>
        public bool IsRangeEmpty(MeasuringRange range)
        {
            if(range.Start == range.End && range.Amount == 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Looks for SpecialDayProfiles which have the right dayProfile and are between 
        /// the begin and end time.
        /// </summary>
        /// <param name="supplier">The supplier object which contains the SpecialDayProfiles.</param>
        /// <param name="dayProfiles">The valid dayProfiles for the current day.</param>
        /// <param name="begin">The start timestamp.</param>
        /// <param name="end">The end timestamp.</param>
        /// <returns>A list of all valid SpecialDayProfiles.</returns>
        public List<SpecialDayProfile> GetSpecialDayProfiles(UsagePointLieferant supplier, List<ushort?> dayProfiles, DateTime begin, DateTime end)
        {
            // Needed if Begin or End are in the middle of a day
            var open = begin.Date;
            var close = end.Date;

            return supplier.AnalysisProfile.TariffChangeTrigger.TimeTrigger
                .SpecialDayProfiles
                .Where(s => dayProfiles.Contains(s.DayId) && s.SpecialDayDate.GetDate() >= open && s.SpecialDayDate.GetDate() <= close)
                .OrderBy(s => s.SpecialDayDate.GetDate()).ToList();
        }

        /// <summary>
        /// Check if the validDayProfiles list and the specialDayProfiles list are null or empty.
        /// </summary>
        /// <param name="validDayProfiles">The list which should contain the current validDayProfiles.</param>
        /// <param name="specialDayProfiles">The list which should contain the current specialDayProfiles.</param>
        /// <returns>True if both lists are not null and not empty</returns>
        public bool CheckUsedLists(List<ushort?> validDayProfiles, List<SpecialDayProfile> specialDayProfiles)
        {
            if (validDayProfiles == null || validDayProfiles.Count < 1)
            {
                return false;
            }

            if (specialDayProfiles == null || specialDayProfiles.Count < 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check if the dayTimeProfiles list are null or empty.
        /// </summary>
        /// <param name="dayTimeProfiles"></param>
        public void CheckInitSettings(List<DayTimeProfile> dayTimeProfiles)
        {
            if (dayTimeProfiles == null || dayTimeProfiles.Count < 1)
            {
                throw new InvalidOperationException("Das aktuelle Tagesprofil enthält keine Informationen zum Tarifwechsel.");
            }
        }

        /// <summary>
        /// Adds the corresponding reading type to the specified registers.
        /// </summary>
        /// <param name="registers">The registers to add the reading type.</param>
        private void UpdateReadingTypeFromOriginalValueList(List<Register> registers)
        {
            foreach (var register in registers)
            {
                var ovl = this.originalValueLists.FirstOrDefault(
                    o => o.Obis.A == register.ObisCode.A && o.Obis.B == register.ObisCode.B
                         && o.Obis.C == register.ObisCode.C && o.Obis.D == register.ObisCode.D && o.Obis.E == 0
                         && o.Obis.F == register.ObisCode.F);

                if (ovl != null)
                {
                    register.SourceType = ovl.MeterReading.ReadingType;
                }
            }
        }

        /// <summary>
        /// Check if the IntervalReading instance has an valid status
        /// </summary>
        /// <param name="reading">The IntervalReading object to check.</param>
        /// <returns>True if the status is valid.</returns>
        private bool IsStatusValid(IntervalReading reading)
        {
            if (reading != null)
            {
                var fnnStatusToPtbStatus = reading.StatusFNN?.MapToStatusPtb();
                var ptbStatus = reading.StatusPTB;

                return fnnStatusToPtbStatus != StatusPTB.CriticalTemporaryError && 
                       fnnStatusToPtbStatus != StatusPTB.FatalError && 
                       ptbStatus != StatusPTB.CriticalTemporaryError && 
                       ptbStatus != StatusPTB.FatalError;
            }

            return false;
        }
    }
}