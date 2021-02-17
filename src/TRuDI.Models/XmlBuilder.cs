namespace TRuDI.Models
{
    using System.Collections.Generic;
    using System.Text;
    using System.Xml.Linq;

    using TRuDI.Models.BasicData;

    public class XmlBuilder
    {
        private XNamespace ar = XNamespace.Get("http://vde.de/AR_2418-6.xsd");
        private XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
        private XNamespace schemaLocation = XNamespace.Get("http://vde.de/AR_2418-6.xsd AR_2418-6.xsd");
        private XNamespace espi = XNamespace.Get("http://naesb.org/espi");
        private XNamespace atom = XNamespace.Get("http://www.w3.org/2005/Atom");

        public Kind ServiceCategoryKind { get; set; }
        public string CustomerId { get; set; }
        public string InvoicingPartyId { get; set; }
        public string SmgwId { get; set; }

        public string FirmwareVersion { get; set; }
        public List<FirmwareComponent> FirmwareComponents { get; set; } = new List<FirmwareComponent>();

        public List<Certificate> Certificates { get; } = new List<Certificate>();

        public string TariffName { get; set; }
        public string UsagePointId { get; set; }

        public byte[] SignedTafProfile { get; set; }

        public List<LogEntry> LogList { get; set; }
        public List<MeterReading> MeterReadings { get; set; }

        public string MissingProperties()
        {
            var sb = new StringBuilder();

            if (string.IsNullOrEmpty(this.CustomerId))
            {
                sb.Append("CustomerId ");
            }

            if (string.IsNullOrEmpty(this.InvoicingPartyId))
            {
                sb.Append("InvoicingPartyId ");
            }

            if (string.IsNullOrEmpty(this.SmgwId))
            {
                sb.Append("SmgwId ");
            }

            if (this.Certificates.Count == 0)
            {
                sb.Append("Certificate ");
            }

            if (string.IsNullOrEmpty(this.UsagePointId))
            {
                sb.Append("UsagePointId ");
            }

            if (string.IsNullOrEmpty(this.TariffName))
            {
                sb.Append("TariffName ");
            }

            return sb.ToString();
        }

        private bool HasAllProperties()
        {
            return
                !string.IsNullOrEmpty(this.CustomerId) &&
                !string.IsNullOrEmpty(this.InvoicingPartyId) &&
                !string.IsNullOrEmpty(this.SmgwId) &&
                !string.IsNullOrEmpty(this.UsagePointId) &&
                !string.IsNullOrEmpty(this.TariffName) &&
                this.Certificates.Count > 0;
        }

        public XDocument GenerateXmlDocument()
        {
            if (!this.HasAllProperties())
            {
                return new XDocument();
            }

            var serviceCategory = new XElement(this.espi + "ServiceCategory", new XElement(this.espi + "kind", (ushort)this.ServiceCategoryKind));
            var customer = new XElement(this.ar + "Customer", new XElement(this.ar + "customerId", this.CustomerId));
            var invoicingParty = new XElement(this.ar + "InvoicingParty", new XElement(this.ar + "invoicingPartyId", this.InvoicingPartyId));

            var smgw = new XElement(this.ar + "SMGW");
            foreach (var cert in this.Certificates)
            {
                smgw.Add(new XElement(this.ar + "certId", cert.CertId));
            }

            smgw.Add(new XElement(this.ar + "smgwId", this.SmgwId.WithNameExtension()));

            if (!string.IsNullOrWhiteSpace(this.FirmwareVersion))
            {
                smgw.Add(new XElement(this.ar + "FirmwareVersion", this.FirmwareVersion));
            }

            if (this.FirmwareComponents.Count > 0)
            {
                foreach (var component in this.FirmwareComponents)
                {
                    var elem = new XElement(this.ar + "FirmwareComponent");
                    elem.Add(new XElement(this.ar + "name", component.Name));

                    if (!string.IsNullOrWhiteSpace(component.Version))
                    {
                        elem.Add(new XElement(this.ar + "version", component.Version));
                    }

                    if (!string.IsNullOrWhiteSpace(component.Checksum))
                    {
                        elem.Add(new XElement(this.ar + "checksum", component.Checksum));
                    }

                    smgw.Add(elem);
                }
            }

            var usagePoint = new XElement(
                this.ar + "UsagePoint",
                serviceCategory,
                new XElement(this.ar + "usagePointId", this.UsagePointId),
                customer,
                invoicingParty,
                smgw);

            if (this.SignedTafProfile != null)
            {
                var vendorConfig = new XElement(this.ar + "VendorConfig");
                vendorConfig.Add(new XElement(this.ar + "tafProfile", this.SignedTafProfile.ToHexBinary()));
                usagePoint.Add(vendorConfig);
            }

            // Certificates
            foreach (var cert in this.Certificates)
            {
                var cer = new XElement(
                    this.ar + "Certificate",
                    new XElement(this.ar + "certId", cert.CertId),
                    new XElement(this.ar + "certType", (int)cert.CertType.Value));

                if (cert.ParentCertId != null)
                {
                    cer.Add(new XElement(this.ar + "parentCertId", cert.ParentCertId.Value));
                }

                cer.Add(new XElement(this.ar + "certContent", cert.CertContent.ToHexBinary()));

                usagePoint.Add(cer);
            }

            var trudiXml = new XDocument(new XElement(
                this.ar + "UsagePoints",
                new XAttribute("xmlns", this.ar),
                new XAttribute(XNamespace.Xmlns + "xsi", this.xsi),
                new XAttribute(this.xsi + "schemaLocation", this.schemaLocation),
                new XAttribute(XNamespace.Xmlns + "espi", this.espi),
                new XAttribute(XNamespace.Xmlns + "atom", this.atom),
                usagePoint));

            usagePoint.Add(new XElement(this.ar + "tariffName", this.TariffName));

            // event logs
            if (this.LogList != null && this.LogList.Count > 0)
            {
                foreach (LogEntry log in this.LogList)
                {
                    var logEntry = new XElement(this.ar + "LogEntry",
                        new XElement(this.ar + "LogEvent",
                            new XElement(this.ar + "level", (byte)log.LogEvent.Level),
                            new XElement(this.ar + "text", log.LogEvent.Text),
                            new XElement(this.ar + "outcome", (byte)log.LogEvent.Outcome),
                            new XElement(this.ar + "timestamp", log.LogEvent.Timestamp.ToString("yyyy-MM-ddTHH:mm:ssK"))));

                    if (log.RecordNumber != null)
                    {
                        logEntry.AddFirst(new XElement(this.ar + "recordNumber", log.RecordNumber));
                    }

                    usagePoint.Add(logEntry);
                }
            }

            if (this.MeterReadings == null || this.MeterReadings.Count == 0)
            {
                return trudiXml;
            }

            foreach (var meterReading in this.MeterReadings)
            {
                var mr = new XElement(
                    this.ar + "MeterReading",
                    new XElement(this.ar + "Meter", new XElement(this.ar + "meterId", meterReading.Meters[0].MeterId)),
                    new XElement(this.ar + "meterReadingId", meterReading.MeterReadingId));

                if (meterReading.ReadingType.MeasurementPeriod == 0)
                {
                    mr.Add(new XElement(
                        this.ar + "ReadingType",
                        new XElement(this.espi + "powerOfTenMultiplier", (short)meterReading.ReadingType.PowerOfTenMultiplier),
                        new XElement(this.espi + "uom", (ushort)meterReading.ReadingType.Uom),
                        new XElement(this.ar + "scaler", meterReading.ReadingType.Scaler),
                        new XElement(this.ar + "obisCode", meterReading.ReadingType.ObisCode),
                        new XElement(this.ar + "qualifiedLogicalName", meterReading.ReadingType.QualifiedLogicalName.WithNameExtension())));
                }
                else
                {
                    mr.Add(new XElement(
                        this.ar + "ReadingType",
                        new XElement(this.espi + "powerOfTenMultiplier", (short)meterReading.ReadingType.PowerOfTenMultiplier),
                        new XElement(this.espi + "uom", (ushort)meterReading.ReadingType.Uom),
                        new XElement(this.ar + "scaler", meterReading.ReadingType.Scaler),
                        new XElement(this.ar + "obisCode", meterReading.ReadingType.ObisCode),
                        new XElement(this.ar + "qualifiedLogicalName", meterReading.ReadingType.QualifiedLogicalName.WithNameExtension()),
                        new XElement(this.ar + "measurementPeriod", meterReading.ReadingType.MeasurementPeriod)));
                }

                foreach (var iBlock in meterReading.IntervalBlocks)
                {
                    var intervalBlock = new XElement(this.ar + "IntervalBlock",
                        new XElement(this.ar + "interval",
                            new XElement(this.ar + "duration", iBlock.Interval.Duration),
                            new XElement(this.ar + "start", iBlock.Interval.Start.ToString("yyyy-MM-ddTHH:mm:ssK"))));

                    foreach (var iReading in iBlock.IntervalReadings)
                    {
                        var intervalReading = new XElement(this.ar + "IntervalReading",
                            new XElement(this.espi + "value", iReading.Value),
                            new XElement(this.ar + "timePeriod",
                                new XElement(this.ar + "duration", iReading.TimePeriod.Duration),
                                new XElement(this.ar + "start", iReading.TimePeriod.Start.ToString("yyyy-MM-ddTHH:mm:ssK"))));

                        if (iReading.TargetTime.HasValue)
                        {
                            intervalReading.Add(new XElement(this.ar + "targetTime", iReading.TargetTime.Value.ToString("yyyy-MM-ddTHH:mm:ssK")));
                        }

                        if (iReading.MeasurementTimeMeter.HasValue)
                        {
                            intervalReading.Add(new XElement(this.ar + "measurementTimeMeter", iReading.MeasurementTimeMeter.Value.ToString("yyyy-MM-ddTHH:mm:ssK")));
                        }

                        if (!string.IsNullOrWhiteSpace(iReading.MeterSignature))
                        {
                            intervalReading.Add(new XElement(this.ar + "meterSig", iReading.MeterSignature));
                        }
                        
                        if (!string.IsNullOrWhiteSpace(iReading.Signature))
                        {
                            intervalReading.Add(new XElement(this.ar + "signature", iReading.Signature));
                        }

                        if (iReading.StatusFNN != null)
                        {
                            intervalReading.Add(new XElement(this.ar + "statusFNN", iReading.StatusFNN.Status));
                        }

                        if (iReading.StatusPTB.HasValue)
                        {
                            intervalReading.Add(new XElement(this.ar + "statusPTB", (int)iReading.StatusPTB));
                        }

                        if (!string.IsNullOrWhiteSpace(iReading.StatusVendor))
                        {
                            intervalReading.Add(new XElement(this.ar + "statusVendor", iReading.StatusVendor));
                        }

                        intervalBlock.Add(intervalReading);
                    }

                    mr.Add(intervalBlock);
                }

                usagePoint.Add(mr);
            }

            return trudiXml;
        }
    }
}
