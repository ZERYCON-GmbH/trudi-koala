namespace TRuDI.TafAdapter.Taf1
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
    /// Default Taf-1 implementation.
    /// </summary>
    public class TafAdapterTaf1 : ITafAdapter
    {
        private List<OriginalValueList> originalValueLists;
        private DateTime billingPeriodStart;
        private DateTime billingPeriodEnd;

        /// <inheritdoc />
        /// <summary>
        /// Calculates the derived register for Taf1.
        /// </summary>
        /// <param name="device">Date from the SMGW. There should be just original value lists.</param>
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

            this.ValidateOriginalValueLists(originalValueLists, supplier, device.MeterReadings.Count);

            var registers = supplier.GetRegister();
            this.UpdateReadingTypeFromOriginalValueList(registers);

            var accountingPeriod = new Taf1Data(registers, supplier.AnalysisProfile.TariffStages);
            accountingPeriod.SetDate(supplier.AnalysisProfile.BillingPeriod.Start, supplier.AnalysisProfile.BillingPeriod.GetEnd());
            this.SetTotalBillingPeriod(accountingPeriod);

            ValidateBillingPeriod();

            var dayProfiles = supplier.AnalysisProfile.TariffChangeTrigger.TimeTrigger.DayProfiles;

            foreach (OriginalValueList ovl in originalValueLists)
            {
                var startReading = ovl.MeterReading.GetIntervalReadingFromDate(billingPeriodStart);
                var endReading = ovl.MeterReading.GetIntervalReadingFromDate(billingPeriodEnd);

                if (startReading == null || !IsStatusValid(startReading))
                {
                    throw new InvalidOperationException($"Zu dem Zeitpunkt {billingPeriodStart} ist kein Wert vorhanden oder der Status kritisch oder fatal.");
                }

                if (endReading == null || !IsStatusValid(endReading))
                {
                    throw new InvalidOperationException($"Zu dem Zeitpunkt {billingPeriodEnd} ist kein Wert vorhanden oder der Status kritisch oder fatal.");
                }

                var dayProfile = this.GetDayProfileNumber(dayProfiles, new ObisId(ovl.MeterReading.ReadingType.ObisCode),
                    supplier.AnalysisProfile);

                var tariffStages = supplier.AnalysisProfile.TariffStages;
                var tariffId = this.GetTariffId(tariffStages, dayProfiles, dayProfile);

                CheckValidSupplierFile(supplier, dayProfile, tariffId);

                var result = this.GetSection(supplier, ovl.MeterReading, startReading, endReading, tariffId);

                accountingPeriod.Add(result);

                accountingPeriod.AddInitialReading(new Reading()
                {
                    Amount = accountingPeriod.AccountingSections.First(s => s.Reading.ObisCode == ovl.MeterReading.ReadingType.ObisCode).Reading.Amount,
                    ObisCode = new ObisId(ovl.MeterReading.ReadingType.ObisCode)
                });

            }

            return new TafAdapterData(typeof(Taf2SummaryView), typeof(Taf2DetailView), accountingPeriod);
        }

        /// <summary>
        /// The main calculation method for the accounting period in Taf-1.
        /// </summary>
        /// <param name="supplier">Contains the calculation data.</param>
        /// <param name="meterReading">The MeterReading instance with the raw data.</param>
        /// <param name="startReading">The intervalReading at the beginning of the section.</param>
        /// <param name="endReading">The intervalReading at the end of the section.</param>
        /// <param name="tariffId">The valid tariffId.</param>
        /// <returns>The calculated AccountingSection</returns>
        public AccountingMonth GetSection(UsagePointLieferant supplier, MeterReading meterReading, IntervalReading startReading, IntervalReading endReading, ushort tariffId)
        {
            var registers = supplier.GetRegister();
            this.UpdateReadingTypeFromOriginalValueList(registers);

            var section = new AccountingMonth(registers)
            {
                Reading = new Reading() { Amount = startReading.Value, ObisCode = new ObisId(meterReading.ReadingType.ObisCode) }
            };

            var start = startReading.TargetTime.Value;
            var end = endReading.TargetTime.Value;
            long amount = (long)(endReading.Value - startReading.Value);

            var range = new MeasuringRange(start, end, tariffId, amount);

            section.Add(range);
            section.Start = start;

            return section;
        }

        /// <summary>
        /// Check if the count of the original value lists are valid and if all meterReadings are original value lists.
        /// The TariffStages count is also checked. The max is one TariffStage per original value list.
        /// </summary>
        /// <param name="originalValueList">The list with all original value lists.</param>
        /// <param name="supplier">raw data from the supplier.</param>
        public void ValidateOriginalValueLists(List<OriginalValueList> originalValueLists, UsagePointLieferant supplier, int meterReadingsCount)
        {
            if (originalValueLists.Count > 3)
            {
                throw new InvalidOperationException("Es werden maximal 3 originäre Messwertlisten unterstützt.");
            }

            if (originalValueLists.Count != meterReadingsCount)
            {
                throw new InvalidOperationException("Es sind nur originäre Messwertlisten zulässig.");
            }


            if (supplier.AnalysisProfile.TariffStages.Count > originalValueLists.Count)
            {
                throw new InvalidOperationException("Die Anzahl der Tarifstufen darf die Anzahl der originären Messwertlisten nicht überschreiten.");
            }
        }

        /// <summary>
        /// The method checks if a specialDayProfile references to another dayId. (In Taf1 just one dayId is allowed (No different tariff stages))
        /// </summary>
        /// <param name="specialDayProfiles">The SpecialDayProfile instances which are checked.</param>
        /// <param name="dayId">The valid dayId.</param>
        /// <returns>True of no other dayId was found.</returns>
        public bool CheckDayIdInPeriod(List<SpecialDayProfile> specialDayProfiles, ushort dayId)
        {
            foreach (SpecialDayProfile profile in specialDayProfiles)
            {
                if (profile.DayId != dayId)
                {
                    return false;
                }
            }

            return true;
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
        /// Set billingPeriodStart and -End. accountingPeriod.Begin and accountingPeriod.End can not be null 
        /// due to the validation process. There must be a billing period.
        /// </summary>
        /// <param name="accountingPeriod"></param>
        public void SetTotalBillingPeriod(Taf1Data accountingPeriod)
        {
            billingPeriodStart = accountingPeriod.Begin;
            billingPeriodEnd = accountingPeriod.End;
        }

        /// <summary>
        /// Check if the given billing period is within supported time spans and whether the start date is the beginning of a month.
        /// </summary>
        public void ValidateBillingPeriod()
        {
            if (billingPeriodStart.Day != 1)
            {
                throw new InvalidOperationException($"Die Abrechnungsperiode {billingPeriodStart} startet nicht am Monatsanfang.");
            }

            if (billingPeriodStart.AddMonths(1) == billingPeriodEnd) { return; }
            else if (billingPeriodStart.AddMonths(2) == billingPeriodEnd) { return; }
            else if (billingPeriodStart.AddMonths(3) == billingPeriodEnd) { return; }
            else if (billingPeriodStart.AddMonths(6) == billingPeriodEnd) { return; }
            else if (billingPeriodStart.AddYears(1) == billingPeriodEnd) { return; }
            else
            {
                throw new InvalidOperationException($"Die angegebene Abrechnungsperiode von {(billingPeriodEnd - billingPeriodStart).TotalDays} Tagen ist ungültigt. Unterstütz werden 1, 2, 3, 6 oder 12 Monate.");
            }
        }

        /// <summary>
        /// For each original value list exactly one dayProfile number is allowed. 
        /// </summary>
        /// <param name="dayProfiles">The list of DayProfiles.</param>
        /// <param name="obisId">The corresponding obisId.</param>
        /// <param name="analysisProfile">The AnalysisProfile which contain the needed tariff stages.</param>
        /// <returns></returns>
        public ushort GetDayProfileNumber(List<DayProfile> dayProfiles, ObisId obisId, AnalysisProfile analysisProfile)
        {
            var profileList = dayProfiles.GetValidDayProfilesForMeterReading(obisId, analysisProfile.TariffStages);

            if (profileList == null || profileList.Count != 1)
            {
                throw new InvalidOperationException($"Es sind {profileList?.Count} Tagesprofile vorhanden. Es ist genau 1 Tagesprofil erlaubt.");
            }

            return (ushort)profileList.First();
        }

        /// <summary>
        /// This Method delivers the tariff id corresponding to the dayProfile.
        /// </summary>
        /// <param name="tariffStages">The list of tariff stages to look at.</param>
        /// <param name="dayProfiles">A dayProfiles list.</param>
        /// <param name="dayProfile">The current dayProfile number. Default value is 1.</param>
        /// <returns>The tariff id</returns>
        public ushort GetTariffId(List<TariffStage> tariffStages, List<DayProfile> dayProfiles, ushort dayProfile = 1)
        {
            var tariffId = tariffStages.First(t => t.TariffNumber == dayProfiles.First(dp => dp.DayId == dayProfile)
            .DayTimeProfiles.First().TariffNumber).TariffNumber;

            return tariffId;
        }

        /// <summary>
        /// Returns the SpecialDayProfiles which are needed. 
        /// </summary>
        /// <param name="supplier">The supplier object which contains the SpecialDayProfiles</param>
        /// <param name="dayProfile">The dayId</param>
        /// <returns>The SpecialDayProfiles corresponding to the dayProfile</returns>
        public List<SpecialDayProfile> GetSpecialDayProfiles(UsagePointLieferant supplier, ushort dayProfile)
        {
            var trigger = supplier.AnalysisProfile.TariffChangeTrigger.TimeTrigger;

            return trigger.SpecialDayProfiles.Where(s => s.DayId == dayProfile)
                .OrderBy(s => s.SpecialDayDate.GetDate()).ToList();
        }

        /// <summary>
        /// In Taf 1 a tariff change is not allowed. This will be checked in this method.
        /// </summary>
        /// <param name="supplier">The supplier object which is checked</param>
        /// <param name="dayProfile">The current day profile number</param>
        public void CheckValidSupplierFile(UsagePointLieferant supplier, ushort dayProfile, ushort tariff)
        {
            var profiles = this.GetSpecialDayProfiles(supplier, dayProfile);

            var days = (int)(billingPeriodEnd - billingPeriodStart).TotalDays;

            if (profiles.Count % days != 0)
            {
                throw new InvalidOperationException($"Die Anzahl der SpecialDayProfile Objekte muss einem vielfachen von {days} entsprechen.");
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
