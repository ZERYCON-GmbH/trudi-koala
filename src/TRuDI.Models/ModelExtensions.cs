namespace TRuDI.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;

    using TRuDI.Models.BasicData;
    using TRuDI.Models.CheckData;

    public static class ModelExtensions
    {
        /// <summary>
        /// Diese Funktion wird benutzt um den von der Xml Datei gelieferten Hex String in ein Byte Array umzuwandeln.
        /// </summary>
        /// <param name="cert">Die Instanz der Klasse Certificate, der das Zertifikat zugewiesen wird</param>
        /// <param name="hex">Der Hex String, der in ein Byte Array umgewandelt wird</param>
        public static void HexStringToByteArray(this Certificate cert, string hex)
        {
            cert.CertContent = Enumerable.Range(0, hex.Length)
                                .Where(x => x % 2 == 0)
                                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                                .ToArray();
        }

        /// <summary>
        /// In der Funktion wird aus dem Byterarray CertContent das Zertifikat erzeugt
        /// </summary>
        /// <returns>Gibt ein X509Certificate2 zurück</returns>
        public static X509Certificate2 GetCert(this Certificate cert)
        {
            // TODO: Absichern der Funktion
            return new X509Certificate2(cert.CertContent);
        }

        /// <summary>
        /// Funktion zur Berechung des Endzeitpunkts des Intervals
        /// </summary>
        /// <returns>Den Endzeitpunkt des Intervals</returns>
        public static DateTime GetEnd(this Interval interval)
        {
            if (interval.Duration == null)
            {
                return interval.Start;
            }

            return interval.Start.AddUtcSeconds(interval.Duration.Value);
        }

        /// <summary>
        /// Funktion zum Test ob ein String einen gültigen Hex String darstellt
        /// </summary>
        /// <param name="hex">Der zu überprüfende String</param>
        /// <returns>Gibt einen Wahrheitswert zurück</returns>
        public static bool ValidateHexString(this string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
            {
                return false;
            }

            if (hex.Length % 2 == 1)
            {
                return false;
            }

            foreach (char c in hex)
            {
                switch (c)
                {
                    case '0':
                        break;
                    case '1':
                        break;
                    case '2':
                        break;
                    case '3':
                        break;
                    case '4':
                        break;
                    case '5':
                        break;
                    case '6':
                        break;
                    case '7':
                        break;
                    case '8':
                        break;
                    case '9':
                        break;
                    case 'a':
                        break;
                    case 'A':
                        break;
                    case 'b':
                        break;
                    case 'B':
                        break;
                    case 'c':
                        break;
                    case 'C':
                        break;
                    case 'd':
                        break;
                    case 'D':
                        break;
                    case 'e':
                        break;
                    case 'E':
                        break;
                    case 'f':
                        break;
                    case 'F':
                        break;
                    default:
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Die Funktion kürzt mögliche Sekundenwerte eines DateTime Objekts
        /// </summary>
        /// <param name="dateTime">Das zu kürzende DateTime Objekt</param>
        /// <returns>Das gekürzte DateTime Objekt</returns>
        public static DateTime GetDateWithoutSeconds(this DateTime dateTime)
        {
            var utcConvertedTime = dateTime.ToUniversalTime();
            var utcWithoutSeconds = new DateTime(utcConvertedTime.Year, utcConvertedTime.Month, utcConvertedTime.Day, utcConvertedTime.Hour, utcConvertedTime.Minute, 0, DateTimeKind.Utc);
            return dateTime.Kind == DateTimeKind.Utc ? utcWithoutSeconds : utcWithoutSeconds.ToLocalTime();
        }

        /// <summary>
        /// Die Funktion liefert einen an das Messperiodenintervall ausgerichteten Zeitstempel zurück.
        /// </summary>
        /// <param name="timestamp">Der auszurichtende Zeitstempel.</param>
        /// <param name="interval">Das Messperiodenintervall.</param>
        /// <param name="toleranceLimit">Toleranzgrenze in Prozent.</param>
        /// <returns>Der ausgerichtete Zeitstempel.</returns>
        public static DateTime GetAlignedTimestamp(DateTime timestamp, int interval = 900, int toleranceLimit = 1)
        {
            if (interval <= 0)
            {
                return timestamp;
            }

            if (interval < 86400)
            {
                var isUtc = timestamp.Kind == DateTimeKind.Utc;
                var captureTimeUtc = timestamp.ToUniversalTime();
                if (!isUtc)
                {
                    captureTimeUtc += TimeZoneInfo.Local.GetUtcOffset(timestamp);
                }

                var diffSpan = (int)(captureTimeUtc - captureTimeUtc.Date).TotalSeconds;
                var diff = interval - (diffSpan % interval);
                if (diff == 0 || diff == interval)
                {
                    return timestamp;
                }

                var previousPeriod = captureTimeUtc.Date.AddSeconds((diffSpan / interval) * interval);
                var window = interval * toleranceLimit / 100;
                if (diff <= window)
                {
                    captureTimeUtc = captureTimeUtc.AddSeconds(diff);
                }
                else if (interval - diff <= window)
                {
                    captureTimeUtc = captureTimeUtc.AddSeconds(diff - interval);
                }
                else
                {
                    return timestamp;
                }

                return isUtc ? captureTimeUtc : (captureTimeUtc - TimeZoneInfo.Local.GetUtcOffset(timestamp)).ToLocalTime();
            }

            if (interval == 86400)
            {
                var limit = 86400 * toleranceLimit / 100;

                var thisDay = timestamp.Date;
                if (timestamp >= thisDay.AddSeconds(-limit) && timestamp <= thisDay.AddSeconds(limit))
                {
                    return thisDay;
                }

                var nextDay = thisDay.AddDays(1);
                if (timestamp >= nextDay.AddSeconds(-limit) && timestamp <= nextDay.AddSeconds(limit))
                {
                    return nextDay;
                }
            }

            return timestamp;
        }

        /// <summary>
        /// Returns true if the specified timestamp is valid for the specified interval.
        /// </summary>
        /// <param name="timestamp">The timestmap to check.</param>
        /// <param name="interval">The period interval.</param>
        /// <param name="toleranceLimit">Tolerance limit in percent.</param>
        /// <returns><c>true</c> if the timestamp is valid.</returns>
        public static bool IsValidMeasurementPeriodTimestamp(this DateTime timestamp, int interval = 900, int toleranceLimit = 1)
        {
            if (interval == 0)
            {
                return true;
            }

            if (interval < 86400)
            {
                var isUtc = timestamp.Kind == DateTimeKind.Utc;
                var captureTimeUtc = timestamp.ToUniversalTime();
                if (!isUtc)
                {
                    captureTimeUtc += TimeZoneInfo.Local.GetUtcOffset(timestamp);
                }

                var diffSpan = (int)(captureTimeUtc - captureTimeUtc.Date).TotalSeconds;
                var diff = interval - (diffSpan % interval);
                if (diff == 0 || diff == interval)
                {
                    return true;
                }

                var previousPeriod = captureTimeUtc.Date.AddSeconds((diffSpan / interval) * interval);
                var window = interval * toleranceLimit / 100;
                if (diff <= window)
                {
                    return true;
                }
                else if (interval - diff <= window)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            if (interval == 86400)
            {
                var limit = 86400 * toleranceLimit / 100;

                var thisDay = timestamp.Date;
                if (timestamp >= thisDay.AddSeconds(-limit) && timestamp <= thisDay.AddSeconds(limit))
                {
                    return true;
                }

                var nextDay = thisDay.AddDays(1);
                if (timestamp >= nextDay.AddSeconds(-limit) && timestamp <= nextDay.AddSeconds(limit))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes readings not valid for the specified interval. Only the first interval reading isn't checked.
        /// </summary>
        /// <param name="readings">List of readings.</param>
        /// <param name="interval">The measurement period.</param>
        /// <param name="toleranceLimit">Tolerance limit in percent.</param>
        public static void FilterIntervalReadings(this List<IntervalReading> readings, int interval, int toleranceLimit = 1)
        {
            for (int i = 1; i < readings.Count; i++)
            {
                var reading = readings[i];
                if (!reading.TimePeriod.Start.IsValidMeasurementPeriodTimestamp(interval, toleranceLimit))
                {
                    readings.RemoveAt(i);
                    i--;
                }
            }
        }
        
        /// <summary>
        /// Validiert den FNNStatus 
        /// </summary>
        /// <param name="statusFNN"></param>
        /// <returns>True wenn der FNN Status gültig ist, False falls nicht</returns>
        public static bool ValidateFNNStatus(this StatusFNN statusFNN)
        {
            return statusFNN.Validate();
        }

        public static DateTime GetDateTimeFromSpecialDayProfile(SpecialDayProfile sdp, DayTimeProfile dtp)
        {
            return new DateTime((int)sdp.SpecialDayDate.Year,
                                (int)sdp.SpecialDayDate.Month,
                                (int)sdp.SpecialDayDate.DayOfMonth,
                                (int)dtp.StartTime.Hour,
                                (int)dtp.StartTime.Minute,
                                (int)dtp.StartTime.Second);
        }

        public static DateTime GetDate(this DayVarType date)
        {
            return new DateTime((int)date.Year, (int)date.Month, (int)date.DayOfMonth);
        }

        public static TimeSpan GetTime(this DayTimeProfile time)
        {
            return new TimeSpan((int)time.StartTime.Hour, (int)time.StartTime.Minute, (int)time.StartTime.Second);
        }

        public static bool IsPeriodInIntervalBlock(this Interval interval, DateTime start, DateTime end)
        {
            if (start >= interval.Start && end <= interval.GetEnd())
            {
                return true;
            }

            return false;
        }
    }
}
