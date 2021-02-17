namespace TRuDI.HanAdapter.Test.Commands.ContractList
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    using TRuDI.HanAdapter.Test;
    using TRuDI.HanAdapter.Test.Commands;
    using TRuDI.Models;

    public class ContractListCommand : CommunicationCommandBase
    {
        public ContractListCommand(CommonCommunicationConfiguration commonCommunicationConfiguration, CommandLineOptions commonOptions)
            : base(commonCommunicationConfiguration, commonOptions)
        {
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

            for (int i = 0; i < contractsResult.contracts.Count; i++)
            {
                var contract = contractsResult.contracts[i];
                Console.WriteLine($"{i} - Contract details:");
                Console.WriteLine($"    TafId:           {contract.TafId}");
                Console.WriteLine($"    TafName:         {contract.TafName}");
                Console.WriteLine($"    SupplierId:      {contract.SupplierId}");
                Console.WriteLine($"    ConsumerId:      {contract.ConsumerId}");
                Console.WriteLine($"    MeteringPointId: {contract.MeteringPointId}");
                Console.WriteLine($"    Begin:           {contract.Begin.ToIso8601Local()}");
                Console.WriteLine($"    End:             {(contract.End?.ToIso8601Local() ?? "-")}");
                Console.WriteLine($"    Meters:          {string.Join(",", contract.Meters)}");

                if (contract.BillingPeriods != null && contract.BillingPeriods.Count > 0)
                {
                    Console.WriteLine("    BillingPeriods:");
                    for (int j = 0; j < contract.BillingPeriods.Count; j++)
                    {
                        var period = contract.BillingPeriods[j];
                        Console.WriteLine(
                            $"      {j} - {period.Begin.ToIso8601Local()} to {(period.End?.ToIso8601Local() ?? "-")}");
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(this.CommonOptions.OutputFile))
            {
                return 0;
            }

            var contracts = contractsResult.contracts.Select(ci => new Contract(ci)).ToArray();
            var xs = new XmlSerializer(contracts.GetType());
            using (var tw = new StreamWriter(this.CommonOptions.OutputFile))
            {
                xs.Serialize(tw, contracts);
            }

            return 0;
        }
    }
}
