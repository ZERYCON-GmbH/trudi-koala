namespace TRuDI.Models.Tests
{
    using System;
    using System.Xml.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TRuDI.HanAdapter.Interface;
    using TRuDI.Models;
    using TRuDI.Models.BasicData;

    [TestClass]
    public class ParsingAndValidationTests
    {
        [TestMethod]
        [DeploymentItem(@"Data\cc_2_2_first_or_last_read_off_by_more_than_60_seconds.xml")]
        public void CC_2_2_FirstOrLastReadOffByMoreThan60Seconds()
        {
            var xml = XDocument.Load(@"Data\cc_2_2_first_or_last_read_off_by_more_than_60_seconds.xml");

            var ctx = new AdapterContext()
            {
                Start = new DateTime(2017, 7, 1, 0, 0, 0),
                End = new DateTime(2017, 8, 1, 0, 0, 0),
                Contract = new ContractInfo() { TafId = TafId.Taf7 }
            };

            var model = RunValidations(xml, ctx);

            Assert.IsNotNull(model);
            Assert.IsTrue(model.MeterReadings.Count == 1 && model.MeterReadings[0].IntervalBlocks.Count == 1);

            var irs = model.MeterReadings[0].IntervalBlocks[0].IntervalReadings;

            //check the 1st measurement with the skewed timestamp
            CheckIntervalReading(irs[0], "2017-07-10T08:48:22+02:00", "2017-07-10T08:48:22+02:00", 41853);

            //checking some more readings
            CheckIntervalReading(irs[1], "2017-07-10T11:45:00+02:00", "2017-07-10T11:45:00+02:00", 41916);
            CheckIntervalReading(irs[2], "2017-07-10T12:00:00+02:00", "2017-07-10T12:00:00+02:00", 41921);

            CheckIntervalReading(irs[141], "2017-07-15T00:30:00+02:00", "2017-07-15T00:30:03+02:00", 41988);

            CheckIntervalReading(irs[1372], "2017-07-31T23:45:00+02:00", "2017-07-31T23:45:00+02:00", 43843);
            CheckIntervalReading(irs[1373], "2017-08-01T00:00:00+02:00", "2017-08-01T00:00:00+02:00", 43848);
        }

        [TestMethod]
        [DeploymentItem(@"Data\cc_2_3_reading_gaps_at_the_begin_of_ctx_period.xml")]
        public void CC_2_3_ReadingGapsAtTheBeginOfCtxPeriod()
        {
            var xml = XDocument.Load(@"Data\cc_2_3_reading_gaps_at_the_begin_of_ctx_period.xml");

            var ctx = new AdapterContext()
            {
                Start = new DateTime(2017, 9, 8, 0, 0, 0),
                End = new DateTime(2017, 9, 10, 0, 0, 0),
                Contract = new ContractInfo() { TafId = TafId.Taf7 }
            };

            var model = RunValidations(xml, ctx);

            Assert.IsNotNull(model);

            Assert.IsTrue(model.MeterReadings.Count == 2 
                && model.MeterReadings[0].IntervalBlocks.Count == 1
                && model.MeterReadings[1].IntervalBlocks.Count == 1);

            var irs1 = model.MeterReadings[0].IntervalBlocks[0].IntervalReadings;
            var irs2 = model.MeterReadings[1].IntervalBlocks[0].IntervalReadings;

            CheckIntervalReading(irs1[0], "2017-09-08T00:30:00+02:00", "2017-09-08T00:30:00+02:00", 11543236000);
            CheckIntervalReading(irs1[1], "2017-09-08T00:45:00+02:00", "2017-09-08T00:45:00+02:00", 11552028000);
            CheckIntervalReading(irs1[189], "2017-09-09T23:45:00+02:00", "2017-09-09T23:45:00+02:00", 13245223000);
            CheckIntervalReading(irs1[190], "2017-09-10T00:00:00+02:00", "2017-09-10T00:00:00+02:00", 13254024000);

            CheckIntervalReading(irs2[0], "2017-09-08T00:00:00+02:00", "2017-09-08T00:00:00+02:00", 11533777000);
            CheckIntervalReading(irs2[1], "2017-09-08T00:15:00+02:00", "2017-09-08T00:15:00+02:00", 11542788000);
            CheckIntervalReading(irs2[191], "2017-09-09T23:45:00+02:00", "2017-09-09T23:45:00+02:00", 13251396000);
            CheckIntervalReading(irs2[192], "2017-09-10T00:00:00+02:00", "2017-09-10T00:00:00+02:00", 13260361000);
        }

        [TestMethod]
        [DeploymentItem(@"Data\cc_2_4_reading_gaps_at_the_end_of_ctx_period.xml")]
        public void CC_2_4_ReadingGapsAtTheEndOfCtxPeriod()
        {
            var xml = XDocument.Load(@"Data\cc_2_4_reading_gaps_at_the_end_of_ctx_period.xml");

            var ctx = new AdapterContext()
            {
                Start = new DateTime(2017, 9, 10, 0, 0, 0),
                End = new DateTime(2017, 9, 18, 14, 0, 0),
                Contract = new ContractInfo() { TafId = TafId.Taf7 }
            };

            var model = RunValidations(xml, ctx);

            Assert.IsNotNull(model);

            Assert.IsTrue(model.MeterReadings.Count == 2
                && model.MeterReadings[0].IntervalBlocks.Count == 1
                && model.MeterReadings[1].IntervalBlocks.Count == 1);

            var irs1 = model.MeterReadings[0].IntervalBlocks[0].IntervalReadings;
            var irs2 = model.MeterReadings[1].IntervalBlocks[0].IntervalReadings;

            CheckIntervalReading(irs1[0], "2017-09-10T00:00:00+02:00", "2017-09-10T00:00:00+02:00", 13254024000);
            CheckIntervalReading(irs1[1], "2017-09-10T00:15:00+02:00", "2017-09-10T00:15:00+02:00", 13262981000);
            CheckIntervalReading(irs1[357], "2017-09-13T17:15:00+02:00", "2017-09-13T17:15:00+02:00", 16463037000);
            CheckIntervalReading(irs1[358], "2017-09-13T17:30:00+02:00", "2017-09-13T17:30:00+02:00", 16472131000);

            CheckIntervalReading(irs2[0], "2017-09-10T00:00:00+02:00", "2017-09-10T00:00:00+02:00", 13260361000);
            CheckIntervalReading(irs2[1], "2017-09-10T00:15:00+02:00", "2017-09-10T00:15:00+02:00", 13269149000);
            CheckIntervalReading(irs2[357], "2017-09-13T17:15:00+02:00", "2017-09-13T17:15:00+02:00", 16468918000);
            CheckIntervalReading(irs2[358], "2017-09-13T17:30:00+02:00", "2017-09-13T17:30:00+02:00", 16478081000);
        }

        [TestMethod]
        [DeploymentItem(@"Data\cc_2_5_reading_gaps_inside_loaded_period.xml")]
        public void CC_2_5_ReadingGapsInsideLoadedPeriod()
        {
            var xml = XDocument.Load(@"Data\cc_2_5_reading_gaps_inside_loaded_period.xml");

            var ctx = new AdapterContext()
            {
                Start = new DateTime(2017, 9, 5, 0, 0, 0),
                End = new DateTime(2017, 9, 9, 14, 0, 0),
                Contract = new ContractInfo() { TafId = TafId.Taf7 }
            };

            var model = RunValidations(xml, ctx);

            Assert.IsNotNull(model);

            Assert.IsTrue(model.MeterReadings.Count == 2
                && model.MeterReadings[0].IntervalBlocks.Count == 1
                && model.MeterReadings[1].IntervalBlocks.Count == 1);

            var irs1 = model.MeterReadings[0].IntervalBlocks[0].IntervalReadings;
            var irs2 = model.MeterReadings[1].IntervalBlocks[0].IntervalReadings;

            CheckIntervalReading(irs1[0], "2017-09-05T00:00:00+02:00", "2017-09-05T00:00:00+02:00", 8931599000);
            CheckIntervalReading(irs1[1], "2017-09-05T00:15:00+02:00", "2017-09-05T00:15:00+02:00", 8940612000);

            CheckIntervalReading(irs1[95], "2017-09-05T23:45:00+02:00", "2017-09-05T23:45:00+02:00", 9789958000);
            // GAP
            CheckIntervalReading(irs1[96], "2017-09-08T00:00:00+02:00", "2017-09-08T00:00:00+02:00", 11525545000);

            CheckIntervalReading(irs1[191], "2017-09-08T23:45:00+02:00", "2017-09-08T23:45:00+02:00", 12379254000);
            CheckIntervalReading(irs1[192], "2017-09-09T00:00:00+02:00", "2017-09-09T00:00:00+02:00", 12388190000);
        }

        [TestMethod]
        [DeploymentItem(@"Data\cc_3_2_query_over_multiple_bill_periods.xml")]
        public void CC_3_2_QueryOverMultipleBillPeriods()
        {
            var xml = XDocument.Load(@"Data\cc_3_2_query_over_multiple_bill_periods.xml");

            var ctx = new AdapterContext()
            {
                Start = new DateTime(2017, 9, 1, 0, 0, 0),
                End = new DateTime(2017, 9, 8, 14, 0, 0),
                Contract = new ContractInfo() { TafId = TafId.Taf6 }
            };
            ctx.BillingPeriod = new BillingPeriod() { Begin = ctx.Start, End = ctx.End };

            var model = RunValidations(xml, ctx);

            Assert.IsNotNull(model);

            Assert.IsTrue(model.MeterReadings.Count == 4
                && model.MeterReadings[0].IntervalBlocks.Count == 1
                && model.MeterReadings[1].IntervalBlocks.Count == 1);

            var irs1 = model.MeterReadings[0].IntervalBlocks[0].IntervalReadings; //673
            var irs2 = model.MeterReadings[1].IntervalBlocks[0].IntervalReadings; //673

            CheckIntervalReading(irs1[0], "2017-09-01T00:00:00+02:00", "2017-09-01T00:00:00+02:00", 5479090000);
            CheckIntervalReading(irs1[1], "2017-09-01T00:15:00+02:00", "2017-09-01T00:15:00+02:00", 5488200000);
            CheckIntervalReading(irs1[671], "2017-09-07T23:45:00+02:00", "2017-09-07T23:45:00+02:00", 11516612000);
            CheckIntervalReading(irs1[672], "2017-09-08T00:00:00+02:00", "2017-09-08T00:00:00+02:00", 11525545000);

            CheckIntervalReading(irs2[0], "2017-09-01T00:00:00+02:00", "2017-09-01T00:00:00+02:00", 5481997000);
            CheckIntervalReading(irs2[1], "2017-09-01T00:15:00+02:00", "2017-09-01T00:15:00+02:00", 5491012000);
            CheckIntervalReading(irs2[671], "2017-09-07T23:45:00+02:00", "2017-09-07T23:45:00+02:00", 11524575000);
            CheckIntervalReading(irs2[672], "2017-09-08T00:00:00+02:00", "2017-09-08T00:00:00+02:00", 11533777000);
        }

        [TestMethod]
        [DeploymentItem(@"Data\result_DrNeuhausBugNo10.xml")]
        public void TestDrNeuhausError()
        {
            var xml = XDocument.Load(@"Data\result_DrNeuhausBugNo10.xml");

            var ctx = new AdapterContext()
            {
                Start = new DateTime(2017, 10, 29, 2, 30, 0),
                End = new DateTime(2017, 11, 7, 10, 30, 0),
                Contract = new ContractInfo() { TafId = TafId.Taf7 }
            };

            var model = RunValidations(xml, ctx);

            Assert.IsNotNull(model);

            Assert.IsTrue(model.MeterReadings.Count == 1 && model.MeterReadings[0].IntervalBlocks.Count == 1);

            var irs = model.MeterReadings[0].IntervalBlocks[0].IntervalReadings;

            // check the 1st measurement with the skewed timestamp
            CheckIntervalReading(irs[0], "2017-10-29T02:45:00+02:00", "2017-10-29T02:45:00+02:00", 741092);
        }

        [TestMethod]
        [DeploymentItem(@"Data\taf7_oml_1day_values_1.xml")]
        public void Taf7OmlGas1DayValues1()
        {
            var xml = XDocument.Load(@"Data\taf7_oml_1day_values_1.xml");

            var ctx = new AdapterContext()
                          {
                              Start = DateTime.Parse("2017-10-25T00:00:00+02:00"),
                              End = DateTime.Parse("2017-11-01T00:00:00+02:00"),
                              Contract = new ContractInfo() { TafId = TafId.Taf7 }
                          };
            ctx.BillingPeriod = new BillingPeriod() { Begin = ctx.Start, End = ctx.End };

            var model = RunValidations(xml, ctx);
            Assert.IsNotNull(model);

            Assert.AreEqual(Uom.Real_energy, model.MeterReadings[0].ReadingType.Uom);

            var irs = model.MeterReadings[0].IntervalBlocks[0].IntervalReadings;
            CheckIntervalReading(irs[0], "2017-10-25T16:21:45+02:00", "2017-10-25T16:21:45+02:00", 100);
            CheckIntervalReading(irs[1], "2017-10-26T00:00:00+02:00", "2017-10-26T00:06:17+02:00", 200);
            CheckIntervalReading(irs[2], "2017-10-27T00:00:00+02:00", "2017-10-27T00:04:05+02:00", 300);
            CheckIntervalReading(irs[3], "2017-10-28T00:00:00+02:00", "2017-10-28T00:00:19+02:00", 400);
            CheckIntervalReading(irs[4], "2017-10-29T00:00:00+02:00", "2017-10-28T23:57:51+02:00", 500);
            CheckIntervalReading(irs[5], "2017-10-30T00:00:00+01:00", "2017-10-30T00:04:43+01:00", 600);
            CheckIntervalReading(irs[6], "2017-10-30T05:59:53+01:00", "2017-10-30T05:59:53+01:00", 650);
            CheckIntervalReading(irs[7], "2017-10-31T00:00:00+01:00", "2017-10-31T00:06:22+01:00", 700);
            CheckIntervalReading(irs[8], "2017-11-01T00:00:00+01:00", "2017-10-31T23:56:59+01:00", 800);
        }

        [TestMethod]
        [DeploymentItem(@"Data\taf7_oml_1.xml")]
        public void Taf7Oml1DayOfData()
        {
            var xml = XDocument.Load(@"Data\taf7_oml_1.xml");

            var ctx = new AdapterContext()
                          {
                              Start = DateTime.Parse("2017-11-04T07:00:00+01:00"),
                              End = DateTime.Parse("2017-11-08T07:00:00+01:00"),
                              Contract = new ContractInfo() { TafId = TafId.Taf7 }
                          };
            ctx.BillingPeriod = new BillingPeriod() { Begin = ctx.Start, End = ctx.End };

            var model = RunValidations(xml, ctx);
            Assert.IsNotNull(model);

            Assert.AreEqual(Uom.Real_energy, model.MeterReadings[0].ReadingType.Uom);

            var irs = model.MeterReadings[0].IntervalBlocks[0].IntervalReadings;
            Assert.AreEqual(97, irs.Count);

            CheckIntervalReading(irs[0], "2017-11-04T07:00:00+01:00", "2017-11-04T07:00:00+01:00", 746361);
            CheckIntervalReading(irs[1], "2017-11-04T07:15:00+01:00", "2017-11-04T07:15:00+01:00", 746371);
            CheckIntervalReading(irs[2], "2017-11-04T07:30:00+01:00", "2017-11-04T07:30:00+01:00", 746381);
            CheckIntervalReading(irs[3], "2017-11-04T07:45:00+01:00", "2017-11-04T07:45:00+01:00", 746391);
            CheckIntervalReading(irs[4], "2017-11-04T08:00:00+01:00", "2017-11-04T08:00:00+01:00", 746402);
            CheckIntervalReading(irs[5], "2017-11-04T08:15:00+01:00", "2017-11-04T08:15:00+01:00", 746412);
            CheckIntervalReading(irs[6], "2017-11-04T08:30:00+01:00", "2017-11-04T08:30:00+01:00", 746422);

            CheckIntervalReading(irs[40], "2017-11-04T17:00:00+01:00", "2017-11-04T17:00:00+01:00", 746769);
            CheckIntervalReading(irs[41], "2017-11-04T17:15:00+01:00", "2017-11-04T17:15:00+01:00", 746779);
            CheckIntervalReading(irs[42], "2017-11-04T17:30:00+01:00", "2017-11-04T17:30:00+01:00", 746789);

            CheckIntervalReading(irs[93], "2017-11-05T06:15:00+01:00", "2017-11-05T06:15:00+01:00", 747309);
            CheckIntervalReading(irs[94], "2017-11-05T06:30:00+01:00", "2017-11-05T06:30:00+01:00", 747319);
            CheckIntervalReading(irs[95], "2017-11-05T06:45:00+01:00", "2017-11-05T06:45:00+01:00", 747329);
            CheckIntervalReading(irs[96], "2017-11-05T07:00:00+01:00", "2017-11-05T07:00:00+01:00", 747339);
        }

        private static UsagePointAdapterTRuDI RunValidations(XDocument xml, AdapterContext ctx)
        {
            Ar2418Validation.ValidateSchema(xml);
            var model = XmlModelParser.ParseHanAdapterModel(xml.Root?.Descendants());

            ModelValidation.ValidateHanAdapterModel(model);
            ContextValidation.ValidateContext(model, ctx);
            return model;
        }

        private static void CheckIntervalReading(IntervalReading ir, string targetTime, string captureTime, long value)
        {
            Assert.AreEqual(ir.Value, value);
            Assert.AreEqual(targetTime, ir.TargetTime?.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual(captureTime, ir.CaptureTime.ToString("yyyy-MM-ddTHH:mm:ssK"));
        }
    }
}
