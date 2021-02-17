namespace TRuDI.HanAdapter.Test.Commands.AdapterList
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    using TRuDI.HanAdapter.Repository;
    using TRuDI.HanAdapter.Test;
    using TRuDI.HanAdapter.Test.Commands;

    public class AdapterListCommand : ICommand
    {
        private readonly CommandLineOptions commonOptions;

        public AdapterListCommand(CommandLineOptions commonOptions)
        {
            this.commonOptions = commonOptions;
        }

        public int Run()
        {
            var exportList = new List<Adapter>();

            foreach (var adapter in HanAdapterRepository.AvailableAdapters)
            {
                Console.WriteLine($"{adapter.FlagId}\t{adapter.ManufacturerName}\t{adapter.BaseNamespace}");

                exportList.Add(
                    new Adapter()
                        {
                            FlagId = adapter.FlagId,
                            ManufacturerName = adapter.ManufacturerName,
                            BaseNamespace = adapter.BaseNamespace,
                        });
            }

            if (string.IsNullOrWhiteSpace(this.commonOptions.OutputFile))
            {
                return 0;
            }

            try
            {
                var xs = new XmlSerializer(exportList.GetType());
                using (var tw = new StreamWriter(this.commonOptions.OutputFile))
                {
                    xs.Serialize(tw, exportList);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }

            return 0;
        }
    }
}
