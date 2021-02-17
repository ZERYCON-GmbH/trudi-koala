namespace TRuDI.HanAdapter.Example.ConfigModel
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using TRuDI.Models.BasicData;

    /// <summary>
    /// The configuration model for the automatically creation of the xml data.
    /// For Taf7 a supplier xml will be created too.
    /// </summary>
    public class XmlConfig
    {
        public string UsagePointId { get; set; }

        public string TariffName { get; set; }

        public string TariffId { get; set; }

        public string CustomerId { get; set; }

        public string InvoicingPartyId { get; set; }

        public string SmgwId { get; set; }

        public ushort ServiceCategoryKind { get; set; }

        public List<CertificateContainer> Certificates { get; set; }

        public int MinLogCount { get; set; }

        public int MaxLogCount { get; set; }

        public List<string> PossibleLogMessages { get; set; }

        public int TariffUseCase { get; set; }

        public int TariffStageCount { get; set; }

        public int DefaultTariffNumber { get; set; }

        public int MaxPeriodUsage { get; set; }

        public int MaxConsumptionValue { get; set; }

        public List<int[]> Taf1Reg { get; set; }

        public List<MeterReadingConfig> MeterReadingConfigs { get; set; }

        public List<TariffStageConfig> TariffStageConfigs { get; set; }

        public List<DayProfileConfig> DayProfiles { get; set; }
    }

    public class CertificateContainer
    {
        public Certificate Certificate { get; set; }

        public string CertContent { get; set; }
    }

    public class MeterReadingConfig
    {
        public string MeterId { get; set; }
    
        public string MeterReadingId { get; set; }

        public bool IsOML { get; set; }

        public int OMLInitValue { get; set; }

        public int Taf2TariffStages { get; set; }

        public int Taf2TariffStage { get; set; }

        public uint? PeriodSeconds { get; set; }

        public byte Taf1PeriodInMonth { get; set; }

        public short? PowerOfTenMultiplier { get; set; }

        public ushort? Uom { get; set; }

        public short? Scaler { get; set; }

        public string ObisCode { get; set; }

        public string UsedStatus { get; set; }
    }

    public class TariffStageConfig
    {
        public int? TariffNumber { get; set; }

        public string Description { get; set; } 

        public string ObisCode { get; set; }
    }

    public class DayProfileConfig
    {
        public ushort? DayId { get; set; }

        public List<DayTimeProfileConfig> DayTimeProfiles { get; set; }
    }

    public class DayTimeProfileConfig
    {
        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public ushort? TariffNumber { get; set; }
    }
}
