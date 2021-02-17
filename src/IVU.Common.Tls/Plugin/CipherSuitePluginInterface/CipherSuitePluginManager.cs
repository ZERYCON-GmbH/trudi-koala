namespace IVU.Common.Tls.Plugin.CipherSuitePluginInterface
{
    using System.Collections.Generic;
    using System.Security.Cryptography.X509Certificates;

    using IVU.Common.Tls.Logging;
    using IVU.Common.Tls.HandshakeLayer.Protocol;

    public class CipherSuitePluginManager
    {
        private ILog logger = LogProvider.For<CipherSuitePluginManager>();

        private CipherSuitePlugin[] _plugins;

        public CipherSuitePluginManager()
        {
            // Register plugins
            var pluginList = new List<CipherSuitePlugin>
            {
                new BouncyCastleCipherPlugin.BouncyCastleCipherSuitePlugin(),
                new BaseCipherSuitePlugin.BaseCipherSuitePlugin(),
                new ARCFourCipherSuitePlugin.ARCFourCipherSuitePlugin(),
                new AriaCipherSuitePlugin.ARIACipherSuitePlugin(),
                new BigIntegerCipherSuitePlugin.BigIntegerCipherSuitePlugin(),
            };

            this._plugins = pluginList.ToArray();
        }

        public CertificatePrivateKey GetPrivateKey(byte[] keyData)
        {
            foreach (CipherSuitePlugin plugin in _plugins)
            {
                string[] signatureIDs = plugin.SupportedSignatureAlgorithms;
                foreach (string id in signatureIDs)
                {
                    SignatureAlgorithm signatureAlgorithm = plugin.GetSignatureAlgorithm(id);
                    CertificatePrivateKey privateKey = signatureAlgorithm.ImportPrivateKey(keyData);
                    if (privateKey != null)
                    {
                        return privateKey;
                    }
                }
            }

            return null;
        }

        public ushort[] GetSupportedSignatureAndHashAlgorithms()
        {
            var sighashes = new List<ushort>();
            foreach (CipherSuitePlugin plugin in _plugins)
            {
                string[] sigIDs = plugin.SupportedSignatureAlgorithms;
                foreach (string sigID in sigIDs)
                {
                    SignatureAlgorithm sig = plugin.GetSignatureAlgorithm(sigID);
                    if (sig == null)
                    {
                        continue;
                    }

                    byte sigType = sig.SignatureAlgorithmType;
                    if (sigType == 0)
                    {
                        // Ignore anonymous
                        continue;
                    }

                    // TODO: Now scans from sha512 to md5, should be configurable?
                    for (byte b = 6; b > 0; b--)
                    {
                        if (!sig.SupportsHashAlgorithmType(b))
                        {
                            continue;
                        }

                        sighashes.Add((ushort)((b << 8) | sigType));
                    }
                }
            }
            return sighashes.ToArray();
        }

        public CipherSuiteId[] GetSupportedCipherSuiteIDs(ProtocolVersion version, X509Certificate certificate, bool includeAnonymous)
        {
            var suites = new List<CipherSuiteId>();
            foreach (CipherSuitePlugin plugin in _plugins)
            {
                ushort[] suiteIDs = plugin.SupportedCipherSuites;
                foreach (ushort id in suiteIDs)
                {
                    CipherSuite cipherSuite = GetCipherSuite(version, id);
                    if (cipherSuite == null)
                    {
                        continue;
                    }
                    if (cipherSuite.IsAnonymous)
                    {
                        if (includeAnonymous)
                        {
                            suites.Add((CipherSuiteId)id);
                        }
                        continue;
                    }

                    string keyexAlg = cipherSuite.KeyExchangeAlgorithm.CertificateKeyAlgorithm;
                    string sigAlg = cipherSuite.SignatureAlgorithm.CertificateKeyAlgorithm;

                    if (keyexAlg != null && !keyexAlg.Equals(certificate.GetKeyAlgorithm()))
                    {
                        continue;
                    }
                    if (sigAlg != null && !sigAlg.Equals(certificate.GetKeyAlgorithm()))
                    {
                        continue;
                    }
                    suites.Add((CipherSuiteId)id);
                }
            }
            return suites.ToArray();
        }

        public SignatureAlgorithm GetSignatureAlgorithmByOid(string oid)
        {
            foreach (CipherSuitePlugin plugin in _plugins)
            {
                string[] supported = plugin.SupportedSignatureAlgorithms;
                foreach (string sig in supported)
                {
                    SignatureAlgorithm sigAlg = plugin.GetSignatureAlgorithm(sig);
                    if (oid.Equals(sigAlg.CertificateKeyAlgorithm))
                    {
                        return sigAlg;
                    }
                }
            }
            return null;
        }

        public CipherSuite GetCipherSuite(ProtocolVersion version, ushort id)
        {
            CipherSuiteInfo cipherSuiteInfo = null;
            foreach (CipherSuitePlugin plugin in _plugins)
            {
                var supported = new List<ushort>(plugin.SupportedCipherSuites);
                if (supported.Contains(id))
                {
                    cipherSuiteInfo = plugin.GetCipherSuiteFromID(id);
                    break;
                }
            }

            if (cipherSuiteInfo == null)
            {
                this.logger?.Debug("CipherSuite ID 0x" + id.ToString("x").PadLeft(2, '0') + " not found");
                return null;
            }

            var cipherSuite = new CipherSuite(version, id, cipherSuiteInfo.CipherSuiteName);
            if (cipherSuiteInfo.KeyExchangeAlgorithmName == null)
            {
                cipherSuite.KeyExchangeAlgorithm = new KeyExchangeAlgorithmNull();
            }
            if (cipherSuiteInfo.SignatureAlgorithmName == null)
            {
                cipherSuite.SignatureAlgorithm = new SignatureAlgorithmNull();
            }
            if (cipherSuiteInfo.BulkCipherAlgorithmName == null)
            {
                cipherSuite.BulkCipherAlgorithm = new BulkCipherAlgorithmNull();
            }
            if (cipherSuiteInfo.MACAlgorithmName == null)
            {
                cipherSuite.MACAlgorithm = new MACAlgorithmNull();
            }

            // These need to be edited in different versions
            string prfName = cipherSuiteInfo.PseudoRandomFunctionName;
            string macName = cipherSuiteInfo.MACAlgorithmName;

            if (version == ProtocolVersion.SSL3_0)
            {
                if (prfName == null)
                {
                    prfName = "SSLv3";
                }
                else
                {
                    // PRF selection not supported, but PRF defined, ignore this suite
                    return null;
                }

                if (macName == null)
                {
                    macName = null;
                }
                else if (macName.Equals("MD5"))
                {
                    macName = "SSLv3_MD5";
                }
                else if (macName.Equals("SHA1"))
                {
                    macName = "SSLv3_SHA1";
                }
                else
                {
                    // Only MD5 and SHA1 MAC accepted in SSLv3, ignore this suite
                    return null;
                }
            }
            else
            {
                if (version.HasSelectablePRF)
                {
                    if (prfName == null)
                    {
                        prfName = "TLS_SHA256";
                    }
                }
                else
                {
                    if (prfName == null)
                    {
                        prfName = "TLSv1";
                    }
                    else
                    {
                        // PRF selection not supported, but PRF defined, ignore this suite
                        return null;
                    }
                }
            }

            foreach (CipherSuitePlugin plugin in _plugins)
            {
                if (cipherSuite.KeyExchangeAlgorithm == null)
                {
                    cipherSuite.KeyExchangeAlgorithm =
                        plugin.GetKeyExchangeAlgorithm(cipherSuiteInfo.KeyExchangeAlgorithmName);
                }
                if (cipherSuite.SignatureAlgorithm == null)
                {
                    cipherSuite.SignatureAlgorithm =
                        plugin.GetSignatureAlgorithm(cipherSuiteInfo.SignatureAlgorithmName);
                }
                if (cipherSuite.PseudoRandomFunction == null)
                {
                    cipherSuite.PseudoRandomFunction =
                        plugin.GetPseudoRandomFunction(prfName);

                    /* Check that the PRF is valid as per RFC 5246 section 7.4.9 */
                    if (cipherSuite.PseudoRandomFunction != null && cipherSuite.PseudoRandomFunction.VerifyDataLength < 12)
                    {
                        this.logger?.Debug("Invalid PseudoRandomFunction, verify data less than 12, ignored");
                        cipherSuite.PseudoRandomFunction = null;
                    }
                }
                if (cipherSuite.BulkCipherAlgorithm == null)
                {
                    cipherSuite.BulkCipherAlgorithm =
                        plugin.GetBulkCipherAlgorithm(cipherSuiteInfo.BulkCipherAlgorithmName);
                }
                if (cipherSuite.MACAlgorithm == null)
                {
                    cipherSuite.MACAlgorithm =
                        plugin.GetMACAlgorithm(macName);
                }
            }

            if (cipherSuite.KeyExchangeAlgorithm == null || !cipherSuite.KeyExchangeAlgorithm.SupportsProtocolVersion(version))
            {
                this.logger?.Trace("KeyExchangeAlgorithm '" + cipherSuiteInfo.KeyExchangeAlgorithmName + "' not found");
                return null;
            }
            if (cipherSuite.SignatureAlgorithm == null || !cipherSuite.SignatureAlgorithm.SupportsProtocolVersion(version))
            {
                this.logger?.Trace("SignatureAlgorithm '" + cipherSuiteInfo.SignatureAlgorithmName + "' not found");
                return null;
            }
            if (cipherSuite.PseudoRandomFunction == null || !cipherSuite.PseudoRandomFunction.SupportsProtocolVersion(version))
            {
                this.logger?.Trace("PseudoRandomFunction '" + cipherSuiteInfo.PseudoRandomFunctionName + "' not found");
                return null;
            }
            if (cipherSuite.BulkCipherAlgorithm == null || !cipherSuite.BulkCipherAlgorithm.SupportsProtocolVersion(version))
            {
                this.logger?.Trace("BulkCipherAlgorithm '" + cipherSuiteInfo.BulkCipherAlgorithmName + "' not found");
                return null;
            }
            if (cipherSuite.MACAlgorithm == null || !cipherSuite.MACAlgorithm.SupportsProtocolVersion(version))
            {
                this.logger?.Trace("MACAlgorithm '" + cipherSuiteInfo.MACAlgorithmName + "' not found");
                return null;
            }

            return cipherSuite;
        }
    }
}
