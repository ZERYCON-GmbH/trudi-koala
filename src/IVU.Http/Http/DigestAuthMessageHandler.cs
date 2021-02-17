namespace IVU.Http
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    using IVU.Http.Headers;
    using IVU.Http.Logging;

    public class DigestAuthMessageHandler : DelegatingHandler
    {
        private static ILog logger = LogProvider.For<DigestAuthMessageHandler>();

        private string _username;
        private string _password;

        private static readonly ConcurrentDictionary<string, AuthorizationParameter> _authorizationCache = new ConcurrentDictionary<string, AuthorizationParameter>();

        private static Random random = new Random();

        public DigestAuthMessageHandler(HttpMessageHandler innerHandler, string username, string password)
            : base(innerHandler)
        {
            this._username = username;
            this._password = password;
        }

        public static void ClearCachedAuthorizationParameter(Uri requestUri)
        {
            for (int i = requestUri.Segments.Length - 1; i > 0; i--)
            {
                var path = string.Join(string.Empty, requestUri.Segments.Take(i)).TrimEnd('/');
                _authorizationCache.TryRemove(path, out _);
            }
        }


        public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AuthorizationParameter authorizationParameter;

            if (!TryGetCachedAuthorizationParameterByUri(request.RequestUri, out authorizationParameter))
            {
                // the AuthorizationParameter ist not in the cache
                var response = await base.SendAsync(request, cancellationToken);

                // if the response doesn't send a 401 status code
                if (response.StatusCode != HttpStatusCode.Unauthorized)
                    return response;

                // no WWW-Authenticate header was returned
                // I make it simple: do nothing and let the client handle this!
                if (response.Headers.WwwAuthenticate == null || !response.Headers.WwwAuthenticate.Any())
                    return response;

                // read the content to return the HTTP connection to the connection pool
                await response.Content.ReadAsStringAsync();

                authorizationParameter = GetAuthorizationParameter(authorizationParameter, response);

                AddAuthenticationHeader(
                    request,
                    this._username, this._password,
                    authorizationParameter);

                return await base.SendAsync(request, cancellationToken);
            }

            // the AuthorizationParameter ist in the cache
            AddAuthenticationHeader(
                request,
                this._username, this._password,
                authorizationParameter);

            var responseThatShouldBeAuthenticated = await base.SendAsync(request, cancellationToken);

            // If already sending an Authorization header but the period of a valid tickets is exceeded, the server sends a 401 status code and the ticket has to be renewed...
            if (responseThatShouldBeAuthenticated.StatusCode == HttpStatusCode.Unauthorized)
            {
                await responseThatShouldBeAuthenticated.Content.ReadAsStringAsync();

                var renewedAuthorizationParameter = GetAuthorizationParameter(
                    authorizationParameter, responseThatShouldBeAuthenticated);

                AddAuthenticationHeader(
                    request,
                    this._username, this._password,
                    renewedAuthorizationParameter);

                responseThatShouldBeAuthenticated = await base.SendAsync(request, cancellationToken);
            }

            return responseThatShouldBeAuthenticated;
        }

        private static AuthorizationParameter GetAuthorizationParameter(
            AuthorizationParameter authorizationParameter, HttpResponseMessage message)
        {
            var wwwAuthenticateHeader = message.Headers.WwwAuthenticate.First();

            string realm;
            string domain;
            string nonce;
            string qop;
            string cnonce;
            string opaque;
            string algorithm;
            DateTime cnonceDate;

            logger.Debug("Received digest authorization parameters: {0}", wwwAuthenticateHeader.Parameter);

            ParseDigestAuthHeaderData(wwwAuthenticateHeader.Parameter, out realm, out domain, out nonce, out qop, out cnonce, out cnonceDate, out opaque, out algorithm);

            // delete the old parameter first, this mostly the case, when the server requires a renewal of the parameter
            AuthorizationParameter oldAuthorizationParameter;
            _authorizationCache.TryRemove(domain, out oldAuthorizationParameter);

            authorizationParameter = new AuthorizationParameter
            {
                realm = realm,
                nonce = nonce,
                qop = qop,
                cnonce = cnonce,
                opaque = opaque,
                algorithm = algorithm,
                cnonceDate = cnonceDate
            };

            _authorizationCache.TryAdd(
                domain, authorizationParameter);

            return authorizationParameter;
        }

        private static void ParseDigestAuthHeaderData(string wwwAuthenticateHeader,
                                                      out string realm, out string domain, out string nonce, out string qop,
                                                      out string cnonce, out DateTime cnonceDate, out string opaque, out string algorithm)
        {
            realm = GrabHeaderAuthorizationParameter("realm", wwwAuthenticateHeader);
            // domain = GrabHeaderAuthorizationParameter("domain", wwwAuthenticateHeader);
            domain = string.Empty;
            nonce = GrabHeaderAuthorizationParameter("nonce", wwwAuthenticateHeader);
            qop = GrabHeaderAuthorizationParameter("qop", wwwAuthenticateHeader);
            opaque = GrabHeaderAuthorizationParameter("opaque", wwwAuthenticateHeader);

            try
            {
                algorithm = GrabHeaderAuthorizationParameter("algorithm", wwwAuthenticateHeader);
            }
            catch
            {
                // algorithm is optional, default to MD5
                algorithm = "MD5";
            }

            if (string.IsNullOrWhiteSpace(algorithm))
            {
                algorithm = "MD5";
            }

            cnonce = random.Next(123400, 9999999).ToString();
            cnonceDate = DateTime.Now;
        }

        private static void AddAuthenticationHeader(HttpRequestMessage request,
                                                    string username, string password, AuthorizationParameter authorizationParameter)
        {
            var digestHeader = GetDigestHeader(request.Method, request.RequestUri.PathAndQuery, username, password, authorizationParameter);
            request.Headers.Authorization = new AuthenticationHeaderValue("Digest", digestHeader);

            logger.Debug("Digest authorization: {0}", digestHeader);
        }

        private static string GrabHeaderAuthorizationParameter(string varName, string header)
        {
            var matchHeader = Regex.Match(header, $"{varName}=\"([^\"]*)\"");
            if (matchHeader.Success)
            {
                return matchHeader.Groups[1].Value;
            }

            matchHeader = Regex.Match(header, $"{varName}=([^,]*)");
            if (matchHeader.Success)
            {
                return matchHeader.Groups[1].Value;
            }

            throw new InvalidOperationException($"Header {varName} not found");
        }

        private static string GetDigestHeader(HttpMethod method,
                                              string path,
                                              string username, string password,
                                              AuthorizationParameter fragments)
        {
            return GetDigestHeader(
                method,
                path,
                username, password,
                fragments.realm, fragments.nonce,
                fragments.qop, fragments.nc,
                fragments.cnonce, fragments.opaque, fragments.algorithm, fragments.cnonceDate);
        }

        private static string GetDigestHeader(HttpMethod method,
                                              string path,
                                              string username, string password,
                                              string realm, string nonce, string qop, int nc,
                                              string cnonce, string opaque, string algorithm, DateTime cnonceDate)
        {
            string digestResponse;

            qop = "auth";

            switch (algorithm.ToUpperInvariant())
            {
                case "MD5":
                    {
                        var ha1 = CalculateMd5Hash($"{username}:{realm}:{password}");
                        var ha2 = CalculateMd5Hash($"{method.Method}:{path}");
                        digestResponse = CalculateMd5Hash($"{ha1}:{nonce}:{nc:x8}:{cnonce}:{qop}:{ha2}");
                    }
                    break;

                case "SHA256":
                case "SHA-256":
                    {
                        var ha1 = CalculateSha256Hash($"{username}:{realm}:{password}");
                        var ha2 = CalculateSha256Hash($"{method.Method}:{path}");
                        digestResponse = CalculateSha256Hash($"{ha1}:{nonce}:{nc:x8}:{cnonce}:{qop}:{ha2}");
                    }
                    break;

                default:
                    throw new Exception($"Digest Access Authentication failed: unsupported hash algorithm {algorithm}");
            }

            if (string.IsNullOrWhiteSpace(opaque))
            {
                return $"username=\"{username}\", realm=\"{realm}\", nonce=\"{nonce}\", uri=\"{path}\", "
                       + $"algorithm=\"{algorithm}\", response=\"{digestResponse}\", qop={qop}, nc={nc:x8}, cnonce=\"{cnonce}\"";
            }

            return $"username=\"{username}\", realm=\"{realm}\", nonce=\"{nonce}\", uri=\"{path}\", "
                       + $"algorithm={algorithm}, response=\"{digestResponse}\", qop=\"{qop}\", nc={nc:x8}, cnonce=\"{cnonce}\", opaque=\"{opaque}\"";
        }

        private static string CalculateMd5Hash(string input)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hash = MD5.Create().ComputeHash(inputBytes);
            var sb = new StringBuilder();
            foreach (var b in hash)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        private static string CalculateSha256Hash(string input)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hash = SHA256.Create().ComputeHash(inputBytes);
            var sb = new StringBuilder();
            foreach (var b in hash)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        private static bool TryGetCachedAuthorizationParameterByUri(
            Uri requestUri, out AuthorizationParameter authorizationParameter)
        {
            authorizationParameter = null;
            for (int i = requestUri.Segments.Length - 1; i > 0; i--)
            {
                string path = string.Join(string.Empty, requestUri.Segments.Take(i)).TrimEnd('/');
                if (_authorizationCache.TryGetValue(path, out authorizationParameter))
                    return true;
            }
            return false;
        }

        private class AuthorizationParameter
        {
            private int _nc = 0;

            public string realm { get; set; }
            public string nonce { get; set; }
            public string qop { get; set; }
            public string cnonce { get; set; }
            public string opaque { get; set; }
            public DateTime cnonceDate { get; set; }
            public string algorithm { get; set; }

            public int nc
            {
                get { return Interlocked.Increment(ref this._nc); }
            }
        }
    }
}