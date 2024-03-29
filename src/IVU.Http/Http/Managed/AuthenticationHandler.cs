﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace IVU.Http.Headers.Managed
{
    using System.Net;
    using System.Net.Http;

    using HttpMessageHandler = IVU.Http.HttpMessageHandler;
    using HttpRequestMessage = IVU.Http.HttpRequestMessage;
    using HttpResponseMessage = IVU.Http.HttpResponseMessage;

    internal sealed class AuthenticationHandler : HttpMessageHandler
    {
        private const string Basic = "Basic";
        private readonly HttpMessageHandler _innerHandler;
        private readonly bool _preAuthenticate;
        private readonly ICredentials _credentials;

        public AuthenticationHandler(bool preAuthenticate, ICredentials credentials, HttpMessageHandler innerHandler)
        {
            Debug.Assert(credentials != null);
            Debug.Assert(innerHandler != null);

            _preAuthenticate = preAuthenticate;
            _credentials = credentials;
            _innerHandler = innerHandler;
        }

        private bool TrySetBasicAuthToken(HttpRequestMessage request)
        {
            NetworkCredential credential = _credentials.GetCredential(request.RequestUri, Basic);
            if (credential == null)
            {
                return false;
            }

            request.Headers.Authorization = new AuthenticationHeaderValue(Basic, BasicAuthenticationHelper.GetBasicTokenForCredential(credential));
            return true;
        }

        public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_preAuthenticate)
            {
                TrySetBasicAuthToken(request);
            }

            HttpResponseMessage response = await _innerHandler.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (!_preAuthenticate && response.StatusCode == HttpStatusCode.Unauthorized)
            {
                HttpHeaderValueCollection<AuthenticationHeaderValue> authenticateValues = response.Headers.WwwAuthenticate;

                foreach (AuthenticationHeaderValue h in authenticateValues)
                {
                    // We only support Basic auth, ignore others
                    if (h.Scheme == Basic)
                    {
                        if (!TrySetBasicAuthToken(request))
                        {
                            break;
                        }

                        response.Dispose();
                        response = await _innerHandler.SendAsync(request, cancellationToken).ConfigureAwait(false);
                        break;
                    }
                }
            }

            return response;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _innerHandler.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
