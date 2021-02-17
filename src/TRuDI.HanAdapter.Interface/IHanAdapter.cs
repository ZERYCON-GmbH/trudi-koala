namespace TRuDI.HanAdapter.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    /// <summary>
    /// Plugin interface for a TRuDI HAN device adapter.
    /// </summary>
    public interface IHanAdapter
    {
        /// <summary>
        /// Connect to SMGW and authenticate by username and password.
        /// </summary>
        /// <param name="deviceId">"Herstellerübergreifende Identifikationsnummer" (DIN 43863-5, e.g. "EXXX0012345678") of the SMGW</param>
        /// <param name="endpoint">The IP endpoint to conntect to (IP address and port)</param>
        /// <param name="user">The KAF username of the Letztverbraucher.</param>
        /// <param name="password">The KAF password of the Letztverbraucher.</param>
        /// <param name="manufacturerSettings">Optional manufacturer specific settings (coming from UI and going to driver).</param>
        /// <param name="timeout">Connect timeout.</param>
        /// <param name="ct">Token for user initiated cancellation.</param>
        /// <param name="progressCallback">This callback must be called regularly.</param>
        /// <returns>Cert is filled with the gateway's TLS certificate when the connection is established. Otherwise the error object exists.</returns>
        Task<(ConnectResult result, AdapterError error)> Connect(
            string deviceId,
            IPEndPoint endpoint,
            string user,
            string password,
            Dictionary<string, string> manufacturerSettings,
            TimeSpan timeout,
            CancellationToken ct,
            Action<ProgressInfo> progressCallback);

        /// <summary>
        /// Connect to SMGW and authenticate by client certificate.
        /// </summary>
        /// <param name="deviceId">"Herstellerübergreifende Identifikationsnummer" (DIN 43863-5, e.g. "EXXX0012345678") of the SMGW</param>
        /// <param name="endpoint">The IP endpoint to conntect to (IP address and port)</param>
        /// <param name="pkcs12Data">Client certificate with private key in PKCS#12 format</param>
        /// <param name="password">Password used to decrypt the PKCS#12 container</param>
        /// <param name="manufacturerSettings">Optional manufacturer settings.</param>
        /// <param name="timeout">Connect timeout.</param>
        /// <param name="ct">Token for user initiated cancellation.</param>
        /// <param name="progressCallback">This callback must be called regularly.</param>
        /// <returns>Cert is filled with the gateway's TLS certificate when the connection is established. Otherwise the error object exists.</returns>
        Task<(ConnectResult result, AdapterError error)> Connect(
            string deviceId,
            IPEndPoint endpoint,
            byte[] pkcs12Data,
            string password,
            Dictionary<string, string> manufacturerSettings,
            TimeSpan timeout,
            CancellationToken ct,
            Action<ProgressInfo> progressCallback);

        /// <summary>
        /// Read contract information from the SMGW. This method is called after the successful connect to the device.
        /// </summary>
        /// <param name="ct">Token for user initiated cancellation.</param>
        /// <param name="progressCallback">This callback must be called regularly.</param>
        /// <returns>List with contract information. If there's no contract for the user, a empty list should be returned.</returns>
        Task<(IReadOnlyList<ContractInfo> contracts, AdapterError error)> LoadAvailableContracts(
            CancellationToken ct,
            Action<ProgressInfo> progressCallback);

        /// <summary>
        /// Loads the data of the specified contract and billing period from the SMGW.
        /// </summary>
        /// <param name="ctx">A set of parameters which specifies what exactly is to be read out.</param>
        /// <param name="ct">Token for user initiated cancellation.</param>
        /// <param name="progressCallback">This callback must be called regularly.</param>
        /// <returns>On success, a XML document according to AR 2418-6.</returns>
        Task<(XDocument trudiXml, AdapterError error)> LoadData(
            AdapterContext ctx,
            CancellationToken ct,
            Action<ProgressInfo> progressCallback);

        /// <summary>
        /// Loads the current derived register values of the specified contract.
        /// </summary>
        /// <param name="contract">The contract to load the registers from.</param>
        /// <param name="ct">Token for user initiated cancellation.</param>
        /// <param name="progressCallback">This callback must be called regularly.</param>
        /// <returns>
        /// On success, a XML document according to AR 2418-6 containing a meter reading with the current tariff registers 
        /// (just like <see cref="LoadData"/>, but without original value list and log data).
        /// </returns>
        Task<(XDocument trudiXml, AdapterError error)> GetCurrentRegisterValues(
            ContractInfo contract,
            CancellationToken ct,
            Action<ProgressInfo> progressCallback);

        /// <summary>
        /// Closes the connection to the gateway.
        /// </summary>
        Task Disconnect();

        /// <summary>
        /// Gets the type of the view component that shows an image of the SMGW.
        /// </summary>
        Type SmgwImageViewComponent { get; }

        /// <summary>
        /// Gets the type of the view component that is used to enter additional parameters needed to connect to the SMGW.
        /// </summary>
        Type ManufacturerParametersViewComponent { get; }
    }
}