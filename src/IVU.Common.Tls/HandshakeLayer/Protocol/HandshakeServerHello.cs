﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using IVU.Common.Tls.Alerts;
using IVU.Common.Tls.Plugin;
using IVU.Common.Tls.Plugin.CipherSuitePluginInterface;

namespace IVU.Common.Tls.HandshakeLayer.Protocol
{
    using IVU.Common.Tls.Alerts;
    using IVU.Common.Tls.Plugin;

    public class HandshakeServerHello : HandshakeMessage
    {
        public ProtocolVersion ServerVersion;
        public HandshakeRandom Random;
        public byte[] SessionID;
        public CipherSuiteId CipherSuite;
        public Byte CompressionMethod;

        public readonly List<HelloExtension> Extensions;

        internal HandshakeServerHello() : this(ProtocolVersion.NULL) { }

        public HandshakeServerHello(ProtocolVersion version)
            : base(HandshakeMessageType.ServerHello, version)
        {
            ServerVersion = version;
            Random = new HandshakeRandom();
            SessionID = new byte[0];
            CipherSuite = 0x0000;
            CompressionMethod = 0x00;
            Extensions = new List<HelloExtension>();
        }

        public override string ToString()
        {
            return $"ServerHello: {this.ServerVersion}, cipher suite: {this.CipherSuite}";
        }

        protected override byte[] EncodeDataBytes(ProtocolVersion ver)
        {
            MemoryStream memStream = new MemoryStream();
            HandshakeStream stream = new HandshakeStream(memStream);

            stream.WriteUInt8(ServerVersion.Major);
            stream.WriteUInt8(ServerVersion.Minor);

            stream.WriteBytes(Random.GetBytes());

            stream.WriteUInt8((Byte)SessionID.Length);
            stream.WriteBytes(SessionID);

            stream.WriteUInt16((ushort)CipherSuite);
            stream.WriteUInt8(CompressionMethod);

            if (Extensions.Count > 0)
            {
                int length = 0;
                foreach (HelloExtension ext in Extensions)
                {
                    if (!ext.SupportsProtocolVersion(ServerVersion))
                        continue;
                    length += 4 + ext.Data.Length;
                }
                stream.WriteUInt16((UInt16)length);
                foreach (HelloExtension ext in Extensions)
                {
                    if (!ext.SupportsProtocolVersion(ServerVersion))
                        continue;

                    HelloExtensionType type = ext.Type;
                    byte[] data = ext.Data;

                    stream.WriteUInt16((ushort)type);
                    stream.WriteUInt16((UInt16)data.Length);
                    stream.WriteBytes(data);
                }
            }

            return memStream.ToArray();
        }

        protected override void DecodeDataBytes(ProtocolVersion ver, byte[] data)
        {
            MemoryStream memStream = new MemoryStream(data);
            HandshakeStream stream = new HandshakeStream(memStream);

            byte major = stream.ReadUInt8();
            byte minor = stream.ReadUInt8();
            ServerVersion = new ProtocolVersion(major, minor);

            byte[] randomBytes = stream.ReadBytes(32);
            Random = new HandshakeRandom(randomBytes);

            int idLength = (int)stream.ReadUInt8();
            SessionID = stream.ReadBytes(idLength);
            Trace.WriteLine($"SessionId: {SessionID}");

            CipherSuite = (CipherSuiteId)stream.ReadUInt16();
            Trace.WriteLine($"Cipher Suite Id: {CipherSuite} refer to list at http://www.thesprawl.org/research/tls-and-ssl-cipher-suites/");
            CompressionMethod = stream.ReadUInt8();

            byte[] extensionList = new byte[0];
            if (!stream.EndOfStream && ServerVersion.HasExtensions)
            {
                UInt16 extensionListLength = stream.ReadUInt16();
                extensionList = stream.ReadBytes(extensionListLength);
            }
            stream.ConfirmEndOfStream();

            int pos = 0;
            while (pos + 4 <= extensionList.Length)
            {
                HelloExtensionType extensionType = (HelloExtensionType)(UInt16)((extensionList[pos] << 8) | extensionList[pos + 1]);
                Trace.WriteLine($"Extension Type { extensionType }");
                UInt16 extensionDataLength = (UInt16)((extensionList[pos + 2] << 8) | extensionList[pos + 3]);
                pos += 4;

                if (pos + extensionDataLength > extensionList.Length)
                {
                    throw new AlertException(AlertDescription.IllegalParameter,
                                             "ServerHello extension data length too large: " + extensionDataLength);
                }

                byte[] extensionData = new byte[extensionDataLength];
                Buffer.BlockCopy(extensionList, pos, extensionData, 0, extensionData.Length);
                pos += extensionData.Length;

                Extensions.Add(new HelloExtension(extensionType, extensionData));
            }
        }
    }
}


