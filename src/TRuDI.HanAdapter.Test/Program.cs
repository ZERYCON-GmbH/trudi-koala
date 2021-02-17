namespace TRuDI.HanAdapter.Test
{
    using System;
    using System.Text;

    class Program
    {
        private static int Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var options = CommandLineOptions.Parse(args);
            if (options?.Command == null)
            {
                return 1;
            }

            try
            {
                return options.Command.Run();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Vorgang wurde abgebrochen.");
                return 2;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 2;
            }
        }
    }
}
