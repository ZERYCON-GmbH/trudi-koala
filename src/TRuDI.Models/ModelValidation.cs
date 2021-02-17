namespace TRuDI.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TRuDI.HanAdapter.Interface;
    using TRuDI.Models.BasicData;
    using TRuDI.Models.CheckData;

    using AnalysisProfile = TRuDI.Models.CheckData.AnalysisProfile;

    // In this class are all methods for the post-xml-schema-validation
    public static class ModelValidation
    {

        // The public method for the validation of the  han adapter model
        public static UsagePointAdapterTRuDI ValidateHanAdapterModel(UsagePointAdapterTRuDI usagePoint)
        {
            var exceptions = new List<Exception>();

            CommonModelValidation(usagePoint, exceptions);

            if (usagePoint.Customer == null)
            {
                exceptions.Add(new InvalidOperationException("Das Element \"UsagePoint\" muss das Element \"Customer\" enthalten."));
            }

            if (usagePoint.Certificates.Count < 1)
            {
                exceptions.Add(new InvalidOperationException("Das Element \"UsagePoint\" muss das Element \"Certificate\" enthalten."));
            }
            else
            {
                foreach (var cert in usagePoint.Certificates)
                {
                    ValidateCertificate(cert, exceptions);
                }
            }

            foreach (var meterReading in usagePoint.MeterReadings)
            {
                ValidateMeterReading(meterReading, exceptions);
            }

            if (usagePoint.LogEntries.Count >= 1)
            {
                foreach (var logEntry in usagePoint.LogEntries)
                {
                    ValidateLogEntry(logEntry, exceptions);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException("Han Adapter Model error:>", exceptions);
            }

            return usagePoint;
        }

        // The public method for the validation of the supplier model
        public static UsagePointLieferant ValidateSupplierModel(UsagePointLieferant usagePoint)
        {
            var exceptions = new List<Exception>();

            CommonModelValidation(usagePoint, exceptions);

            if (usagePoint.Certificates.Count >= 1)
            {
                foreach (Certificate cert in usagePoint.Certificates)
                {
                    ValidateCertificate(cert, exceptions);
                }
            }

            ValidateAnalysisProfile(usagePoint.AnalysisProfile, exceptions);
            ValidateTaf7SupplierDayProfiles(usagePoint, exceptions);

            if (exceptions.Any())
            {
                throw new AggregateException("Supplier Model error:>", exceptions);
            }

            return usagePoint;

        }

        private static void CommonModelValidation(UsagePoint usagePoint, List<Exception> exceptions)
        {
            if (string.IsNullOrWhiteSpace(usagePoint.UsagePointId))
            {
                exceptions.Add(new InvalidOperationException("Das Element \"UsagePointId\" hat keinen gültigen Wert."));
            }

            if (string.IsNullOrWhiteSpace(usagePoint.TariffName))
            {
                exceptions.Add(new InvalidOperationException("Das Element \"TariffName\" hat keinen gültigen Wert."));
            }

            if (usagePoint.InvoicingParty == null)
            {
                exceptions.Add(new InvalidOperationException("Das Element \"UsagePoint\" muss das Element \"InvoicingParty\" enthalten."));
            }

            if (usagePoint.Smgw == null)
            {
                exceptions.Add(new InvalidOperationException("Das Element \"UsagePoint\" muss das Element \"SMGW\" enthalten."));
            }
            else if (usagePoint.GetType() == typeof(UsagePointAdapterTRuDI))
            {
                ValidateSMGW(usagePoint.Smgw, exceptions);
            }

            if (usagePoint.ServiceCategory == null)
            {
                exceptions.Add(new InvalidOperationException("Das Element \"UsagePoint\" muss das Element \"ServiceCategory\" enthalten."));
            }
            else
            {
                ValidateServiceCategoryKind(usagePoint.ServiceCategory.Kind, exceptions);
            }

        }

        // Validation of the Certificate instance
        private static void ValidateCertificate(Certificate cert, List<Exception> exceptions)
        {
            if (cert.CertType.HasValue)
            {
                ValidateCertificateCertType(cert.CertType, exceptions);
            }
            else
            {
                exceptions.Add(new InvalidOperationException("Das Element \"CertType\" ist nicht angegeben oder enthält keinen Wert."));
            }

            if (cert.CertContent == null)
            {
                exceptions.Add(new InvalidOperationException("Es wurden keine Zertifikatsdaten gefunden."));
            }
        }

        // Validate if the Certification Type is a valid type
        private static void ValidateCertificateCertType(CertType? type, List<Exception> exceptions)
        {
            switch (type)
            {
                case CertType.SmgwHan:
                case CertType.Signatur:
                case CertType.SubCA:
                    break;

                default:
                    exceptions.Add(new InvalidOperationException($"Ungültiger Wert für das Element \"CerType\": \"{type}\"."));
                    break;
            }
        }

        // Validation of the SMGW instance
        private static void ValidateSMGW(SMGW smgw, List<Exception> exceptions)
        {
            if (string.IsNullOrWhiteSpace(smgw.SmgwId))
            {
                exceptions.Add(new InvalidOperationException("Die Smart Meter Gateway ID enthält keinen gültigen Wert."));
            }

            if (smgw.CertIds.Count < 1)
            {
                exceptions.Add(new InvalidOperationException("Das Element \"SMGW\" enthält keine Zertifikat-IDs (\"certIds\")."));
            }
        }

        // Validate if the kind of the ServiceCategory is valid
        private static void ValidateServiceCategoryKind(Kind? kind, List<Exception> exceptions)
        {
            switch (kind)
            {
                case Kind.Cold:
                case Kind.Communication:
                case Kind.Electricity:
                case Kind.Gas:
                case Kind.Heat:
                case Kind.Pressure:
                case Kind.Time:
                case Kind.Water:
                    break;

                default:
                    exceptions.Add(new InvalidOperationException($"Ungültiger Wert für \"ServiceCategory/Kind\": \"{kind}\"."));
                    break;
            }
        }

        // Validation of an MeterReading instance
        private static void ValidateMeterReading(MeterReading meterReading, List<Exception> exceptions)
        {
            if (string.IsNullOrWhiteSpace(meterReading.MeterReadingId))
            {
                exceptions.Add(new InvalidOperationException("Das Element \"MeterReadingId\" enthält keinen gültigen Wert."));
            }

            if (meterReading.ReadingType == null)
            {
                exceptions.Add(new InvalidOperationException("Das Element \"MeterReading\" muss das Element \"ReadingType\" enthalten."));
            }
            else
            {
                ValidateReadingType(meterReading.ReadingType, exceptions);
            }

            if (meterReading.Meters.Count < 1)
            {
                exceptions.Add(new InvalidOperationException("Das Element \"MeterReading\" muss das Elemnt \"Meter\" enthalten."));
            }

            if (meterReading.IntervalBlocks.Count < 1)
            {
            }
            else
            {
                foreach (IntervalBlock intervalBlock in meterReading.IntervalBlocks)
                {
                    ValidateIntervalBlock(intervalBlock, exceptions);
                }
            }
        }

        // Validation of an LogEntry instance
        private static void ValidateLogEntry(LogEntry entry, List<Exception> exceptions)
        {
            if (entry.LogEvent != null)
            {
                ValidateLogEvent(entry.LogEvent, exceptions);
            }
        }

        // Validation of an LogEvent instance
        private static void ValidateLogEvent(LogEvent logEvent, List<Exception> exceptions)
        {
            ValidateLogEventLevel(logEvent.Level, exceptions);
            ValidateLogEventOutcome(logEvent.Outcome, exceptions);
        }

        // Validate if the level of the LogEvent is valid
        private static void ValidateLogEventLevel(Level? level, List<Exception> exceptions)
        {
            switch (level)
            {
                case Level.INFO:
                case Level.WARNING:
                case Level.ERROR:
                case Level.FATAL:
                case Level.EXTENSION:
                    break;

                default:
                    exceptions.Add(new InvalidOperationException($"Ungültiger Wert für \"LogEvent/Level\": \"{level}\"."));
                    break;
            }
        }

        // Validate if the outcome of the LogEvent is valid
        private static void ValidateLogEventOutcome(Outcome? outcome, List<Exception> exceptions)
        {
            switch (outcome)
            {
                case null:
                case Outcome.SUCCESS:
                case Outcome.FAILURE:
                case Outcome.EXTENSION:
                    break;

                default:
                    exceptions.Add(new InvalidOperationException($"Ungültiger Wert für \"LogEvent/Outcome\": \"{outcome}\"."));
                    break;
            }
        }

        // Validation of a ReadingType instance
        private static void ValidateReadingType(ReadingType readingType, List<Exception> exceptions)
        {
            if (string.IsNullOrWhiteSpace(readingType.ObisCode))
            {
                exceptions.Add(new InvalidOperationException("Das Element \"ObisCode\" innerhalb des Elements \"ReadingType\" enthält keinen gültigen Wert."));
            }
            else
            {
                if (!readingType.ObisCode.ValidateHexString())
                {
                    exceptions.Add(new InvalidOperationException("Das Element \"ObisCode\" innerhalb des Elements \"ReadingType\" enthält keinen gültigen Wert."));
                }
            }

            if (string.IsNullOrEmpty(readingType.QualifiedLogicalName))
            {
                exceptions.Add(new InvalidOperationException("Das Element \"QualifiedLogicalName\" innerhalb des Elements \"ReadingType\" enthält keinen gültigen Wert."));
            }

            if (readingType.PowerOfTenMultiplier.HasValue)
            {
                ValidateReadingTypePowerOfTenMultiplier(readingType.PowerOfTenMultiplier, exceptions);
            }
            else
            {
                exceptions.Add(new InvalidOperationException("Das Element \"PowerOfTenMultiplier\" innerhalb des Elements \"ReadingType\" enthält keinen gültigen Wert."));
            }

            if (readingType.Uom.HasValue)
            {
                ValidateReadingTypeUom(readingType.Uom, exceptions);
            }
            else
            {
                exceptions.Add(new InvalidOperationException("Das Element \"Uom\" innerhalb des Elements \"ReadingType\" enthält keinen gültigen Wert."));
            }
        }

        // Validate if the powerOfTenMultiplier of the ReadingType is valid
        private static void ValidateReadingTypePowerOfTenMultiplier(PowerOfTenMultiplier? powerOfTenMultiplier, List<Exception> exceptions)
        {
            switch (powerOfTenMultiplier)
            {
                case PowerOfTenMultiplier.deca:
                case PowerOfTenMultiplier.Giga:
                case PowerOfTenMultiplier.hecto:
                case PowerOfTenMultiplier.kilo:
                case PowerOfTenMultiplier.Mega:
                case PowerOfTenMultiplier.micro:
                case PowerOfTenMultiplier.mili:
                case PowerOfTenMultiplier.None:
                    break;

                default:
                    exceptions.Add(new InvalidOperationException($"Ungültiger Wert für \"ReadingType/powerOfTenMultiplier\": \"{powerOfTenMultiplier}\"."));
                    break;
            }
        }

        // Validate if the uom of the ReadingType is valid
        private static void ValidateReadingTypeUom(Uom? uom, List<Exception> exceptions)
        {
            switch (uom)
            {
                case Uom.Ampere:
                case Uom.Ampere_hours:
                case Uom.Ampere_squared:
                case Uom.Apparent_energy:
                case Uom.Apparent_power:
                case Uom.Cubic_feet:
                case Uom.Cubic_feet_per_hour:
                case Uom.Cubic_meter:
                case Uom.Cubic_meter_per_hour:
                case Uom.Frequency:
                case Uom.Joule:
                case Uom.Not_Applicable:
                case Uom.Power_factor:
                case Uom.Reactive_energie:
                case Uom.Reactive_power:
                case Uom.Real_energy:
                case Uom.Real_power:
                case Uom.US_Gallons:
                case Uom.US_Gallons_per_hour:
                case Uom.Volltage:
                case Uom.Volts_squared:
                case Uom.AngleDegrees:
                    break;

                default:
                    exceptions.Add(new InvalidOperationException($"Ungültiger Wert für \"ReadingType/Uom\": \"{uom}\"."));
                    break;
            }
        }

        // Validation of an IntervalBlock instance
        private static void ValidateIntervalBlock(IntervalBlock intervalBlock, List<Exception> exceptions)
        {
            if (intervalBlock.Interval == null)
            {
                exceptions.Add(new InvalidOperationException("Das Element \"IntervalBlock\" muss das Element \"Interval\" enthalten."));
            }
            else
            {
                ValidateInterval(intervalBlock.Interval, "interval", exceptions);
            }

            if (intervalBlock.IntervalReadings.Count < 1)
            {
                exceptions.Add(new InvalidOperationException("Das Element \"IntervalBlock\" enthält kein Element \"IntervalReading\"."));
            }
            else
            {
                foreach (IntervalReading intervalReading in intervalBlock.IntervalReadings)
                {
                    ValidateIntervalReading(intervalReading, exceptions);
                }
            }
        }

        // Validation of an Interval instance
        private static bool ValidateInterval(Interval interval, string name, List<Exception> exceptions)
        {
            if (!interval.Duration.HasValue)
            {
                exceptions.Add(new InvalidOperationException($"Das Element \"Duration\" innerhalb von \"{name}\" enthält keinen gültigen Wert."));
                return false;
            }

            if (interval.Start == DateTime.MinValue)
            {
                exceptions.Add(new InvalidOperationException($"Das Element \"Start\" innerhalb von \"{name}\" enthält keinen gültigen Wert."));
                return false;
            }

            return true;
        }

        // Validation of an IntervalReading instance
        private static void ValidateIntervalReading(IntervalReading intervalReading, List<Exception> exceptions)
        {
            if (intervalReading.TimePeriod == null)
            {
                exceptions.Add(new InvalidOperationException("Das Element \"IntervalReading\" muss das Element \"TimePeriod\" enthalten."));
            }
            else
            {
                ValidateInterval(intervalReading.TimePeriod, "timePeriod", exceptions);
            }

            if (!intervalReading.Value.HasValue)
            {
                exceptions.Add(new InvalidOperationException("Das Element \"value\" enthält keinen gültigen Wert."));
            }

            ValidateSetStatus(intervalReading, exceptions);
        }

        private static void ValidateSetStatus(IntervalReading intervalReading, List<Exception> exceptions)
        {

            if (intervalReading.StatusFNN == null && !intervalReading.StatusPTB.HasValue)
            {
                exceptions.Add(new InvalidOperationException("Das Element \"IntervalReading\" muss mindestens \"StatusFNN\" oder \"StatusPTB\" mit einem gültigen Wert enthalten."));
                return;
            }

            if (intervalReading.StatusFNN != null && !intervalReading.StatusPTB.HasValue)
            {
                if (string.IsNullOrWhiteSpace(intervalReading.StatusFNN.Status))
                {
                    exceptions.Add(new InvalidOperationException("Das Element \"IntervalReading\" muss mindestens \"StatusFNN\" oder \"StatusPTB\" mit einem gültigen Wert enthalten."));
                }
                else
                {
                    if (!intervalReading.StatusFNN.ValidateFNNStatus())
                    {
                        exceptions.Add(new InvalidOperationException("Das Element \"StatusFNN\" hat keinen gültigen Wert."));
                    }
                }
            }
            else if (intervalReading.StatusFNN == null && intervalReading.StatusPTB.HasValue)
            {
                ValidateIntervalReadingStatusPTB(intervalReading.StatusPTB, exceptions);
            }
        }

        // Validate if the StatusPTB of ReadingType is valid
        private static void ValidateIntervalReadingStatusPTB(StatusPTB? statusPtb, List<Exception> exceptions)
        {
            switch (statusPtb)
            {
                case StatusPTB.FatalError:
                case StatusPTB.NoError:
                case StatusPTB.CriticalTemporaryError:
                case StatusPTB.TemporaryError:
                case StatusPTB.Warning:
                    break;

                default:
                    exceptions.Add(new InvalidOperationException($"Ungültiger Wert für \"StatusPTB\": \"{statusPtb}\"."));
                    break;
            }
        }

        // Validation of an AnalysisProfile instance
        private static void ValidateAnalysisProfile(AnalysisProfile analysisProfile, List<Exception> exceptions)
        {
            if (analysisProfile.TariffStages.Count < 1)
            {
                exceptions.Add(new InvalidOperationException("Das Element \"AnalysisProfile\" muss das Element \"TariffStage\" enthalten."));
            }
            else
            {
                foreach (var tariffStage in analysisProfile.TariffStages)
                {
                    ValidateTariffStage(tariffStage, exceptions);
                }
            }

            if (analysisProfile.TariffChangeTrigger == null)
            {
                exceptions.Add(new InvalidOperationException("Das Element \"AnalysisProfile\" muss das Element \"TariffChangeTrigger\" enthalten."));
            }
            else
            {
                ValidateTariffChangeTrigger(analysisProfile.TariffChangeTrigger, exceptions);
            }

            if (ValidateInterval(analysisProfile.BillingPeriod, "Billing Period", exceptions))
            {
                // Validate if we have at least one full day in the BillingPeriod
                if ((analysisProfile.BillingPeriod.GetEnd().Date - analysisProfile.BillingPeriod.Start.Date).Days < 1)
                {
                    exceptions.Add(new InvalidOperationException("Die Abrechnungsperiode in der Tarifdatei muss mindestens einen vollen Tag umfassen."));
                }
            }

            ValidateAnalysisProfileTariffUseCase(analysisProfile.TariffUseCase, exceptions);
        }

        // Taf-7: Validate if the supplier periods have a duration of 15 minutes
        private static void ValidateTaf7SupplierDayProfiles(UsagePointLieferant supplier, List<Exception> exceptions)
        {
            var profiles = supplier.AnalysisProfile.TariffChangeTrigger.TimeTrigger.DayProfiles;

            foreach (DayProfile profile in profiles)
            {
                var dtProfiles = profile.DayTimeProfiles;
                for (int i = 0; i < dtProfiles.Count; i++)
                {
                    if (i + 1 == dtProfiles.Count)
                    {
                        break;
                    }

                    var current = new TimeSpan((int)dtProfiles[i].StartTime.Hour, (int)dtProfiles[i].StartTime.Minute, 0);
                    var next = new TimeSpan((int)dtProfiles[i + 1].StartTime.Hour, (int)dtProfiles[i + 1].StartTime.Minute, 0);

                    if ((int)(next - current).TotalSeconds == 900)
                    {
                        continue;
                    }

                    exceptions.Add(new InvalidOperationException($"TAF-7: Die Tarifschaltzeiten für Tagesprofil {profile.DayId} in der Tarifdatei des Lieferanten sind nicht für jede 15-Minuten-Messperiode angegeben: {current} zu {next}"));
                }
            }
        }

        // Validate if the TariffUseCase of AnalysisProfile is valid
        private static void ValidateAnalysisProfileTariffUseCase(TafId? tariffUseCase, List<Exception> exceptions)
        {
            switch (tariffUseCase)
            {
                case TafId.Taf1:
                    break;
                case TafId.Taf2:
                    break;
                case TafId.Taf6:
                    break;
                case TafId.Taf7:
                    break;

                case TafId.Taf9:
                default:
                    exceptions.Add(new InvalidOperationException($"Ungültiger Wert für \"TariffUseCase\": \"{tariffUseCase}\"."));
                    break;
            }
        }

        // Validation of an TariffStage instance
        private static void ValidateTariffStage(TariffStage tariffStage, List<Exception> exceptions)
        {
            if (!tariffStage.ObisCode.ValidateHexString())
            {
                exceptions.Add(new FormatException("Das Element \"ObisCode\" innerhalb des Elements \"TariffStage\" enthält einen ungülten Wert."));
            }
        }

        // Validation of an TariffChangeTrigger instance
        private static void ValidateTariffChangeTrigger(TariffChangeTrigger trigger, List<Exception> exceptions)
        {
            if (trigger.TimeTrigger == null)
            {
                exceptions.Add(new InvalidOperationException("Das Element \"TariffChangeTrigger\" muss ein Element \"TimeTrigger\" enthalten."));
            }
            else
            {
                ValidateTimeTrigger(trigger.TimeTrigger, exceptions);
            }
        }

        // Validation of an TimeTrigger instance
        private static void ValidateTimeTrigger(TimeTrigger trigger, List<Exception> exceptions)
        {
            if (trigger.DayProfiles.Count < 1)
            {
                exceptions.Add(new InvalidOperationException("Das Element \"TimeTrigger\" muss ein Element \"DayProfile\" enthalten."));
            }
            else
            {
                foreach (DayProfile profile in trigger.DayProfiles)
                {
                    ValidateDayProfile(profile, exceptions);
                }
            }

            if (trigger.SpecialDayProfiles.Count < 1)
            {
                exceptions.Add(new InvalidOperationException("Das Element \"TimeTrigger\" muss ein Element \"SpecialDayProfile\" enthalten."));
            }
            else
            {
                foreach (SpecialDayProfile profile in trigger.SpecialDayProfiles)
                {
                    ValidateSpecialDayProfile(profile, exceptions);
                }
            }

        }

        // Validation of an DayProfile instance
        private static void ValidateDayProfile(DayProfile dayProfile, List<Exception> exceptions)
        {
            if (dayProfile.DayTimeProfiles.Count < 1)
            {
                exceptions.Add(new InvalidOperationException("Das Element \"DayProfile\" muss ein Element \"DayTimeProfile\" enthalten."));
            }
            else
            {
                foreach (DayTimeProfile profile in dayProfile.DayTimeProfiles)
                {
                    ValidateDayTimeProfile(profile, exceptions);
                }
            }
        }

        // Validation of an SpecialDayProfile instance
        private static void ValidateSpecialDayProfile(SpecialDayProfile profile, List<Exception> exceptions)
        {
            if (profile.DayProfile == null)
            {
                exceptions.Add(new InvalidOperationException("Das Element \"SpecialDayProfile\" muss ein Element \"DayProfile\" enthalten."));
            }

            ValidateDayVarType(profile.SpecialDayDate, exceptions);
        }

        // Validation of an DayTimeProfile instance
        private static void ValidateDayTimeProfile(DayTimeProfile profile, List<Exception> exceptions)
        {
            if (profile.StartTime == null)
            {
                exceptions.Add(new InvalidOperationException("Das Element \"DayTimeProfile\" muss ein Element \"StartTime\" enthalten."));
            }
        }

        // Validation of an DayVarType instance
        private static void ValidateDayVarType(DayVarType day, List<Exception> exceptions)
        {
            if (!day.DayOfMonth.HasValue)
            {
                exceptions.Add(new InvalidOperationException("Das Element \"SpecialDayDate\" ist ungültig."));
            }

            if (!day.Month.HasValue)
            {
                day.Monthly = true;
            }

            if (!day.Year.HasValue)
            {
                day.Yearly = true;
            }
        }
    }
}
