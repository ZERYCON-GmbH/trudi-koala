// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace IVU.Http.Headers.Managed
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;

    using IVU.Common.Tls;
    using IVU.Common.Tls.HandshakeLayer.Protocol;
    using IVU.Common.Tls.Plugin;
    using IVU.Http.Logging;

    internal sealed class HttpConnectionHandler : HttpMessageHandler
    {
        private static ILog logger = LogProvider.For<HttpConnectionHandler>();

        private readonly HttpConnectionSettings _settings;
        private readonly HttpConnectionPools _connectionPools;

        public HttpConnectionHandler(HttpConnectionSettings settings)
        {
            _settings = settings;
            _connectionPools = new HttpConnectionPools(settings._maxConnectionsPerServer);
        }

        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var key = new HttpConnectionKey(request.RequestUri);

            HttpConnectionPool pool = _connectionPools.GetOrAddPool(key);
            ValueTask<HttpConnection> connectionTask = pool.GetConnectionAsync(
                state => state.handler.CreateConnection(state.request, state.key, state.pool, cancellationToken),
                (handler: this, request: request, key: key, pool: pool));

            return connectionTask.IsCompletedSuccessfully ?
                connectionTask.Result.SendAsync(request, cancellationToken) :
                SendAsyncWithAwaitedConnection(connectionTask, request, cancellationToken);
        }

        private async Task<HttpResponseMessage> SendAsyncWithAwaitedConnection(
            ValueTask<HttpConnection> connectionTask, HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpConnection connection = await connectionTask.ConfigureAwait(false);
            return await connection.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        private async ValueTask<TlsStream> EstablishSslConnection(string host, HttpRequestMessage request, Stream stream, CancellationToken cancellationToken)
        {
            logger.Trace("HTTP connection handler: Establish TLS connection");

            var secParams = new SecurityParameters();
            secParams.CipherSuiteIDs.Add(CipherSuiteId.TLS_ECDHE_ECDSA_WITH_AES_128_CBC_SHA256);
            secParams.CipherSuiteIDs.Add(CipherSuiteId.TLS_ECDHE_ECDSA_WITH_AES_256_CBC_SHA384);
            secParams.CipherSuiteIDs.Add(CipherSuiteId.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256);
            secParams.CipherSuiteIDs.Add(CipherSuiteId.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384);
            secParams.MinimumVersion = ProtocolVersion.TLS1_2;
            secParams.MaximumVersion = ProtocolVersion.TLS1_2;

            if (this._settings._clientCertificate != null)
            {
                var pluginManager = new IVU.Common.Tls.Plugin.CipherSuitePluginInterface.CipherSuitePluginManager();
                var privateKey = pluginManager.GetPrivateKey(this._settings._clientCertificate.Key);
                secParams.AddCertificate(new X509CertificateCollection { this._settings._clientCertificate.Certificate }, privateKey);
            }

            secParams.ClientCertificateSelectionCallback += (client, server) =>
                {
                    return 0;
                };

            secParams.ServerCertificateValidationCallback = certificates =>
                {
                    if (this._settings?._serverCertificateCustomValidationCallback == null)
                    {
                        logger.Error("No server certificate validation callback provided!");
                        return false;
                    }

                    return this._settings?._serverCertificateCustomValidationCallback(
                        request,
                        new X509Certificate2(certificates[0]),
                        null,
                        SslPolicyErrors.None) == true;
                };

            var tlsSession = new SecureSession(stream, secParams);
            try
            {
                await tlsSession.PerformClientHandshake(cancellationToken);
                logger.Trace("HTTP connection handler: TLS connection successfull establish");
            }
            catch (Exception ex)
            {
                tlsSession.Close();
                logger.ErrorException("Failed to establish TLS connection: {0}", ex, ex.Message);
                throw new TlsConnectFailed("Failed to establish TLS connection", ex);
            }

            return new TlsStream(tlsSession);
        }

        private async ValueTask<HttpConnection> CreateConnection(HttpRequestMessage request, HttpConnectionKey key, HttpConnectionPool pool, CancellationToken cancellationToken)
        {
            logger.Trace("HTTP connection handler: Create connection");

            Uri uri = request.RequestUri;

            Stream stream = await ConnectHelper.ConnectAsync(uri.IdnHost, uri.Port).ConfigureAwait(false);

            TransportContext transportContext = null;

            if (uri.Scheme == UriScheme.Https)
            {
                stream = await this.EstablishSslConnection(uri.IdnHost, request, stream, cancellationToken).ConfigureAwait(false);
            }

            return new HttpConnection(pool, key, uri.IdnHost, stream, transportContext, false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connectionPools.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
