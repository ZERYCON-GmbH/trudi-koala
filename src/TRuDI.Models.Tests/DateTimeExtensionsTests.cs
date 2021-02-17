namespace TRuDI.Models.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DateTimeExtensionsTests
    {
        [TestMethod]
        public void TestToFormatedString()
        {
            var target = new DateTime(2017, 1, 2, 3, 4, 5, DateTimeKind.Local);
            Assert.AreEqual("02.01.2017 03:04", target.ToFormatedString());

            DateTime? target2 = new DateTime(2017, 1, 2, 3, 4, 5, DateTimeKind.Local);
            Assert.AreEqual("02.01.2017 03:04", target2.ToFormatedString());

            target2 = null;
            Assert.AreEqual(string.Empty, target2.ToFormatedString());
        }

        [TestMethod]
        public void TestGetEndTimeOrNow()
        {
            DateTime? target = new DateTime(2017, 1, 2, 3, 4, 5, DateTimeKind.Local);
            Assert.AreEqual("2017-01-02T03:04:05+01:00", target.GetEndTimeOrNow().ToIso8601Local());

            target = null;
            var before = DateTime.Now;
            var result = target.GetEndTimeOrNow();
            var after = DateTime.Now;

            Assert.IsTrue(result >= before && result <= after);

            target = new DateTime(2027, 1, 2, 3, 4, 5, DateTimeKind.Local);
            before = DateTime.Now;
            result = target.GetEndTimeOrNow();
            after = DateTime.Now;

            Assert.IsTrue(result >= before && result <= after);
        }

        [TestMethod]
        public void TestRoundDown()
        {
            DateTime target = new DateTime(2017, 1, 2, 3, 4, 5, DateTimeKind.Local);
            Assert.AreEqual("2017-01-02T03:00:00+01:00", target.RoundDown(15).ToIso8601Local());
        }

        [TestMethod]
        public void TestDayStart()
        {
            DateTime target = new DateTime(2017, 1, 2, 3, 4, 5, DateTimeKind.Local);
            Assert.AreEqual("2017-01-02T00:00:00+01:00", target.DayStart().ToIso8601Local());
        }
        
        [TestMethod]
        public void TestDayEnd()
        {
            DateTime target = new DateTime(2017, 1, 2, 3, 4, 5, DateTimeKind.Local);
            Assert.AreEqual("2017-01-02T23:59:59+01:00", target.DayEnd().ToIso8601Local());
        }

        [TestMethod]
        public void TestNextDayStart()
        {
            DateTime target = new DateTime(2017, 1, 2, 3, 4, 5, DateTimeKind.Local);
            Assert.AreEqual("2017-01-03T00:00:00+01:00", target.NextDayStart().ToIso8601Local());
        }

        [TestMethod]
        public void TestToIso8601()
        {
            DateTime target = new DateTime(2017, 1, 2, 3, 4, 5, DateTimeKind.Local);
            Assert.AreEqual("2017-01-02T02:04:05Z", target.ToIso8601());

            DateTime? target2 = new DateTime(2017, 1, 2, 3, 4, 5, DateTimeKind.Local);
            Assert.AreEqual("2017-01-02T02:04:05Z", target2.ToIso8601());

            target2 = null;
            Assert.AreEqual(string.Empty, target2.ToIso8601());
        }

        [TestMethod]
        public void TestAddUtcSeconds()
        {
            DateTime target = new DateTime(2017, 1, 2, 3, 4, 5, DateTimeKind.Local);
            Assert.AreEqual("2017-01-02T03:04:20+01:00", target.AddUtcSeconds(15).ToIso8601Local());

            target = new DateTime(2017, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            Assert.AreEqual("2017-01-02T04:04:20+01:00", target.AddUtcSeconds(15).ToLocalTime().ToIso8601Local());
        }

        [TestMethod]
        public void TestGetDateTimePickerEndDate()
        {
            DateTime target = new DateTime(2017, 1, 2, 3, 4, 5, DateTimeKind.Local);
            Assert.AreEqual("2017-01-02T03:04:05+01:00", target.GetDateTimePickerEndDate().ToIso8601Local());

            target = new DateTime(2017, 1, 2, 0, 0, 0, DateTimeKind.Local);
            Assert.AreEqual("2017-01-01T23:59:59+01:00", target.GetDateTimePickerEndDate().ToIso8601Local());
        }

        [TestMethod]
        public void TestGetPrevMeasurementPeriod()
        {
            var target = new DateTime(2018, 3, 12, 14, 24, 39, DateTimeKind.Local);
            Assert.AreEqual("2018-03-12T14:24:39+01:00", target.GetPrevMeasurementPeriod(TimeSpan.FromMinutes(0)).ToIso8601Local());
            Assert.AreEqual("2018-03-12T14:15:00+01:00", target.GetPrevMeasurementPeriod(TimeSpan.FromMinutes(15)).ToIso8601Local());
            Assert.AreEqual("2018-03-12T14:00:00+01:00", target.GetPrevMeasurementPeriod(TimeSpan.FromMinutes(60)).ToIso8601Local());
            Assert.AreEqual("2018-03-12T14:20:00+01:00", target.GetPrevMeasurementPeriod(TimeSpan.FromMinutes(5)).ToIso8601Local());
            Assert.AreEqual("2018-03-12T00:00:00+01:00", target.GetPrevMeasurementPeriod(TimeSpan.FromDays(1)).ToIso8601Local());
        }
    }
}
