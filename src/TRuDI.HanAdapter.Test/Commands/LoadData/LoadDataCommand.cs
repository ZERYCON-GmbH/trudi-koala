namespace TRuDI.HanAdapter.Test.Commands.LoadData
{
    using System;
    using System.Linq;

    using TRuDI.HanAdapter.Interface;
    using TRuDI.HanAdapter.Test;
    using TRuDI.HanAdapter.Test.Commands;
    using TRuDI.Models;

    public class LoadDataCommand : CommunicationCommandBase
    {
        private readonly LoadDataConfiguration loadDataConfiguration;

        public LoadDataCommand(LoadDataConfiguration loadDataConfiguration, CommonCommunicationConfiguration commonCommunicationConfiguration, CommandLineOptions commonOptions)
            : base(commonCommunicationConfiguration, commonOptions)
        {
            this.loadDataConfiguration = loadDataConfiguration;
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

            var ctx = new AdapterContext();
            ctx.WithLogdata = true;

            ctx.Contract = contractsResult.contracts.FirstOrDefault(
                c =>
                    {
                        if (!string.IsNullOrWhiteSpace(this.loadDataConfiguration.UsagePointId))
                        {
                            if (this.loadDataConfiguration.UsagePointId != c.MeteringPointId)
                            {
                                return false;
                            }
                        }

                        if (this.loadDataConfiguration.UseTaf6 && c.TafId != TafId.Taf6)
                        {
                            return false;
                        }

                        return this.loadDataConfiguration.TariffName == c.TafName;
                    });

            if (ctx.Contract == null)
            {
                Console.WriteLine("Es wurde kein passender TAF gefunden.");
                return 2;
            }

            if (ctx.Contract.TafId != TafId.Taf7)
            {
                if (ctx.Contract.BillingPeriods == null || ctx.Contract.BillingPeriods.Count
                    <= this.loadDataConfiguration.BillingPeriodIndex)
                {
                    Console.WriteLine("Angegebene Abrechnungsperiode nicht gefunden.");
                    return 2;
                }

                ctx.BillingPeriod = ctx.Contract.BillingPeriods[this.loadDataConfiguration.BillingPeriodIndex];

                if (this.loadDataConfiguration.Start != null)
                {
                    ctx.Start = this.loadDataConfiguration.Start.Value;
                }
                else
                {
                    ctx.Start = ctx.BillingPeriod.Begin;
                }

                if (this.loadDataConfiguration.End != null)
                {
                    ctx.End = this.loadDataConfiguration.End.Value;
                }
                else if (ctx.BillingPeriod.End != null)
                {
                    ctx.End = ctx.BillingPeriod.End.Value;
                }
                else
                {
                    ctx.End = DateTime.Now;
                }
            }
            else
            {
                if (this.loadDataConfiguration.Start == null || this.loadDataConfiguration.End == null)
                {
                    Console.WriteLine("Bei TAF-7 muss ein Start- und End-Zeitpunkt angegeben werden.");
                    return 2;
                }

                ctx.Start = this.loadDataConfiguration.Start.Value;
                ctx.End = this.loadDataConfiguration.End.Value;
            }

            var loadDataResult = hanAdapter.LoadData(
                ctx,
                this.CommonCommunicationConfiguration.CreateCancellationToken(),
                this.ProgressCallback).Result;


            if (loadDataResult.error != null)
            {
                Console.WriteLine("LoadData failed: {0}", loadDataResult.error);
                return 2;
            }

            try
            {
                if (this.loadDataConfiguration.SkipValidation)
                {
                    Ar2418Validation.ValidateSchema(loadDataResult.trudiXml);
                    var model = XmlModelParser.ParseHanAdapterModel(loadDataResult.trudiXml.Root?.Descendants());
                    ModelValidation.ValidateHanAdapterModel(model);
                    ContextValidation.ValidateContext(model, ctx);
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

            loadDataResult.trudiXml.Save(this.CommonOptions.OutputFile);
            return 0;
        }
    }
}
