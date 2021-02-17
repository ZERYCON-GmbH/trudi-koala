namespace TRuDI.Models.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TRuDI.Models;
    using TRuDI.Models.BasicData;

    [TestClass]
    public class MeterReadingExtensionsTests
    {
        [TestMethod]
        [DeploymentItem(@"Data\IF_Adapter_TRuDI_DatenTAF2.xml")]
        public void TestIsOriginalValueList()
        {
            var xml = XDocument.Load(@"Data\IF_Adapter_TRuDI_DatenTAF2.xml");
            var model = XmlModelParser.ParseHanAdapterModel(xml.Root.Descendants());

            Assert.IsTrue(model.MeterReadings[0].IsOriginalValueList());
            Assert.IsFalse(model.MeterReadings[1].IsOriginalValueList());
        }

        [TestMethod]
        [DeploymentItem(@"Data\IF_Adapter_TRuDI_DatenTAF2.xml")]
        public void TestGetMeasurementPeriodTaf2()
        {
            var xml = XDocument.Load(@"Data\IF_Adapter_TRuDI_DatenTAF2.xml");
            var model = XmlModelParser.ParseHanAdapterModel(xml.Root.Descendants());

            Assert.AreEqual(TimeSpan.Zero, model.MeterReadings[0].GetMeasurementPeriod());
        }

        [TestMethod]
        public void TestGetMeasurementPeriod()
        {
            var readings = new List<IntervalReading>();
            readings.Add(new IntervalReading { TimePeriod = new Interval { Start = DateTime.Parse("2017-10-29T01:00:01+02:00") } });
            readings.Add(new IntervalReading { TimePeriod = new Interval { Start = DateTime.Parse("2017-10-29T02:00:01+02:00") } });
            readings.Add(new IntervalReading { TimePeriod = new Interval { Start = DateTime.Parse("2017-10-29T02:00:00+01:00") } });
            readings.Add(new IntervalReading { TimePeriod = new Interval { Start = DateTime.Parse("2017-10-29T03:00:01+01:00") } });
            Assert.AreEqual(60, (int)readings.GetMeasurementPeriod().TotalMinutes);

            readings = new List<IntervalReading>();
            readings.Add(new IntervalReading { TimePeriod = new Interval { Start = DateTime.Parse("2017-08-31T23:57:01+02:00") } });
            readings.Add(new IntervalReading { TimePeriod = new Interval { Start = DateTime.Parse("2017-09-02T00:00:01+02:00") } });
            readings.Add(new IntervalReading { TimePeriod = new Interval { Start = DateTime.Parse("2017-09-03T00:07:00+01:00") } });
            readings.Add(new IntervalReading { TimePeriod = new Interval { Start = DateTime.Parse("2017-09-03T23:53:01+01:00") } });
            readings.Add(new IntervalReading { TimePeriod = new Interval { Start = DateTime.Parse("2017-09-06T03:07:01+01:00") } });
            readings.Add(new IntervalReading { TimePeriod = new Interval { Start = DateTime.Parse("2017-09-08T00:01:01+01:00") } });
            Assert.AreEqual(1440, (int)readings.GetMeasurementPeriod().TotalMinutes);

            readings = new List<IntervalReading>();
            readings.Add(new IntervalReading { TimePeriod = new Interval { Start = DateTime.Parse("2017-10-28T14:00:01+02:00") }, StatusPTB = StatusPTB.CriticalTemporaryError });
            readings.Add(new IntervalReading { TimePeriod = new Interval { Start = DateTime.Parse("2017-10-28T16:00:01+02:00") }, StatusPTB = StatusPTB.CriticalTemporaryError });
            readings.Add(new IntervalReading { TimePeriod = new Interval { Start = DateTime.Parse("2017-10-28T18:00:01+02:00") }, StatusPTB = StatusPTB.CriticalTemporaryError });
            readings.Add(new IntervalReading { TimePeriod = new Interval { Start = DateTime.Parse("2017-10-28T20:00:01+02:00") }, StatusPTB = StatusPTB.CriticalTemporaryError });
            readings.Add(new IntervalReading { TimePeriod = new Interval { Start = DateTime.Parse("2017-10-28T22:00:01+02:00") }, StatusPTB = StatusPTB.CriticalTemporaryError });
            readings.Add(new IntervalReading { TimePeriod = new Interval { Start = DateTime.Parse("2017-10-29T00:00:01+02:00") }, StatusPTB = StatusPTB.CriticalTemporaryError });
            readings.Add(new IntervalReading { TimePeriod = new Interval { Start = DateTime.Parse("2017-10-29T01:00:01+02:00") } });
            readings.Add(new IntervalReading { TimePeriod = new Interval { Start = DateTime.Parse("2017-10-29T02:00:01+02:00") } });
            readings.Add(new IntervalReading { TimePeriod = new Interval { Start = DateTime.Parse("2017-10-29T02:00:00+01:00") } });
            readings.Add(new IntervalReading { TimePeriod = new Interval { Start = DateTime.Parse("2017-10-29T03:00:01+01:00") } });
            Assert.AreEqual(60, (int)readings.GetMeasurementPeriod().TotalMinutes);
        }

        [TestMethod]
        public void TestGetMatchingMeasurementPeriod()
        {
            Assert.AreEqual(60, MeterReadingExtensions.GetMatchingMeasurementPeriod(60));
            Assert.AreEqual(120, MeterReadingExtensions.GetMatchingMeasurementPeriod(120));
            Assert.AreEqual(120, MeterReadingExtensions.GetMatchingMeasurementPeriod(121));
            Assert.AreEqual(180, MeterReadingExtensions.GetMatchingMeasurementPeriod(180));
            Assert.AreEqual(180, MeterReadingExtensions.GetMatchingMeasurementPeriod(181));
            Assert.AreEqual(180, MeterReadingExtensions.GetMatchingMeasurementPeriod(179));
            Assert.AreEqual(240, MeterReadingExtensions.GetMatchingMeasurementPeriod(240));
            Assert.AreEqual(300, MeterReadingExtensions.GetMatchingMeasurementPeriod(300));
            Assert.AreEqual(300, MeterReadingExtensions.GetMatchingMeasurementPeriod(301));
            Assert.AreEqual(300, MeterReadingExtensions.GetMatchingMeasurementPeriod(299));
            Assert.AreEqual(600, MeterReadingExtensions.GetMatchingMeasurementPeriod(600));
            Assert.AreEqual(600, MeterReadingExtensions.GetMatchingMeasurementPeriod(602));
            Assert.AreEqual(600, MeterReadingExtensions.GetMatchingMeasurementPeriod(598));

            Assert.AreEqual(900, MeterReadingExtensions.GetMatchingMeasurementPeriod(900));
            Assert.AreEqual(900, MeterReadingExtensions.GetMatchingMeasurementPeriod(904));
            Assert.AreEqual(900, MeterReadingExtensions.GetMatchingMeasurementPeriod(899));

            Assert.AreEqual(1800, MeterReadingExtensions.GetMatchingMeasurementPeriod(1800));
            Assert.AreEqual(1800, MeterReadingExtensions.GetMatchingMeasurementPeriod(1810));
            Assert.AreEqual(1800, MeterReadingExtensions.GetMatchingMeasurementPeriod(1790));

            Assert.AreEqual(86400, MeterReadingExtensions.GetMatchingMeasurementPeriod(86400));
            Assert.AreEqual(86400, MeterReadingExtensions.GetMatchingMeasurementPeriod(86408));
            Assert.AreEqual(86400, MeterReadingExtensions.GetMatchingMeasurementPeriod(86488));
            Assert.AreEqual(86400, MeterReadingExtensions.GetMatchingMeasurementPeriod(86388));

            Assert.AreEqual(2592000, MeterReadingExtensions.GetMatchingMeasurementPeriod(2592000));
            Assert.AreEqual(2592000, MeterReadingExtensions.GetMatchingMeasurementPeriod(2592400));
            Assert.AreEqual(2592000, MeterReadingExtensions.GetMatchingMeasurementPeriod(2591000));

            Assert.AreEqual(0, MeterReadingExtensions.GetMatchingMeasurementPeriod(8591000));
            Assert.AreEqual(0, MeterReadingExtensions.GetMatchingMeasurementPeriod(200));
        }

        [TestMethod]
        [DeploymentItem(@"Data\IF_Adapter_TRuDI_DatenTAF7.xml")]
        public void TestGetMeasurementPeriodTaf7()
        {
            var xml = XDocument.Load(@"Data\IF_Adapter_TRuDI_DatenTAF7.xml");
            var model = XmlModelParser.ParseHanAdapterModel(xml.Root.Descendants());

            Assert.AreEqual(TimeSpan.FromMinutes(15), model.MeterReadings[0].GetMeasurementPeriod());
        }

        [TestMethod]
        [DeploymentItem(@"Data\IF_Adapter_TRuDI_DatenTAF7.xml")]
        public void TestGetGapCountTaf7()
        {
            var xml = XDocument.Load(@"Data\IF_Adapter_TRuDI_DatenTAF7.xml");
            var model = XmlModelParser.ParseHanAdapterModel(xml.Root.Descendants());

            Assert.AreEqual(0, model.MeterReadings[0].IntervalBlocks.FirstOrDefault().GetGapCount(TimeSpan.FromMinutes(15)));
        }

        [TestMethod]
        [DeploymentItem(@"Data\IF_Adapter_TRuDI_DatenTAF7_With_Gaps.xml")]
        public void TestGetGapCountTaf7WithGaps()
        {
            var xml = XDocument.Load(@"Data\IF_Adapter_TRuDI_DatenTAF7_With_Gaps.xml");
            var model = XmlModelParser.ParseHanAdapterModel(xml.Root.Descendants());

            Assert.AreEqual(3, model.MeterReadings[0].IntervalBlocks.FirstOrDefault().GetGapCount(TimeSpan.FromMinutes(15)));
        }
    }
}
