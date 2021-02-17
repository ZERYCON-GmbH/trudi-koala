namespace TRuDI.HanAdapter.Test.Commands.CurrentRegisters
{
    using System;
    using System.Linq;

    using TRuDI.HanAdapter.Interface;
    using TRuDI.HanAdapter.Test;
    using TRuDI.HanAdapter.Test.Commands;
    using TRuDI.Models;

    public class CurrentRegistersCommand : CommunicationCommandBase
    {
        private readonly CurrentRegistersConfiguration currentRegistersConfiguration;

        public CurrentRegistersCommand(
            CurrentRegistersConfiguration currentRegistersConfiguration, 
            CommonCommunicationConfiguration commonCommunicationConfiguration, 
            CommandLineOptions commonOptions)
            : base(commonCommunicationConfiguration, commonOptions)
        {
            this.currentRegistersConfiguration = currentRegistersConfiguration;
        }

        public override int Run()
        {
            var connectResult = this.Connect();
            if (connectResult.error != null)
            {
                Console.WriteLine("Connect failed: {0}", connectResult.error);
                return 2;
            }

            var hanAdapter = this.CommonCommunicationConfiguration.HanAdapter;

            var contractsResult = hanAdapter.LoadAvailableContracts(
                this.CommonCommunicationConfiguration.CreateCancellationToken(),
                this.ProgressCallback).Result;

            if (contractsResult.error != null)
            {
                Console.WriteLine("LoadAvailableContracts failed: {0}", contractsResult.error);
                return 2;
            }

            var contract = contractsResult.contracts.FirstOrDefault(
                c =>
                    {
                        if (!string.IsNullOrWhiteSpace(this.currentRegistersConfiguration.UsagePointId))
                        {
                            if (this.currentRegistersConfiguration.UsagePointId != c.MeteringPointId)
                            {
                                return false;
                            }
                        }

                        if (c.TafId == TafId.Taf6)
                        {
                            return false;
                        }

                        return this.currentRegistersConfiguration.TariffName == c.TafName;
                    });

            if (contract == null)
            {
                Console.WriteLine("Es wurde kein passender TAF gefunden.");
                return 2;
            }
            
            var currentRegisterResult = hanAdapter.GetCurrentRegisterValues(
                contract,
                this.CommonCommunicationConfiguration.CreateCancellationToken(),
                this.ProgressCallback).Result;


            if (currentRegisterResult.error != null)
            {
                Console.WriteLine("GetCurrentRegisterValues failed: {0}", currentRegisterResult.error);
                return 2;
            }

            try
            {
                if (this.currentRegistersConfiguration.SkipValidation)
                {
                    Ar2418Validation.ValidateSchema(currentRegisterResult.trudiXml);
                    var model = XmlModelParser.ParseHanAdapterModel(currentRegisterResult.trudiXml.Root?.Descendants());
                    ModelValidation.ValidateHanAdapterModel(model);
                    ContextValidation.ValidateContext(model, null);
                }
            }
            catch (AggregateException ex)
            {
                foreach (var err in ex.InnerExceptions)
                {
                    Console.WriteLine(err.Message);
                }

                return 2;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 2;
            }
            
            if (string.IsNullOrWhiteSpace(this.CommonOptions.OutputFile))
            {
                return 0;
            }

            currentRegisterResult.trudiXml.Save(this.CommonOptions.OutputFile);
            return 0;
        }
    }
}
