using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using IVU.Common.Tls.Alerts;
using IVU.Common.Tls.Plugin;

namespace IVU.Common.Tls.HandshakeLayer.Protocol
{
    using System.Diagnostics;

    using IVU.Common.Tls.Alerts;
    using IVU.Common.Tls.Plugin;

    public class HandshakeCertificateRequest : HandshakeMessage
    {
        public readonly List<CertificateType> CertificateTypes;
        public readonly List<ushort> SignatureAndHashAlgorithms;
        public readonly List<string> CertificateAuthorities;

        public HandshakeCertificateRequest(ProtocolVersion version) : base(HandshakeMessageType.CertificateRequest, version)
        {
            CertificateTypes = new List<CertificateType>();
            SignatureAndHashAlgorithms = new List<ushort>();
            CertificateAuthorities = new List<string>();
        }

        protected override byte[] EncodeDataBytes(ProtocolVersion version)
        {
            int typesLength = 0;
            if (CertificateTypes.Count > 255)
            {
                throw new Exception("Number of certificate types too large: " + CertificateTypes.Count);
            }
            else {
                typesLength = CertificateTypes.Count;
            }

            int sighashLength = 0;
            if (version.HasSelectableSighash)
            {
                if (SignatureAndHashAlgorithms.Count > 65535)
                {
                    throw new Exception("Number of sighash values too large: " + SignatureAndHashAlgorithms.Count);
                }
                else {
                    sighashLength = 2 * SignatureAndHashAlgorithms.Count;
                }
            }

            int authsLength = 0;
            foreach (string name in CertificateAuthorities)
            {
                // TODO: Should support punycode as well?
                authsLength += 2;
                authsLength += Encoding.ASCII.GetBytes(name).Length;
                if (authsLength > 65535)
                {
                    throw new Exception("Certificate authorities length too large");
                }
            }

            MemoryStream memStream = new MemoryStream();
            HandshakeStream stream = new HandshakeStream(memStream);

            stream.WriteUInt8((byte)typesLength);
            foreach (byte type in CertificateTypes)
            {
                stream.WriteUInt8(type);
            }

            if (version.HasSelectableSighash)
            {
                stream.WriteUInt16((UInt16)sighashLength);
                foreach (UInt16 sighash in SignatureAndHashAlgorithms)
                {
                    stream.WriteUInt16(sighash);
                }
            }

            stream.WriteUInt16((UInt16)authsLength);
            foreach (string name in CertificateAuthorities)
            {
                // TODO: Should support punycode as well?
                int nameLen = Encoding.ASCII.GetBytes(name).Length;
                stream.WriteUInt16((UInt16)nameLen);
                stream.WriteBytes(Encoding.ASCII.GetBytes(name));
            }

            return memStream.ToArray();
        }

        protected override void DecodeDataBytes(ProtocolVersion version, byte[] data)
        {
            CertificateTypes.Clear();
            CertificateAuthorities.Clear();

            MemoryStream memStream = new MemoryStream(data);
            HandshakeStream stream = new HandshakeStream(memStream);

            int typesLength = stream.ReadUInt8();
            for (int i = 0; i < typesLength; i++)
            {
                CertificateTypes.Add((CertificateType)stream.ReadUInt8());
            }

            if (version.HasSelectableSighash)
            {
                int sighashLength = stream.ReadUInt16();
                if ((sighashLength % 2) != 0)
                {
                    throw new AlertException(AlertDescription.IllegalParameter,
                                             "SianatureAndHashAlgorithms length invalid: " + sighashLength);
                }

                byte[] sighashData = stream.ReadBytes(sighashLength);
                for (int i = 0; i < sighashLength; i += 2)
                {
                    SignatureAndHashAlgorithms.Add((UInt16)((sighashData[i] << 8) | sighashData[i + 1]));
                }
            }

            int authsLength = stream.ReadUInt16();
            byte[] authData = stream.ReadBytes(authsLength);
            stream.ConfirmEndOfStream();

            int position = 0;
            while (position < authData.Length)
            {
                int authLength = (authData[position] << 8) | authData[position + 1];
                position += 2;

                if (position > authData.Length)
                {
                    throw new AlertException(AlertDescription.IllegalParameter,
                                             "Authorities total length doesn't match contents");
                }

                string name = Encoding.ASCII.GetString(authData, position, authLength);
                position += authLength;

                CertificateAuthorities.Add(name);
            }
        }
    }
}
