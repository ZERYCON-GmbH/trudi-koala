namespace TRuDI.HanAdapter.Test.Commands
{
    using Microsoft.Extensions.CommandLineUtils;

    using TRuDI.HanAdapter.Test.Commands.AdapterList;
    using TRuDI.HanAdapter.Test.Commands.ContractList;
    using TRuDI.HanAdapter.Test.Commands.CurrentRegisters;
    using TRuDI.HanAdapter.Test.Commands.LoadData;

    public static class RootCommandConfiguration
    {
        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            app.Command("adapter-list", c => AdapterListCommandConfiguration.Configure(c, options));
            app.Command("contract-list", c => ContractListCommandConfiguration.Configure(c, options));
            app.Command("load-data", c => LoadDataCommandConfiguration.Configure(c, options));
            app.Command("current-registers", c => CurrentRegistersCommandConfiguration.Configure(c, options));

            app.OnExecute(() =>
            {
                options.Command = new RootCommand(app);
                return 0;
            });
        }
    }
}