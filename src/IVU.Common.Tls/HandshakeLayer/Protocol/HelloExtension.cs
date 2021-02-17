using System;
using IVU.Common.Tls.Alerts;
using IVU.Common.Tls.Plugin;

namespace IVU.Common.Tls.HandshakeLayer.Protocol
{
    using IVU.Common.Tls.Alerts;
    using IVU.Common.Tls.Plugin;

    public class HelloExtension
    {
        public readonly HelloExtensionType Type;
        private byte[] _data;

        public byte[] Data
        {
            get { return EncodeDataBytes(); }
            set { DecodeDataBytes(value); }
        }

        public HelloExtension(HelloExtensionType type)
        {
            Type = type;
            _data = new byte[0];
        }

        public HelloExtension(HelloExtensionType type, byte[] data)
        {
            if (data == null)
            {
                throw new AlertException(AlertDescription.InternalError, "Trying to create HandshakeExtension with null data");
            }

            Type = type;
            Data = data;
        }

        public virtual bool SupportsProtocolVersion(ProtocolVersion version)
        {
            return version.HasExtensions;
        }

        protected virtual byte[] EncodeDataBytes()
        {
            return _data;
        }

        protected virtual void DecodeDataBytes(byte[] data)
        {
            _data = (byte[])data.Clone();
        }

        public override string ToString()
        {
            return $"Type: {this.GetType().Name}";
        }
    }
}
