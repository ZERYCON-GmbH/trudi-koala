namespace TRuDI.Models
{
    using System;

    public static class DateTimeExtensions
    {
        public static string ToFormatedString(this DateTime timestamp)
        {
            return timestamp.ToLocalTime().ToString("dd.MM.yyyy HH:mm");
        }

        public static string ToFormatedString(this DateTime? timestamp)
        {
            if (timestamp == null)
            {
                return string.Empty;
            }

            return timestamp.Value.ToFormatedString();
        }

        public static DateTime GetEndTimeOrNow(this DateTime? timestamp)
        {
            if (timestamp == null)
            {
                return DateTime.Now;
            }

            if (timestamp.Value.ToUniversalTime() > DateTime.UtcNow)
            {
                return DateTime.Now;
            }

            return timestamp.Value;
        }

        public static DateTime RoundDown(this DateTime value, int minutes)
        {
            var diff = value.Minute % minutes;
            return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute - diff, 0, value.Kind);
        }

        public static DateTime DayStart(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 0, 0, 0, value.Kind);
        }

        public static DateTime DayEnd(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 23, 59, 59, value.Kind);
        }
        public static DateTime NextDayStart(this DateTime value)
        {
            value = value.Kind == DateTimeKind.Utc
                        ? value.ToUniversalTime().AddDays(1)
                        : value.ToUniversalTime().AddDays(1).ToLocalTime();

            return new DateTime(value.Year, value.Month, value.Day, 0, 0, 0, value.Kind);
        }

        public static string ToIso8601Local(this DateTime timestamp)
        {
            return timestamp.ToString("yyyy-MM-ddTHH:mm:ssK");
        }

        public static string ToIso8601(this DateTime timestamp)
        {
            return timestamp.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

        /// <summary>
        /// If the time is 00:00:00 this method returns 23:59:59  of the previous day.
        /// </summary>
        /// <param name="value">The date/time value to convert.</param>
        /// <returns>The modified date/time.</returns>
        public static DateTime GetDateTimePickerEndDate(this DateTime value)
        {
            if (value.Hour == 0 && value.Minute == 0 && value.Second == 0)
            {
                return value.AddSeconds(-1);
            }

            return value;
        }

        public static string ToIso8601(this DateTime? timestamp)
        {
            if (timestamp == null)
            {
                return string.Empty;
            }

            return timestamp.Value.ToIso8601();
        }

        public static DateTime AddUtcSeconds(this DateTime t, double seconds)
        {
            if (t.Kind != DateTimeKind.Utc)
            {
                var timeUtc = t.ToUniversalTime();
                return timeUtc.AddSeconds(seconds).ToLocalTime();
            }

            return t.AddSeconds(seconds);
        }

        public static DateTime GetPrevMeasurementPeriod(this DateTime t, TimeSpan measurementPeriod)
        {
            if (measurementPeriod == TimeSpan.Zero)
            {
                return t;
            }

            if (measurementPeriod < TimeSpan.FromDays(1))
            {
                var baseTimestamp = new DateTime(t.Year, t.Month, t.Day, 0, 0, 0, t.Kind);

                var span = t.ToUniversalTime() - baseTimestamp.ToUniversalTime();
                if (span == TimeSpan.Zero)
                {
                    return t;
                }

                return baseTimestamp.AddUtcSeconds((int)span.TotalSeconds / (int)measurementPeriod.TotalSeconds * (int)measurementPeriod.TotalSeconds);
            }

            if (measurementPeriod == TimeSpan.FromDays(1))
            {
                return new DateTime(t.Year, t.Month, t.Day, 0, 0, 0, t.Kind);
            }

            if (measurementPeriod == TimeSpan.FromDays(31) || measurementPeriod == TimeSpan.FromDays(30) || measurementPeriod == TimeSpan.FromDays(29) || measurementPeriod == TimeSpan.FromDays(28))
            {
                return new DateTime(t.Year, t.Month, 1, 0, 0, 0, t.Kind);
            }

            return t;
        }
    }
}
