namespace TRuDI.Models
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;

    using TRuDI.HanAdapter.Interface;

    public class ObisId
    {
        public ObisId()
        {
        }

        public ObisId(ObisId src)
        {
            this.A = src.A;
            this.B = src.B;
            this.C = src.C;
            this.D = src.D;
            this.E = src.E;
            this.F = src.F;
        }

        public ObisId(string value)
        {
            if (!TryParse(value, out var id))
            {
                throw new ArgumentException($"Invalid OBIS value: \"{value}\"", nameof(value));
            }

            this.A = id.A;
            this.B = id.B;
            this.C = id.C;
            this.D = id.D;
            this.E = id.E;
            this.F = id.F;
        }

        public static bool TryParse(string value, out ObisId id)
        {
            id = null;

            if (Regex.IsMatch(value, "[0-9A-Fa-f]{12}"))
            {
                var longValue = ulong.Parse(value, NumberStyles.HexNumber);
                if (longValue > 0xFFFFFFFFFFFF)
                {
                    return false;
                }

                id = new ObisId();
                id.A = (byte)((longValue >> 40) & 0xFF);
                id.B = (byte)((longValue >> 32) & 0xFF);
                id.C = (byte)((longValue >> 24) & 0xFF);
                id.D = (byte)((longValue >> 16) & 0xFF);
                id.E = (byte)((longValue >> 8) & 0xFF);
                id.F = (byte)(longValue & 0xFF);
                return true;
            }

            var match = Regex.Match(value, @"^(?<A>\d+)-(?<B>\d+)\:(?<C>\d+)\.(?<D>\d+)\.(?<E>\d+)\*(?<F>\d+)$|^(?<A>\d+)-(?<B>\d+)\:(?<C>\d+)\.(?<D>\d+)\.(?<E>\d+)$|^(?<C>\d+)\.(?<D>\d+)\.(?<E>\d+)$");
            if (match.Success)
            {
                id = new ObisId();

                if (match.Groups["A"].Success)
                {
                    id.A = byte.Parse(match.Groups["A"].Value);
                }
                else
                {
                    id.A = 1;
                }

                if (match.Groups["B"].Success)
                {
                    id.B = byte.Parse(match.Groups["B"].Value);
                }
                else
                {
                    id.B = 0;
                }

                id.C = byte.Parse(match.Groups["C"].Value);
                id.D = byte.Parse(match.Groups["D"].Value);
                id.E = byte.Parse(match.Groups["E"].Value);

                if (match.Groups["F"].Success)
                {
                    id.F = byte.Parse(match.Groups["F"].Value);
                }
                else
                {
                    id.F = 255;
                }

                return true;
            }

            return false;
        }


        public ObisMedium Medium => (ObisMedium)this.A;

        public byte A { get; set; }
        public byte B { get; set; }
        public byte C { get; set; }
        public byte D { get; set; }
        public byte E { get; set; }
        public byte F { get; set; }

        public override string ToString()
        {
            return $"{this.A}-{this.B}:{this.C}.{this.D}.{this.E}*{this.F}";
        }

        public string ToHexString()
        {
            return $"{this.A:X2}{this.B:X2}{this.C:X2}{this.D:X2}{this.E:X2}{this.F:X2}";
        }

        public static bool operator ==(ObisId a, string b)
        {
            if (!TryParse(b, out var bv))
            {
                return false;
            }

            return a == bv;
        }
        public static bool operator !=(ObisId a, string b)
        {
            return !(a == b);
        }

        public static bool operator ==(string a, ObisId b)
        {
            if (!TryParse(a, out var av))
            {
                return false;
            }

            return av == b;
        }

        public static bool operator !=(string a, ObisId b)
        {
            return !(a == b);
        }

        public static bool operator ==(ObisId a, ObisId b)
        {
            if (ReferenceEquals(null, a) && ReferenceEquals(null, b))
            {
                return true;
            }

            if (ReferenceEquals(null, a) || ReferenceEquals(null, b))
            {
                return false;
            }

            return a.A == b.A 
                && a.B == b.B
                && a.C == b.C
                && a.D == b.D
                && a.E == b.E
                && a.F == b.F;
        }

        public static bool operator !=(ObisId a, ObisId b)
        {
            return !(a == b);
        }

        public bool Equals(ObisId other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is ObisId && this.Equals((ObisId)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            var hashCode = -2070488475;
            hashCode = hashCode * -1521134295 + this.Medium.GetHashCode();
            hashCode = hashCode * -1521134295 + this.A.GetHashCode();
            hashCode = hashCode * -1521134295 + this.B.GetHashCode();
            hashCode = hashCode * -1521134295 + this.C.GetHashCode();
            hashCode = hashCode * -1521134295 + this.D.GetHashCode();
            hashCode = hashCode * -1521134295 + this.E.GetHashCode();
            hashCode = hashCode * -1521134295 + this.F.GetHashCode();
            return hashCode;
        }
    }
}
