namespace TRuDI.HanAdapter.Example.ConfigModel
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;

    using TRuDI.HanAdapter.Interface;

    /// <summary>
    /// The configuration model for the dummy HanAdapter
    /// </summary>
    public class HanAdapterExampleConfig
    {
        // This elements are needed for the connection simulation. 
        public string DeviceId { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public String IPAddress { get; set; }

        public int IPPort { get; set; }

        public TimeSpan TimeToConnect { get; set; }

        public string Cert { get; set; }

        public FirmwareVersion Version { get; set; }

        //Context data. For the configuration of the adapter context.
        public List<ContractInfo> Contracts { get; set; }

        public BillingPeriod BillingPeriod { get; set; }

        public DateTime? Start { get; set; }

        public DateTime? End { get; set; }

        public bool WithLogData { get; set; }

        public ContractInfo Contract { get; set; }

        //Configuration for the XmlFactory
        public XmlConfig XmlConfig { get; set; }

        //If Taf7 is used, the created supplier data will be stored in this XDocument.
        public XDocument SupplierXml { get; set; }
    }
}
