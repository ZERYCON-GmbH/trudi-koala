namespace TRuDI.HanAdapter.Test.Commands.AdapterList
{
    using Microsoft.Extensions.CommandLineUtils;

    public static class AdapterListCommandConfiguration
    {
        public static void Configure(CommandLineApplication command, CommandLineOptions options)
        {
            command.Description = "Generiert eine List mit allen bekannten HAN-Adaptern.";
            command.HelpOption("--help|-h|-?");

            command.OnExecute(() =>
            {
                options.Command = new AdapterListCommand(options);

                return 0;
            });

        }
    }
}