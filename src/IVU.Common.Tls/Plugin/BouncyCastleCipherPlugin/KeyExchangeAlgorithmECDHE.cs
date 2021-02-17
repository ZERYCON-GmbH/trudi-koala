using System.Linq;

namespace IVU.Common.Tls.Plugin.BouncyCastleCipherPlugin
{
    using System;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;

    using IVU.Common.Tls.HandshakeLayer.Protocol;
    using IVU.Common.Tls.Plugin.CipherSuitePluginInterface;
    using IVU.Common.Tls.Logging;

    using Org.BouncyCastle.Asn1;
    using Org.BouncyCastle.Asn1.Sec;
    using Org.BouncyCastle.Asn1.X9;
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Agreement;
    using Org.BouncyCastle.Crypto.Generators;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Crypto.Tls;
    using Org.BouncyCastle.Math;
    using Org.BouncyCastle.Math.EC;
    using Org.BouncyCastle.Security;

    using KeyExchangeAlgorithm = IVU.Common.Tls.Plugin.CipherSuitePluginInterface.KeyExchangeAlgorithm;
    using ProtocolVersion = IVU.Common.Tls.Plugin.ProtocolVersion;

    public class KeyExchangeAlgorithmEcdhe : KeyExchangeAlgorithm
    {
        private readonly ILog logger = LogProvider.For<KeyExchangeAlgorithm>();

        private ECDomainParameters domainParameters;
        private ECPrivateKeyParameters privateKey;
        private ECPublicKeyParameters publicKey;

        private byte[] preMasterSecret;

        private string[] knownCurveNames = { "secp192r1", "secp256r1", "secp384r1", "secp512r1", "brainpoolp256r1", "brainpoolp384r1", "brainpoolp512r1" };

        public override string CertificateKeyAlgorithm => null;

        public override bool SupportsProtocolVersion(ProtocolVersion version)
        {
            return true;
        }


        /// <summary>
        /// Called by TLS server to create his ephemeral keys.
        /// TODO: get information about which ECC curve should be used
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public override byte[] GetServerKeys(ProtocolVersion version, CertificatePrivateKey certPrivateKey)
        {
            var ms = new MemoryStream(certPrivateKey.KeyValue);
            var pemReader = new Org.BouncyCastle.OpenSsl.PemReader(new StreamReader(ms));
            var keyParam = pemReader.ReadObject();
            var key = keyParam as AsymmetricCipherKeyPair;
            var pk = key.Public as ECPublicKeyParameters;
            var certCurve = pk.Q.Curve;

            string curveName = string.Empty;
            for (int i = 0; i < knownCurveNames.Length; i++)
            {
                var curveParams = SecNamedCurves.GetByName(knownCurveNames[i]);
                if (certCurve.GetHashCode() == curveParams.Curve.GetHashCode())
                {
                    curveName = knownCurveNames[i];
                    break;
                }
            }

            if (curveName == string.Empty)
            {
                throw new InvalidOperationException("Could not find EC curve for server private key");
            }

            this.logger?.Debug("Getting server keys for curve '{0}'.", curveName);
            this.GenerateKeys(curveName);

            byte[] pubKeyBytes = this.publicKey.Q.GetEncoded();
            byte[] serverKeyBytes = new byte[4 + pubKeyBytes.Length];
            serverKeyBytes[0] = 3;

            // get named curve for curve id 
            if (Enum.TryParse<EccNamedCurve>(curveName, true, out var curve) == false)
            {
                throw new InvalidOperationException("Could not find named curve for: " + curveName);
            }
            serverKeyBytes[2] = (byte)curve;

            serverKeyBytes[3] = (byte)pubKeyBytes.Length;
            Buffer.BlockCopy(pubKeyBytes, 0, serverKeyBytes, 4, pubKeyBytes.Length);

            return serverKeyBytes;
        }



        /// <summary>
        /// Called by client to get his ephemaral ECC keys
        /// TODO: get information about which ECC curve should be used
        /// </summary>
        /// <param name="version"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public override byte[] ProcessServerKeys(ProtocolVersion version, byte[] data, X509Certificate serverCertificate)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            if (data.Length < 4) throw new ArgumentException(nameof(data));

            //if (data[0] != 3 || data[1] != 0 || data[2] != 23)
            if (data[0] != 3 || data[1] != 0)
            {
                throw new ArgumentException(nameof(data));
            }

            //if (data[3] != 65 || data[4] > data.Length - 4)
            if (data[4] > data.Length - 4)
            {
                throw new ArgumentException(nameof(data));
            }


            // Extract the public key from the data
            byte[] ecPointData = new byte[data[3]];
            Buffer.BlockCopy(data, 4, ecPointData, 0, ecPointData.Length);

            ushort curveNameId = (ushort)(data[1] << 8 | data[2]);
            var ecPoint = this.ObtainCurveNameAndPoint(curveNameId, ecPointData);
            var theirPublicKey = new ECPublicKeyParameters(ecPoint, this.domainParameters);

            // Calculate the actual agreement
            var agreement = new ECDHBasicAgreement();
            agreement.Init(this.privateKey);
            this.preMasterSecret = BigIntegerToByteArray(agreement.CalculateAgreement(theirPublicKey), ecPoint.Curve.FieldSize / 8);

            var signature = new byte[data.Length - 4 - data[3]];
            Buffer.BlockCopy(data, 4 + data[3], signature, 0, signature.Length);
            return signature;
        }

        private ECPoint ObtainCurveNameAndPoint(ushort curveNameId, byte[] ecPointData)
        {
            //var data = Asn1Object.FromByteArray(cert.GetKeyAlgorithmParameters());
            //this.logger?.Debug($"Server certificate's key algorithm parameters curve name: {curveName}");
            //var oid = new DerObjectIdentifier(data.ToString());
            //var curve = SecNamedCurves.GetByOid(oid);
            //var curveName = SecNamedCurves.GetName(oid);

            var curveName = NamedCurve.GetCurveName(curveNameId);

            // Generate our private key
            this.GenerateKeys(curveName);
            var ecPoint = this.domainParameters.Curve.DecodePoint(ecPointData);
            this.logger?.Debug($"Matching curve {curveName} found");
            return ecPoint;
        }

        public override byte[] GetClientKeys(ProtocolVersion version, ProtocolVersion clientVersion, CertificatePublicKey publicKey)
        {
            byte[] ecPoint = this.publicKey.Q.GetEncoded();

            byte[] ret = new byte[1 + ecPoint.Length];
            ret[0] = (byte)ecPoint.Length;
            Buffer.BlockCopy(ecPoint, 0, ret, 1, ecPoint.Length);
            return ret;
        }

        public override void ProcessClientKeys(ProtocolVersion version, ProtocolVersion clientVersion, CertificatePrivateKey privateKey, byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (data.Length < 1)
            {
                throw new ArgumentException("data");
            }

            byte[] ecPointData = new byte[data[0]];
            Buffer.BlockCopy(data, 1, ecPointData, 0, ecPointData.Length);
            ECPoint ecPoint = domainParameters.Curve.DecodePoint(ecPointData);
            var theirPublicKey = new ECPublicKeyParameters(ecPoint, domainParameters);

            // Calculate the actual agreement
            var agreement = new ECDHBasicAgreement();
            agreement.Init(this.privateKey);
            var pmsLength = this.privateKey.Parameters.Curve.FieldSize / 8;
            preMasterSecret = BigIntegerToByteArray(agreement.CalculateAgreement(theirPublicKey), pmsLength);
            this.logger?.Debug("Pre-Master secret: " + BitConverter.ToString(preMasterSecret));
        }

        public override byte[] GetMasterSecret(PseudoRandomFunction prf, byte[] seed)
        {
            var masterSecret = prf.CreateDeriveBytes(preMasterSecret, "master secret", seed).GetBytes(48);
            this.logger?.Debug("Master Secret: " + BitConverter.ToString(masterSecret));
            return masterSecret;
        }


        private void GenerateKeys(string curveName)
        {
            this.logger?.Debug($"ECDHE: Creating ephemeral ecc domain parameters and keys for curve {curveName}...");
            X9ECParameters ecParams = SecNamedCurves.GetByName(curveName);
            this.domainParameters = new ECDomainParameters(
                ecParams.Curve,
                ecParams.G,
                ecParams.N,
                ecParams.H,
                ecParams.GetSeed());
            var keyGenParams = new ECKeyGenerationParameters(this.domainParameters, new SecureRandom());

            var generator = new ECKeyPairGenerator();
            generator.Init(keyGenParams);
            var keyPair = generator.GenerateKeyPair();

            this.privateKey = (ECPrivateKeyParameters)keyPair.Private;
            this.publicKey = (ECPublicKeyParameters)keyPair.Public;
        }

        private static byte[] BigIntegerToByteArray(BigInteger input, int length)
        {
            byte[] result = new byte[length];
            byte[] inputBytes = input.ToByteArray();
            Array.Reverse(inputBytes);
            Buffer.BlockCopy(inputBytes, 0, result, 0, System.Math.Min(inputBytes.Length, result.Length));
            Array.Reverse(result);
            return result;
        }
    }
}
