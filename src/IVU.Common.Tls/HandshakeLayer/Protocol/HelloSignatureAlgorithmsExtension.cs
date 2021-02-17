
namespace IVU.Common.Tls.HandshakeLayer.Protocol
{
    using System.Collections.Generic;
    using System.Linq;

    using IVU.Common.Tls.Alerts;
    using IVU.Common.Tls.Plugin;
    
    public class HelloSignatureAlgorithmsExtension : HelloExtension
    {
        public readonly List<SignatureAndHashAlgorithm> SupportedSignatureAlgorithms;

        public HelloSignatureAlgorithmsExtension()
            : base(HelloExtensionType.SignatureAlgorithms)
        {
            this.SupportedSignatureAlgorithms = new List<SignatureAndHashAlgorithm>();
        }

        public HelloSignatureAlgorithmsExtension(SignatureAndHashAlgorithm[] supported)
            : base(HelloExtensionType.SignatureAlgorithms)
        {
            this.SupportedSignatureAlgorithms = new List<SignatureAndHashAlgorithm>(supported);
        }

        public override bool SupportsProtocolVersion(ProtocolVersion version)
        {
            return version.HasSelectableSighash;
        }

        protected override byte[] EncodeDataBytes()
        {
            int count = SupportedSignatureAlgorithms.Count;

            byte[] data = new byte[2 + 2 * count];
            data[0] = (byte)((2 * count) >> 8);
            data[1] = (byte)(2 * count);
            for (int i = 0; i < count; i++)
            {
                data[2 + i * 2] = (byte)(SupportedSignatureAlgorithms[i].HashAlgorithm);
                data[2 + i * 2 + 1] = (byte)(SupportedSignatureAlgorithms[i].SignatureAlgorithm);
            }

            return data;
        }

        protected override void DecodeDataBytes(byte[] data)
        {
            if (data.Length < 2 || (data.Length % 2) != 0)
            {
                throw new AlertException(AlertDescription.IllegalParameter, "SignatureAlgorithms extension data length invalid: " + data.Length);
            }

            int len = (data[0] << 8) | data[1];
            if (len != data.Length - 2)
            {
                throw new AlertException(AlertDescription.IllegalParameter, "SignatureAlgorithms extension data invalid");
            }
            for (int i = 0; i < len / 2; i++)
            {
                var tuple = new SignatureAndHashAlgorithm
                {
                    HashAlgorithm = (HashAlgorithmType)(data[2 + i * 2]),
                    SignatureAlgorithm = (SignatureAlgorithmType)data[2 + i * 2 + 1]
                };
                SupportedSignatureAlgorithms.Add(tuple);
            }
        }

        public override string ToString()
        {
            return $"Type: {this.Type}, Signature algorithms: [{string.Join(",", this.SupportedSignatureAlgorithms.Select(s => s.HashAlgorithm + "_" + s.SignatureAlgorithm))}]";
        }
    }
}