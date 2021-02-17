namespace TRuDI.HanAdapter.Test
{
    using Microsoft.Extensions.CommandLineUtils;

    using Serilog;
    using Serilog.Core;
    using Serilog.Events;

    using TRuDI.HanAdapter.Test.Commands;

    public class CommandLineOptions
    {
        public static CommandLineOptions Parse(string[] args)
        {
            var options = new CommandLineOptions();

            var app = new CommandLineApplication
            {
                Name = "TRuDI.HanAdapter.Test",
                FullName = "TRuDI HAN Adapter Test Application"
            };

            app.HelpOption("-?|-h|--help");

            var logFileOption = app.Option("-l|--log <log-file>", "Logmeldungen werden in die angegebene Datei geschrieben.", CommandOptionType.SingleValue);
            var logConsole = app.Option("--log-console", "Logmeldungen werden auf der Konsole ausgegeben.", CommandOptionType.NoValue);
            var logLevelOption = app.Option("--loglevel <log-level>", "Log Level: verbose, debug, info, warning, error, fatal. Standard ist info.", CommandOptionType.SingleValue);
            var testFileOption = app.Option("-t|--test <test-config>", "Aktiviert den Test-HAN-Adapter mit der angegebenen Konfigurationsdatei.", CommandOptionType.SingleValue);

            var outputFile = app.Option("-o|--output <output-file>", "Ausgabedatei.", CommandOptionType.SingleValue);
            
            RootCommandConfiguration.Configure(app, options);

            var result = app.Execute(args);

            if (result != 0)
            {
                return null;
            }

            // Init logging...
            var logLevelSwitch = new LoggingLevelSwitch { MinimumLevel = LogEventLevel.Information };
            if (logLevelOption.HasValue())
            {
                switch (logLevelOption.Value().ToLowerInvariant())
                {
                    case "verbose":
                        logLevelSwitch.MinimumLevel = LogEventLevel.Verbose;
                        break;

                    case "debug":
                        logLevelSwitch.MinimumLevel = LogEventLevel.Debug;
                        break;

                    case "info":
                        logLevelSwitch.MinimumLevel = LogEventLevel.Information;
                        break;

                    case "warning":
                        logLevelSwitch.MinimumLevel = LogEventLevel.Warning;
                        break;

                    case "error":
                        logLevelSwitch.MinimumLevel = LogEventLevel.Error;
                        break;

                    case "fatal":
                        logLevelSwitch.MinimumLevel = LogEventLevel.Fatal;
                        break;
                }
            }

            if (logFileOption.HasValue())
            {
                if (logConsole.HasValue())
                {
                    Log.Logger = new LoggerConfiguration().MinimumLevel.ControlledBy(logLevelSwitch)
                        .WriteTo.Trace()
                        .WriteTo.ColoredConsole()
                        .WriteTo.File(logFileOption.Value())
                        .CreateLogger();
                }
                else
                {
                    Log.Logger = new LoggerConfiguration().MinimumLevel.ControlledBy(logLevelSwitch)
                        .WriteTo.Trace()
                        .WriteTo.File(logFileOption.Value())
                        .CreateLogger();
                }
            }
            else
            {
                if (logConsole.HasValue())
                {
                    Log.Logger = new LoggerConfiguration().MinimumLevel.ControlledBy(logLevelSwitch)
                        .WriteTo.Trace()
                        .WriteTo.ColoredConsole()
                        .CreateLogger();
                }
                else
                {
                    Log.Logger = new LoggerConfiguration().MinimumLevel.ControlledBy(logLevelSwitch)
                        .WriteTo.Trace()
                        .CreateLogger();
                }
            }

            // Output file
            if (outputFile.HasValue())
            {
                options.OutputFile = outputFile.Value();
            }

            return options;
        }

        public ICommand Command { get; set; }

        public string OutputFile { get; set; }
    }
}
