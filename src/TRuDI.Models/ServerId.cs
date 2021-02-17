namespace TRuDI.Models
{
    using System.Globalization;
    using System.Text.RegularExpressions;

    using TRuDI.HanAdapter.Interface;

    /// <summary>
    /// Identification number for measuring devices.
    /// </summary>
    public class ServerId
    {
        public string Id { get; }
        public string FlagId { get; }
        public byte ProductionBlock { get; }
        public uint Number { get; }
        public uint ProductionYear { get; }
        public ObisMedium Medium { get; }

        public ServerIdType Type { get; }

        public bool IsValid { get; }
        private string rawServerId { get; }

        public ServerId(string serverId)
        {
            this.rawServerId = serverId;

            if (string.IsNullOrWhiteSpace(serverId))
            {
                return;
            }

            serverId = serverId.ToUpperInvariant();

            // Check DIN 43863-5:2012-01, decimal notation
            var match = Regex.Match(serverId.ToUpperInvariant(), @"([0-9E]{1})\s*([A-Z]{3})\s*([0-9]{2})\s*([0-9]{8})");
            if (match.Success)
            {
                this.Id = serverId;

                this.Medium = (ObisMedium)byte.Parse(match.Groups[1].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                this.FlagId = match.Groups[2].Value;
                this.ProductionBlock = byte.Parse(match.Groups[3].Value);
                this.Number = uint.Parse(match.Groups[4].Value);
                this.IsValid = true;
                this.Type = ServerIdType.DIN_43863_5_2012_1;
                return;
            }

            // Check DIN 43863-5:2012-01, hex notation
            match = Regex.Match(serverId.ToUpperInvariant(), "^0A0[0-9E]{1}[0-9A-F]{16}$");
            if (match.Success)
            {
                this.Medium = (ObisMedium)byte.Parse(serverId.Substring(3, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                this.FlagId = $"{(char)byte.Parse(serverId.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture)}{(char)byte.Parse(serverId.Substring(6, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture)}{(char)byte.Parse(serverId.Substring(8, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture)}";
                this.ProductionBlock = byte.Parse(serverId.Substring(10, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                this.Number = uint.Parse(serverId.Substring(12), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                this.IsValid = true;
                this.Type = ServerIdType.DIN_43863_5_2012_1;
                return;
            }

            // Check Wireless-M-Bus address
            var serverIdTmp = serverId.Length == 18 ? serverId : $"01{serverId}";
            match = Regex.Match(serverIdTmp.ToUpperInvariant(), "^01[0-9A-F]{4}[0-9]{8}[0-9A-F]{4}$");
            if (match.Success)
            {
                // 0  2    6        14 16
                // XX FFFF SSSSSSSS VV MM
                this.Medium = this.ConvertMBusMediumToObisMedium(byte.Parse(serverIdTmp.Substring(16, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
                this.FlagId = this.ConvertMBusManufacturerId(
                    byte.Parse(serverIdTmp.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture),
                    byte.Parse(serverIdTmp.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
                this.Number = uint.Parse(serverIdTmp.Substring(6, 8), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                this.Number = uint.Parse(
                    ((uint)((this.Number & 0xFF) << 24) | ((this.Number & 0xFF00) << 8)
                     | ((this.Number & 0xFF0000) >> 8) | ((this.Number & 0xFF000000) >> 24)).ToString("X8"));

                this.IsValid = true;
                this.Type = ServerIdType.WirelessMBusAddress;
                return;
            }

            // Check M-Bus address
            match = Regex.Match(serverId.ToUpperInvariant(), "^02[0-9]{8}[0-9A-F]{8}$");
            if (match.Success)
            {
                // 0  2        10   14 16
                // XX SSSSSSSS FFFF VV MM
                this.Medium = this.ConvertMBusMediumToObisMedium(byte.Parse(serverId.Substring(16, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
                this.FlagId = this.ConvertMBusManufacturerId(
                    byte.Parse(serverId.Substring(10, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture),
                    byte.Parse(serverId.Substring(12, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
                this.Number = uint.Parse(serverId.Substring(2, 8), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                this.Number = uint.Parse(
                    ((uint)((this.Number & 0xFF) << 24) | ((this.Number & 0xFF00) << 8)
                                                        | ((this.Number & 0xFF0000) >> 8) | ((this.Number & 0xFF000000) >> 24)).ToString("X8"));

                this.IsValid = true;
                this.Type = ServerIdType.WiredMBusAddress;
                return;
            }

            // Check DIN 43863-5:2010-07
            match = Regex.Match(serverId.ToUpperInvariant(), "^09[0-9A-F]{18}$");
            if (match.Success)
            {
                // 0  2  4      10 12     
                // XX MM FFFFFF PP SSSSSSSS
                this.Medium = (ObisMedium)byte.Parse(serverId.Substring(3, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                this.FlagId = $"{(char)byte.Parse(serverId.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture)}{(char)byte.Parse(serverId.Substring(6, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture)}{(char)byte.Parse(serverId.Substring(8, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture)}";
                this.ProductionBlock = byte.Parse(serverId.Substring(10, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                this.Number = uint.Parse(serverId.Substring(12), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                this.IsValid = true;
                this.Type = ServerIdType.DIN_43863_5_2010_7;
                return;
            }

            // Check DIN 43863-5:2010-02
            match = Regex.Match(serverId.ToUpperInvariant(), "^06[0-9A-F]{18}$");
            if (match.Success)
            {
                // 0  2      8     
                // XX FFFFFF NNNNNNNNNNNN
                this.FlagId = $"{(char)byte.Parse(serverId.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture)}{(char)byte.Parse(serverId.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture)}{(char)byte.Parse(serverId.Substring(6, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture)}";

                var rawNumber = ulong.Parse(serverId.Substring(8), NumberStyles.HexNumber, CultureInfo.InvariantCulture).ToString("D13");
                this.Medium = (ObisMedium)int.Parse(rawNumber.Substring(0, 1), CultureInfo.InvariantCulture);
                this.ProductionYear = uint.Parse(rawNumber.Substring(1, 2), CultureInfo.InvariantCulture);
                this.ProductionBlock = byte.Parse(rawNumber.Substring(3, 2), CultureInfo.InvariantCulture);
                this.Number = uint.Parse(rawNumber.Substring(5), CultureInfo.InvariantCulture);
                this.IsValid = true;
                this.Type = ServerIdType.DIN_43863_5_2010_2;
                return;
            }

            this.Type = ServerIdType.Unknown;
        }

        public string ToStringWithoutSpace()
        {
            switch (this.Type)
            {
                case ServerIdType.DIN_43863_5_2010_7:
                    return $"{(byte)this.Medium:X1}{this.FlagId}{this.ProductionBlock:X2}{this.Number:D8}";

                case ServerIdType.DIN_43863_5_2012_1:
                    return $"{(byte)this.Medium:X1}{this.FlagId}{this.ProductionBlock:D2}{this.Number:D8}";

                case ServerIdType.DIN_43863_5_2010_2:
                    return $"{(byte)this.Medium:X1}{this.FlagId}{this.ProductionYear:D2}{this.ProductionBlock:D2}{this.Number:D8}";

                case ServerIdType.EthernetMacAddress:
                    return this.rawServerId.ToUpperInvariant();

                case ServerIdType.WiredMBusAddress:
                case ServerIdType.WirelessMBusAddress:
                    return this.rawServerId.ToUpperInvariant();

                default:
                    return this.rawServerId;
            }
        }

        public override string ToString()
        {
            switch (this.Type)
            {
                case ServerIdType.DIN_43863_5_2010_7:
                    return $"{(byte)this.Medium:X1} {this.FlagId} {this.ProductionBlock:X2} {this.Number:D8}";

                case ServerIdType.DIN_43863_5_2012_1:
                    return $"{(byte)this.Medium:X1} {this.FlagId} {this.ProductionBlock:D2} {this.Number:D8}";

                case ServerIdType.DIN_43863_5_2010_2:
                    return $"{(byte)this.Medium:X1} {this.FlagId} {this.ProductionYear:D2} {this.ProductionBlock:D2} {this.Number:D8}";

                case ServerIdType.EthernetMacAddress:
                    return this.rawServerId.ToUpperInvariant();

                case ServerIdType.WiredMBusAddress:
                case ServerIdType.WirelessMBusAddress:
                    return $"{this.FlagId} {this.Number:D8}";

                default:
                    return this.rawServerId;
            }
        }

        public string ToHexString()
        {
            switch (this.Type)
            {
                case ServerIdType.DIN_43863_5_2012_1:
                    return $"0A{(byte)this.Medium:X2}{(byte)this.FlagId[0]:X2}{(byte)this.FlagId[1]:X2}{(byte)this.FlagId[2]:X2}{this.ProductionBlock:X2}{this.Number:X8}";

                case ServerIdType.DIN_43863_5_2010_7:
                case ServerIdType.DIN_43863_5_2010_2:
                    return this.rawServerId.ToUpperInvariant();

                case ServerIdType.EthernetMacAddress:
                    return this.rawServerId.ToUpperInvariant();

                case ServerIdType.WiredMBusAddress:
                case ServerIdType.WirelessMBusAddress:
                    return this.rawServerId.ToUpperInvariant();

                default:
                    return this.rawServerId;
            }
        }

        private ObisMedium ConvertMBusMediumToObisMedium(byte medium)
        {
            switch (medium)
            {
                case 0x02:
                    return ObisMedium.Electricity;

                case 0x08:
                    return ObisMedium.HeatCostAllocator;

                case 0x0A:
                case 0x0B:
                    return ObisMedium.Cooling;

                case 0x04:
                case 0x0D:
                case 0x0C:
                    return ObisMedium.Heat;

                case 0x03:
                    return ObisMedium.Gas;

                case 0x07:
                case 0x16:
                case 0x17:
                    return ObisMedium.WaterCold;

                case 0x06:
                    return ObisMedium.WaterHot;

                default:
                    return ObisMedium.Abstract;
            }
        }

        private string ConvertMBusManufacturerId(byte id1, byte id2)
        {
            var compressedBits = (ushort)(id1 | (id2 << 8));

            return new string(
                new char[]
                    {
                        (char)(((compressedBits & 0x7C00) >> 10) + 64),
                        (char)(((compressedBits & 0x3E0) >> 5) + 64),
                        (char)((compressedBits & 0x1F) + 64)
                    });
        }

        public enum ServerIdType : byte
        {
            Unknown = 0x00,
            DIN_43863_5_2012_1 = 0x0A,
            DIN_43863_5_2010_2 = 0x06,
            DIN_43863_5_2010_7 = 0x09,
            EthernetMacAddress = 0x05,
            WirelessMBusAddress = 0x01,
            WiredMBusAddress = 0x02,
        }
    }
}
