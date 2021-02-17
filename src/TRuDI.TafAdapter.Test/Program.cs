namespace TRuDI.TafAdapter.Test
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml.Linq;

    using Microsoft.Extensions.CommandLineUtils;

    using TRuDI.HanAdapter.Interface;
    using TRuDI.Models;
    using TRuDI.Models.BasicData;
    using TRuDI.TafAdapter.Interface;
    using TRuDI.TafAdapter.Repository;
    using TRuDI.TafAdapter.Taf1;
    using TRuDI.TafAdapter.Taf2;

    class Program
    {
        private static int Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "TRuDI.TafAdapter.Test",
                FullName = "TRuDI TAF Adapter Test Application"
            };

            var outputFile = app.Option("-o|--output <output-file>", "Ausgabedatei.", CommandOptionType.SingleValue);
            var dataFile = app.Option("-d|--data <data-file>", "Daten aus dem SMGW.", CommandOptionType.SingleValue);
            var tariffFile = app.Option("-t|--taf <taf-file>", "TAF-Datei.", CommandOptionType.SingleValue);

            app.HelpOption("-? | -h | --help");
            app.Execute(args);

            if (!dataFile.HasValue())
            {
                Console.WriteLine("Keine Datendatei angegeben.");
                return 1;
            }

            var data = LoadDataFile(dataFile.Value());
            if (data == null)
            {
                return 1;
            }

            if (!tariffFile.HasValue())
            {
                Console.WriteLine("Keine TAF-Datei angegeben.");
                return 1;
            }

            var tafData = LoadTafFile(tariffFile.Value());
            if (tafData == null)
            {
                return 1;
            }

            var tafAdapter = TafAdapterRepository.LoadAdapter(tafData.AnalysisProfile.TariffUseCase);

            try
            {
                var result = tafAdapter.Calculate(data, tafData);
                string formattedResult;

                switch (tafData.AnalysisProfile.TariffUseCase)
                {
                    case TafId.Taf1:
                        formattedResult = FormatTaf1(result);
                        break;

                    case TafId.Taf2:
                        formattedResult = FormatTaf2(result);
                        break;

                    default:
                        Console.WriteLine($"{tafData.AnalysisProfile.TariffUseCase} kann mit diesem Programm nicht verarbeitet werden.");
                        return 1;
                }

                Console.WriteLine(formattedResult);

                if (outputFile.HasValue())
                {
                    File.WriteAllText(outputFile.Value(), formattedResult);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }

            return 0;
        }

        private static string FormatTaf2(TafAdapterData result)
        {
            var data = result.Data as Taf2Data;
            var sb = new StringBuilder();

            sb.AppendLine("TAF-2");
            sb.AppendLine($"Begin: {data.Begin.ToIso8601Local()}");
            sb.AppendLine($"End:   {data.End.ToIso8601Local()}");

            foreach (var reg in data.SummaryRegister)
            {
                sb.AppendLine($"   {reg.ObisCode.ToString().PadRight(16)}  {reg.Amount}");
            }
            
            foreach (var acs in data.AccountingSections)
            {
                sb.AppendLine();
                sb.AppendLine(acs.Start.ToIso8601Local());

                foreach (var mr in acs.MeasuringRanges)
                {
                    sb.AppendLine($"   {mr.Start} - {mr.End}   {mr.TariffId.ToString().PadRight(3)}   {mr.Amount}");
                }

                sb.AppendLine();

                foreach (var reg in acs.SummaryRegister)
                {
                    sb.AppendLine($"   {reg.ObisCode.ToString().PadRight(16)}  {reg.Amount}");
                }
            }

            return sb.ToString();
        }

        private static string FormatTaf1(TafAdapterData result)
        {
            var data = result.Data as Taf1Data;
            var sb = new StringBuilder();

            sb.AppendLine("TAF-1");
            sb.AppendLine($"Begin: {data.Begin.ToIso8601Local()}");
            sb.AppendLine($"End:   {data.End.ToIso8601Local()}");

            foreach (var reg in data.SummaryRegister)
            {
                sb.AppendLine($"   {reg.ObisCode.ToString().PadRight(16)}  {reg.Amount}");
            }

            return sb.ToString();
        }

        private static UsagePointAdapterTRuDI LoadDataFile(string filename)
        {
            try
            {
                var xml = XDocument.Load(filename);

                Ar2418Validation.ValidateSchema(xml);
                var model = XmlModelParser.ParseHanAdapterModel(xml.Root?.Descendants());
                ModelValidation.ValidateHanAdapterModel(model);
                ContextValidation.ValidateContext(model, null);

                return model;
            }
            catch (AggregateException ex)
            {
                foreach (var err in ex.InnerExceptions)
                {
                    Console.WriteLine(err.Message);
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private static UsagePointLieferant LoadTafFile(string filename)
        {
            try
            {
                var xml = XDocument.Load(filename);

                Ar2418Validation.ValidateSchema(xml);
                var model = XmlModelParser.ParseSupplierModel(xml.Root?.Descendants());
                ModelValidation.ValidateSupplierModel(model);

                return model;
            }
            catch (AggregateException ex)
            {
                foreach (var err in ex.InnerExceptions)
                {
                    Console.WriteLine(err.Message);
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
