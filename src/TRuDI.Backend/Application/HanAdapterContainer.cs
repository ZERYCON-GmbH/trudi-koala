namespace TRuDI.Backend.Application
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using Serilog;

    using TRuDI.Backend.Exceptions;
    using TRuDI.Backend.Models;
    using TRuDI.HanAdapter.Example;
    using TRuDI.HanAdapter.Interface;
    using TRuDI.HanAdapter.Repository;

    /// <summary>
    /// Wrapper class for HAN adapters.
    /// </summary>
    public class HanAdapterContainer
    {
        /// <summary>
        /// The HAN adapter information.
        /// </summary>
        private readonly HanAdapterInfo hanAdapterInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="HanAdapterContainer"/> class.
        /// </summary>
        /// <param name="hanAdapterInfo">The HAN adapter information.</param>
        /// <param name="deviceId">The device identifier.</param>
        public HanAdapterContainer(HanAdapterInfo hanAdapterInfo, string deviceId)
        {
            this.DeviceId = deviceId;
            this.hanAdapterInfo = hanAdapterInfo;

            if (this.hanAdapterInfo.Name == nameof(HanAdapterExample))
            {
                this.Adapter = new HanAdapterExample(Program.CommandLineArguments.TestConfiguration);
            }
            else
            {
                this.Adapter = hanAdapterInfo.CreateInstance();
            }
        }

        /// <summary>
        /// Gets the HAN adapter instance.
        /// </summary>
        public IHanAdapter Adapter { get; }

        /// <summary>
        /// Gets the device identifier used for this HAN adapter.
        /// </summary>
        public string DeviceId { get; }

        /// <summary>
        /// Gets the gateway image view of the HAN adapter.
        /// </summary>
        public string GatewayImageView => this.Adapter?.SmgwImageViewComponent?.Name;

        /// <summary>
        /// Gets the manufacturer parameters view of the HAN adapter.
        /// </summary>
        public string ManufacturerParametersView => this.Adapter?.ManufacturerParametersViewComponent?.Name;

        /// <summary>
        /// Gets the resource file from the HAN adapter.
        /// </summary>
        /// <param name="path">The path to the resource file.</param>
        /// <returns>Tuple with the content data and the content MIME type.</returns>
        /// <exception cref="FileNotFoundException">Resource file wasn't found</exception>
        public (byte[] data, string contentType) GetResourceFile(string path)
        {
            var resourceName = this.hanAdapterInfo.BaseNamespace + "." + path.Replace('/', '.');
            var stream = this.hanAdapterInfo.Assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                Log.Error("Resource file wasn't found: {0}, path: {1}", resourceName, path);
                throw new FileNotFoundException("Resource file wasn't found", resourceName);
            }

            var data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);

            var contentType = string.Empty;
            if (path.EndsWith(".png"))
            {
                contentType = "image/png";
            }
            else if (path.EndsWith(".jpg") || path.EndsWith(".jpeg"))
            {
                contentType = "image/jpeg";
            }

            return (data, contentType);
        }

        /// <summary>
        /// Connects to the SMGW using this HAN adapter instance.
        /// </summary>
        /// <param name="connectData">The connect data.</param>
        /// <param name="clientCert">The client cert.</param>
        /// <param name="manufacturerParameters">The manufacturer parameters.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <param name="progressCallback">The progress callback.</param>
        /// <returns>The connect result.</returns>
        /// <exception cref="HanAdapterException">AdapterError from the HAN adapter.</exception>
        public async Task<ConnectResult> Connect(
            ConnectData connectData,
            CertData clientCert,
            Dictionary<string, string> manufacturerParameters,
            CancellationToken ct,
            Action<ProgressInfo> progressCallback)
        {
            var endpoint = new IPEndPoint(IPAddress.Parse(connectData.Address), connectData.Port);
            (ConnectResult result, AdapterError error) connectResult;

            switch (connectData.AuthMode)
            {
                case AuthMode.UserPassword:
                    connectResult = await this.Adapter.Connect(
                                        connectData.DeviceId,
                                        endpoint,
                                        connectData.Username,
                                        connectData.Password,
                                        manufacturerParameters,
                                        TimeSpan.FromSeconds(30),
                                        ct,
                                        progressCallback);
                    break;

                case AuthMode.ClientCertificate:
                    connectResult = await this.Adapter.Connect(
                                        connectData.DeviceId,
                                        endpoint,
                                        clientCert.Data,
                                        clientCert.Password,
                                        manufacturerParameters,
                                        TimeSpan.FromSeconds(30),
                                        ct,
                                        progressCallback);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(connectData.AuthMode));
            }

            if (connectResult.error != null)
            {
                throw new HanAdapterException(connectResult.error);
            }

            return connectResult.result;
        }

        /// <summary>
        /// Loads the data from the SMGW related to the specified adapter context.
        /// </summary>
        /// <param name="ctx">The adapter context with contract and billing period to read.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <param name="progressCallback">The progress callback.</param>
        /// <returns>The XML document according to AR 2418-6.</returns>
        /// <exception cref="HanAdapterException">AdapterError from the HAN adapter.</exception>
        public async Task<XDocument> LoadData(AdapterContext ctx, CancellationToken ct, Action<ProgressInfo> progressCallback)
        {
            var result = await this.Adapter.LoadData(ctx, ct, progressCallback);

            if (result.error != null)
            {
                throw new HanAdapterException(result.error);
            }

            return result.trudiXml;
        }

        /// <summary>
        /// Gets the current register values of the specified adapter context.
        /// </summary>
        /// <param name="ctx">The adapter context with contract and billing period to read.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <param name="progressCallback">The progress callback.</param>
        /// <returns>The XML document according to AR 2418-6.</returns>
        /// <exception cref="HanAdapterException">AdapterError from the HAN adapter.</exception>
        public async Task<XDocument> GetCurrentRegisterValues(AdapterContext ctx, CancellationToken ct, Action<ProgressInfo> progressCallback)
        {
            var result = await this.Adapter.GetCurrentRegisterValues(ctx.Contract, ct, progressCallback);

            if (result.error != null)
            {
                throw new HanAdapterException(result.error);
            }

            return result.trudiXml;
        }

        /// <summary>
        /// Loads the available contracts.
        /// </summary>
        /// <param name="ct">The cancellation token.</param>
        /// <param name="progressCallback">The progress callback.</param>
        /// <returns>List of contracts.</returns>
        /// <exception cref="HanAdapterException">AdapterError from the HAN adapter.</exception>
        public async Task<IReadOnlyList<ContractInfo>> LoadAvailableContracts(CancellationToken ct, Action<ProgressInfo> progressCallback)
        {
            var result = await this.Adapter.LoadAvailableContracts(ct, progressCallback);

            if (result.error != null)
            {
                throw new HanAdapterException(result.error);
            }

            if (result.contracts != null)
            {
                // Adjust billing period ends to the end of the contract
                foreach (var contract in result.contracts)
                {
                    if (contract.End != null && contract.End < DateTime.Now && contract.BillingPeriods != null)
                    {
                        foreach (var billingPeriod in contract.BillingPeriods)
                        {
                            if (billingPeriod.End == null)
                            {
                                billingPeriod.End = contract.End;
                            }
                        }
                    }
                }
            }

            return result.contracts;
        }
    }
}
