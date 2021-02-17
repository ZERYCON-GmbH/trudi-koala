namespace IVU.Common.Tls.HandshakeLayer.Protocol
{
    using IVU.Common.Tls.Alerts;
    using IVU.Common.Tls.HandshakeLayer.Protocol;
    using System;
    using System.Collections.Generic;

    public class HelloEccPointFormatsExtension : HelloExtension
    {
        public readonly List<EccPointFormats> SupportedEcPointFormats;

        protected HelloEccPointFormatsExtension(HelloExtensionType type)
            : base(type)
        {
        }

        public HelloEccPointFormatsExtension()
            : base(HelloExtensionType.EcPointFormats)
        {
            this.SupportedEcPointFormats = new List<EccPointFormats>();
        }

        public HelloEccPointFormatsExtension(EccPointFormats curveName)
            : base(HelloExtensionType.EcPointFormats)
        {
            this.SupportedEcPointFormats = new List<EccPointFormats>();
            this.SupportedEcPointFormats.Add(curveName);
        }

        public HelloEccPointFormatsExtension(IList<EccPointFormats> curveNames)
            : base(HelloExtensionType.EcPointFormats)
        {
            this.SupportedEcPointFormats = new List<EccPointFormats>();
            this.SupportedEcPointFormats.AddRange(curveNames);
        }

        protected override byte[] EncodeDataBytes()
        {
            int count = this.SupportedEcPointFormats.Count;

            byte[] data = new byte[count + 1];
            data[0] = (byte)count;
            for (int i = 0; i < count; i++)
            {
                data[i + 1] = (byte)this.SupportedEcPointFormats[i];
            }

            return data;
        }

        public override string ToString()
        {
            return $"Type: {this.Type}, Elliptic Curves Point Formats: [{string.Join(",", this.SupportedEcPointFormats)}]";
        }

        protected override void DecodeDataBytes(byte[] data)
        {
            if (data.Length < 1)
            {
                throw new AlertException(AlertDescription.IllegalParameter, "Elliptic Curves Supported Point Format extension data length invalid: " + data.Length);
            }

            int len = (data[0]);
            if (len != data.Length - 2)
            {
                throw new AlertException(AlertDescription.IllegalParameter, "Elliptic Curves Supported Point Format extension data invalid");
            }

            for (int i = 0; i < len; i++)
            {
                this.SupportedEcPointFormats.Add((EccPointFormats)data[2 + i]);
            }
        }
    }
}
