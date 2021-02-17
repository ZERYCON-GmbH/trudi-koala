namespace TRuDI.HanAdapter.Test.Commands
{
    using System;
    using System.Collections.Generic;

    using Serilog;

    using TRuDI.HanAdapter.Interface;

    public abstract class CommunicationCommandBase : ICommand
    {
        protected readonly CommandLineOptions CommonOptions;
        protected readonly CommonCommunicationConfiguration CommonCommunicationConfiguration;

        public CommunicationCommandBase(CommonCommunicationConfiguration commonCommunicationConfiguration, CommandLineOptions commonOptions)
        {
            this.CommonOptions = commonOptions;
            this.CommonCommunicationConfiguration = commonCommunicationConfiguration;
        }

        public abstract int Run();


        protected (ConnectResult result, AdapterError error) Connect()
        {
            var hanAdapter = this.CommonCommunicationConfiguration.HanAdapter;

            if (this.CommonCommunicationConfiguration.ClientCert != null)
            {
                return hanAdapter.Connect(
                    this.CommonCommunicationConfiguration.ServerId,
                    this.CommonCommunicationConfiguration.IpEndpoint,
                    this.CommonCommunicationConfiguration.ClientCert,
                    this.CommonCommunicationConfiguration.Password,
                    new Dictionary<string, string>(),
                    TimeSpan.FromSeconds(30),
                    this.CommonCommunicationConfiguration.CreateCancellationToken(),
                    this.ProgressCallback).Result;
            }

            return hanAdapter.Connect(
                this.CommonCommunicationConfiguration.ServerId,
                this.CommonCommunicationConfiguration.IpEndpoint,
                this.CommonCommunicationConfiguration.Username,
                this.CommonCommunicationConfiguration.Password,
                new Dictionary<string, string>(),
                TimeSpan.FromSeconds(30),
                this.CommonCommunicationConfiguration.CreateCancellationToken(),
                this.ProgressCallback).Result;
        }

        protected void ProgressCallback(ProgressInfo progressInfo)
        {
            Log.Information("Progress Callback: {@progressInfo}", progressInfo);
        }
    }
}