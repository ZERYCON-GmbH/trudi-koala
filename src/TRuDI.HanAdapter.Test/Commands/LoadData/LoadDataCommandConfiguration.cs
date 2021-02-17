namespace TRuDI.HanAdapter.Test.Commands.LoadData
{
    using System;

    using Microsoft.Extensions.CommandLineUtils;

    public static class LoadDataCommandConfiguration
    {
        public static void Configure(CommandLineApplication command, CommandLineOptions options)
        {
            command.Description = "Liest die Daten des angegebenen Vertrags aus dem SMGW.";
            command.HelpOption("--help|-h|-?");

            var communicationConfiguration = new CommonCommunicationConfiguration();
            communicationConfiguration.Init(command);

            var loadDataConfiguration = new LoadDataConfiguration();
            loadDataConfiguration.Init(command);

            command.OnExecute(() =>
                {
                    try
                    {
                        communicationConfiguration.VerifyParameters();
                        loadDataConfiguration.VerifyParameters();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return 2;
                    }
                    
                    options.Command = new LoadDataCommand(loadDataConfiguration, communicationConfiguration, options);

                    return 0;
                });
        }
    }
}