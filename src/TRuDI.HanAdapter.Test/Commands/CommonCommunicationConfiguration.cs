namespace TRuDI.HanAdapter.Test.Commands
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading;

    using Microsoft.Extensions.CommandLineUtils;

    using TRuDI.HanAdapter.Interface;
    using TRuDI.HanAdapter.Repository;
    using TRuDI.Models;

    public class CommonCommunicationConfiguration
    {
        private CommandOption username;
        private CommandOption password;
        private CommandOption pkcs12file;
        private CommandOption serverId;
        private CommandOption address;
        private CommandOption port;
        private CommandOption timeout;
        
        public string ServerId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public byte[] ClientCert { get; set; }
        public IPEndPoint IpEndpoint { get; set; }
        public uint Timeout { get; set; }
        public IHanAdapter HanAdapter { get; set; }

        public void Init(CommandLineApplication app)
        {
            this.username = app.Option("--user <username>", "Benutzername", CommandOptionType.SingleValue);
            this.pkcs12file = app.Option("--cert <cert-file>", "PKCS#12-Datei mit Client-Zertifikat und dazugehörigen Key", CommandOptionType.SingleValue);
            this.password = app.Option("--pass <password>", "Passwort zum Benutzernamen oder ggf. für die PKCS#12-Datei.", CommandOptionType.SingleValue);

            this.serverId = app.Option("--id <serverid>", "Herstellerübergreifende ID des SMGW (z.B. \"EABC0012345678\")", CommandOptionType.SingleValue);
            this.address = app.Option("--addr <address>", "IP-Adresse des SMGW.", CommandOptionType.SingleValue);
            this.port = app.Option("--port <port>", "Port des SMGW.", CommandOptionType.SingleValue);

            this.timeout = app.Option("--timeout <timeout>", "Timeout in Sekunden nachdem der Vorgang über das CancellationToken abgebrochen wird.", CommandOptionType.SingleValue);
        }

        public void VerifyParameters()
        {
            if (this.pkcs12file.HasValue())
            {
                if (File.Exists(this.pkcs12file.Value()))
                {
                    this.ClientCert = File.ReadAllBytes(this.pkcs12file.Value());

                    if (this.password.HasValue())
                    {
                        this.Password = this.password.Value();
                    }
                }
                else
                {
                    throw new Exception($"Specified PKCS#12 file wasn't found: {this.pkcs12file.Value()}");
                }
            }
            else
            {
                if (!this.username.HasValue() || !this.password.HasValue())
                {
                    throw new Exception("No username/password or client certificate was specified.");
                }

                this.Username = this.username.Value();
                this.Password = this.password.Value();
            }

            if (!this.serverId.HasValue())
            {
                throw new Exception("No device ID was specified.");
            }

            var id = new ServerId(this.serverId.Value());
            if (!id.IsValid || id.Medium != ObisMedium.Communication)
            {
                throw new Exception($"Invalid device id: {this.serverId.Value()}");
            }

            this.ServerId = this.serverId.Value();

            if (!this.address.HasValue())
            {
                throw new Exception("No IP address was specified.");
            }

            if (!IPAddress.TryParse(this.address.Value(), out var ip))
            {
                throw new Exception($"Invalid IP address: {this.address.Value()}");
            }

            if (!this.port.HasValue())
            {
                throw new Exception("No port was specified.");
            }

            if (!ushort.TryParse(this.port.Value(), out var p))
            {
                throw new Exception($"Invalid port: {this.port.Value()}");
            }

            this.IpEndpoint = new IPEndPoint(ip, p);

            if (this.timeout.HasValue())
            {
                if (!uint.TryParse(this.timeout.Value(), out var t))
                {
                    throw new Exception($"Invalid timeout: {this.timeout.Value()}");
                }

                this.Timeout = t;
            }

            try
            {
                var hanAdapterInfo = HanAdapterRepository.LoadAdapter(this.ServerId);
                this.HanAdapter = hanAdapterInfo.CreateInstance();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load the HAN adapter: {ex.Message}", ex);
            }
        }

        public CancellationToken CreateCancellationToken()
        {
            if (this.Timeout == 0)
            {
                return CancellationToken.None;
            }

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(this.Timeout));
            return cts.Token;
        }
    }
}