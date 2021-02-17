using System.Linq;
using System.Text;

namespace IVU.Common.Tls.Plugin.BouncyCastleCipherPlugin
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;

    using IVU.Common.Tls.Logging;
    using IVU.Common.Tls.Plugin.CipherSuitePluginInterface;

    using Org.BouncyCastle.Asn1;
    using Org.BouncyCastle.Asn1.Sec;
    using Org.BouncyCastle.Asn1.X9;
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Crypto.Signers;
    using Org.BouncyCastle.Math;
    using Org.BouncyCastle.Math.EC;
    using Org.BouncyCastle.OpenSsl;

    public class SignatureAlgorithmECDSA : SignatureAlgorithm
    {
        private static readonly ILog Log = LogProvider.For<SignatureAlgorithmECDSA>();

        /*
        private readonly string P256OID = "1.2.840.10045.3.1.7";
        private readonly string P384OID = "1.3.132.0.34";
        private readonly string P521OID = "1.3.132.0.35";
        */

        public override string CertificateKeyAlgorithm => "1.2.840.10045.2.1";

        public override byte SignatureAlgorithmType => 3;

        public override bool SupportsProtocolVersion(ProtocolVersion version)
        {
            return true;
        }

        public override bool SupportsHashAlgorithmType(byte hashAlgorithm)
        {
            // Known HashAlgorithms listed here
            switch (hashAlgorithm)
            {
                case 1: // HashAlgorithm.md5
                case 2: // HashAlgorithm.sha1
                case 4: // HashAlgorithm.sha256
                case 5: // HashAlgorithm.sha384
                case 6: // HashAlgorithm.sha512
                    return true;
                default:
                    return false;
            }
        }

        public override CertificatePrivateKey ImportPrivateKey(byte[] keyData)
        {
            // remove any other lines before private key as it is blowing up the pem reader
            var ascii = Encoding.ASCII.GetString(keyData);
            var lines = ascii.Split(new string[] {"\n","\r"}, StringSplitOptions.RemoveEmptyEntries).ToList();
            var start = lines.IndexOf("-----BEGIN EC PRIVATE KEY-----");
            lines = lines.Skip(start).ToList();
            ascii = string.Join("\n", lines);
            keyData = Encoding.ASCII.GetBytes(ascii);

            // read private key 
            var ms = new MemoryStream(keyData);
            var pemReader = new PemReader(new StreamReader(ms));
            var keyParam = pemReader.ReadObject();
            var kp = keyParam as AsymmetricCipherKeyPair;
            if (kp?.Private == null)
            {
                throw new NullReferenceException();
            }

            Log.Debug("--- ImportedPrivateKey type: {0}", keyParam.GetType());

            return new CertificatePrivateKey(this.CertificateKeyAlgorithm, keyData);
        }

        public override byte[] SignData(ProtocolVersion version, byte[] data, HashAlgorithm hashAlgorithm, CertificatePrivateKey privateKey)
        {
            var ms = new MemoryStream(privateKey.KeyValue);
            var pemReader = new PemReader(new StreamReader(ms));
            var keyParam = pemReader.ReadObject();
            var key = keyParam as AsymmetricCipherKeyPair;
            
            var signer = new ECDsaSigner();
            signer.Init(true, key.Private);
            var hashedData = hashAlgorithm.ComputeHash(data);
            Log.Trace("Hashing data (S):" + BitConverter.ToString(data));
            Log.Trace("Hashed data (S):" + BitConverter.ToString(hashedData));

            var bitLength = ((ECPrivateKeyParameters) key.Private).Parameters.Curve.FieldSize;
            Log.Trace("Signing for bit length {@l}", bitLength);
            var sig = signer.GenerateSignature(hashedData);
            var maxRetries = 500;
            while (sig[0].BitLength != bitLength || sig[1].BitLength != bitLength)
            {
                sig = signer.GenerateSignature(hashedData);

                if (maxRetries-- < 1)
                {
                    throw new InvalidOperationException("SignData finally failed");
                }
            }

            // test verify
            var _tmpEcPubkey = key.Public as ECPublicKeyParameters;
            Log.Debug("Associated PubKey Q: " + BitConverter.ToString(_tmpEcPubkey.Q.GetEncoded()));
            var signerTest = new ECDsaSigner();
            signerTest.Init(false, key.Public);
            var result = signerTest.VerifySignature(hashedData, sig[0], sig[1]);
            if (!result)
            {
                throw new CryptographicUnexpectedOperationException("Invalid!!!");
            }

            Log.Debug("R value: " + sig[0].SignValue + " " + sig[0].LongValue);
            Log.Debug("S value: " + sig[1].SignValue + " " + sig[1].LongValue);
            // TODO: check how BigIntegers are encoded before sent over the wire
            // Maybe DER encoding of R and S as integer would do it. However for the moment it works with stuffing 0x00 in.
            var rl = new List<byte>(sig[0].ToByteArray());
            var sl = new List<byte>(sig[1].ToByteArray());
            //while (rl.Count < 33)
            //{
            //    rl.Insert(0, 0x00);
            //}

            //while (sl.Count < 33)
            //{
            //    sl.Insert(0, 0x00);
            //}
            var r = rl.ToArray();
            var s = sl.ToArray();
            var rs = new byte[r.Length + s.Length];
            Buffer.BlockCopy(r, 0, rs, 0, r.Length);
            Buffer.BlockCopy(s, 0, rs, r.Length, s.Length);
            
            var derSig = DEREncodeSignature(rs);
            // Log.Debug("DER Signature (S): " + BitConverter.ToString(derSig));
            Log.Trace("Signature R (S)" + BitConverter.ToString(r));
            Log.Trace("Signature S (S)" + BitConverter.ToString(s));
            return derSig;
        }

        public override bool VerifyData(ProtocolVersion version, byte[] data, HashAlgorithm hashAlgorithm, CertificatePublicKey publicKey, byte[] signature)
        {
            if (!CertificateKeyAlgorithm.Equals(publicKey.Oid))
            {
                throw new Exception("ECDSA signature verification requires ECDSA public key");
            }

            // Decode the public key parameters
            string curveOid = DER2OID(publicKey.Parameters);
            if (curveOid == null)
            {
                throw new Exception("Unsupported ECDSA public key parameters");
            }

            // Get parameters from the curve OID
            X9ECParameters ecParams = SecNamedCurves.GetByOid(new DerObjectIdentifier(curveOid));
            if (ecParams == null)
            {
                throw new Exception("Unsupported ECC curve type OID: " + curveOid);
            }

            // Construct domain parameters
            ECDomainParameters domainParameters = new ECDomainParameters(ecParams.Curve,
                                                                         ecParams.G, ecParams.N, ecParams.H,
                                                                         ecParams.GetSeed());

            // Decode the public key data
            byte[] ecPointData = publicKey.KeyValue;
            if (ecPointData[0] != 0x04)
            {
                throw new Exception("Only uncompressed ECDSA keys supported, format: " + ecPointData[0]);
            }

            Org.BouncyCastle.Math.EC.ECPoint ecPoint = domainParameters.Curve.DecodePoint(ecPointData);
            ECPublicKeyParameters theirPublicKey = new ECPublicKeyParameters(ecPoint, domainParameters);
            
            // Hash input data buffer
            byte[] dataHash;
            if (hashAlgorithm == null)
            {
                dataHash = TLSv1HashData(data);
            }
            else
            {
                dataHash = hashAlgorithm.ComputeHash(data);
            }

            // Actually verify the signature
            BigInteger[] sig = DERDecodeSignature(signature);
            var r = sig[0];
            var s = sig[1];
            Log.Trace("Hashed data (V): " + BitConverter.ToString(dataHash));
            Log.Trace("Public Key Q (V): " + BitConverter.ToString(theirPublicKey.Q.GetEncoded()));
            Log.Trace("Signature R (V): " + BitConverter.ToString(r.ToByteArrayUnsigned()));
            Log.Trace("Signature S (V): " + BitConverter.ToString(s.ToByteArrayUnsigned()));
            Log.Trace("R value: " + sig[0].SignValue + " " + sig[0].LongValue);
            Log.Trace("S value: " + sig[1].SignValue + " " + sig[1].LongValue);


            ECDsaSigner signer = new ECDsaSigner();
            signer.Init(false, theirPublicKey);
            var result = signer.VerifySignature(dataHash, r, s);
            Log.Debug("Signature verification status: {0}.", result);
            return result;
        }

        // SSL 3.0, TLS 1.0 and TLS 1.1 all use MD5+SHA1 for signature
        private static byte[] TLSv1HashData(byte[] buffer)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] md5Hash = md5.ComputeHash(buffer);

            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] sha1Hash = sha1.ComputeHash(buffer);

            byte[] hash = new byte[md5Hash.Length + sha1Hash.Length];
            Buffer.BlockCopy(md5Hash, 0, hash, 0, md5Hash.Length);
            Buffer.BlockCopy(sha1Hash, 0, hash, md5Hash.Length, sha1Hash.Length);
            return hash;
        }

        private static string DER2OID(byte[] oid)
        {
            try
            {
                if (oid[0] != 0x06 || oid[1] >= 128 || oid[1] != oid.Length - 2)
                {
                    return null;
                }

                byte firstByte = oid[2];
                string ret = (firstByte / 40) + "." + (firstByte % 40) + ".";
                for (int i = 3; i < oid.Length; i++)
                {
                    if (oid[i] < 128)
                    {
                        ret += (int)oid[i];
                    }
                    else if (oid[i] >= 128 && oid[i + 1] < 128)
                    {
                        ret += (int)(((oid[i] & 0x7f) << 7) | oid[i + 1]);
                        i++;
                    }
                    else
                    {
                        return null;
                    }

                    if (i != oid.Length - 1)
                    {
                        ret += ".";
                    }
                }
                return ret;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static byte[] DEREncodeSignature(byte[] signature)
        {
            // This is the largest we can encode, adds 8 more bytes
            if (signature.Length > 65526 || (signature.Length % 2) != 0)
            {
                throw new Exception("Invalid signature length");
            }

            int vectorLength = (signature.Length / 2 < 128) ? 2 + (signature.Length / 2) :
                               (signature.Length / 2 < 256) ? 3 + (signature.Length / 2) :
                               4 + (signature.Length / 2);

            byte[] encoded = new byte[2 * vectorLength];
            encoded[0] = 0x02;
            DEREncodeVector(signature, 0, signature.Length / 2, encoded, 1);
            encoded[vectorLength] = 0x02;
            DEREncodeVector(signature, signature.Length / 2, signature.Length / 2, encoded, vectorLength + 1);

            int retLength = (encoded.Length < 128) ? 2 + encoded.Length :
                            (encoded.Length < 256) ? 3 + encoded.Length :
                            4 + encoded.Length;

            byte[] ret = new byte[retLength];
            ret[0] = 0x30;
            DEREncodeVector(encoded, 0, encoded.Length, ret, 1);
            return ret;
        }

        private static BigInteger[] DERDecodeSignature(byte[] data)
        {
            // This is the largest we can decode, check type
            if (data.Length > 65538 || data[0] != 0x30)
            {
                throw new Exception("Invalid signature");
            }

            int fullConsumed;
            byte[] encoded = DERDecodeVector(data, 1, out fullConsumed);
            if (encoded[0] != 0x02)
                throw new Exception("Invalid signature");

            int rConsumed;
            List<byte> R = new List<byte>(DERDecodeVector(encoded, 1, out rConsumed));
            if (encoded[1 + rConsumed] != 0x02)
                throw new Exception("Invalid signature");

            int sConsumed;
            List<byte> S = new List<byte>(DERDecodeVector(encoded, 1 + rConsumed + 1, out sConsumed));
            if (1 + rConsumed + 1 + sConsumed != encoded.Length)
                throw new Exception("Invalid signature");

            // Return an array containing both R and S
            R.Insert(0,0x00);
            S.Insert(0,0x00);
            return new BigInteger[] { new BigInteger(R.ToArray()), new BigInteger(S.ToArray()) };
        }

        private static void DEREncodeVector(byte[] vector, int idx1, int length, byte[] output, int idx2)
        {
            if (length < 128)
            {
                output[idx2++] = (byte)length;
            }
            else if (length < 256)
            {
                output[idx2++] = 0x81;
                output[idx2++] = (byte)length;
            }
            else
            {
                output[idx2++] = 0x82;
                output[idx2++] = (byte)(length >> 8);
                output[idx2++] = (byte)(length);
            }
            Buffer.BlockCopy(vector, idx1, output, idx2, length);
        }

        private static byte[] DERDecodeVector(byte[] input, int idx, out int consumed)
        {
            int length;
            if (input[idx] < 128)
            {
                length = input[idx];
                consumed = 1 + length;
                idx += 1;
            }
            else if (input[idx] == 0x81)
            {
                length = input[idx + 1];
                consumed = 2 + length;
                idx += 2;
            }
            else if (input[idx] == 0x82)
            {
                length = (input[idx + 1] << 8) | input[idx + 2];
                consumed = 3 + length;
                idx += 3;
            }
            else
            {
                throw new Exception("Unsupported DER vector length");
            }

            byte[] ret = new byte[length];
            Buffer.BlockCopy(input, idx, ret, 0, ret.Length);
            return ret;
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
