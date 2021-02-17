namespace TRuDI.HanAdapter.Example.ConfigModel
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using TRuDI.HanAdapter.Interface;
    using TRuDI.Models.BasicData;
    using TRuDI.Models.CheckData;

    /// <summary>
    /// This class is used to create the neccessary xml files for the dummyAdapter.
    /// </summary>
    public class XmlFactory
    {
        /// <summary>
        /// The List contains the needed namespaces for the xml file.
        /// </summary>
        private List<(string name, XNamespace ns)> Namespaces { get; set; }

        /// <summary>
        /// The configuration settings to create the xml files automatically 
        /// </summary>
        private HanAdapterExampleConfig HanConfiguration { get; set; }

        /// <summary>
        /// The namespaces will be set in the constructor. 
        /// </summary>
        /// <param name="HanConfiguration">A settings object</param>
        public XmlFactory(HanAdapterExampleConfig HanConfiguration)
        {
            SetNamespaces();
            this.HanConfiguration = HanConfiguration;
        }

        /// <summary>
        /// This method is the entry point for creating the xml file.
        /// </summary>
        /// <returns>the xml data as a XDocument</returns>
        public XDocument BuildTafXml()
        {
            var UsagePoints = new XElement(Namespaces.FirstOrDefault(ns => ns.name == "ar").ns + "UsagePoints",
                                               new XAttribute("xmlns", Namespaces.FirstOrDefault(ns => ns.name == "ar").ns),
                                               new XAttribute(XNamespace.Xmlns + "xsi", Namespaces.FirstOrDefault(ns => ns.name == "xsi").ns),
                                               new XAttribute(Namespaces.FirstOrDefault(ns => ns.name == "xsi").ns + "schemaLocation", Namespaces.FirstOrDefault(ns => ns.name == "schemaLocation").ns),
                                               new XAttribute(XNamespace.Xmlns + "espi", Namespaces.FirstOrDefault(ns => ns.name == "espi").ns),
                                               new XAttribute(XNamespace.Xmlns + "atom", Namespaces.FirstOrDefault(ns => ns.name == "atom").ns),
                                                   BuildUsagePoint(false));

            //if (HanConfiguration.Contract.TafId == TafId.Taf7)
            //{
            //    BuildSupplierXml(BuildUsagePoint(true));
            //}

            return new XDocument(UsagePoints);
        }

        /// <summary>
        /// Used to build the supplierXml in case of Taf7. Its called in BuidlUsagePoint. The supplier xml file will be stored in an HanAdapterExampleConfig instance in SupplierXml.
        /// </summary>
        /// <param name="usagePoint">The usagePoint which shall be used.</param>
        private void BuildSupplierXml(XElement usagePoint)
        {
            var ar = Namespaces.FirstOrDefault(ns => ns.name == "ar").ns;

            var UsagePoints = new XElement(ar + "UsagePoints",
                                               new XAttribute("xmlns", ar),
                                               new XAttribute(XNamespace.Xmlns + "xsi", Namespaces.FirstOrDefault(ns => ns.name == "xsi").ns),
                                               new XAttribute(Namespaces.FirstOrDefault(ns => ns.name == "xsi").ns + "schemaLocation", Namespaces.FirstOrDefault(ns => ns.name == "schemaLocation").ns),
                                               new XAttribute(XNamespace.Xmlns + "espi", Namespaces.FirstOrDefault(ns => ns.name == "espi").ns),
                                               new XAttribute(XNamespace.Xmlns + "atom", Namespaces.FirstOrDefault(ns => ns.name == "atom").ns),
                                                   usagePoint);

           HanConfiguration.SupplierXml =  new XDocument(UsagePoints);
        }

        /// <summary>
        /// The central method for building the usagePoint Element. 
        /// </summary>
        /// <param name="isSupplierXml">If Taf7 is choosed, this element has to be true to create the supplierXml instead of the data xml.</param>
        /// <returns>The XElement usagePoint</returns>
        private XElement BuildUsagePoint(bool isSupplierXml)
        {
            var ar = Namespaces.FirstOrDefault(ns => ns.name == "ar").ns;
            var usagePoint = new XElement(ar + "UsagePoint");

            SetCommonElements(usagePoint, isSupplierXml, ar);

            if (isSupplierXml)
            {
                BuildUsagePointSupplier(usagePoint, ar);
            }
            else
            {
                if (HanConfiguration.WithLogData)
                {
                    LogEntries(ar).ForEach(log => usagePoint.Add(log));
                }

                MeterReadings(ar).ForEach(mr => usagePoint.Add(mr));
            }
            
            return usagePoint;
        }

        /// <summary>
        /// Creates the supplierXml file for Taf7
        /// </summary>
        /// <param name="usagePoint">The usagePoint which shall be used.</param>
        /// <param name="ar">The main namespace</param>
        /// <returns>The XElement usagePoint for the supplier xml file.</returns>
        private XElement BuildUsagePointSupplier(XElement usagePoint, XNamespace ar)
        {
            usagePoint.Add(GetAnalysisProfile(ar));

            return usagePoint;
        }

        /// <summary>
        /// Sets the first elements for the xml. In case of Taf7 it is used for both xml files.
        /// </summary>
        /// <param name="usagePoint">THe usagePoint which shall be used.</param>
        /// <param name="isSupplierXml">If Taf7 is choosed, this element has to be true to create the supplierXml instead of the data xml.</param>
        /// <param name="ar">The main namespace.</param>
        private void SetCommonElements(XElement usagePoint, bool isSupplierXml, XNamespace ar)
        {
            usagePoint.Add(GetServiceCategory());
            usagePoint.Add(new XElement(ar + "usagePointId", HanConfiguration.XmlConfig.UsagePointId));

            if (isSupplierXml && HanConfiguration.XmlConfig.CustomerId != null)
            {
                usagePoint.Add(GetCustomer(ar));
            }
            else if (!isSupplierXml)
            {
                usagePoint.Add(GetCustomer(ar));
            }

            usagePoint.Add(GetInvoicingParty(ar));

            usagePoint.Add(GetSmgw(ar));
            if (isSupplierXml && HanConfiguration.XmlConfig.Certificates.Count > 0)
            {
                GetCertificates(ar).ForEach(cert => usagePoint.Add(cert));
            }
            else if (!isSupplierXml)
            {
                GetCertificates(ar).ForEach(cert => usagePoint.Add(cert));
            }

            usagePoint.Add(new XElement(ar + "tariffName", HanConfiguration.XmlConfig.TariffName));
        }

        /// <summary>
        /// Sets the ServiceCategory XElement.
        /// </summary>
        /// <returns>The ServiceCategory XElement.</returns>
        private XElement GetServiceCategory()
        {
            var espi = Namespaces.FirstOrDefault(ns => ns.name == "espi").ns;
            return new XElement(espi + "ServiceCategory", new XElement(espi + "kind", HanConfiguration.XmlConfig.ServiceCategoryKind));
        }

        /// <summary>
        /// Sets the InvoicingParty XElement.
        /// </summary>
        /// <param name="ar">The main namespace.</param>
        /// <returns>THe InvoicingParty XElement</returns>
        private XElement GetInvoicingParty(XNamespace ar)
        {
            return new XElement(ar + "InvoicingParty", new XElement(ar + "invoicingPartyId", HanConfiguration.XmlConfig.InvoicingPartyId));
        }

        /// <summary>
        /// Sets the Customer XElement.
        /// </summary>
        /// <param name="ar">The main namespace.</param>
        /// <returns>The Customer XElement</returns>
        private XElement GetCustomer(XNamespace ar)
        {
            return new XElement(ar + "Customer", new XElement(ar + "customerId", HanConfiguration.XmlConfig.CustomerId));
        }

        /// <summary>
        /// Sets the Smgw XElement. For each existing certificate the certId will be stored in smgw.certIds.
        /// </summary>
        /// <param name="ar">The main namesapce.</param>
        /// <returns>The Smgw XElement</returns>
        private XElement GetSmgw(XNamespace ar)
        {
            var smgw = new XElement(ar + "SMGW");

            if (HanConfiguration.XmlConfig.Certificates != null)
            {
                foreach (CertificateContainer cert in HanConfiguration.XmlConfig.Certificates)
                {
                    smgw.Add(new XElement(ar + "certId", cert.Certificate.CertId));
                }  
            }

            smgw.Add(new XElement(ar + "smgwId", HanConfiguration.XmlConfig.SmgwId));

            return smgw;
        }

        /// <summary>
        /// This method creates available certificates.
        /// </summary>
        /// <param name="ar">The main namespace.</param>
        /// <returns>The Certificate XElements</returns>
        private List<XElement> GetCertificates(XNamespace ar)
        {
            var certs = new List<XElement>();
           
            foreach (CertificateContainer cert in HanConfiguration.XmlConfig.Certificates)
            {
                var cer = new XElement(ar + "Certificate",
                                            new XElement(ar + "certId", cert.Certificate.CertId),
                                            new XElement(ar + "certType", (byte)cert.Certificate.CertType)
                                           );

                if (cert.Certificate.ParentCertId.HasValue)
                {
                    cer.Add(new XElement(ar + "parentCertId", cert.Certificate.ParentCertId));
                }

                cer.Add(new XElement(ar + "certContent", cert.CertContent));

                certs.Add(cer);
            }
            return certs;
        }


        /// <summary>
        /// Creates the LogEntries for the xml file.
        /// </summary>
        /// <param name="ar">The main namespace.</param>
        /// <returns>LogEntriy XElements for the xml file.</returns>
        private List<XElement> LogEntries(XNamespace ar)
        {
            var logEntries = new List<XElement>();
            var logData = this.CreateLogs(HanConfiguration);

            foreach (var log in logData)
            {
               logEntries.Add(new XElement(ar + "LogEntry", 
                   new XElement(ar + "recordNumber", log.RecordNumber), 
                   new XElement(GetLogEvent(log.LogEvent, ar))));
            }
            return logEntries;
        }

        /// <summary>
        /// Sets an XElement Interval for the xmlFile
        /// </summary>
        /// <param name="elementName">The used name in the xml file.</param>
        /// <param name="interval">Contains the needed data to create the xml elements.</param>
        /// <param name="ar">The main namespace.</param>
        /// <returns>An interval XElement</returns>
        private XElement GetInterval(string elementName, Interval interval, XNamespace ar)
        {
            var period = new XElement(ar + elementName);

            period.Add(new XElement(ar + "duration", interval.Duration));
            period.Add(new XElement(ar + "start", interval.Start.ToString("yyyy-MM-ddTHH:mm:ssK")));

            return period;
        }

        /// <summary>
        /// Creates an LogEvent xml element. It is needed for the single LogEntries.
        /// </summary>
        /// <param name="logEvent">Contains the needed data to create the xml elements.</param>
        /// <param name="ar">The main namespace.</param>
        /// <returns>A single logEvent for an LogEntry</returns>
        private XElement GetLogEvent(LogEvent logEvent, XNamespace ar)
        {
            return new XElement(ar + "LogEvent", 
                new XElement(ar + "level", (byte)logEvent.Level), 
                new XElement(ar + "text", logEvent.Text), 
                new XElement(ar + "outcome", (byte)logEvent.Outcome),
                new XElement(ar + "timestamp", logEvent.Timestamp.ToString("yyyy-MM-ddTHH:mm:ssK")));
        }

        /// <summary>
        /// This method creates a singele  ReadingType for an MeterReading objeck.
        /// </summary>
        /// <param name="readingType">Contains the needed data to create the xml element.</param>
        /// <returns>A single ReadingType instance for an meterReading object</returns>
        private XElement GetReadingType(ReadingType readingType)
        {
            var ar = Namespaces.FirstOrDefault(ns => ns.name == "ar").ns;
            var espi = Namespaces.FirstOrDefault(ns => ns.name == "espi").ns;

            return new XElement(ar + "ReadingType",
                     new XElement(espi + "powerOfTenMultiplier", (short)readingType.PowerOfTenMultiplier),
                     new XElement(espi + "uom", (ushort)readingType.Uom),
                     new XElement(ar + "scaler", readingType.Scaler),
                     new XElement(ar + "obisCode", readingType.ObisCode),
                     new XElement(ar + "qualifiedLogicalName", readingType.QualifiedLogicalName));
        }

        /// <summary>
        /// The IntervalReadings are the objects which hold the actual measuring vaule. In this method 
        /// they will be created for the xml file.
        /// </summary>
        /// <param name="readings">Contains the needed data to create the xml elements.</param>
        /// <param name="ar">The main namespace.</param>
        /// <returns>Returns a list of intervalReading XElements for an IntervalBlock XElement.</returns>
        private List<XElement> IntervalReadings(List<IntervalReading> readings, XNamespace ar)
        {
            var espi = Namespaces.FirstOrDefault(ns => ns.name == "espi").ns;
            var intervalReadings = new List<XElement>();

            foreach (IntervalReading iReading in readings)
            {
                XElement intervalReading = new XElement(ar + "IntervalReading");

                intervalReading.Add(new XElement(espi + "value", iReading.Value));
                intervalReading.Add(GetInterval("timePeriod", iReading.TimePeriod, ar));
                if (iReading.StatusPTB.HasValue)
                {
                    intervalReading.Add(new XElement(ar + "statusPTB", (byte)iReading.StatusPTB));
                }
                else
                {
                    intervalReading.Add(new XElement(ar + "statusFNN", iReading.StatusFNN.Status));
                }

                intervalReadings.Add(intervalReading);
            }

            return intervalReadings;
        }

        /// <summary>
        /// In this method the intervalBlocks XElements for the meterReading object will be created.
        /// </summary>
        /// <param name="intervalBlocks">Contains the needed data to create the xml elements.</param>
        /// <param name="ar">The main namespace</param>
        /// <returns>A list of intervaBlock XEmelents which must be added th an meterReading object.</returns>
        private List<XElement> IntervalBlocks(List<IntervalBlock> intervalBlocks, XNamespace ar)
        {
            var blocks = new List<XElement>();

            foreach(IntervalBlock ib in intervalBlocks)
            {
                var intervalBlock = new XElement(ar + "IntervalBlock");
                intervalBlock.Add(GetInterval("interval", ib.Interval, ar));

                var intervalReadings = IntervalReadings(ib.IntervalReadings, ar);
                foreach (XElement ir in intervalReadings)
                {
                    intervalBlock.Add(ir);
                }

                blocks.Add(intervalBlock);
            }

            return blocks;
        }

        /// <summary>
        /// Sets the meter XElement.
        /// </summary>
        /// <param name="meter">Contains the needed data to create the xml element.</param>
        /// <param name="ar">The main namespace.</param>
        /// <returns>The meter XElement.</returns>
        private XElement GetMeter(Meter meter, XNamespace ar)
        {
            return new XElement(ar + "Meter", new XElement(ar + "meterId", meter.MeterId));
        }

        /// <summary>
        /// In this method the meterReadings will be created. 
        /// </summary>
        /// <param name="ar">The main namespace.</param>
        /// <returns>A list of MeterReadings XElements for the xml file.</returns>
        private List<XElement> MeterReadings(XNamespace ar)
        {
            var meterReadings = new List<XElement>();
            var readingData = this.CreateMeterReadings(HanConfiguration);

            foreach(MeterReading meterReading in readingData)
            {
                var mr = new XElement(ar + "MeterReading");

                mr.Add(GetMeter(meterReading.Meters[0], ar));
                mr.Add(new XElement(ar + "meterReadingId", meterReading.MeterReadingId));
                mr.Add(GetReadingType(meterReading.ReadingType));

                var intervalBlocks = IntervalBlocks(meterReading.IntervalBlocks, ar);

                foreach(XElement ib in intervalBlocks)
                {
                    mr.Add(ib);
                }

                meterReadings.Add(mr);
            }

            return meterReadings;
        }

        /// <summary>
        /// Creates the AnalysisProfile XElement. It is used for the supplier xml file.
        /// </summary>
        /// <param name="ar">The main namespace.</param>
        /// <returns>An analysisProfile XElement for the supplier xml file.</returns>
        private XElement GetAnalysisProfile(XNamespace ar)
        {
            var analysisProfile = new XElement(ar + "AnalysisProfile");

            analysisProfile.Add(new XElement(ar + "tariffUseCase", HanConfiguration.XmlConfig.TariffUseCase));
            analysisProfile.Add(new XElement(ar + "tariffId", HanConfiguration.XmlConfig.TariffName));

            if (this.HanConfiguration.BillingPeriod.End != null)
            {
                analysisProfile.Add(
                    GetInterval(
                        "billingPeriod",
                        new Interval()
                            {
                                Start = HanConfiguration.BillingPeriod.Begin,
                                Duration = (uint)(HanConfiguration.BillingPeriod.End.Value.ToUniversalTime()
                                                  - HanConfiguration.BillingPeriod.Begin.ToUniversalTime())
                                    .TotalSeconds
                            },
                        ar));
            }

            TariffStages(ar).ForEach(ts => analysisProfile.Add(ts));
            analysisProfile.Add(new XElement(ar + "defaultTariffNumber", HanConfiguration.XmlConfig.DefaultTariffNumber));
            analysisProfile.Add(GetTariffChangeTrigger(ar));

            return analysisProfile;
        }

        /// <summary>
        /// Creates the TariffStages XElements for the supplier xml files.
        /// </summary>
        /// <param name="ar">The main namespace.</param>
        /// <returns>A list with tariffStage XElements</returns>
        private List<XElement> TariffStages(XNamespace ar)
        {
            var tariffStages = new List<XElement>();
           
            foreach (TariffStageConfig config in HanConfiguration.XmlConfig.TariffStageConfigs)
            {
                var tariffStage = new XElement(ar + "TariffStage");

                tariffStage.Add(new XElement(ar + "tariffNumber", config.TariffNumber));

                if (config.Description != null)
                {
                    tariffStage.Add(new XElement(ar + "description", config.Description));
                }

                tariffStage.Add(new XElement(ar + "obisCode", config.ObisCode));

                tariffStages.Add(tariffStage);
            }

            return tariffStages;
        }

        /// <summary>
        /// Sets the TariffChangeTrigger. It is used for the supplier xml.
        /// </summary>
        /// <param name="ar">The main namespace.</param>
        /// <returns>A TariffCnageTrigger XElement.</returns>
        private XElement GetTariffChangeTrigger(XNamespace ar)
        {
            var trigger = new XElement(ar + "TariffChangeTrigger");
            trigger.Add(GetTimeTrigger(ar));

            return trigger;
        }

        /// <summary>
        /// Sets the TimeTrigger XElement which is used for the supplier xml.
        /// </summary>
        /// <param name="ar">The main namespace.</param>
        /// <returns>A TimeTrigger XElement.</returns>
        private XElement GetTimeTrigger(XNamespace ar)
        {
            var timeTrigger = new XElement(ar + "TimeTrigger");
            
            DayProfiles(ar).ForEach(dp => timeTrigger.Add(dp));
            SpecialDayProfiles(ar).ForEach(sdp => timeTrigger.Add(sdp));

           return timeTrigger;
        }

        /// <summary>
        /// Creates a list of DayProfiles XElements which are contained in the TimeTrigger XElement.
        /// </summary>
        /// <param name="ar">The main namespace.</param>
        /// <returns>A list of DayProfile XElements.</returns>
        private List<XElement> DayProfiles(XNamespace ar)
        {
            var dayProfiles = new List<XElement>();
            var dayProfilesData = this.CreateDayProfiles(HanConfiguration);

            foreach (var profile in dayProfilesData)
            {
                var dayProfile = new XElement(ar + "DayProfile", new XElement(ar + "dayId", profile.DayId));

                DayTimeProfiles(profile.DayTimeProfiles, ar).ForEach(dtp => dayProfile.Add(dtp));

                dayProfiles.Add(dayProfile);
            }

            return dayProfiles;
        }

        /// <summary>
        /// Creates the DayTimeProfiles XElements which are contained in a DayProfile XElement.
        /// </summary>
        /// <param name="profiles">Contains the needed data to create the xml elements.</param>
        /// <param name="ar">The main namespace.</param>
        /// <returns>A list of DayTimeProfiles XElements for an DayProfile XElement.</returns>
        private List<XElement> DayTimeProfiles(List<DayTimeProfile> profiles, XNamespace ar)
        {
            var dayTimeProfiles = new List<XElement>();

            foreach(DayTimeProfile dayTimeProfile in profiles)
            {
                var dtp = new XElement(ar + "DayTimeProfile");

                dtp.Add(GetTimeVarType(dayTimeProfile.StartTime, "startTime", ar));
                dtp.Add(new XElement(ar + "tariffNumber", dayTimeProfile.TariffNumber));

                dayTimeProfiles.Add(dtp);
            }

            return dayTimeProfiles;
        }

        /// <summary>
        /// Creates a TimeVarType Element. They are used in DayTimeProfiles.
        /// </summary>
        /// <param name="timeVarType">Contains the needed data to create the xml elements.</param>
        /// <param name="typeName">The name of the xml element.</param>
        /// <param name="ar">The main namespace.</param>
        /// <returns>A TimeVarType XElement.</returns>
        private XElement GetTimeVarType(TimeVarType timeVarType, string typeName, XNamespace ar)
        {
            var type = new XElement(ar + typeName);

            type.Add(new XElement(ar + "hour", timeVarType.Hour));
            type.Add(new XElement(ar + "minute", timeVarType.Minute));

            return type;
        }

        /// <summary>
        /// Creates the SpecialDayProfiles which are stored in the TimeTrigger element.
        /// </summary>
        /// <param name="ar">The main namespace.</param>
        /// <returns>A list of specialDayProfiles XElements.</returns>
        private List<XElement> SpecialDayProfiles(XNamespace ar)
        {
            var specialDayProfiles = new List<XElement>();
            var specialDayData = this.CreateSpecialDayProfiles(HanConfiguration);

            foreach (SpecialDayProfile profile in specialDayData)
            {
                var specialDayProfile = new XElement(ar + "SpecialDayProfile");

                specialDayProfile.Add(GetDayVarType(profile.SpecialDayDate, "specialDayDate", ar));

                specialDayProfile.Add(new XElement(ar + "dayId", profile.DayId));

                specialDayProfiles.Add(specialDayProfile);
            }

            return specialDayProfiles;
        }

        /// <summary>
        /// Creates a DayVarType element. They are used in the SpecialDayProfile XElements.
        /// </summary>
        /// <param name="dayVarType">Contains the needed data to create the xml elements.</param>
        /// <param name="typeName">The xml element name for th DayVarType.</param>
        /// <param name="ar">The main namespace.</param>
        /// <returns>A DayVarType XElement.</returns>
        private XElement GetDayVarType(DayVarType dayVarType, string typeName, XNamespace ar)
        {
            var type = new XElement(ar + typeName);

            type.Add(new XElement(ar + "year", dayVarType.Year));
            type.Add(new XElement(ar + "month", dayVarType.Month));
            type.Add(new XElement(ar + "day_of_month", dayVarType.DayOfMonth));

            return type;
        }

        /// <summary>
        /// Sets the needed namespaces for the xml file.
        /// </summary>
        private void SetNamespaces()
        {
            this.Namespaces = new List<(string name, XNamespace ns)>();

            XNamespace ar = XNamespace.Get("http://vde.de/AR_2418-6.xsd");
            Namespaces.Add(("ar", ar));

            XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
            Namespaces.Add(("xsi", xsi));

            XNamespace schemaLocation = XNamespace.Get("http://vde.de/AR_2418-6.xsd AR_2418-6.xsd");
            Namespaces.Add(("schemaLocation", schemaLocation));

            XNamespace espi = XNamespace.Get("http://naesb.org/espi");
            Namespaces.Add(("espi", espi));

            XNamespace atom = XNamespace.Get("http://www.w3.org/2005/Atom");
            Namespaces.Add(("atom", atom));   
        }
    }
}
