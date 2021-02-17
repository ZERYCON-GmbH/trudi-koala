
namespace IVU.Common.Tls.HandshakeLayer.Protocol
{
    using System;
    using System.Collections.Generic;

    using IVU.Common.Tls.Alerts;

    public class HelloEccExtension : HelloExtension
    {
        public readonly List<EccNamedCurve> SupportedEllipticCurves;

        protected HelloEccExtension(HelloExtensionType type)
            : base(type)
        {
            this.SupportedEllipticCurves = new List<EccNamedCurve>();
        }

        public HelloEccExtension()
            : base(HelloExtensionType.EllipticCurves)
        {
            this.SupportedEllipticCurves = new List<EccNamedCurve>();
        }

        public HelloEccExtension(EccNamedCurve curveName)
            : base(HelloExtensionType.EllipticCurves)
        {
            this.SupportedEllipticCurves = new List<EccNamedCurve>();
            this.SupportedEllipticCurves.Add(curveName);
        }

        public HelloEccExtension(IList<EccNamedCurve> curveNames)
            : base(HelloExtensionType.EllipticCurves)
        {
            this.SupportedEllipticCurves = new List<EccNamedCurve>();
            this.SupportedEllipticCurves.AddRange(curveNames);
        }

        protected override byte[] EncodeDataBytes()
        {
            int count = this.SupportedEllipticCurves.Count;

            byte[] data = new byte[2 + 2 * count];
            data[0] = (byte)((2 * count) >> 8);
            data[1] = (byte)(2 * count);
            for (int i = 0; i < count; i++)
            {
                var curveNameBytes = BitConverter.GetBytes((ushort)this.SupportedEllipticCurves[i]);
                data[2 + i * 2] = curveNameBytes[1];
                data[2 + i * 2 + 1] = curveNameBytes[0];
            }

            return data;
        }

        public override string ToString()
        {
            return $"Type: {this.Type}, Elliptic Curves: [{string.Join(",", this.SupportedEllipticCurves)}]";
        }

        protected override void DecodeDataBytes(byte[] data)
        {
            if (data.Length < 2 || (data.Length % 2) != 0)
            {
                throw new AlertException(AlertDescription.IllegalParameter, "Elliptic Curves extension data length invalid: " + data.Length);
            }

            int len = (data[0] << 8) | data[1];
            if (len != data.Length - 2)
            {
                throw new AlertException(AlertDescription.IllegalParameter, "Elliptic Curves extension data invalid");
            }

            for (int i = 0; i < len / 2; i++)
            {
                this.SupportedEllipticCurves.Add((EccNamedCurve)((data[2 + i * 2] << 8) | data[2 + i * 2 + 1]));
            }
        }
    }
}