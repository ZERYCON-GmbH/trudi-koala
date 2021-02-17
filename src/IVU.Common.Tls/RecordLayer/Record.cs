using System;
using System.Diagnostics;
using IVU.Common.Tls.Plugin;

namespace IVU.Common.Tls.RecordLayer
{
    using IVU.Common.Tls.Plugin;

    [DebuggerDisplay("Type: {Type} Version: {Version} Seq: {SequenceNumber} Len: {FragmentLength}")]
    public class Record
    {
        private byte[] fragment;

        public RecordType Type { get; }

        public ProtocolVersion Version { get; }

        public UInt16 Epoch { get; set; }

        public UInt64 SequenceNumber { get; set; }

        public byte[] Fragment
        {
            get  { return this.fragment; }
            set
            {
                if (value == null)
                {
                    this.fragment = new byte[0];
                    return;
                }

                fragment = value;
            }
        }

        public int FragmentLength => this.Fragment?.Length ?? 0;

        public Record(RecordType type, ProtocolVersion version)
            : this(type, version, new byte[0])
        {
        }

        public Record(RecordType type, ProtocolVersion version, byte[] fragment)
        {
            this.Type = type;
            this.Version = version;
            this.Fragment = fragment;
        }

        public Record(byte[] header)
        {
            if (header == null || header.Length < 5)
            {
                throw new ArgumentException("Invalid record header");
            }

            this.Type = (RecordType)header[0];
            this.Version = new ProtocolVersion(header[1], header[2]);
            if (this.Version.IsUsingDatagrams)
            {
                this.Epoch = (UInt16)((header[3] << 8) | header[4]);
                for (int i = 0; i < 6; i++)
                {
                    this.SequenceNumber <<= 8;
                    this.SequenceNumber |= header[5 + i];
                }
                this.Fragment = new byte[(header[11] << 8) | header[12]];
            }
            else
            {
                this.Fragment = new byte[(header[3] << 8) | header[4]];
            }
        }

        public byte[] GetBytes()
        {
            byte[] ret;
            if (this.Version.IsUsingDatagrams)
            {
                ret = new byte[13 + this.fragment.Length];

                ret[0] = (byte)Type;
                ret[1] = this.Version.Major;
                ret[2] = this.Version.Minor;
                ret[3] = (byte)(this.Epoch >> 8);
                ret[4] = (byte)(this.Epoch);
                for (int i = 0; i < 6; i++)
                {
                    ret[5 + i] = (byte)(this.SequenceNumber >> (5 - i));
                }
                ret[11] = (byte)(this.fragment.Length >> 8);
                ret[12] = (byte)(this.fragment.Length);
                Array.Copy(this.fragment, 0, ret, 12, this.fragment.Length);
            }
            else
            {
                ret = new byte[5 + this.fragment.Length];

                ret[0] = (byte)Type;
                ret[1] = this.Version.Major;
                ret[2] = this.Version.Minor;
                ret[3] = (byte)(this.fragment.Length >> 8);
                ret[4] = (byte)(this.fragment.Length);
                Array.Copy(this.fragment, 0, ret, 5, this.fragment.Length);
            }
            return ret;
        }
    }
}
