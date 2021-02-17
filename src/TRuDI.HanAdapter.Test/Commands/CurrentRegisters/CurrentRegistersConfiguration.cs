namespace TRuDI.HanAdapter.Test.Commands.CurrentRegisters
{
    using System;

    using Microsoft.Extensions.CommandLineUtils;

    public class CurrentRegistersConfiguration
    {
        private CommandOption usagePointId;
        private CommandOption tariffName;
        private CommandOption skipValidation;

        public string UsagePointId { get; private set; }
        public string TariffName { get; private set; }
        public bool SkipValidation { get; private set; }

        public void Init(CommandLineApplication app)
        {
            this.usagePointId = app.Option("--usagepointid <usagePointId>", "Messlokation (optional)", CommandOptionType.SingleValue);
            this.tariffName = app.Option("--tariffname <tariffName>", "Identifikation des Tarifs", CommandOptionType.SingleValue);
            this.skipValidation = app.Option("--skip-validation", "XML-Validierung nicht durchführen", CommandOptionType.NoValue);
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
            this.SkipValidation = this.skipValidation.HasValue();
        }
    }
}