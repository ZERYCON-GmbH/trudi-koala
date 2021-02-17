namespace TRuDI.HanAdapter.Test.Commands.LoadData
{
    using System;

    using Microsoft.Extensions.CommandLineUtils;

    public class LoadDataConfiguration
    {
        private CommandOption usagePointId;
        private CommandOption tariffName;
        private CommandOption billingperiod;
        private CommandOption start;
        private CommandOption end;
        private CommandOption skipValidation;
        private CommandOption useTaf6;

        public string UsagePointId { get; private set; }
        public string TariffName { get; private set; }
        public int BillingPeriodIndex { get; private set; }
        public DateTime? Start { get; private set; }
        public DateTime? End { get; private set; }
        public bool SkipValidation { get; private set; }
        public bool UseTaf6 { get; private set; }

        public void Init(CommandLineApplication app)
        {
            this.usagePointId = app.Option("--usagepointid <usagePointId>", "Messlokation (optional)", CommandOptionType.SingleValue);
            this.tariffName = app.Option("--tariffname <tariffName>", "Identifikation des Tarifs", CommandOptionType.SingleValue);
            this.billingperiod = app.Option("--billingperiod <index>", "Index der Abrechnungsperiode (bei TAF-7 nicht benötigt)", CommandOptionType.SingleValue);
            this.start = app.Option("--start <start>", "Zeitstempel, formatiert nach ISO8601", CommandOptionType.SingleValue);
            this.end = app.Option("--end <end>", "Zeitstempel, formatiert nach ISO8601", CommandOptionType.SingleValue);
            this.skipValidation = app.Option("--skip-validation", "XML-Validierung nicht durchführen", CommandOptionType.NoValue);
            this.useTaf6 = app.Option("--taf6", "TAF-6-Abrechnungsperiode verwenden (nicht bei TAF-7)", CommandOptionType.NoValue);
        }

        public void VerifyParameters()
        {
            if (this.usagePointId.HasValue())
            {
                this.UsagePointId = this.usagePointId.Value();
            }

            if (!this.tariffName.HasValue())
            {
                throw new Exception("Kein Tarifname angegeben.");
            }

            this.TariffName = this.tariffName.Value();

            if (this.billingperiod.HasValue())
            {
                if (uint.TryParse(this.billingperiod.Value(), out var bpi))
                {
                    this.BillingPeriodIndex = (int)bpi;
                }
            }

            if (this.start.HasValue())
            {
                if (DateTime.TryParse(this.start.Value(), out var s))
                {
                    this.Start = s;
                }
                else
                {
                    throw new Exception($"Ungültiger Start-Zeitstempel: {this.start.Value()}");
                }
            }

            if (this.end.HasValue())
            {
                if (DateTime.TryParse(this.end.Value(), out var e))
                {
                    this.End = e;
                }
                else
                {
                    throw new Exception($"Ungültiger End-Zeitstempel: {this.end.Value()}");
                }
            }

            if (this.Start.HasValue && this.End.HasValue && this.Start >= this.End)
            {
                throw new Exception("Start muss kleiner als End sein.");
            }

            this.SkipValidation = this.skipValidation.HasValue();
            this.UseTaf6 = this.useTaf6.HasValue();
        }
    }
}