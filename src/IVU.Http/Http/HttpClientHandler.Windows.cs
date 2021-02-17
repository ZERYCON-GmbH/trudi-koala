// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace IVU.Http
{
    using System;
    using System.Net;

    using IVU.Http.Headers.Managed;
    using IVU.Http.Http;

    // This implementation uses the System.Net.Http.WinHttpHandler class on Windows.  Other platforms will need to use
    // their own platform specific implementation.
    public partial class HttpClientHandler : HttpMessageHandler
    {
        private readonly ManagedHandler _managedHandler;

        public HttpClientHandler()
        {
            if (UseManagedHandler)
            {
                _managedHandler = new ManagedHandler();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                _managedHandler.Dispose();
            }

            base.Dispose(disposing);
        }

        public virtual bool SupportsAutomaticDecompression => true;
        public virtual bool SupportsProxy => true;
        public virtual bool SupportsRedirectConfiguration => true;

        public bool UseCookies
        {
            get => _managedHandler.UseCookies;
            set
            {
                _managedHandler.UseCookies = value;
            }
        }

        public CookieContainer CookieContainer
        {
            get => _managedHandler.CookieContainer;
            set
            {
                _managedHandler.CookieContainer = value;
            }
        }

        public ClientCertificateOption ClientCertificateOptions
        {
            get => _managedHandler.ClientCertificateOptions;
            set
            {
                _managedHandler.ClientCertificateOptions = value;
            }
        }

        public DecompressionMethods AutomaticDecompression
        {
            get => _managedHandler.AutomaticDecompression;
            set
            {
                _managedHandler.AutomaticDecompression = value;
            }
        }

        public bool UseProxy
        {
            get => _managedHandler.UseProxy;
            set
            {
                _managedHandler.UseProxy = value;
            }
        }

        public IWebProxy Proxy
        {
            get => _managedHandler.Proxy;
            set
            {
                _managedHandler.Proxy = value;
            }
        }

        public ICredentials DefaultProxyCredentials
        {
            get => _managedHandler.DefaultProxyCredentials;
            set
            {
                _managedHandler.DefaultProxyCredentials = value;
            }
        }

        public bool PreAuthenticate
        {
            get => _managedHandler.PreAuthenticate;
            set
            {
                _managedHandler.PreAuthenticate = value;
            }
        }

        public bool UseDefaultCredentials
        {
            // WinHttpHandler doesn't have a separate UseDefaultCredentials property.  There
            // is just a ServerCredentials property.  So, we need to map the behavior.
            //
            // This property only affect .ServerCredentials and not .DefaultProxyCredentials.

            get => _managedHandler.UseDefaultCredentials;
            set
            {
                _managedHandler.UseDefaultCredentials = value;
            }
        }

        public ICredentials Credentials
        {
            get => _managedHandler.Credentials;
            set
            {
                _managedHandler.Credentials = value;
            }
        }

        public bool AllowAutoRedirect
        {
            get => _managedHandler.AllowAutoRedirect;
            set
            {
                _managedHandler.AllowAutoRedirect = value;
            }
        }

        public int MaxAutomaticRedirections
        {
            get => _managedHandler.MaxAutomaticRedirections;
            set
            {
                _managedHandler.MaxAutomaticRedirections = value;
            }
        }

        public int MaxConnectionsPerServer
        {
            get => _managedHandler.MaxConnectionsPerServer;
            set
            {
                _managedHandler.MaxConnectionsPerServer = value;
            }
        }

        public int MaxResponseHeadersLength
        {
            get => _managedHandler.MaxResponseHeadersLength;
            set
            {
                _managedHandler.MaxResponseHeadersLength = value;
            }
        }

        public ClientCertificateWithKey ClientCertificate
        {
            get
            {
                return this._managedHandler.ClientCertificate;
            }

            set
            {
                this._managedHandler.ClientCertificate = value;
            }
        }

        public Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> ServerCertificateCustomValidationCallback
        {
            get => _managedHandler.ServerCertificateCustomValidationCallback;
            set
            {
                _managedHandler.ServerCertificateCustomValidationCallback = value;
            }
        }

        public bool CheckCertificateRevocationList
        {
            get => _managedHandler.CheckCertificateRevocationList;
            set
            {
                _managedHandler.CheckCertificateRevocationList = value;
            }
        }

        public SslProtocols SslProtocols
        {
            get => _managedHandler.SslProtocols;
            set
            {
                _managedHandler.SslProtocols = value;
            }
        }

        public IDictionary<String, object> Properties => _managedHandler.Properties;


        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return _managedHandler.SendAsync(request, cancellationToken);
        }
    }
}
