namespace TRuDI.Models
{
    using System;
    using System.Globalization;

    using TRuDI.Models.BasicData;

    public static class UnitExtensions
    {
        private static readonly CultureInfo CultureInfoDe = CultureInfo.GetCultureInfo("DE");
        private static readonly string[] DecimalFormatLookup = { "N6", "N1", "N2", "N3", "N4", "N5", "N6", "N7", "N8", "N9" };

        public static string GetSiPrefix(this PowerOfTenMultiplier multiplier)
        {
            switch (multiplier)
            {
                case PowerOfTenMultiplier.micro:
                    return "μ";

                case PowerOfTenMultiplier.mili:
                    return "m";

                case PowerOfTenMultiplier.None:
                    return string.Empty;

                case PowerOfTenMultiplier.deca:
                    return "da";

                case PowerOfTenMultiplier.hecto:
                    return "h";

                case PowerOfTenMultiplier.kilo:
                    return "k";

                case PowerOfTenMultiplier.Mega:
                    return "M";

                case PowerOfTenMultiplier.Giga:
                    return "G";

                default:
                    return string.Empty;
            }
        }

        public static string GetUnitSymbol(this Uom uom)
        {
            switch (uom)
            {
                case Uom.Not_Applicable:
                    return string.Empty;

                case Uom.Ampere:
                    return "A";

                case Uom.Volltage:
                    return "V";

                case Uom.Joule:
                    return "J";

                case Uom.Frequency:
                    return "Hz";

                case Uom.AngleDegrees:
                    return "°";

                case Uom.Real_power:
                    return "W";

                case Uom.Cubic_meter:
                    return "m³";

                case Uom.Apparent_power:
                    return "VA";

                case Uom.Reactive_power:
                    return "var";

                case Uom.Power_factor:
                    return "CosPhi";

                case Uom.Volts_squared:
                    return "V²";

                case Uom.Ampere_squared:
                    return "A²";

                case Uom.Apparent_energy:
                    return "VAh";

                case Uom.Real_energy:
                    return "Wh";

                case Uom.Reactive_energie:
                    return "varh";

                case Uom.Ampere_hours:
                    return "Ah";

                case Uom.Cubic_feet:
                    return "ft³";

                case Uom.Cubic_feet_per_hour:
                    return "ft³/h";

                case Uom.Cubic_meter_per_hour:
                    return "m³/h";

                case Uom.US_Gallons:
                    return "US gl";

                case Uom.US_Gallons_per_hour:
                    return "US gl/h";

                default:
                    return string.Empty;
            }
        }

        public static string GetDisplayUnit(this Uom? uom, PowerOfTenMultiplier multiplier)
        {
            if (uom == null)
            {
                return string.Empty;
            }

            return uom.Value.GetDisplayUnit(multiplier);
        }

        public static string GetDisplayUnit(this Uom uom, PowerOfTenMultiplier multiplier)
        {
            if (uom == Uom.Not_Applicable)
            {
                return string.Empty;
            }

            // Special case for Wh --> return kWh
            if (multiplier == PowerOfTenMultiplier.None && uom == Uom.Real_energy)
            {
                return "kWh";
            }

            return multiplier.GetSiPrefix() + uom.GetUnitSymbol();
        }

        public static string GetDisplayValue(this long? value, ReadingType readingType)
        {
            return value.GetDisplayValue(readingType.Uom.Value, readingType.PowerOfTenMultiplier.Value, readingType.Scaler);
        }

        public static string GetDisplayValue(this long? value, Uom uom, PowerOfTenMultiplier multiplier, int scaler)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return value.Value.GetDisplayValue(uom, multiplier, scaler);
        }

        public static string GetDisplayValue(this long value, ReadingType readingType)
        {
            return value.GetDisplayValue(
                readingType.Uom.Value,
                readingType.PowerOfTenMultiplier.Value,
                readingType.Scaler);
        }

        public static string GetDisplayValue(this long value, Uom uom, PowerOfTenMultiplier multiplier, int scaler)
        {
            if (multiplier == PowerOfTenMultiplier.None && uom == Uom.Real_energy)
            {
                scaler = scaler - 3;
                decimal scaledValue = scaler != 0 ? value * (decimal)Math.Pow(10, scaler) : value;
                return scaledValue.ToString(scaler < 0 && scaler >= -9 ? DecimalFormatLookup[scaler * -1] : "N", CultureInfoDe);
            }
            else
            {
                decimal scaledValue = scaler != 0 ? value * (decimal)Math.Pow(10, scaler) : value;
                return scaledValue.ToString(scaler < 0 && scaler >= -9 ? DecimalFormatLookup[scaler * -1] : "N", CultureInfoDe);
            }
        }
    }
}