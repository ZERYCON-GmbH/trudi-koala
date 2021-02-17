namespace TRuDI.HanAdapter.Test.Commands.CurrentRegisters
{
    using System;

    using Microsoft.Extensions.CommandLineUtils;

    public static class CurrentRegistersCommandConfiguration
    {
        public static void Configure(CommandLineApplication command, CommandLineOptions options)
        {
            command.Description = "Liest die aktuellen Registerwerte für den angegebenen Vertrag aus dem SMGW.";
            command.HelpOption("--help|-h|-?");

            var communicationConfiguration = new CommonCommunicationConfiguration();
            communicationConfiguration.Init(command);

            var currentRegistersConfiguration = new CurrentRegistersConfiguration();
            currentRegistersConfiguration.Init(command);

            command.OnExecute(() =>
                {
                    try
                    {
                        communicationConfiguration.VerifyParameters();
                        currentRegistersConfiguration.VerifyParameters();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return 2;
                    }

                    options.Command = new CurrentRegistersCommand(currentRegistersConfiguration, communicationConfiguration, options);

                    return 0;
                });
        }
    }
}