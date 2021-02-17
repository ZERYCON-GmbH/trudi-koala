namespace TRuDI.TafAdapter.Taf2.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;

    using TRuDI.Models;
    using TRuDI.Models.BasicData;
    using TRuDI.TafAdapter.Interface.Taf2;

    [TestClass]
    public class TafAdapterTaf2Tests
    {
        [TestMethod]
        public void TestAddAmountToDayRegister()
        {
            var reg181 = new Register() { ObisCode = new ObisId("0100010801FF"), Amount = 0, TariffId = 1 };
            var reg182 = new Register() { ObisCode = new ObisId("0100010802FF"), Amount = 0, TariffId = 2 };
            var reg18x = new Register() { ObisCode = new ObisId("010001083FFF"), Amount = 0, TariffId = 63 };

            var target = new AccountingDay(new[] { reg181, reg182, reg18x });

            target.Add(new MeasuringRange() { Start = DateTime.Now.AddMonths(-1), End = DateTime.Now, TariffId = 1, Amount = 25 }, new ObisId("0100010801FF"));
            target.Add(new MeasuringRange() { Start = DateTime.Now.AddMonths(-1), End = DateTime.Now, TariffId = 2, Amount = 25 }, new ObisId("0100010802FF"));
            target.Add(new MeasuringRange() { Start = DateTime.Now.AddMonths(-1), End = DateTime.Now, TariffId = 63, Amount = 25 }, new ObisId("0100010803FF"));
           


            Assert.AreEqual(25, reg181.Amount);
            Assert.AreEqual(25, reg182.Amount);
            Assert.AreEqual(25, reg18x.Amount);

            target.Add(new MeasuringRange() { Start = DateTime.Now.AddMonths(-1), End = DateTime.Now, TariffId = 1, Amount = 5  }, new ObisId("0100010801FF"));
            target.Add(new MeasuringRange() { Start = DateTime.Now.AddMonths(-1), End = DateTime.Now, TariffId = 2, Amount = 5  }, new ObisId("0100010802FF"));
            target.Add(new MeasuringRange() { Start = DateTime.Now.AddMonths(-1), End = DateTime.Now, TariffId = 63, Amount = 5 }, new ObisId("0100010803FF"));

            Assert.AreEqual(30, reg181.Amount);
            Assert.AreEqual(30, reg182.Amount);
            Assert.AreEqual(30, reg18x.Amount);

            target.Add(new MeasuringRange() { Start = DateTime.Now.AddMonths(-1), End = DateTime.Now, TariffId = 1, Amount = 15  }, new ObisId("0100010801FF"));
            target.Add(new MeasuringRange() { Start = DateTime.Now.AddMonths(-1), End = DateTime.Now, TariffId = 2, Amount = 15  }, new ObisId("0100010802FF"));
            target.Add(new MeasuringRange() { Start = DateTime.Now.AddMonths(-1), End = DateTime.Now, TariffId = 63, Amount = 15 }, new ObisId("0100010803FF"));


            Assert.AreEqual(45, reg181.Amount);
            Assert.AreEqual(45, reg182.Amount);
            Assert.AreEqual(45, reg18x.Amount);

            Assert.AreEqual(9, target.MeasuringRanges.Count);
        }

        [TestMethod]
        public void TestAddAmountToPeriodRange()
        {
            var reg181 = new Register() { ObisCode = new ObisId("0100010801FF"), Amount = 0, TariffId = 1 };
            var reg182 = new Register() { ObisCode = new ObisId("0100010802FF"), Amount = 0, TariffId = 2 };
            var reg18x = new Register() { ObisCode = new ObisId("010001083FFF"), Amount = 0, TariffId = 63 };

            var dayReg181 = new Register() { ObisCode = new ObisId("010001083FFF"), Amount = 0, TariffId = 1 };
            var dayReg182 = new Register() { ObisCode = new ObisId("010001083FFF"), Amount = 0, TariffId = 2 };
            var dayReg18x = new Register() { ObisCode = new ObisId("010001083FFF"), Amount = 0, TariffId = 63 };

            var day = new AccountingDay(new[] { dayReg181, dayReg182, dayReg18x });

            var target = new Taf2Data(new[] { reg181, reg182, reg18x }, null); //TODO second null parameter added blindly just in order to compile successfully

            day.Add(new MeasuringRange() { Start = DateTime.Now.AddMonths(-1), End = DateTime.Now, TariffId = 1, Amount = 50 }, new ObisId("0100010801FF"));
            day.Add(new MeasuringRange() { Start = DateTime.Now.AddMonths(-1), End = DateTime.Now, TariffId = 2, Amount = 25 }, new ObisId("0100010802FF"));
            day.Add(new MeasuringRange() { Start = DateTime.Now.AddMonths(-1), End = DateTime.Now, TariffId = 63, Amount = 10 }, new ObisId("0100010803FF"));

            target.Add(day);

            Assert.AreEqual(50, reg181.Amount);
            Assert.AreEqual(25, reg182.Amount);
            Assert.AreEqual(10, reg18x.Amount);

            day.Add(new MeasuringRange() { Start = DateTime.Now.AddMonths(-1), End = DateTime.Now, TariffId = 1, Amount = 25 }, new ObisId("0100010801FF"));
            day.Add(new MeasuringRange() { Start = DateTime.Now.AddMonths(-1), End = DateTime.Now, TariffId = 2, Amount = 15 }, new ObisId("0100010802FF"));
            day.Add(new MeasuringRange() { Start = DateTime.Now.AddMonths(-1), End = DateTime.Now, TariffId = 63, Amount = 5 }, new ObisId("0100010803FF"));

            target.Add(day);

            Assert.AreEqual(125, reg181.Amount);
            Assert.AreEqual(65, reg182.Amount);
            Assert.AreEqual(25, reg18x.Amount);

            day.Add(new MeasuringRange() { Start = DateTime.Now.AddMonths(-1), End = DateTime.Now, TariffId = 1, Amount = 25 }, new ObisId("0100010801FF"));
            day.Add(new MeasuringRange() { Start = DateTime.Now.AddMonths(-1), End = DateTime.Now, TariffId = 2, Amount = 20 }, new ObisId("0100010802FF"));
            day.Add(new MeasuringRange() { Start = DateTime.Now.AddMonths(-1), End = DateTime.Now, TariffId = 63, Amount = 5 }, new ObisId("0100010803FF"));

            target.Add(day);

            Assert.AreEqual(225, reg181.Amount);
            Assert.AreEqual(125, reg182.Amount);
            Assert.AreEqual(45, reg18x.Amount);

            Assert.AreEqual(3, target.AccountingSections.Count);
        }

        [TestMethod]
        public void TestMeasuringRangeKonstruktorWithMeterReading()
        {
            var mrObis163 = new MeterReading
            {
                ReadingType = new ReadingType()
                {
                    ObisCode = "010001083FFF"
                }
            };

            var mrObis263 = new MeterReading
            {
                ReadingType = new ReadingType()
                {
                    ObisCode = "010002083FFF"
                }
            };

            var mrObis63 = new MeterReading
            {
                ReadingType = new ReadingType()
                {
                    ObisCode = "010000083FFF"
                }
            };

            var target = new MeasuringRange(DateTime.Now.AddMonths(-1), DateTime.Now, mrObis163, 1);

            Assert.AreEqual(63, target.TariffId);

            target = new MeasuringRange(DateTime.Now.AddMonths(-1), DateTime.Now, mrObis263, 1);

            Assert.AreEqual(63, target.TariffId);

            target = new MeasuringRange(DateTime.Now.AddMonths(-1), DateTime.Now, mrObis63, 1);

            Assert.AreEqual(63, target.TariffId);

            var code = "010002083FFF";
            for (int i = 3; i < 256; i++)
            {
                var obisC = i.ToString("X2");
                var obisCode = code.Substring(0, 4) + obisC + code.Substring(6);

                mrObis63.ReadingType.ObisCode = obisCode;
                target = new MeasuringRange(DateTime.Now.AddMonths(-1), DateTime.Now, mrObis63, 1);
                Assert.AreEqual(63, target.TariffId);
            }
        }

        [TestMethod]
        public void TestSetIntervalReading()
        {
            var adapter = new TafAdapterTaf2();
            var reading = new MeterReading();

            var irSet1 = new List<IntervalReading>
            {
                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 1, 1), Duration = 900 } },
                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 1, 1, 0, 15, 0), Duration = 900 } },
                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 1, 1, 0, 30, 0), Duration = 900 } },
                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 1, 1, 0, 45, 0), Duration = 900 } },
                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 1, 1, 1, 0, 0), Duration = 900 } },

                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 1, 15, 12, 0, 0), Duration = 900 } },
                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 1, 15, 12, 15, 0), Duration = 900 } },
                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 1, 15, 12, 30, 0), Duration = 900 } },
                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 1, 15, 12, 45, 0), Duration = 900 } },
                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 1, 15, 13, 0, 0), Duration = 900 } },

                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 1, 31, 23, 0, 0), Duration = 900 } },
                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 1, 31, 23, 15, 0), Duration = 900 } },
                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 1, 31, 23, 30, 0), Duration = 900 } },
                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 1, 31, 23, 45, 0), Duration = 900 } },
                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 2, 1), Duration = 900 } }
            };

            irSet1.ForEach(i => i.TargetTime = i.CaptureTime);

            reading.IntervalBlocks.Add(new IntervalBlock()
            {
                Interval = new Interval()
                {
                    Start = new DateTime(2017, 1, 1),
                    Duration = 2678400
                },
                IntervalReadings = irSet1
            });

            var irSet2 = new List<IntervalReading>
            {
                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 2, 1), Duration = 900 } },
                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 2, 1, 0, 15, 0), Duration = 900 } },
                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 2, 1, 0, 30, 0), Duration = 900 } },
                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 2, 1, 0, 45, 0), Duration = 900 } },
                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 2, 1, 1, 0, 0), Duration = 900 } },

                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 2, 15, 12, 0, 0), Duration = 900 } },
                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 2, 15, 12, 15, 0), Duration = 900 } },
                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 2, 15, 12, 30, 0), Duration = 900 } },
                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 2, 15, 12, 45, 0), Duration = 900 } },
                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 2, 15, 13, 0, 0), Duration = 900 } },

                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 2, 28, 23, 0, 0), Duration = 900 } },
                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 2, 28, 23, 15, 0), Duration = 900 } },
                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 2, 28, 23, 30, 0), Duration = 900 } },
                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 2, 28, 23, 45, 0), Duration = 900 } },
                new IntervalReading() { TimePeriod = new Interval() { Start = new DateTime(2017, 3, 1), Duration = 900 } }
            };

            irSet2.ForEach(i => i.TargetTime = i.CaptureTime);

            reading.IntervalBlocks.Add(new IntervalBlock()
            {
                Interval = new Interval()
                {
                    Start = new DateTime(2017, 2, 1),
                    Duration = 2419200
                },
                IntervalReadings = irSet2
            });

            // Testing time periods included in meterReading.IntervalBlocks[0]
            var date = new DateTime(2017, 1, 1);
            var ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                var result = adapter.SetIntervalReading(reading, date, index, 96);
                Assert.AreEqual(date, result.end);
                Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
            }

            date = new DateTime(2017, 1, 1, 0, 15, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                var result = adapter.SetIntervalReading(reading, date, index, 96);
                Assert.AreEqual(date, result.end);
                Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
            }

            date = new DateTime(2017, 1, 1, 0, 30, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                var result = adapter.SetIntervalReading(reading, date, index, 96);
                Assert.AreEqual(date, result.end);
                Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
            }

            date = new DateTime(2017, 1, 1, 0, 45, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                var result = adapter.SetIntervalReading(reading, date, index, 96);
                Assert.AreEqual(date, result.end);
                Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
            }

            date = new DateTime(2017, 1, 1, 1, 0, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                var result = adapter.SetIntervalReading(reading, date, index, 96);
                Assert.AreEqual(date, result.end);
                Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
            }

            date = new DateTime(2017, 1, 15, 12, 0, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                var result = adapter.SetIntervalReading(reading, date, index, 96);
                Assert.AreEqual(date, result.end);
                Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
            }

            date = new DateTime(2017, 1, 15, 12, 15, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                var result = adapter.SetIntervalReading(reading, date, index, 96);
                Assert.AreEqual(date, result.end);
                Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
            }

            date = new DateTime(2017, 1, 15, 12, 30, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                var result = adapter.SetIntervalReading(reading, date, index, 96);
                Assert.AreEqual(date, result.end);
                Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
            }

            date = new DateTime(2017, 1, 15, 12, 45, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                var result = adapter.SetIntervalReading(reading, date, index, 96);
                Assert.AreEqual(date, result.end);
                Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
            }

            date = new DateTime(2017, 1, 15, 13, 0, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                var result = adapter.SetIntervalReading(reading, date, index, 96);
                Assert.AreEqual(date, result.end);
                Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
            }

            date = new DateTime(2017, 1, 31, 23, 0, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                var result = adapter.SetIntervalReading(reading, date, index, 96);
                Assert.AreEqual(date, result.end);
                Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
            }

            date = new DateTime(2017, 1, 31, 23, 15, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                var result = adapter.SetIntervalReading(reading, date, index, 96);
                Assert.AreEqual(date, result.end);
                Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
            }

            date = new DateTime(2017, 1, 31, 23, 30, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                var result = adapter.SetIntervalReading(reading, date, index, 96);
                Assert.AreEqual(date, result.end);
                Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
            }

            date = new DateTime(2017, 1, 31, 23, 45, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                if(index == 95)
                {
                    var result = adapter.SetIntervalReading(reading, date, index, 96);
                    Assert.AreEqual(date.AddSeconds(900), result.end);
                    Assert.AreEqual(ir.TimePeriod.Start.AddSeconds(900), result.reading.TimePeriod.Start);
                }
                else
                {
                    var result = adapter.SetIntervalReading(reading, date, index, 96);
                    Assert.AreEqual(date, result.end);
                    Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
                }
            }


            // Testing time periods included in meterReading.IntervalBlocks[1]
            date = new DateTime(2017, 2, 1);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                var result = adapter.SetIntervalReading(reading, date, index, 96);
                Assert.AreEqual(date, result.end);
                Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
            }

            date = new DateTime(2017, 2, 1, 0, 15, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                var result = adapter.SetIntervalReading(reading, date, index, 96);
                Assert.AreEqual(date, result.end);
                Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
            }

            date = new DateTime(2017, 2, 1, 0, 30, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                var result = adapter.SetIntervalReading(reading, date, index, 96);
                Assert.AreEqual(date, result.end);
                Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
            }

            date = new DateTime(2017, 2, 1, 0, 45, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                var result = adapter.SetIntervalReading(reading, date, index, 96);
                Assert.AreEqual(date, result.end);
                Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
            }

            date = new DateTime(2017, 2, 1, 1, 0, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                var result = adapter.SetIntervalReading(reading, date, index, 96);
                Assert.AreEqual(date, result.end);
                Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
            }

            date = new DateTime(2017, 2, 15, 12, 0, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                var result = adapter.SetIntervalReading(reading, date, index, 96);
                Assert.AreEqual(date, result.end);
                Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
            }

            date = new DateTime(2017, 2, 15, 12, 15, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                var result = adapter.SetIntervalReading(reading, date, index, 96);
                Assert.AreEqual(date, result.end);
                Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
            }

            date = new DateTime(2017, 2, 15, 12, 30, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                var result = adapter.SetIntervalReading(reading, date, index, 96);
                Assert.AreEqual(date, result.end);
                Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
            }

            date = new DateTime(2017, 2, 15, 12, 45, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                var result = adapter.SetIntervalReading(reading, date, index, 96);
                Assert.AreEqual(date, result.end);
                Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
            }

            date = new DateTime(2017, 2, 15, 13, 0, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                var result = adapter.SetIntervalReading(reading, date, index, 96);
                Assert.AreEqual(date, result.end);
                Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
            }

            date = new DateTime(2017, 2, 28, 23, 0, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                var result = adapter.SetIntervalReading(reading, date, index, 96);
                Assert.AreEqual(date, result.end);
                Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
            }

            date = new DateTime(2017, 2, 28, 23, 15, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                var result = adapter.SetIntervalReading(reading, date, index, 96);
                Assert.AreEqual(date, result.end);
                Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
            }

            date = new DateTime(2017, 2, 28, 23, 30, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                var result = adapter.SetIntervalReading(reading, date, index, 96);
                Assert.AreEqual(date, result.end);
                Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
            }

            date = new DateTime(2017, 2, 28, 23, 45, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                if (index == 95)
                {
                    var result = adapter.SetIntervalReading(reading, date, index, 96);
                    Assert.AreEqual(date.AddSeconds(900), result.end);
                    Assert.AreEqual(ir.TimePeriod.Start.AddSeconds(900), result.reading.TimePeriod.Start);
                }
                else
                {
                    var result = adapter.SetIntervalReading(reading, date, index, 96);
                    Assert.AreEqual(date, result.end);
                    Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
                }
            }

            date = new DateTime(2017, 3, 1);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                var result = adapter.SetIntervalReading(reading, date, index, 96);
                Assert.AreEqual(date, result.end);
                Assert.AreEqual(ir.TimePeriod.Start, result.reading.TimePeriod.Start);
            }


            // Testing time periods not included in meterReading.IntervalBlocks
            date = new DateTime(2017, 1, 28, 9, 0, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                Assert.AreEqual((null, date), adapter.SetIntervalReading(reading, date, index, 96));
            }

            date = new DateTime(2017, 1, 13, 14, 30, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                Assert.AreEqual((null, date), adapter.SetIntervalReading(reading, date, index, 96));
            }

            date = new DateTime(2017, 5, 5, 9, 0, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                Assert.AreEqual((null, date), adapter.SetIntervalReading(reading, date, index, 96));
            }

            date = new DateTime(2017, 4, 28, 23, 45, 0);
            ir = new IntervalReading() { TimePeriod = new Interval() { Start = date, Duration = 900 } };
            for (int index = 0; index < 96; index++)
            {
                if (index == 95)
                {
                    Assert.AreEqual((null, date), adapter.SetIntervalReading(reading, date, index, 96));
                }
                else
                {
                    Assert.AreEqual((null, date), adapter.SetIntervalReading(reading, date, index, 96));
                }
            }
        }
        
        [TestMethod]
        [DeploymentItem(@"Data\resultJan-Sep2017.xml")]
        [DeploymentItem(@"Data\supplierXmlJan-Sep2017.xml")]
        public void TestImportExportDirections()
        {
            var deviceXml  = XDocument.Load(@"Data\resultJan-Sep2017.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplierXmlJan-Sep2017.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-01-01T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-09-01T01:00:00+02:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(6, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(115500, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(117740, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:2.8.1*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(3, data.SummaryRegister[2].TariffId);

            Assert.AreEqual("1-0:2.8.2*255", data.SummaryRegister[3].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[3].Amount);
            Assert.AreEqual(4, data.SummaryRegister[3].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[4].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[4].Amount);
            Assert.AreEqual(63, data.SummaryRegister[4].TariffId);

            Assert.AreEqual("1-0:2.8.63*255", data.SummaryRegister[5].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[5].Amount);
            Assert.AreEqual(63, data.SummaryRegister[5].TariffId);
        }

        [TestMethod]
        [DeploymentItem(@"Data\result_single_day.xml")]
        [DeploymentItem(@"Data\supplier_single_day.xml")]
        public void TestSingleDayWithoutGaps()
        {
            var deviceXml = XDocument.Load(@"Data\result_single_day.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_single_day.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-05T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(360, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(600, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Gap appears in measurement data around a tariffstage switch
        /// (missing interfalls from 5:00 to 6:45)
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_single_day_2h-gap.xml")]
        [DeploymentItem(@"Data\supplier_single_day.xml")]
        public void TestSingleDayWithGapOnTariffChange()
        {
            var deviceXml = XDocument.Load(@"Data\result_single_day_2h-gap.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_single_day.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-05T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(310, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(560, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(90, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        [TestMethod]
        [DeploymentItem(@"Data\result_single_day_2h_PTBstatus3.xml")]
        [DeploymentItem(@"Data\supplier_single_day.xml")]
        public void TestSingleDayWithCriticalPTBStatusOnTariffChange()
        {
            var deviceXml = XDocument.Load(@"Data\result_single_day_2h_PTBstatus3.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_single_day.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-05T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(310, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(560, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(90, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        [TestMethod]
        [DeploymentItem(@"Data\result_single_day.xml")]
        [DeploymentItem(@"Data\supplier_single_day_maxTariffChange.xml")]
        public void TestSingleDayMaxTarifChangeWithoutGaps()
        {
            var deviceXml = XDocument.Load(@"Data\result_single_day.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_single_day_maxTariffChange.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-05T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(470, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(490, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Gap appears in measurement data around a tariffstage switch
        /// (missing interfalls from 5:00 to 6:45)
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_single_day_2h-gap.xml")]
        [DeploymentItem(@"Data\supplier_single_day_maxTariffChange.xml")]
        public void TestSingleDayMaxTariffChangeWithGapOnTariffChange()
        {
            var deviceXml = XDocument.Load(@"Data\result_single_day_2h-gap.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_single_day_maxTariffChange.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-05T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(420, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(450, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(90, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }


        /// <summary>
        /// Gap appears in measurement data inside one single tariffstage
        /// It should not affect the calculated result
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_single_day_simple_gap.xml")]
        [DeploymentItem(@"Data\supplier_single_day.xml")]
        public void TestSingleDayWithGap()
        {
            var deviceXml = XDocument.Load(@"Data\result_single_day_simple_gap.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_single_day.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-05T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(360, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(600, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }


        /// <summary>
        /// Gap appears for one measuring period a 6:00 (tariff switch)
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_single_day_1_missing_period_at_tariff_switch.xml")]
        [DeploymentItem(@"Data\supplier_single_day.xml")]
        public void TestSingleDayWithOnePeriodGapAtTariffSwitch()
        {
            var deviceXml = XDocument.Load(@"Data\result_single_day_1_missing_period_at_tariff_switch.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_single_day.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-05T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(350, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(590, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(20, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        [TestMethod]
        [DeploymentItem(@"Data\result_single_day_dst_oct.xml")]
        [DeploymentItem(@"Data\supplier_single_day_dst_oct.xml")]
        public void TestSingleDay_SummerToWinterTime()
        {
            var deviceXml = XDocument.Load(@"Data\result_single_day_dst_oct.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_single_day_dst_oct.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-10-29T00:00:00+02:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-10-30T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(360, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(600, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        [TestMethod]
        [DeploymentItem(@"Data\result_single_day_dst_march.xml")]
        [DeploymentItem(@"Data\supplier_single_day_dst_march.xml")]
        public void TestSingleDay_WinterToSummerTime()
        {
            var deviceXml = XDocument.Load(@"Data\result_single_day_dst_march.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_single_day_dst_march.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-03-26T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-03-27T00:00:00+02:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(320, data.SummaryRegister[0].Amount); 
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(600, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Checks a 3 day data file against a supplier file that contains only the first day of the data file.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_3_days.xml")]
        [DeploymentItem(@"Data\supplier_single_day.xml")]
        public void Test3DayDataFirstDayTariffFile()
        {
            var deviceXml = XDocument.Load(@"Data\result_3_days.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_single_day.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-05T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(400, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(600, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Checks a 3 day data file against a supplier file that contains only the second day of the data file.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_3_days.xml")]
        [DeploymentItem(@"Data\supplier_single_day_2.xml")]
        public void Test3DayDataSecondDayTariffFile()
        {
            var deviceXml = XDocument.Load(@"Data\result_3_days.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_single_day_2.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-05T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-06T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(400, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(600, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Checks a 3 day data file against a supplier file that contains only the last day of the data file.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_3_days.xml")]
        [DeploymentItem(@"Data\supplier_single_day_3.xml")]
        public void Test3DayDataLastDayTariffFile()
        {
            var deviceXml = XDocument.Load(@"Data\result_3_days.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_single_day_3.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-06T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-07T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(360, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(600, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Checks a 3 day data file against a supplier file that contains first 2 days of the data file.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_3_days.xml")]
        [DeploymentItem(@"Data\supplier_2_day.xml")]
        public void Test3DayData2DayTariffFile()
        {
            var deviceXml = XDocument.Load(@"Data\result_3_days.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_2_day.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-06T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(800, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(1200, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Checks a 3 day data file against a supplier file that contains first 2 days of the data file. 
        /// The measuring period 2017-11-05 00:00:00 is missing.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_3_days_gap_day_change_1.xml")]
        [DeploymentItem(@"Data\supplier_2_day.xml")]
        public void Test3DayData2DayTariffFileGap1()
        {
            var deviceXml = XDocument.Load(@"Data\result_3_days_gap_day_change_1.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_2_day.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-06T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(800, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(1200, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }
        
        /// <summary>
        /// Checks a 3 day data file against a supplier file that contains first 2 days of the data file. 
        /// The measuring periods 2017-11-05 00:00:00 to 2017-11-05 00:15:00 are missing.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_3_days_gap_day_change_2.xml")]
        [DeploymentItem(@"Data\supplier_2_day.xml")]
        public void Test3DayData2DayTariffFileGap2()
        {
            var deviceXml = XDocument.Load(@"Data\result_3_days_gap_day_change_2.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_2_day.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-06T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(800, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(1200, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Checks a 3 day data file against a supplier file that contains first 2 days of the data file. 
        /// The measuring periods 2017-11-04 23:45:00 to 2017-11-05 00:15:00 are missing.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_3_days_gap_day_change_3.xml")]
        [DeploymentItem(@"Data\supplier_2_day.xml")]
        public void Test3DayData2DayTariffFileGap3()
        {
            var deviceXml = XDocument.Load(@"Data\result_3_days_gap_day_change_3.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_2_day.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-06T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(800, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(1200, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Checks a 3 day data file against a supplier file that contains first 2 days of the data file. 
        /// The measuring periods 2017-11-04 23:45:00 and 2017-11-05 00:15:00 are missing. The period 2017-11-05 00:00:00 has PTB status 3.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_3_days_gap_day_change_3_DayChangePTBstatus3.xml")]
        [DeploymentItem(@"Data\supplier_2_day.xml")]
        public void Test3DayData2DayTariffFileGapAndPTBStatus3()
        {
            var deviceXml = XDocument.Load(@"Data\result_3_days_gap_day_change_3_DayChangePTBstatus3.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_2_day.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-06T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(800, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(1200, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Checks a 3 day data file against a supplier file that contains first 2 days of the data file. 
        /// The measuring periods 2017-11-04 23:45:00 and 2017-11-05 00:15:00 are missing. The period 2017-11-05 00:00:00 has PTB status 3.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_3_days_gap_day_change_3_DayChangePTBstatus3_2.xml")]
        [DeploymentItem(@"Data\supplier_2_day.xml")]
        public void Test3DayData2DayTariffFileGapAndPTBStatus3_2()
        {
            var deviceXml = XDocument.Load(@"Data\result_3_days_gap_day_change_3_DayChangePTBstatus3_2.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_2_day.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-06T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(800, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(1200, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }


        /// <summary>
        /// Checks a 3 day data file against a supplier file that contains first 2 days of the data file. 
        /// The measuring periods 2017-11-04 23:45:00 to 2017-11-05 05:30:00 are missing.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_3_days_gap_day_change_4.xml")]
        [DeploymentItem(@"Data\supplier_2_day.xml")]
        public void Test3DayData2DayTariffFileGap4()
        {
            var deviceXml = XDocument.Load(@"Data\result_3_days_gap_day_change_4.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_2_day.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-06T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(800, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(1200, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Checks a 3 day data file against a supplier file that contains first 2 days of the data file. 
        /// The measuring periods 2017-11-04 23:45:00 to 2017-11-05 05:45:00 are missing.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_3_days_gap_day_change_5.xml")]
        [DeploymentItem(@"Data\supplier_2_day.xml")]
        public void Test3DayData2DayTariffFileGap5()
        {
            var deviceXml = XDocument.Load(@"Data\result_3_days_gap_day_change_5.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_2_day.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-06T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(500, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(1200, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(300, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Checks a 3 day data file against a supplier file that contains first 2 days of the data file. 
        /// The measuring periods 2017-11-04 23:45:00 to 2017-11-05 06:15:00 are missing.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_3_days_gap_day_change_6.xml")]
        [DeploymentItem(@"Data\supplier_2_day.xml")]
        public void Test3DayData2DayTariffFileGap6()
        {
            var deviceXml = XDocument.Load(@"Data\result_3_days_gap_day_change_6.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_2_day.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-06T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(500, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(1180, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(320, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Checks a 3 day data file against a supplier file that contains first 2 days of the data file. 
        /// The measuring period 2017-11-04 23:45:00 is missing.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_3_days_gap_day_change_7.xml")]
        [DeploymentItem(@"Data\supplier_2_day.xml")]
        public void Test3DayData2DayTariffFileGap7()
        {
            var deviceXml = XDocument.Load(@"Data\result_3_days_gap_day_change_7.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_2_day.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-06T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(800, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(1200, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Checks a 3 day data file against a supplier file that contains first 2 days of the data file. 
        /// The measuring period from 2017-11-04 23:45:00 are missing.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_3_days_gap_day_change_8.xml")]
        [DeploymentItem(@"Data\supplier_2_day.xml")]
        public void Test3DayData2DayTariffFileGap8()
        {
            var deviceXml = XDocument.Load(@"Data\result_3_days_gap_day_change_8.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_2_day.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-06T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(340, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(600, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(1060, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Checks a 3 day data file against a supplier file that contains all 3 days of the data file. 
        /// The measuring periods 2017-11-04 23:45:00 to 2017-11-06 00:15:00 are missing.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_3_days_gap_day_change_9.xml")]
        [DeploymentItem(@"Data\supplier_3_day.xml")]
        public void Test3DayData2DayTariffFileGap9()
        {
            var deviceXml = XDocument.Load(@"Data\result_3_days_gap_day_change_9.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_3_day.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-07T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(690, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(1200, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(1070, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Checks a 3 day data file against a supplier file that contains all 3 days of the data file. 
        /// The measuring periods 2017-11-04 23:45:00 to 2017-11-05 23:45:00 and 
        /// 2017-11-06 00:15:00 to 2017-11-06 23:45:00 are missing.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_3_days_gap_day_change_10.xml")]
        [DeploymentItem(@"Data\supplier_3_day.xml")]
        public void Test3DayData2DayTariffFileGap10()
        {
            var deviceXml = XDocument.Load(@"Data\result_3_days_gap_day_change_10.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_3_day.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-07T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(340, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(600, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(2020, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Checks a 3 day data file against a supplier file that contains all 3 days of the data file. 
        /// The measuring periods 2017-11-04 23:45:00 to 2017-11-06 23:45:00 are missing.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_3_days_gap_day_change_11.xml")]
        [DeploymentItem(@"Data\supplier_3_day.xml")]
        public void Test3DayData2DayTariffFileGap11()
        {
            var deviceXml = XDocument.Load(@"Data\result_3_days_gap_day_change_11.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_3_day.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-07T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(340, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(600, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(2020, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Checks a 3 day data file against a supplier file that contains all 3 days of the data file. 
        /// The measuring periods 2017-11-04 23:45:00 to 2017-11-05 00:15:00 are missing and the day profile changes.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_3_days_gap_day_change_3.xml")]
        [DeploymentItem(@"Data\supplier_2_day_2_dayProfile.xml")]
        public void Test3DayData2DayTariffFileGap12()
        {
            var deviceXml = XDocument.Load(@"Data\result_3_days_gap_day_change_3.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_2_day_2_dayProfile.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-06T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(940, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(980, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(80, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Checks a 3 day data file against a supplier file that contains all 3 days of the data file. 
        /// The measuring periods is from 2017-11-4 04:00:00 to 2017-11-4 22:00:00.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_3_days.xml")]
        [DeploymentItem(@"Data\supplier_1_day_4_to_22.xml")]
        public void Test3DayData2DayTariffFile18Hours()
        {
            var deviceXml = XDocument.Load(@"Data\result_3_days.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_day_4_to_22.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T04:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-04T22:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(120, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(600, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Checks a 3 day data file against a supplier file that contains all 3 days of the data file. 
        /// The measuring periods is from 2017-11-4 04:00:00 to 2017-11-5 04:00:00
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_3_days.xml")]
        [DeploymentItem(@"Data\supplier_2_day_4_to_4.xml")]
        public void Test3DayData2DayTariffFile1Day4to4()
        {
            var deviceXml = XDocument.Load(@"Data\result_3_days.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_2_day_4_to_4.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T04:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-05T04:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(400, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(600, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Checks a 3 day data file against a supplier file that contains all 3 days of the data file. 
        /// The measuring periods is from 2017-11-4 04:00:00 to 2017-11-6 05:00:00
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_3_days.xml")]
        [DeploymentItem(@"Data\supplier_3_day_4_to_5.xml")]
        public void Test3DayData3DayTariffFile2Day4to5()
        {
            var deviceXml = XDocument.Load(@"Data\result_3_days.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_3_day_4_to_5.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T04:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-06T05:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(840, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(1200, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Checks a 3 day data file against a supplier file that contains all 3 days of the data file. 
        /// The measuring period ist just from 2017-11-04 05:45:00 to 2017-11-04 06:00:00
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_3_days.xml")]
        [DeploymentItem(@"Data\supplier_3_day_minPeriod.xml")]
        public void TestMinimalPeriodData()
        {
            var deviceXml = XDocument.Load(@"Data\result_3_days.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_3_day_minPeriod.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T05:45:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-04T06:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(10, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Checks a 3 day data file against a supplier file that contains 2 days more. 
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_3_days.xml")]
        [DeploymentItem(@"Data\supplier_5_day.xml")]
        public void TestMissingDataOnEnd()
        {
            var deviceXml = XDocument.Load(@"Data\result_3_days.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_5_day.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-07T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(1160, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(1800, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Checks a 3 day data file against a supplier file that contains 2 days more. 
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_3_days_1_hour.xml")]
        [DeploymentItem(@"Data\supplier_5_day.xml")]
        public void TestMissingDataOnEnd2()
        {
            var deviceXml = XDocument.Load(@"Data\result_3_days_1_hour.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_5_day.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-07T01:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(1200, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(1800, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Checks a 3 day data file against a supplier file that contains 2 days more. 
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_3_days_1_hour_2.xml")]
        [DeploymentItem(@"Data\supplier_5_day.xml")]
        public void TestMissingDataOnEnd3()
        {
            var deviceXml = XDocument.Load(@"Data\result_3_days_1_hour_2.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_5_day.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T01:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-07T01:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(1160, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(1800, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Tests various gaps over several days:
        /// There is only data at:
        /// 26.10.2017 08:45
        /// - gap -
        /// 26.10.2017 10:00 - 10:45
        /// - gap -
        /// 29.10.2017 00:00
        /// - gap -
        /// 29.10.2017 02:15 - 03:30
        /// - gap -
        /// 31.10.2017 01:15
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\many_gaps_result_2017-10-26.xml")]
        [DeploymentItem(@"Data\many_gaps_supplier_2017-10-26.xml")]
        public void TestManyGaps1()
        {
            var deviceXml = XDocument.Load(@"Data\many_gaps_result_2017-10-26.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\many_gaps_supplier_2017-10-26.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-10-26T08:45:00+02:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-10-31T01:15:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(6, data.SummaryRegister.Count);

            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(16090000, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(5294000, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[4].ObisCode.ToString());
            Assert.AreEqual(7100000, data.SummaryRegister[4].Amount);
            Assert.AreEqual(63, data.SummaryRegister[4].TariffId);

            Assert.AreEqual("1-0:2.8.1*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(3, data.SummaryRegister[2].TariffId);

            Assert.AreEqual("1-0:2.8.2*255", data.SummaryRegister[3].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[3].Amount);
            Assert.AreEqual(4, data.SummaryRegister[3].TariffId);

            Assert.AreEqual("1-0:2.8.63*255", data.SummaryRegister[5].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[5].Amount);
            Assert.AreEqual(63, data.SummaryRegister[5].TariffId);
        }

        /// <summary>
        /// Tests various gaps over several days:
        /// There is only data at:
        /// 26.10.2017 08:45
        /// - gap -
        /// 26.10.2017 10:00 - 10:45
        /// - gap -
        /// 28.10.2017 23:45 - 29.10.2017 00:00
        /// - gap -
        /// 29.10.2017 02:15 - 03:30
        /// - gap -
        /// 31.10.2017 01:15
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\many_gaps_result_2017-10-26_2.xml")]
        [DeploymentItem(@"Data\many_gaps_supplier_2017-10-26.xml")]
        public void TestManyGaps2()
        {
            var deviceXml = XDocument.Load(@"Data\many_gaps_result_2017-10-26_2.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\many_gaps_supplier_2017-10-26.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-10-26T08:45:00+02:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-10-31T01:15:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(6, data.SummaryRegister.Count);

            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(16091000, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(5294000, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[4].ObisCode.ToString());
            Assert.AreEqual(7099000, data.SummaryRegister[4].Amount);
            Assert.AreEqual(63, data.SummaryRegister[4].TariffId);

            Assert.AreEqual("1-0:2.8.1*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(3, data.SummaryRegister[2].TariffId);

            Assert.AreEqual("1-0:2.8.2*255", data.SummaryRegister[3].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[3].Amount);
            Assert.AreEqual(4, data.SummaryRegister[3].TariffId);

            Assert.AreEqual("1-0:2.8.63*255", data.SummaryRegister[5].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[5].Amount);
            Assert.AreEqual(63, data.SummaryRegister[5].TariffId);
        }

        /// <summary>
        /// The measurement period of the result file is 30 minutes.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_single_day_period_30min.xml")]
        [DeploymentItem(@"Data\supplier_single_day_4.xml")]
        public void TestSingleDay30MinPeriodWithoutGaps()
        {
            var deviceXml = XDocument.Load(@"Data\result_single_day_period_30min.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_single_day_4.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-02T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(180, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(300, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// The measurement period of the result file is 30 minutes.
        /// The first IntervalReading is missed.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_single_day_period_30min_Gap1.xml")]
        [DeploymentItem(@"Data\supplier_single_day_4.xml")]
        public void TestSingleDay30MinPeriodWithGapAtStart()
        {
            var deviceXml = XDocument.Load(@"Data\result_single_day_period_30min_Gap1.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_single_day_4.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-01T00:30:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-02T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(170, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(300, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// The measurement period of the result file is 30 minutes.
        /// Single Gap at first tariff change.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_single_day_period_30min_Gap2.xml")]
        [DeploymentItem(@"Data\supplier_single_day_4.xml")]
        public void TestSingleDay30MinPeriodWithGapAtTariffChange1()
        {
            var deviceXml = XDocument.Load(@"Data\result_single_day_period_30min_Gap2.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_single_day_4.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-02T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(170, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(290, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(20, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// The measurement period of the result file is 30 minutes.
        /// Two IntervalReadings at first tariff change are missed (05:30 and 06:00)
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_single_day_period_30min_Gap3.xml")]
        [DeploymentItem(@"Data\supplier_single_day_4.xml")]
        public void TestSingleDay30MinPeriodWithGapAtTariffChange2()
        {
            var deviceXml = XDocument.Load(@"Data\result_single_day_period_30min_Gap3.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_single_day_4.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-02T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(160, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(290, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(30, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// The measurement period of the result file is 30 minutes.
        /// Three IntervalReadings at first tariff change are missed (05:30, 06:00, 06:30)
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_single_day_period_30min_Gap4.xml")]
        [DeploymentItem(@"Data\supplier_single_day_4.xml")]
        public void TestSingleDay30MinPeriodWithGapAtTariffChange3()
        {
            var deviceXml = XDocument.Load(@"Data\result_single_day_period_30min_Gap4.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_single_day_4.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-02T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(160, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(280, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(40, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// The measurement period of the result file is 30 minutes.
        /// Three IntervalReadings at first tariff change are missed (05:30, 06:00, 06:30) and
        /// two IntervalReadings at the second tariff change (21:00, 21:30)
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_single_day_period_30min_Gap5.xml")]
        [DeploymentItem(@"Data\supplier_single_day_4.xml")]
        public void TestSingleDay30MinPeriodWithGapAtTariffChange4()
        {
            var deviceXml = XDocument.Load(@"Data\result_single_day_period_30min_Gap5.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_single_day_4.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-02T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(140, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(270, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(70, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// The measurement period of the result file is 30 minutes.
        /// Three IntervalReadings at first tariff change are missed (05:30, 06:00, 06:30) and
        /// two IntervalReadings at the second tariff change (21:00, 21:30). 
        /// Also the last two IntervalReadings are missed (23:00, 00:00)
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_single_day_period_30min_Gap6.xml")]
        [DeploymentItem(@"Data\supplier_single_day_4.xml")]
        public void TestSingleDay30MinPeriodWithGapAtTariffChange5()
        {
            var deviceXml = XDocument.Load(@"Data\result_single_day_period_30min_Gap6.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_single_day_4.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-01T23:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(120, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(270, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(70, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// The measurement period of the result file is 30 minutes.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_five_days_period_30min.xml")]
        [DeploymentItem(@"Data\supplier_five_days.xml")]
        public void TestFiveDays30MinPeriodWithoutGaps()
        {
            var deviceXml = XDocument.Load(@"Data\result_five_days_period_30min.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_five_days.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-06T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(900, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(1500, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// The measurement period of the result file is 30 minutes.
        /// Gap at daychange (1.11 23:00 - 2.11 1:30)
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_five_days_period_30min_Gap1.xml")]
        [DeploymentItem(@"Data\supplier_five_days.xml")]
        public void TestFiveDays30MinPeriodWithGap1()
        {
            var deviceXml = XDocument.Load(@"Data\result_five_days_period_30min_Gap1.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_five_days.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-06T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(900, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(1500, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// The measurement period of the result file is 30 minutes.
        /// Gap at last tariff change till next day (1.11 21:00 - 2.11 1:30)
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_five_days_period_30min_Gap2.xml")]
        [DeploymentItem(@"Data\supplier_five_days.xml")]
        public void TestFiveDays30MinPeriodWithGap2()
        {
            var deviceXml = XDocument.Load(@"Data\result_five_days_period_30min_Gap2.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_five_days.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-06T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(800, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(1490, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(110, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// The measurement period of the result file is 30 minutes.
        /// 5 Days with many Gaps. The gaps at:
        /// 1.11.2017 21:00 - 2.11.2017 1:30
        /// 2.11.2017 6:00 - 3.11.2017 5:30
        /// 4.11.2017 0:00 - 4.11.2017 6:30
        /// 4.11.2017 20:00 - 5.11.2017 4:30
        /// 5.11.2017 21:00 - 5.11.2017 22:00
        /// 5.11.2017 23:00 - 5.11.2017 23:30
        /// 
        /// The tariff changes at 0:00, 6:00 and 21:00
        /// 
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_five_days_period_30min_Gap3.xml")]
        [DeploymentItem(@"Data\supplier_five_days.xml")]
        public void TestFiveDays30MinPeriodWithGap3()
        {
            var deviceXml = XDocument.Load(@"Data\result_five_days_period_30min_Gap3.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_five_days.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-06T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(290, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(1130, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(980, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// The measurement period of the result file is 30 minutes.
        /// 5 Days with one big gap (1.11.2017 21:00 5.11.2017 6:30)
        /// 
        /// The tariff changes at 0:00, 6:00 and 21:00
        /// 
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_five_days_period_30min_Gap4.xml")]
        [DeploymentItem(@"Data\supplier_five_days.xml")]
        public void TestFiveDays30MinPeriodWithGap4()
        {
            var deviceXml = XDocument.Load(@"Data\result_five_days_period_30min_Gap4.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_five_days.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-06T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(180, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(570, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(1650, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// The measurement period of the result file is 45 minutes.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_five_days_period_45min.xml")]
        [DeploymentItem(@"Data\supplier_five_days.xml")]
        public void TestFiveDays45MinPeriodWithoutGap()
        {
            var deviceXml = XDocument.Load(@"Data\result_five_days_period_45min.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_five_days.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-06T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(600, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(1000, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// The measurement period of the result file is 45 minutes.
        /// A single gap (1.11 19:30 - 1.11 21:45)
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_five_days_period_45min_Gap1.xml")]
        [DeploymentItem(@"Data\supplier_five_days.xml")]
        public void TestFiveDays45MinPeriodWithGap1()
        {
            var deviceXml = XDocument.Load(@"Data\result_five_days_period_45min_Gap1.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_five_days.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-06T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(580, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(970, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(50, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// The measurement period of the result file is 1 day.
        /// Supplier file is with tariff change within the day. 
        /// All must be count in the error register.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month_period_1day.xml")]
        [DeploymentItem(@"Data\supplier_1_month.xml")]
        public void TestSingleMonth1DayPeriodWithoutGap()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month_period_1day.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_month.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-12-01T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(300, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// The measurement period of the result file is 1 day.
        /// Supplier file is without tariff change within the day. 
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month_period_1day.xml")]
        [DeploymentItem(@"Data\supplier_1_month_noTariffChange.xml")]
        public void TestSingleMonth1DayPeriodWithoutGap2()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month_period_1day.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_month_noTariffChange.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-12-01T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(300, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// The measurement period of the result file is 1 day.
        /// Tariff Change at the weekend 
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month_period_1day.xml")]
        [DeploymentItem(@"Data\supplier_1_month_tariffChangeWeekend.xml")]
        public void TestSingleMonth1DayPeriodWithoutGap3()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month_period_1day.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_month_tariffChangeWeekend.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-12-01T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(220, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(80, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// The measurement period of the result file is 1 day.
        /// Tariff Change at the weekend: Gap at 3.11.2017 and 4.11.2017 
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month_period_1day_Gap1.xml")]
        [DeploymentItem(@"Data\supplier_1_month_tariffChangeWeekend.xml")]
        public void TestSingleMonth1DayPeriodWithGap()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month_period_1day_Gap1.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_month_tariffChangeWeekend.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-12-01T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(200, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(70, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(30, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// The measurement period of the result file is 1 hour.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month_period_1hour.xml")]
        [DeploymentItem(@"Data\supplier_1_month.xml")]
        public void TestSingleMonth1HourPeriodWithoutGap()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month_period_1hour.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_month.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-12-01T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(2700, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(4500, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// The measurement period of the result file is 1 hour.
        /// A Single Gap 1.11.2017 (5:00, 6:00)
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month_period_1hour_Gap1.xml")]
        [DeploymentItem(@"Data\supplier_1_month.xml")]
        public void TestSingleMonth1HourPeriodWithGap1()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month_period_1hour_Gap1.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_month.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-12-01T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(2680, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(4490, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(30, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// The measurement period of the result file is 1 hour.
        /// A Single Gap 1.11.2017 (5:00, 6:00)
        ///  
        /// Additional Gaps:
        /// 
        /// 2.11.2017 0:00
        /// 3.11.2017 0:00
        /// 4.11.2017 0:00
        /// 5.11.2017 0:00
        /// 
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month_period_1hour_Gap2.xml")]
        [DeploymentItem(@"Data\supplier_1_month.xml")]
        public void TestSingleMonth1HourPeriodWithGap2()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month_period_1hour_Gap2.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_month.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-12-01T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(2680, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(4490, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(30, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// The measurement period of the result file is 1 hour.
        /// A Single Gap 1.11.2017 (5:00, 6:00)
        ///  
        /// Additional Gaps:
        /// 
        /// 2.11.2017 0:00
        /// 3.11.2017 0:00
        /// 4.11.2017 0:00
        /// 5.11.2017 0:00
        /// 
        /// 16.11.2017 21:00 - 24.11.2017 18:00
        /// 
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month_period_1hour_Gap3.xml")]
        [DeploymentItem(@"Data\supplier_1_month.xml")]
        public void TestSingleMonth1HourPeriodWithGap3()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month_period_1hour_Gap3.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_month.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-12-01T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(1960, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(3300, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(1940, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Testing supplier file with wrong DayId. Exception at parsing is expected.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month_period_1hour.xml")]
        [DeploymentItem(@"Data\supplier_1_month_wrongDayId.xml")]
        public void TestWrongDayIdSupplierFileParsingException()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month_period_1hour.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_month_wrongDayId.xml");
            
            var ex = Assert.ThrowsException<AggregateException>(
                () => XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants()));

        }

        /// <summary>
        /// The measurement period of the result file is 6 hour.
        /// The Tariffchange is at 6:00 and 21:00. Due to the measurement period the 
        /// value at 21:00 is missed for the whole month.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month_period_6hour.xml")]
        [DeploymentItem(@"Data\supplier_1_month.xml")]
        public void TestSingleMonth6HourPeriodWithWrongSupplierFile()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month_period_6hour.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_month.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-12-01T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(300, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(600, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(300, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// The measurement period of the result file is 6 hour.
        /// The Tariffchange is at 6:00 and 18:00. 
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month_period_6hour.xml")]
        [DeploymentItem(@"Data\supplier_1_month_lastTariffChangeAt18.xml")]
        public void TestSingleMonth6HourPeriodWithoutGap()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month_period_6hour.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_month_lastTariffChangeAt18.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-12-01T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(600, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(600, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// The measurement period of the result file is 6 hour.
        /// The Tariffchange is at 6:00 and 18:00.
        /// 
        ///  Gaps at:
        /// 
        ///  1.11.2017 6:00 and 12:00
        ///  3.11.2017 0:00 and 6:00
        ///  4.11.2017 12:00 - 5.11.2017 12:00
        ///  6.11.2017 0:00 and 6:00
        ///  8.11.2017 0:00 - 17.11.2017 6:00
        ///  19.11.2017 12:00 - 23.11.2017 6:00
        /// 
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month_period_6hour_manyGaps.xml")]
        [DeploymentItem(@"Data\supplier_1_month_lastTariffChangeAt18.xml")]
        public void TestSingleMonth6HourPeriodWithManyGaps()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month_period_6hour_manyGaps.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_month_lastTariffChangeAt18.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-12-01T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(250, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(240, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(710, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// The measurement period of the result file is 3 hour.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month_period_3hour.xml")]
        [DeploymentItem(@"Data\supplier_1_month.xml")]
        public void TestSingleMonth3HourPeriodWithoutGaps()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month_period_3hour.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_month.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-12-01T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(900, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(1500, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(0, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// The measurement period of the result file is 3 hour.
        /// 
        /// Gaps at:
        /// 
        /// 1.11.2017 0:00
        /// 1.11.2017 6:00 - 15.11.2017 3:00
        /// 16.11.2017 21:00 - 18.11.2017 00:00
        /// 23.11.2017 09:00 - 23.11.2017 18:00
        /// 24.11.2017 0:00 - 24.11.2017 3:00 
        /// 28.11.2017 6:00 - 30.11.2017 18:00
        /// 1.12.2017 0:00
        /// 
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month_period_3hour_manyGaps.xml")]
        [DeploymentItem(@"Data\supplier_1_month.xml")]
        public void TestSingleMonth3HourPeriodWithManyGaps()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month_period_3hour_manyGaps.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_month.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-01T03:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-30T21:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(300, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(590, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(1490, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }

        /// <summary>
        /// Checks a 3 day data file against a supplier file that contains only the last day of the data file.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_3_days_out_of_raster.xml")]
        [DeploymentItem(@"Data\supplier_3_day.xml")]
        public void TestValueOutOfRaster()
        {
            var deviceXml = XDocument.Load(@"Data\result_3_days_out_of_raster.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_3_day.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf2();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-11-04T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-07T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf2Data;

            Assert.AreEqual(3, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(1150, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:1.8.2*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(1790, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);

            Assert.AreEqual("1-0:1.8.63*255", data.SummaryRegister[2].ObisCode.ToString());
            Assert.AreEqual(20, data.SummaryRegister[2].Amount);
            Assert.AreEqual(63, data.SummaryRegister[2].TariffId);
        }
    }
}
