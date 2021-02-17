namespace TRuDI.Models.Tests
{
    using System;
    using System.Linq;
    using System.Xml.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TRuDI.Models;
    using TRuDI.Models.BasicData;

    [TestClass]
    public class OriginalValueListTests
    {
        [TestMethod]
        [DeploymentItem(@"Data\IF_Adapter_TRuDI_DatenTAF2.xml")]
        public void TestIsOriginalValueListTaf2()
        {
            var xml = XDocument.Load(@"Data\IF_Adapter_TRuDI_DatenTAF2.xml");
            var model = XmlModelParser.ParseHanAdapterModel(xml.Root.Descendants());

            var target = new OriginalValueList(model.MeterReadings[0], Kind.Electricity);
        }

        [TestMethod]
        [DeploymentItem(@"Data\IF_Adapter_TRuDI_DatenTAF7.xml")]
        public void TestIsOriginalValueListTaf7()
        {
            var xml = XDocument.Load(@"Data\IF_Adapter_TRuDI_DatenTAF7.xml");
            var model = XmlModelParser.ParseHanAdapterModel(xml.Root.Descendants());

            var target = new OriginalValueList(model.MeterReadings[0], Kind.Electricity);

            Assert.AreEqual("1-0:1.8.0*255", target.Obis.ToString());
            Assert.AreEqual(0, target.GapCount);

            Assert.AreEqual(DateTime.Parse("2017-06-26T11:30:00+02:00"), target.Start);
            Assert.AreEqual(DateTime.Parse("2017-06-26T12:00:00+02:00"), target.End);

            var items = target.GetReadings(DateTime.MinValue, DateTime.MaxValue).ToList();
            Assert.AreEqual(3, items.Count);

            Assert.AreEqual(DateTime.Parse("2017-06-26T11:30:00+02:00"), items[0].TimePeriod.Start);
            Assert.AreEqual(DateTime.Parse("2017-06-26T11:45:00+02:00"), items[1].TimePeriod.Start);
            Assert.AreEqual(DateTime.Parse("2017-06-26T12:00:00+02:00"), items[2].TimePeriod.Start);
        }

        [TestMethod]
        [DeploymentItem(@"Data\oml_error_dup_timestamps.xml")]
        public void TestOriginalValueListDupTimestamps()
        {
            var xml = XDocument.Load(@"Data\oml_error_dup_timestamps.xml");
            var model = XmlModelParser.ParseHanAdapterModel(xml.Root.Descendants());

            var target = new OriginalValueList(model.MeterReadings[0], Kind.Electricity);

            Assert.AreEqual("1-0:1.8.0*255", target.Obis.ToString());
            Assert.AreEqual(0, target.GapCount);

            Assert.AreEqual(DateTime.Parse("2018-03-12T00:00:00+01:00"), target.Start);
            Assert.AreEqual(DateTime.Parse("2018-03-12T02:00:00+01:00"), target.End);

            var items = target.GetReadings(DateTime.MinValue, DateTime.MaxValue).ToList();
            Assert.AreEqual(9, items.Count);
        }

        [TestMethod]
        [DeploymentItem(@"Data\result_oml_gas_0_period.xml")]
        public void TestIsOriginalValueListGasZeroMeasurementPeriod()
        {
            var xml = XDocument.Load(@"Data\result_oml_gas_0_period.xml");
            var model = XmlModelParser.ParseHanAdapterModel(xml.Root.Descendants());

            var target = new OriginalValueList(model.MeterReadings[0], Kind.Gas);

            Assert.AreEqual("7-0:3.1.0*255", target.Obis.ToString());
            Assert.AreEqual(3, target.GapCount);

            Assert.AreEqual(DateTime.Parse("2017-11-29T17:09:00+01:00"), target.Start);
            Assert.AreEqual(DateTime.Parse("2017-11-30T06:05:44+01:00"), target.End);

            var items = target.GetReadings(DateTime.MinValue, DateTime.MaxValue).ToList();
            Assert.AreEqual(4, items.Count);

            Assert.AreEqual(DateTime.Parse("2017-11-29T17:09:00+01:00"), items[0].TimePeriod.Start);
            Assert.AreEqual(DateTime.Parse("2017-11-30T00:04:46+01:00"), items[1].TimePeriod.Start);
            Assert.AreEqual(DateTime.Parse("2017-11-30T05:20:22+01:00"), items[2].TimePeriod.Start);
            Assert.AreEqual(DateTime.Parse("2017-11-30T06:05:44+01:00"), items[3].TimePeriod.Start);
        }

        [TestMethod]
        [DeploymentItem(@"Data\result_oml_start_not_aligned.xml")]
        public void TestIsOriginalValueListStartNotAligned()
        {
            var xml = XDocument.Load(@"Data\result_oml_start_not_aligned.xml");
            var model = XmlModelParser.ParseHanAdapterModel(xml.Root.Descendants());

            var target = new OriginalValueList(model.MeterReadings[0], Kind.Electricity);

            Assert.AreEqual("1-0:1.8.0*255", target.Obis.ToString());
            //Assert.AreEqual(3, target.GapCount);

            Assert.AreEqual(DateTime.Parse("2018-03-12T14:24:39+01:00"), target.Start);
            Assert.AreEqual(DateTime.Parse("2018-03-13T10:15:00+01:00"), target.End);

            var items = target.GetReadings(DateTime.MinValue, DateTime.MaxValue).ToList();
            Assert.AreEqual(83, items.Count);

            //Assert.AreEqual(DateTime.Parse("2017-11-29T17:09:00+01:00"), items[0].TimePeriod.Start);
        }

        [TestMethod]
        [DeploymentItem(@"Data\IF_Adapter_TRuDI_DatenTAF7_With_Gaps.xml")]
        public void TestIsOriginalValueListTaf7WithGaps()
        {
            var xml = XDocument.Load(@"Data\IF_Adapter_TRuDI_DatenTAF7_With_Gaps.xml");
            var model = XmlModelParser.ParseHanAdapterModel(xml.Root.Descendants());

            var target = new OriginalValueList(model.MeterReadings[0], Kind.Electricity);

            Assert.AreEqual("1-0:1.8.0*255", target.Obis.ToString());
            Assert.AreEqual(3, target.GapCount);

            Assert.AreEqual(DateTime.Parse("2017-06-26T11:30:00+02:00"), target.Start);
            Assert.AreEqual(DateTime.Parse("2017-06-26T15:15:00+02:00"), target.End);

            // Get only 2 items
            var items = target.GetReadings(DateTime.Parse("2017-06-26T11:45:00+02:00"), DateTime.Parse("2017-06-26T12:00:00+02:00")).ToList();
            Assert.AreEqual(2, items.Count);

            Assert.AreEqual(DateTime.Parse("2017-06-26T11:45:00+02:00"), items[0].TimePeriod.Start);
            Assert.IsNotNull(items[0].Value);

            Assert.AreEqual(DateTime.Parse("2017-06-26T12:00:00+02:00"), items[1].TimePeriod.Start);
            Assert.IsNotNull(items[1].Value);


            // Get all items
            items = target.GetReadings(DateTime.MinValue, DateTime.MaxValue).ToList();
            Assert.AreEqual(16, items.Count);

            Assert.AreEqual(DateTime.Parse("2017-06-26T11:30:00+02:00"), items[0].TimePeriod.Start);
            Assert.IsNotNull(items[0].Value);

            Assert.AreEqual(DateTime.Parse("2017-06-26T11:45:00+02:00"), items[1].TimePeriod.Start);
            Assert.IsNotNull(items[1].Value);

            Assert.AreEqual(DateTime.Parse("2017-06-26T12:00:00+02:00"), items[2].TimePeriod.Start);
            Assert.IsNotNull(items[2].Value);

            Assert.AreEqual(DateTime.Parse("2017-06-26T12:15:00+02:00"), items[3].TimePeriod.Start);
            Assert.IsNull(items[3].Value);

            Assert.AreEqual(DateTime.Parse("2017-06-26T12:30:00+02:00"), items[4].TimePeriod.Start);
            Assert.IsNotNull(items[4].Value);

            Assert.AreEqual(DateTime.Parse("2017-06-26T12:45:00+02:00"), items[5].TimePeriod.Start);
            Assert.IsNotNull(items[5].Value);

            Assert.AreEqual(DateTime.Parse("2017-06-26T13:00:00+02:00"), items[6].TimePeriod.Start);
            Assert.IsNotNull(items[6].Value);

            Assert.AreEqual(DateTime.Parse("2017-06-26T13:15:00+02:00"), items[7].TimePeriod.Start);
            Assert.IsNull(items[7].Value);

            Assert.AreEqual(DateTime.Parse("2017-06-26T13:30:00+02:00"), items[8].TimePeriod.Start);
            Assert.IsNull(items[8].Value);

            Assert.AreEqual(DateTime.Parse("2017-06-26T13:45:00+02:00"), items[9].TimePeriod.Start);
            Assert.IsNotNull(items[9].Value);

            Assert.AreEqual(DateTime.Parse("2017-06-26T14:00:00+02:00"), items[10].TimePeriod.Start);
            Assert.IsNull(items[10].Value);

            Assert.AreEqual(DateTime.Parse("2017-06-26T14:15:00+02:00"), items[11].TimePeriod.Start);
            Assert.IsNull(items[11].Value);

            Assert.AreEqual(DateTime.Parse("2017-06-26T14:30:00+02:00"), items[12].TimePeriod.Start);
            Assert.IsNull(items[12].Value);

            Assert.AreEqual(DateTime.Parse("2017-06-26T14:45:00+02:00"), items[13].TimePeriod.Start);
            Assert.IsNull(items[13].Value);

            Assert.AreEqual(DateTime.Parse("2017-06-26T15:00:00+02:00"), items[14].TimePeriod.Start);
            Assert.IsNotNull(items[14].Value);

            Assert.AreEqual(DateTime.Parse("2017-06-26T15:15:00+02:00"), items[15].TimePeriod.Start);
            Assert.IsNotNull(items[15].Value);

            // Get only gap items
            items = target.GetReadings(DateTime.Parse("2017-06-26T14:15:00+02:00"), DateTime.Parse("2017-06-26T14:30:00+02:00")).ToList();
            Assert.AreEqual(2, items.Count);

            Assert.AreEqual(DateTime.Parse("2017-06-26T14:15:00+02:00"), items[0].TimePeriod.Start);
            Assert.IsNull(items[0].Value);

            Assert.AreEqual(DateTime.Parse("2017-06-26T14:30:00+02:00"), items[1].TimePeriod.Start);
            Assert.IsNull(items[1].Value);

            // Get items with gap at beginn
            items = target.GetReadings(DateTime.Parse("2017-06-26T14:45:00+02:00"), DateTime.Parse("2017-06-26T15:15:00+02:00")).ToList();
            Assert.AreEqual(3, items.Count);

            Assert.AreEqual(DateTime.Parse("2017-06-26T14:45:00+02:00"), items[0].TimePeriod.Start);
            Assert.IsNull(items[0].Value);

            Assert.AreEqual(DateTime.Parse("2017-06-26T15:00:00+02:00"), items[1].TimePeriod.Start);
            Assert.IsNotNull(items[1].Value);

            Assert.AreEqual(DateTime.Parse("2017-06-26T15:15:00+02:00"), items[2].TimePeriod.Start);
            Assert.IsNotNull(items[2].Value);

            // Get items with gap at end
            items = target.GetReadings(DateTime.Parse("2017-06-26T13:45:00+02:00"), DateTime.Parse("2017-06-26T14:15:00+02:00")).ToList();
            Assert.AreEqual(3, items.Count);

            Assert.AreEqual(DateTime.Parse("2017-06-26T13:45:00+02:00"), items[0].TimePeriod.Start);
            Assert.IsNotNull(items[0].Value);

            Assert.AreEqual(DateTime.Parse("2017-06-26T14:00:00+02:00"), items[1].TimePeriod.Start);
            Assert.IsNull(items[1].Value);

            Assert.AreEqual(DateTime.Parse("2017-06-26T14:15:00+02:00"), items[2].TimePeriod.Start);
            Assert.IsNull(items[2].Value);
        }

        [TestMethod]
        [DeploymentItem(@"Data\result_3_days_geterrorlist.xml")]
        public void TestGetErrors()
        {
            var xml = XDocument.Load(@"Data\result_3_days_geterrorlist.xml");
            var model = XmlModelParser.ParseHanAdapterModel(xml.Root.Descendants());

            var target = new OriginalValueList(model.MeterReadings[0], Kind.Electricity);

            var errors = target.GetErrorsList();

            Assert.AreEqual(4, errors.Count);

            Assert.AreEqual(96, errors[0].ValueCount);
            Assert.AreEqual(0, errors[0].GapCount);
            Assert.IsFalse(errors[0].HasErrors);
            Assert.AreEqual(0, errors[0].FatalErrorCount);
            Assert.AreEqual(0, errors[0].WarningCount);
            Assert.AreEqual(0, errors[0].TempErrorCount);
            Assert.AreEqual(0, errors[0].CriticalTempErrorCount);
            Assert.AreEqual(96, errors[0].OkCount);
            Assert.AreEqual("2017-11-04T00:00:00+01:00", errors[0].Timestamp.ToIso8601Local());

            Assert.AreEqual(96, errors[1].ValueCount);
            Assert.AreEqual(1, errors[1].GapCount);
            Assert.IsTrue(errors[1].HasErrors);
            Assert.AreEqual(3, errors[1].FatalErrorCount);
            Assert.AreEqual(1, errors[1].WarningCount);
            Assert.AreEqual(2, errors[1].TempErrorCount);
            Assert.AreEqual(3, errors[1].CriticalTempErrorCount);
            Assert.AreEqual(86, errors[1].OkCount);
            Assert.AreEqual("2017-11-05T00:00:00+01:00", errors[1].Timestamp.ToIso8601Local());

            Assert.AreEqual(96, errors[2].ValueCount);
            Assert.AreEqual(0, errors[2].GapCount);
            Assert.IsTrue(errors[2].HasErrors);
            Assert.AreEqual(1, errors[2].FatalErrorCount);
            Assert.AreEqual(0, errors[2].WarningCount);
            Assert.AreEqual(0, errors[2].TempErrorCount);
            Assert.AreEqual(0, errors[2].CriticalTempErrorCount);
            Assert.AreEqual(95, errors[2].OkCount);
            Assert.AreEqual("2017-11-06T00:00:00+01:00", errors[2].Timestamp.ToIso8601Local());

            Assert.AreEqual(1, errors[3].ValueCount);
            Assert.AreEqual(0, errors[3].GapCount);
            Assert.IsFalse(errors[0].HasErrors);
            Assert.AreEqual(0, errors[3].FatalErrorCount);
            Assert.AreEqual(0, errors[3].WarningCount);
            Assert.AreEqual(0, errors[3].TempErrorCount);
            Assert.AreEqual(0, errors[3].CriticalTempErrorCount);
            Assert.AreEqual(1, errors[3].OkCount);
            Assert.AreEqual("2017-11-07T00:00:00+01:00", errors[3].Timestamp.ToIso8601Local());
        }

        [TestMethod]
        [DeploymentItem(@"Data\result_historic_consumption.xml")]
        public void TestHistoricConsumption()
        {
            var xml = XDocument.Load(@"Data\result_historic_consumption.xml");
            var model = XmlModelParser.ParseHanAdapterModel(xml.Root.Descendants());

            var target = new OriginalValueList(model.MeterReadings[0], Kind.Electricity);

            var values = target.HistoricValues;

            Assert.AreEqual(50, values.Count);
            CheckValue(values[0], "2019-12-31T00:00:00+01:00", "2020-01-01T00:00:00+01:00", 960, TimeUnit.Day);
            CheckValue(values[1], "2019-12-30T00:00:00+01:00", "2019-12-31T00:00:00+01:00", 960, TimeUnit.Day);
            CheckValue(values[2], "2019-12-29T00:00:00+01:00", "2019-12-30T00:00:00+01:00", 960, TimeUnit.Day);
            CheckValue(values[3], "2019-12-28T00:00:00+01:00", "2019-12-29T00:00:00+01:00", 960, TimeUnit.Day);
            CheckValue(values[4], "2019-12-27T00:00:00+01:00", "2019-12-28T00:00:00+01:00", 960, TimeUnit.Day);
            CheckValue(values[5], "2019-12-26T00:00:00+01:00", "2019-12-27T00:00:00+01:00", 960, TimeUnit.Day);
            CheckValue(values[6], "2019-12-25T00:00:00+01:00", "2019-12-26T00:00:00+01:00", 960, TimeUnit.Day);
            CheckValue(values[7], "2019-12-23T00:00:00+01:00", "2019-12-30T00:00:00+01:00", 6720, TimeUnit.Week);
            CheckValue(values[8], "2019-12-16T00:00:00+01:00", "2019-12-23T00:00:00+01:00", 6720, TimeUnit.Week);
            CheckValue(values[9], "2019-12-09T00:00:00+01:00", "2019-12-16T00:00:00+01:00", 6720, TimeUnit.Week);
            CheckValue(values[10], "2019-12-02T00:00:00+01:00", "2019-12-09T00:00:00+01:00", 6720, TimeUnit.Week);
            CheckValue(values[11], "2019-12-01T00:00:00+01:00", "2020-01-01T00:00:00+01:00", 29760, TimeUnit.Month);
            CheckValue(values[12], "2019-11-01T00:00:00+01:00", "2019-12-01T00:00:00+01:00", 28800, TimeUnit.Month);
            CheckValue(values[13], "2019-10-01T00:00:00+02:00", "2019-11-01T00:00:00+01:00", 29800, TimeUnit.Month);
            CheckValue(values[14], "2019-09-01T00:00:00+02:00", "2019-10-01T00:00:00+02:00", 28800, TimeUnit.Month);
            CheckValue(values[15], "2019-08-01T00:00:00+02:00", "2019-09-01T00:00:00+02:00", 29760, TimeUnit.Month);
            CheckValue(values[16], "2019-07-01T00:00:00+02:00", "2019-08-01T00:00:00+02:00", 29760, TimeUnit.Month);
            CheckValue(values[17], "2019-06-01T00:00:00+02:00", "2019-07-01T00:00:00+02:00", 28800, TimeUnit.Month);
            CheckValue(values[18], "2019-05-01T00:00:00+02:00", "2019-06-01T00:00:00+02:00", 29760, TimeUnit.Month);
            CheckValue(values[19], "2019-04-01T00:00:00+02:00", "2019-05-01T00:00:00+02:00", 28800, TimeUnit.Month);
            CheckValue(values[20], "2019-03-01T00:00:00+01:00", "2019-04-01T00:00:00+02:00", 29720, TimeUnit.Month);
            CheckValue(values[21], "2019-02-01T00:00:00+01:00", "2019-03-01T00:00:00+01:00", 26880, TimeUnit.Month);
            CheckValue(values[22], "2019-01-01T00:00:00+01:00", "2019-02-01T00:00:00+01:00", 29760, TimeUnit.Month);
            CheckValue(values[23], "2018-12-01T00:00:00+01:00", "2019-01-01T00:00:00+01:00", 29760, TimeUnit.Month);
            CheckValue(values[24], "2018-11-01T00:00:00+01:00", "2018-12-01T00:00:00+01:00", 28800, TimeUnit.Month);
            CheckValue(values[25], "2018-10-01T00:00:00+02:00", "2018-11-01T00:00:00+01:00", 29800, TimeUnit.Month);
            CheckValue(values[26], "2018-09-01T00:00:00+02:00", "2018-10-01T00:00:00+02:00", 28800, TimeUnit.Month);
            CheckValue(values[27], "2018-08-01T00:00:00+02:00", "2018-09-01T00:00:00+02:00", 29760, TimeUnit.Month);
            CheckValue(values[28], "2018-07-01T00:00:00+02:00", "2018-08-01T00:00:00+02:00", 29760, TimeUnit.Month);
            CheckValue(values[29], "2018-06-01T00:00:00+02:00", "2018-07-01T00:00:00+02:00", 28800, TimeUnit.Month);
            CheckValue(values[30], "2018-05-01T00:00:00+02:00", "2018-06-01T00:00:00+02:00", 29760, TimeUnit.Month);
            CheckValue(values[31], "2018-04-01T00:00:00+02:00", "2018-05-01T00:00:00+02:00", 28800, TimeUnit.Month);
            CheckValue(values[32], "2018-03-01T00:00:00+01:00", "2018-04-01T00:00:00+02:00", 29720, TimeUnit.Month);
            CheckValue(values[33], "2018-02-01T00:00:00+01:00", "2018-03-01T00:00:00+01:00", 26880, TimeUnit.Month);
            CheckValue(values[34], "2018-01-01T00:00:00+01:00", "2018-02-01T00:00:00+01:00", 29760, TimeUnit.Month);
            CheckValue(values[35], "2017-12-01T00:00:00+01:00", "2018-01-01T00:00:00+01:00", 29760, TimeUnit.Month);
            CheckValue(values[36], "2017-11-01T00:00:00+01:00", "2017-12-01T00:00:00+01:00", 28800, TimeUnit.Month);
            CheckValue(values[37], "2017-10-01T00:00:00+02:00", "2017-11-01T00:00:00+01:00", 29800, TimeUnit.Month);
            CheckValue(values[38], "2017-09-01T00:00:00+02:00", "2017-10-01T00:00:00+02:00", 28800, TimeUnit.Month);
            CheckValue(values[39], "2017-08-01T00:00:00+02:00", "2017-09-01T00:00:00+02:00", 29760, TimeUnit.Month);
            CheckValue(values[40], "2017-07-01T00:00:00+02:00", "2017-08-01T00:00:00+02:00", 29760, TimeUnit.Month);
            CheckValue(values[41], "2017-06-01T00:00:00+02:00", "2017-07-01T00:00:00+02:00", 28800, TimeUnit.Month);
            CheckValue(values[42], "2017-05-01T00:00:00+02:00", "2017-06-01T00:00:00+02:00", 29760, TimeUnit.Month);
            CheckValue(values[43], "2017-04-01T00:00:00+02:00", "2017-05-01T00:00:00+02:00", 28800, TimeUnit.Month);
            CheckValue(values[44], "2017-03-01T00:00:00+01:00", "2017-04-01T00:00:00+02:00", 29720, TimeUnit.Month);
            CheckValue(values[45], "2017-02-01T00:00:00+01:00", "2017-03-01T00:00:00+01:00", 26880, TimeUnit.Month);
            CheckValue(values[46], "2017-01-01T00:00:00+01:00", "2017-02-01T00:00:00+01:00", 29760, TimeUnit.Month);
            CheckValue(values[47], "2019-01-01T00:00:00+01:00", "2020-01-01T00:00:00+01:00", 350400, TimeUnit.Year);
            CheckValue(values[48], "2018-01-01T00:00:00+01:00", "2019-01-01T00:00:00+01:00", 350400, TimeUnit.Year);
            CheckValue(values[49], "2017-01-01T00:00:00+01:00", "2018-01-01T00:00:00+01:00", 350400, TimeUnit.Year);

            void CheckValue(HistoricConsumption item, string start, string end, long value, TimeUnit unitOfTime)
            {
                Assert.AreEqual(item.Value, value);
                Assert.AreEqual(item.Begin.ToIso8601Local(), start);
                Assert.AreEqual(item.End.ToIso8601Local(), end);
                Assert.AreEqual(item.UnitOfTime, unitOfTime);
            }
        }

        [TestMethod]
        [DeploymentItem(@"Data\result_oml_winter_to_summer_time.xml")]
        public void TestIsOriginalValueListWinterToSummerTimeGap()
        {
            var xml = XDocument.Load(@"Data\result_oml_winter_to_summer_time.xml");
            var model = XmlModelParser.ParseHanAdapterModel(xml.Root.Descendants());

            var target = new OriginalValueList(model.MeterReadings[0], Kind.Electricity);

            Assert.AreEqual("1-0:1.8.0*255", target.Obis.ToString());
            Assert.AreEqual(0, target.GapCount);

            Assert.AreEqual(DateTime.Parse("2017-03-26T00:00:00+01:00"), target.Start);
            Assert.AreEqual(DateTime.Parse("2017-03-26T06:00:00+02:00"), target.End);

            var items = target.GetReadings(DateTime.MinValue, DateTime.MaxValue).ToList();
            Assert.AreEqual(21, items.Count);

            Assert.AreEqual(DateTime.Parse("2017-03-26T00:00:00+01:00"), items[0].TimePeriod.Start);
            Assert.AreEqual(DateTime.Parse("2017-03-26T00:15:00+01:00"), items[1].TimePeriod.Start);
            Assert.AreEqual(DateTime.Parse("2017-03-26T00:30:00+01:00"), items[2].TimePeriod.Start);
            Assert.AreEqual(DateTime.Parse("2017-03-26T00:45:00+01:00"), items[3].TimePeriod.Start);
            Assert.AreEqual(DateTime.Parse("2017-03-26T01:00:00+01:00"), items[4].TimePeriod.Start);
            Assert.AreEqual(DateTime.Parse("2017-03-26T01:15:00+01:00"), items[5].TimePeriod.Start);
            Assert.AreEqual(DateTime.Parse("2017-03-26T01:30:00+01:00"), items[6].TimePeriod.Start);
            Assert.AreEqual(DateTime.Parse("2017-03-26T01:45:00+01:00"), items[7].TimePeriod.Start);
            Assert.AreEqual(DateTime.Parse("2017-03-26T03:00:00+02:00"), items[8].TimePeriod.Start);
            Assert.AreEqual(DateTime.Parse("2017-03-26T03:15:00+02:00"), items[9].TimePeriod.Start);
            Assert.AreEqual(DateTime.Parse("2017-03-26T03:30:00+02:00"), items[10].TimePeriod.Start);
            Assert.AreEqual(DateTime.Parse("2017-03-26T03:45:00+02:00"), items[11].TimePeriod.Start);
            Assert.AreEqual(DateTime.Parse("2017-03-26T04:00:00+02:00"), items[12].TimePeriod.Start);
            Assert.AreEqual(DateTime.Parse("2017-03-26T04:15:00+02:00"), items[13].TimePeriod.Start);
            Assert.AreEqual(DateTime.Parse("2017-03-26T04:30:00+02:00"), items[14].TimePeriod.Start);
            Assert.AreEqual(DateTime.Parse("2017-03-26T04:45:00+02:00"), items[15].TimePeriod.Start);
            Assert.AreEqual(DateTime.Parse("2017-03-26T05:00:00+02:00"), items[16].TimePeriod.Start);
            Assert.AreEqual(DateTime.Parse("2017-03-26T05:15:00+02:00"), items[17].TimePeriod.Start);
            Assert.AreEqual(DateTime.Parse("2017-03-26T05:30:00+02:00"), items[18].TimePeriod.Start);
            Assert.AreEqual(DateTime.Parse("2017-03-26T05:45:00+02:00"), items[19].TimePeriod.Start);
            Assert.AreEqual(DateTime.Parse("2017-03-26T06:00:00+02:00"), items[20].TimePeriod.Start);
        }
    }
}
