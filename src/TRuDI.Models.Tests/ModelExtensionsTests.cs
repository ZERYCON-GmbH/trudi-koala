namespace TRuDI.Models.Tests
{
    using System;
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TRuDI.Models;
    using TRuDI.Models.BasicData;

    [TestClass]
    public class ModelExtensionsTests
    {
        [TestMethod]
        public void TestGetSmoothCaptureTime()
        {
            var expected = new DateTime(2017, 1, 1, 10, 15, 0, DateTimeKind.Local);
            var actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 1, 1, 10, 15, 0, DateTimeKind.Local));
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 1, 1, 10, 15, 0, DateTimeKind.Utc);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 1, 1, 10, 15, 7, DateTimeKind.Utc));
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 1, 1, 10, 15, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 1, 1, 10, 14, 53, DateTimeKind.Local));
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 1, 1, 10, 15, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 1, 1, 10, 15, 9, DateTimeKind.Local));
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 1, 1, 10, 15, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 1, 1, 10, 14, 51, DateTimeKind.Local));
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 1, 1, 10, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 1, 1, 10, 0, 18, DateTimeKind.Local), 3600);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 1, 1, 0, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 1, 1, 0, 12, 0, DateTimeKind.Local), 86400);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 1, 1, 0, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2016, 12, 31, 23, 53, 5, DateTimeKind.Local), 86400);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            
            // Periode von 2 Stunden (7200 Sekunden) Toleranzbereich 72 Sekunden (Sommerzeit)

            var period = 7200;
            expected = new DateTime(2017, 8, 1, 0, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 8, 1, 0, 1, 12, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 8, 1, 0, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 8, 1, 0, 1, 11, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 8, 1, 0, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 7, 31, 23, 58, 48, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 8, 1, 0, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 7, 31, 23, 58, 49, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 8, 1, 0, 1, 13, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 8, 1, 0, 1, 13, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 7, 31, 23, 58, 47, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 7, 31, 23, 58, 47, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            // Periode von 3 Stunden (10800 Sekunden) Toleranzbereich 108 Sekunden (Sommerzeit)

            period = 10800;
            expected = new DateTime(2017, 8, 1, 0, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 8, 1, 0, 1, 48, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 8, 1, 0, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 8, 1, 0, 1, 47, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 8, 1, 0, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 7, 31, 23, 58, 12, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 8, 1, 0, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 7, 31, 23, 58, 13, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 8, 1, 0, 1, 49, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 8, 1, 0, 1, 49, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 7, 31, 23, 58, 11, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 7, 31, 23, 58, 11, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            // Periode von 6 Stunden (21600 Sekunden) Toleranzbereich 216 Sekunden (Sommerzeit)

            period = 21600;
            expected = new DateTime(2017, 8, 1, 0, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 8, 1, 0, 3, 36, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 8, 1, 0, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 8, 1, 0, 3, 35, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 8, 1, 0, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 7, 31, 23, 56, 24, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 8, 1, 0, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 7, 31, 23, 56, 25, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 8, 1, 0, 3, 37, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 8, 1, 0, 3, 37, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 7, 31, 23, 56, 23, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 7, 31, 23, 56, 23, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            // Periode von 2 Stunden (7200 Sekunden) Toleranzbereich 72 Sekunden (Winterzeit)

            period = 7200;
            expected = new DateTime(2017, 11, 1, 0, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 11, 1, 0, 1, 12, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 11, 1, 0, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 11, 1, 0, 1, 11, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 11, 1, 0, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 10, 31, 23, 58, 48, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 11, 1, 0, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 10, 31, 23, 58, 49, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 11, 1, 0, 1, 13, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 11, 1, 0, 1, 13, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 10, 31, 23, 58, 47, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 10, 31, 23, 58, 47, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            // Periode von 3 Stunden (10800 Sekunden) Toleranzbereich 108 Sekunden (Winterzeit)

            period = 10800;
            expected = new DateTime(2017, 11, 1, 0, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 11, 1, 0, 1, 48, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 11, 1, 0, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 11, 1, 0, 1, 47, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 11, 1, 0, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 10, 31, 23, 58, 12, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 11, 1, 0, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 10, 31, 23, 58, 13, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 11, 1, 0, 1, 49, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 11, 1, 0, 1, 49, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 10, 31, 23, 58, 11, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 10, 31, 23, 58, 11, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            // Periode von 6 Stunden (21600 Sekunden) Toleranzbereich 216 Sekunden (Winterzeit)

            period = 21600;
            expected = new DateTime(2017, 11, 1, 0, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 11, 1, 0, 3, 36, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 11, 1, 0, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 11, 1, 0, 3, 35, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 11, 1, 0, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 10, 31, 23, 56, 24, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 11, 1, 0, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 10, 31, 23, 56, 25, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 11, 1, 0, 3, 37, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 11, 1, 0, 3, 37, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            expected = new DateTime(2017, 10, 31, 23, 56, 23, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 10, 31, 23, 56, 23, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);


            // Umstellung auf Sommerzeit 2017 

            period = 900;
            expected = new DateTime(2017, 3, 26, 2, 30, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 3, 26, 2, 30, 0, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

            // Umstellung auf Winterzeit 2017 

            expected = new DateTime(2017, 10, 29, 3, 0, 0, DateTimeKind.Local);
            actual = ModelExtensions.GetAlignedTimestamp(new DateTime(2017, 10, 29, 3, 0, 0, DateTimeKind.Local), period);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected.Kind, actual.Kind);

        }

        [TestMethod]
        public void TestIsValidMeasurementPeriodTimestamp()
        {
            Assert.IsTrue(ModelExtensions.IsValidMeasurementPeriodTimestamp(new DateTime(2016, 12, 31, 23, 53, 5, DateTimeKind.Local), 86400));
            Assert.IsFalse(ModelExtensions.IsValidMeasurementPeriodTimestamp(new DateTime(2016, 12, 31, 23, 40, 5, DateTimeKind.Local), 86400));
            Assert.IsFalse(ModelExtensions.IsValidMeasurementPeriodTimestamp(new DateTime(2016, 12, 31, 23, 00, 5, DateTimeKind.Local), 86400));
            Assert.IsTrue(ModelExtensions.IsValidMeasurementPeriodTimestamp(new DateTime(2017, 01, 01, 00, 03, 7, DateTimeKind.Local), 86400));
            Assert.IsTrue(ModelExtensions.IsValidMeasurementPeriodTimestamp(new DateTime(2016, 12, 31, 17, 45, 5, DateTimeKind.Local), 900));
            Assert.IsTrue(ModelExtensions.IsValidMeasurementPeriodTimestamp(new DateTime(2016, 12, 31, 17, 44, 55, DateTimeKind.Local), 900));
            Assert.IsFalse(ModelExtensions.IsValidMeasurementPeriodTimestamp(new DateTime(2016, 12, 31, 17, 40, 5, DateTimeKind.Local), 900));
            Assert.IsFalse(ModelExtensions.IsValidMeasurementPeriodTimestamp(new DateTime(2016, 12, 31, 17, 45, 30, DateTimeKind.Local), 900));
        }

        /// <summary>
        /// testing the extension on days when DST change happens
        /// </summary>
        [TestMethod]
        public void TestIntervalGetEnd()
        {
            Interval _interval = new Interval() { Start = new DateTime(2017, 3, 26, 0, 0, 0, DateTimeKind.Local), Duration = 3600 * 6 };

            Assert.IsTrue(_interval.GetEnd().Kind == DateTimeKind.Local);
            Assert.IsTrue(_interval.GetEnd().Hour == 7);
            Assert.IsTrue(_interval.GetEnd().Minute == 0);
            Assert.IsTrue(_interval.GetEnd().IsDaylightSavingTime());

            _interval = new Interval() { Start = new DateTime(2017, 10, 29, 0, 0, 0, DateTimeKind.Unspecified), Duration = 3600 * 6 };

            Assert.IsTrue(_interval.GetEnd().Kind == DateTimeKind.Local);
            Assert.IsTrue(_interval.GetEnd().Hour == 5);
            Assert.IsTrue(_interval.GetEnd().Minute == 0);
            Assert.IsFalse(_interval.GetEnd().IsDaylightSavingTime());


            _interval = new Interval() { Start = new DateTime(2017, 3, 26, 0, 0, 0, DateTimeKind.Utc), Duration = 3600 * 6 };

            Assert.IsTrue(_interval.GetEnd().Kind == DateTimeKind.Utc);
            Assert.IsTrue(_interval.GetEnd().Hour == 6);
            Assert.IsTrue(_interval.GetEnd().Minute == 0);
            Assert.IsFalse(_interval.GetEnd().IsDaylightSavingTime()); //always FALSE for UTC Time

            _interval = new Interval() { Start = new DateTime(2017, 10, 29, 0, 0, 0, DateTimeKind.Utc), Duration = 3600 * 6 };

            Assert.IsTrue(_interval.GetEnd().Kind == DateTimeKind.Utc);
            Assert.IsTrue(_interval.GetEnd().Hour == 6);
            Assert.IsTrue(_interval.GetEnd().Minute == 0);
            Assert.IsFalse(_interval.GetEnd().IsDaylightSavingTime()); //always FALSE for UTC Time
        }

        [TestMethod]
        public void TestAddUtcSeconds()
        {
            DateTime t = new DateTime(2017, 3, 26, 0, 0, 0, DateTimeKind.Local);

            double offset = (double)(6 * 3600);

            Assert.IsTrue(t.AddUtcSeconds(offset).Kind == DateTimeKind.Local);
            Assert.IsTrue(t.AddUtcSeconds(offset).Hour == 7);
            Assert.IsTrue(t.AddUtcSeconds(offset).Minute == 0);
            Assert.IsTrue(t.AddUtcSeconds(offset).IsDaylightSavingTime());

            t = new DateTime(2017, 10, 29, 0, 0, 0, DateTimeKind.Unspecified);

            Assert.IsTrue(t.AddUtcSeconds(offset).Kind == DateTimeKind.Local);
            Assert.IsTrue(t.AddUtcSeconds(offset).Hour == 5);
            Assert.IsTrue(t.AddUtcSeconds(offset).Minute == 0);
            Assert.IsFalse(t.AddUtcSeconds(offset).IsDaylightSavingTime());


            t = new DateTime(2017, 3, 26, 0, 0, 0, DateTimeKind.Utc);

            Assert.IsTrue(t.AddUtcSeconds(offset).Kind == DateTimeKind.Utc);
            Assert.IsTrue(t.AddUtcSeconds(offset).Hour == 6);
            Assert.IsTrue(t.AddUtcSeconds(offset).Minute == 0);
            Assert.IsFalse(t.AddUtcSeconds(offset).IsDaylightSavingTime()); //always FALSE for UTC Time

            t = new DateTime(2017, 10, 29, 0, 0, 0, DateTimeKind.Utc);

            Assert.IsTrue(t.AddUtcSeconds(offset).Kind == DateTimeKind.Utc);
            Assert.IsTrue(t.AddUtcSeconds(offset).Hour == 6);
            Assert.IsTrue(t.AddUtcSeconds(offset).Minute == 0);
            Assert.IsFalse(t.AddUtcSeconds(offset).IsDaylightSavingTime()); //always FALSE for UTC Time

        }

        [TestMethod]
        public void TestFilterIntervalReadings()
        {
            var readings = new List<IntervalReading>();
            readings.Add(new IntervalReading { TimePeriod = new Interval { Start = DateTime.Parse("2017-10-28T01:00:01+02:00") } });
            readings.Add(new IntervalReading { TimePeriod = new Interval { Start = DateTime.Parse("2017-10-28T02:00:10+02:00") } });
            readings.Add(new IntervalReading { TimePeriod = new Interval { Start = DateTime.Parse("2017-10-28T03:10:01+02:00") } });
            readings.Add(new IntervalReading { TimePeriod = new Interval { Start = DateTime.Parse("2017-10-28T04:00:01+02:00") } });

            readings.FilterIntervalReadings(3600);
            Assert.AreEqual("2017-10-28T01:00:01+02:00", readings[0].TimePeriod.Start.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-10-28T02:00:10+02:00", readings[1].TimePeriod.Start.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-10-28T04:00:01+02:00", readings[2].TimePeriod.Start.ToString("yyyy-MM-ddTHH:mm:ssK"));
        }
    }
}
