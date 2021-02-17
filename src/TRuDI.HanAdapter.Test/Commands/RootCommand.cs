namespace TRuDI.HanAdapter.Test.Commands
{
    using Microsoft.Extensions.CommandLineUtils;

    public class RootCommand : ICommand
    {
        private readonly CommandLineApplication app;

        public RootCommand(CommandLineApplication app)
        {
            this.app = app;
        }

        public int Run()
        {
            this.app.ShowHelp();
            return 1;
        }
    }
}
