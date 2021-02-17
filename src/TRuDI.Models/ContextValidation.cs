namespace TRuDI.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TRuDI.HanAdapter.Interface;
    using TRuDI.Models.BasicData;
    using TRuDI.Models.CheckData;

    public static class ContextValidation
    {
        // Validation of additional requirements
        public static void ValidateContext(UsagePointAdapterTRuDI usagePoint, AdapterContext ctx)
        {
            var exceptions = new List<Exception>();

            if (ctx != null)
            {
                ValidateMeterReadingCount(usagePoint, ctx, exceptions);
                ValidateMaximumTimeSpan(ctx, exceptions);

                if (ctx.Contract.TafId == TafId.Taf2)
                {
                    ValidateTaf2RegisterDurations(usagePoint, exceptions);
                }

                if (ctx.Contract.TafId == TafId.Taf7)
                {
                    ValidateTaf7MeterReadingsAreOriginalValueLists(usagePoint, exceptions);
                    ValidateTaf7PeriodsInInterval(usagePoint, exceptions);
                }
            }

            CompareCertIds(usagePoint, exceptions);
            ValidateCertificateRawData(usagePoint, exceptions);
            ValidateIntervalBlockConsistence(usagePoint, exceptions);

            if (exceptions.Any())
            {
                throw new AggregateException("Context error:>", exceptions);
            }
        }

        // Validation of additional requirements with additional supplier xml (only Taf-7)
        public static void ValidateContext(UsagePointAdapterTRuDI usagePoint, UsagePointLieferant supplierModel, AdapterContext ctx)
        {
            ValidateContext(usagePoint, ctx);

            var exceptions = new List<Exception>();

            ValidateTaf7ModelSupplierCompatibility(usagePoint, supplierModel, exceptions);
            ValidateSupplierModelTariffStageCount(supplierModel, exceptions);
            ValidateSpecialDayProfilesWithinBillingPeriod(supplierModel, exceptions);
            ValidateSupplierModelCompletelyEnrolledCalendar(usagePoint, supplierModel, exceptions);
            ValidateTarifStageOccurence(supplierModel, exceptions);
            ValidateSupplierModelDayProfileOccurence(supplierModel, exceptions);

            if (exceptions.Any())
            {
                throw new AggregateException("Taf-7 Context error:>", exceptions);
            }
        }

        // Comparision of the CertIds
        private static void CompareCertIds(UsagePointAdapterTRuDI usagePoint, List<Exception> exceptions)
        {
            var smgwCertIds = usagePoint.Smgw.CertIds;
            List<byte> certificateCertIds = new List<byte>();

            foreach (Certificate cert in usagePoint.Certificates)
            {
                certificateCertIds.Add((byte)cert.CertId);
            }

            if (smgwCertIds.Count == certificateCertIds.Count)
            {
                bool match = false;
                foreach (byte sId in smgwCertIds)
                {
                    foreach (byte cId in certificateCertIds)
                    {
                        if (sId == cId)
                        {
                            match = true;
                            break;
                        }
                    }

                    if (!match)
                    {
                        exceptions.Add(new InvalidOperationException($"Das Smart Meter Gateway-Zertifikat mit der ID {sId} konnte nicht gefunden werden."));
                    }
                }
            }
            else
            {
                exceptions.Add(new InvalidOperationException("Die Anzahl der \"CertIds\" im Element \"SMGW\" entspricht nicht der Nummer der Zertifikate."));
            }
        }

        // Check if the raw data of the certificates are valid certificates
        private static void ValidateCertificateRawData(UsagePointAdapterTRuDI usagePoint, List<Exception> exceptions)
        {
            foreach (Certificate certificate in usagePoint.Certificates)
            {
                try
                {
                    var cert = certificate.GetCert();
                }
                catch (Exception e)
                {
                    exceptions.Add(new InvalidOperationException($"Zertifikat konnte nicht gelesen werden: ID={certificate.CertId}, Typ={certificate.CertType}", e));
                }
            }
        }

        // Check if the count of MeterReadings are Valid due to the TAF
        private static void ValidateMeterReadingCount(UsagePointAdapterTRuDI usagePoint, AdapterContext ctx, List<Exception> exceptions)
        {
            var meterReadingCount = usagePoint.MeterReadings.Count;

            if (ctx.BillingPeriod != null && ctx.BillingPeriod.End.HasValue)
            {
                if (ctx.Contract.TafId == TafId.Taf1 && meterReadingCount < 2)
                {
                    exceptions.Add(new InvalidOperationException("TAF-1 benötigt mindestens 2 Elemente von Typ \"MeterReading\"."));
                }

                if ((ctx.Contract.TafId == TafId.Taf2) && meterReadingCount < 5)
                {
                    exceptions.Add(new InvalidOperationException("TAF-2 benötigt mindestens 5 Elemente vom Typ \"MeterReading\"."));
                }
            }
        }

        // Check of the meterReadingIds are unique
        private static void ValidateMeterReadingIds(UsagePointAdapterTRuDI usagePoint, List<Exception> exceptions)
        {
            for (int i = 0; i < usagePoint.MeterReadings.Count; i++)
            {
                var reading = usagePoint.MeterReadings[i];
                for (int j = i + 1; j < usagePoint.MeterReadings.Count; j++)
                {
                    if (reading.MeterReadingId == usagePoint.MeterReadings[j].MeterReadingId)
                    {
                        exceptions.Add(new InvalidOperationException("Das Element \"MeterReadingId\" enthält keine eindeutigen Werte."));
                    }
                }
            }
        }

        // Check if there is a gap between multiple IntervalBlocks or if the overlap
        private static void ValidateIntervalBlockConsistence(UsagePointAdapterTRuDI usagePoint, List<Exception> exceptions)
        {
            foreach (MeterReading reading in usagePoint.MeterReadings)
            {
                var intervalBlocks = reading.IntervalBlocks.OrderBy(ib => ib.Interval.Start).ToList();

                for (int i = 0; i < intervalBlocks.Count; i++)
                {
                    if (i < intervalBlocks.Count - 1)
                    {
                        if (intervalBlocks[i].Interval.GetEnd() < intervalBlocks[i + 1].Interval.Start)
                        {
                            exceptions.Add(new InvalidOperationException("Lücke zwischen zwei Intervallblöcken."));
                        }
                        else if (intervalBlocks[i].Interval.GetEnd() > intervalBlocks[i + 1].Interval.Start)
                        {
                            exceptions.Add(new InvalidOperationException("Es wurden zwei überlappende Intervallblöcke gefunden."));
                        }
                    }
                }

            }
        }

        // Validation of the maximum TimeSpan
        private static void ValidateMaximumTimeSpan(AdapterContext ctx, List<Exception> exceptions)
        {
            var queryStart = ctx.Start;
            var queryEnd = ctx.End;

            // 3 years have 94694400 seconds
            if (queryEnd - queryStart > TimeSpan.FromDays(1096))
            {
                exceptions.Add(new InvalidOperationException("Maximale Zeitspanne von 3 Jahren für eine Ablesung wurde überschritten."));
            }
        }

        // Taf-2: Validate if the durations in the different MeterReadings match
        private static void ValidateTaf2RegisterDurations(UsagePointAdapterTRuDI usagePoint, List<Exception> exceptions)
        {
            var interval = usagePoint.MeterReadings.FirstOrDefault(mr => !mr.IsOriginalValueList())?.GetMeterReadingInterval();
            if (interval == null)
            {
                return;
            }

            foreach (MeterReading reading in usagePoint.MeterReadings)
            {
                if (!reading.IsOriginalValueList())
                {
                    var intervalTest = reading.GetMeterReadingInterval();

                    if (interval.Duration != intervalTest.Duration)
                    {
                        exceptions.Add(new InvalidOperationException("TAF-2: Die Dauer der einzelnen Ablesungen stimmen nicht überein."));
                    }
                }
            }
        }

        // Taf-7: Validate if all meter readings are original value lists
        private static void ValidateTaf7MeterReadingsAreOriginalValueLists(UsagePointAdapterTRuDI model, List<Exception> exceptions)
        {
            foreach (MeterReading reading in model.MeterReadings)
            {
                if (!reading.IsOriginalValueList())
                {
                    exceptions.Add(new InvalidOperationException("TAF-7: The MeterReading is not an Original Value List."));
                }
            }
        }

        // Taf-7: Validate if all IntervalReadings are within the interval of the IntervalBlock
        private static void ValidateTaf7PeriodsInInterval(UsagePointAdapterTRuDI model, List<Exception> exceptions)
        {
            var originalValueLists = model.MeterReadings.Where(mr => mr.IsOriginalValueList());

            foreach (var reading in originalValueLists)
            {
                foreach (var ib in reading.IntervalBlocks)
                {
                    var intervalBlockEnd = ib.Interval.GetEnd();
                    foreach (var ir in ib.IntervalReadings)
                    {
                        if (ir.CaptureTime.ToUniversalTime() < ib.Interval.Start.ToUniversalTime())
                        {
                            exceptions.Add(new InvalidOperationException($"TAF-7: IntervalReading befindet sich nicht innerhalb des Zeitbereichs eines IntervalBlocks: Start des IntervalBlocks: {ib.Interval.Start}, Zeitpunkt des IntervalReading: {ir.TimePeriod.Start}, Kennziffer: {reading.ReadingType.ObisCode}"));
                        }
                        else if (ir.CaptureTime.ToUniversalTime() > intervalBlockEnd.ToUniversalTime())
                        {
                            exceptions.Add(new InvalidOperationException($"TAF-7: IntervalReading befindet sich nicht innerhalb des Zeitbereichs eines IntervalBlocks: Ende des IntervalBlocks: {intervalBlockEnd}, Zeitpunkt des IntervalReading: {ir.TimePeriod.GetEnd()}, Kennziffer: {reading.ReadingType.ObisCode}"));
                        }
                    }
                }
            }
        }


        // Validate of all the neccessary ids of the model xml match with the ids in the supplier xml
        private static void ValidateTaf7ModelSupplierCompatibility(UsagePointAdapterTRuDI model, UsagePointLieferant supplier, List<Exception> exceptions)
        {
            if (model.UsagePointId != supplier.UsagePointId)
            {
                exceptions.Add(new InvalidOperationException($"TAF-7: Die ID der Messlokation \"{model.UsagePointId}\" stimmt nicht mit der ID der Messlokation \"{supplier.UsagePointId}\" aus der Tarifdatei des Lieferanten überein."));
            }

            if (model.InvoicingParty.InvoicingPartyId != supplier.InvoicingParty.InvoicingPartyId)
            {
                exceptions.Add(new InvalidOperationException($"TAF-7: Die ID des Rechnungsstellers \"{model.InvoicingParty.InvoicingPartyId}\" stimmt nicht mit der ID des Rechnungssteller \"{supplier.InvoicingParty.InvoicingPartyId}\" aus der Tarifdatei des Lieferanten überein."));
            }

            if (model.ServiceCategory.Kind != supplier.ServiceCategory.Kind)
            {
                exceptions.Add(new InvalidOperationException($"TAF-7: Die Service-Kategory \"{model.ServiceCategory.Kind}\" stimmt nicht mit der Service-Kategory \"{supplier.ServiceCategory.Kind}\" aus der Tarifdatei des Lieferanten überein."));
            }

            if (string.Compare(model.Smgw.SmgwId, supplier.Smgw.SmgwId, StringComparison.OrdinalIgnoreCase) != 0)
            {
                exceptions.Add(new InvalidOperationException($"TAF-7: Die ID des Smart Meter Gateway \"{model.Smgw.SmgwId}\" stimmt nicht mit der ID \"{supplier.Smgw.SmgwId}\" aus der Tarifdatei des Lieferanten überein."));
            }

            if (model.TariffName != supplier.TariffName)
            {
                exceptions.Add(new InvalidOperationException($"TAF-7: Der Tarifname \"{model.TariffName}\" stimmt nicht mit dem Tariffnamen \"{supplier.TariffName}\" aus der Tarifdatei des Lieferanten überein."));
            }
        }

        // Check whether all referenced tarif stages which are used in the DayTimeProfiles are valid
        private static void ValidateTarifStageOccurence(UsagePointLieferant supplier, List<Exception> exceptions)
        {
            var tarifStages = new List<ushort>();

            foreach (TariffStage stage in supplier.AnalysisProfile.TariffStages)
            {
                tarifStages.Add(stage.TariffNumber);
            }

            foreach (DayProfile dayProfile in supplier.AnalysisProfile.TariffChangeTrigger.TimeTrigger.DayProfiles)
            {
                foreach (DayTimeProfile dtProfile in dayProfile.DayTimeProfiles)
                {
                    bool match = false;
                    foreach (int stage in tarifStages)
                    {
                        if (stage == dtProfile.TariffNumber)
                        {
                            match = true;
                            break;
                        }
                    }
                    if (!match)
                    {
                        exceptions.Add(new InvalidOperationException($"TAF-7: Ungültige Tarif-Nummer innerhalb eines Tagesprofils: {dtProfile.TariffNumber}"));
                    }
                }
            }
        }

        // Check whether all referenced DayIds in SpecialDayProfiles are valid DayIds
        private static void ValidateSupplierModelDayProfileOccurence(UsagePointLieferant supplier, List<Exception> exceptions)
        {
            var dayProfileIds = new List<ushort?>();

            foreach (DayProfile profile in supplier.AnalysisProfile.TariffChangeTrigger.TimeTrigger.DayProfiles)
            {
                dayProfileIds.Add(profile.DayId);
            }

            foreach (SpecialDayProfile spProfile in supplier.AnalysisProfile.TariffChangeTrigger.TimeTrigger.SpecialDayProfiles)
            {
                bool match = false;
                foreach (var id in dayProfileIds)
                {
                    if (id == spProfile.DayId)
                    {
                        match = true;
                        break;
                    }
                }
                if (!match)
                {
                    exceptions.Add(new InvalidOperationException("TAF-7: Das Element \"DayProfile\" im Element \"SpecialDayProfile\" ist ungültig."));
                }
            }
        }

        // Validates the maximum amount of tariff stages
        private static void ValidateSupplierModelTariffStageCount(UsagePointLieferant supplier, List<Exception> exceptions)
        {
            if (supplier.AnalysisProfile.TariffStages.Count > 20)
            {
                var stages = supplier.AnalysisProfile.TariffStages;
                int errorRegisterCount = 0;
                foreach (TariffStage stage in stages)
                {
                    var obisId = new ObisId(stage.ObisCode);
                    if (obisId.E == 63)
                    {
                        errorRegisterCount++;
                    }
                }
                if (stages.Count - errorRegisterCount > 20)
                {
                    exceptions.Add(new InvalidOperationException("Es sind maximal 20 Tarifstuffen zulässig."));
                }
            }
        }

        // Check if the delivered supplier xml has an completely enrolled calendar
        private static void ValidateSupplierModelCompletelyEnrolledCalendar(UsagePointAdapterTRuDI model, UsagePointLieferant supplier, List<Exception> exceptions)
        {
            var period = supplier.AnalysisProfile.BillingPeriod;
            var profiles = supplier.AnalysisProfile.TariffChangeTrigger.TimeTrigger.SpecialDayProfiles;

            var sdpCheckList = new Dictionary<DateTime, int>();
            var timestamp = period.Start;
            var hasCounted = false;
            while (timestamp <= period.GetEnd().AddDays(-1))
            {
                sdpCheckList.Add(timestamp, 0);

                timestamp = timestamp.AddDays(1);
            }

            foreach (var profile in profiles)
            {
                if (sdpCheckList.ContainsKey(profile.SpecialDayDate.GetDate()))
                {
                    sdpCheckList[profile.SpecialDayDate.GetDate()] += 1;
                    hasCounted = true;
                }
            }

            foreach (var item in sdpCheckList)
            {
                if (item.Value == 0 && hasCounted)
                {
                    exceptions.Add(new InvalidOperationException($"Tagesprofil für Tag {item.Key:dd.MM.yyy} nicht vorhanden."));
                }
            }
        }

        // Check if any SpecialDayProfiles are within the billing period
        private static void ValidateSpecialDayProfilesWithinBillingPeriod(UsagePointLieferant supplier, List<Exception> exceptions)
        {
            var begin = supplier.AnalysisProfile.BillingPeriod.Start;
            var end = supplier.AnalysisProfile.BillingPeriod.GetEnd();
            var counter = 0;

            foreach (SpecialDayProfile profile in supplier.AnalysisProfile.TariffChangeTrigger.TimeTrigger.SpecialDayProfiles)
            {
                if (profile.SpecialDayDate.GetDate() >= begin && profile.SpecialDayDate.GetDate() <= end)
                {
                    counter++;
                }
            }

            if (counter == 0)
            {
                exceptions.Add(new InvalidOperationException("Die Abrechnungsperiode in der Tarifdatei des Lieferanten umfasst keinen vollen Tagesprofil."));
            }
        }
    }
}
