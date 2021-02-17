namespace TRuDI.HanAdapter.Test.Commands.ContractList
{
    using System;

    using Microsoft.Extensions.CommandLineUtils;

    public static class ContractListCommandConfiguration
    {
        public static void Configure(CommandLineApplication command, CommandLineOptions options)
        {
            command.Description = "Liest die Liste der für den Benutzer verfügbaren Verträge aus dem SMGW.";
            command.HelpOption("--help|-h|-?");

            var communicationConfiguration = new CommonCommunicationConfiguration();

            communicationConfiguration.Init(command);

            command.OnExecute(() =>
                {
                    try
                    {
                        communicationConfiguration.VerifyParameters();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return 2;
                    }

                    options.Command = new ContractListCommand(communicationConfiguration, options);

                    return 0;
                });
        }
    }
}