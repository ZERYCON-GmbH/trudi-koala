namespace TRuDI.TafAdapter.Interface.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using TRuDI.Models;
    using TRuDI.Models.BasicData;
    using TRuDI.Models.CheckData;
    using TRuDI.TafAdapter.Interface.Taf2;

    [TestClass]
    public class TafAdapterInterfaceExtensionsTests
    {
        [TestMethod]
        public void TestGetRegister()
        {
            var supplier = new UsagePointLieferant();
            var analysisProfile = new AnalysisProfile();

            var obisId181 =  "0100010801FF";
            var obisId182 =  "0100010802FF";
            var obisId183 =  "0100010803FF";  
            var obisId1863 = "010001083FFF";

            var obisId281 =  "0100020801FF";
            var obisId282 =  "0100020802FF";
            var obisId2863 = "010002083FFF";

            var tariffStage1 = new TariffStage() { Description = "T1", ObisCode = obisId181, TariffNumber = 1 };
            var tariffStage2 = new TariffStage() { Description = "T2", ObisCode = obisId182, TariffNumber = 2 };
            var tariffStage3 = new TariffStage() { Description = "T3", ObisCode = obisId183, TariffNumber = 3 };
            var tariffStage4 = new TariffStage() { Description = "T4", ObisCode = obisId1863, TariffNumber = 4 };
            var tariffStage5 = new TariffStage() { Description = "T5", ObisCode = obisId281, TariffNumber = 5 };
            var tariffStage6 = new TariffStage() { Description = "T6", ObisCode = obisId282, TariffNumber = 6 };
            var tariffStage7 = new TariffStage() { Description = "T7", ObisCode = obisId2863, TariffNumber = 7 };

            analysisProfile.TariffStages.Add(tariffStage1);
            analysisProfile.TariffStages.Add(tariffStage2);
            analysisProfile.TariffStages.Add(tariffStage3);
            analysisProfile.TariffStages.Add(tariffStage4);
            analysisProfile.TariffStages.Add(tariffStage5);
            analysisProfile.TariffStages.Add(tariffStage6);
            analysisProfile.TariffStages.Add(tariffStage7);

            supplier.AnalysisProfile = analysisProfile;
            var target = supplier.GetRegister();

            Assert.AreEqual(7, target.Count);

            foreach (Register reg in target)
            {
                Assert.AreEqual(0, reg.Amount);
            }

            Assert.AreEqual("0100010801FF", target[0].ObisCode.ToHexString());
            Assert.AreEqual("0100010802FF", target[1].ObisCode.ToHexString());
            Assert.AreEqual("0100010803FF", target[2].ObisCode.ToHexString());
            Assert.AreEqual("010001083FFF", target[3].ObisCode.ToHexString());
            Assert.AreEqual("0100020801FF", target[4].ObisCode.ToHexString());
            Assert.AreEqual("0100020802FF", target[5].ObisCode.ToHexString());
            Assert.AreEqual("010002083FFF", target[6].ObisCode.ToHexString());

            Assert.AreEqual(1, target[0].TariffId);
            Assert.AreEqual(2, target[1].TariffId);
            Assert.AreEqual(3, target[2].TariffId);
            Assert.AreEqual(4, target[3].TariffId);
            Assert.AreEqual(5, target[4].TariffId);
            Assert.AreEqual(6, target[5].TariffId);
            Assert.AreEqual(7, target[6].TariffId);

            supplier.AnalysisProfile.TariffUseCase = HanAdapter.Interface.TafId.Taf2;

            target = supplier.GetRegister();

            Assert.AreEqual(1, target[0].TariffId);
            Assert.AreEqual(2, target[1].TariffId);
            Assert.AreEqual(3, target[2].TariffId);
            Assert.AreEqual(63, target[3].TariffId);
            Assert.AreEqual(5, target[4].TariffId);
            Assert.AreEqual(6, target[5].TariffId);
            Assert.AreEqual(63, target[6].TariffId);

            var analysisProfile2 = new AnalysisProfile
            {
                TariffUseCase = HanAdapter.Interface.TafId.Taf2
            };

            analysisProfile2.TariffStages.Add(tariffStage1);
            analysisProfile2.TariffStages.Add(tariffStage2);
            analysisProfile2.TariffStages.Add(tariffStage3);
            analysisProfile2.TariffStages.Add(tariffStage5);
            analysisProfile2.TariffStages.Add(tariffStage6);

            supplier.AnalysisProfile = analysisProfile2;

            target = supplier.GetRegister();

            foreach (Register reg in target)
            {
                Assert.AreEqual(0, reg.Amount);
            }

            Assert.AreEqual(7, target.Count);

            Assert.AreEqual("0100010801FF", target[0].ObisCode.ToHexString());
            Assert.AreEqual("0100010802FF", target[1].ObisCode.ToHexString());
            Assert.AreEqual("0100010803FF", target[2].ObisCode.ToHexString());
            Assert.AreEqual("0100020801FF", target[3].ObisCode.ToHexString());
            Assert.AreEqual("0100020802FF", target[4].ObisCode.ToHexString());
            Assert.AreEqual("010001083FFF", target[5].ObisCode.ToHexString());
            Assert.AreEqual("010002083FFF", target[6].ObisCode.ToHexString());

            Assert.AreEqual(1, target[0].TariffId);
            Assert.AreEqual(2, target[1].TariffId);
            Assert.AreEqual(3, target[2].TariffId);
            Assert.AreEqual(5, target[3].TariffId);
            Assert.AreEqual(6, target[4].TariffId);
            Assert.AreEqual(63, target[5].TariffId);
            Assert.AreEqual(63, target[6].TariffId);

            var analysisProfile3 = new AnalysisProfile
            {
                TariffUseCase = HanAdapter.Interface.TafId.Taf2
            };

            var obisId381 = "0100030801FF";
            var obisId481 = "0100040801FF";
            var obisId482 = "0100040802FF";

            var tariffStage8 = new TariffStage() { Description = "T8", ObisCode = obisId381, TariffNumber = 8 };
            var tariffStage9 = new TariffStage() { Description = "T9", ObisCode = obisId481, TariffNumber = 9 };
            var tariffStage10 = new TariffStage() { Description = "T10", ObisCode = obisId482, TariffNumber = 10 };

            analysisProfile3.TariffStages.Add(tariffStage1);
            analysisProfile3.TariffStages.Add(tariffStage2);
            analysisProfile3.TariffStages.Add(tariffStage3);
            analysisProfile3.TariffStages.Add(tariffStage5);
            analysisProfile3.TariffStages.Add(tariffStage6);
            analysisProfile3.TariffStages.Add(tariffStage8);
            analysisProfile3.TariffStages.Add(tariffStage9);
            analysisProfile3.TariffStages.Add(tariffStage10);

            supplier.AnalysisProfile = analysisProfile3;

            target = supplier.GetRegister();

            foreach (Register reg in target)
            {
                Assert.AreEqual(0, reg.Amount);
            }

            Assert.AreEqual(12, target.Count);

            Assert.AreEqual("0100010801FF", target[0].ObisCode.ToHexString());
            Assert.AreEqual("0100010802FF", target[1].ObisCode.ToHexString());
            Assert.AreEqual("0100010803FF", target[2].ObisCode.ToHexString());
            Assert.AreEqual("0100020801FF", target[3].ObisCode.ToHexString());
            Assert.AreEqual("0100020802FF", target[4].ObisCode.ToHexString());
            Assert.AreEqual("0100030801FF", target[5].ObisCode.ToHexString());
            Assert.AreEqual("0100040801FF", target[6].ObisCode.ToHexString());
            Assert.AreEqual("0100040802FF", target[7].ObisCode.ToHexString());
            Assert.AreEqual("010001083FFF", target[8].ObisCode.ToHexString());
            Assert.AreEqual("010002083FFF", target[9].ObisCode.ToHexString());
            Assert.AreEqual("010003083FFF", target[10].ObisCode.ToHexString());
            Assert.AreEqual("010004083FFF", target[11].ObisCode.ToHexString());
        
            Assert.AreEqual(1, target[0].TariffId);
            Assert.AreEqual(2, target[1].TariffId);
            Assert.AreEqual(3, target[2].TariffId);
            Assert.AreEqual(5, target[3].TariffId);
            Assert.AreEqual(6, target[4].TariffId);
            Assert.AreEqual(8, target[5].TariffId);
            Assert.AreEqual(9, target[6].TariffId);
            Assert.AreEqual(10, target[7].TariffId);
            Assert.AreEqual(63, target[8].TariffId);
            Assert.AreEqual(63, target[9].TariffId);
            Assert.AreEqual(63, target[10].TariffId);
            Assert.AreEqual(63, target[11].TariffId);
        }

        [TestMethod]
        public void TestGetValidDayProfilesForMeterReading()
        {
            var dayTimeProfiles1 = new List<DayTimeProfile>();
            var dayTimeProfiles2 = new List<DayTimeProfile>();
            var dayTimeProfiles3 = new List<DayTimeProfile>();
            var dayTimeProfiles4 = new List<DayTimeProfile>();
           
            var hours = 0;
            var minutes = new int[] { 0, 15, 30, 45 };

            // Initialize dayTimeProfiles1 (just one tariff for the whole day)
            for (int i = 0; i < 96; i++)
            {
                var index = i % (minutes.Length);
                var dtp = new DayTimeProfile()
                {
                    TariffNumber = 1,

                    StartTime = new TimeVarType() { Hour = (byte)hours, Minute = (byte)minutes[index] }
                };

                hours = index == 3 ? hours + 1 : hours;
                dayTimeProfiles1.Add(dtp);
            }

            // Initialize dayTimeProfiles2 (tariff 2 from 0 to 6; tariff 1 from 6 to 21; tariff 2 from 21 to 0)
            hours = 0;
            for (int i = 0; i < 96; i++)
            {
                var index = i % (minutes.Length);
                var dtp = new DayTimeProfile()
                {
                    StartTime = new TimeVarType() { Hour = (byte)hours, Minute = (byte)minutes[index] }
                };

                if(hours > 5 && hours < 21)
                {
                    dtp.TariffNumber = 1;
                }
                else
                {
                    dtp.TariffNumber = 2;
                }

                hours = index == 3 ? hours + 1 : hours;
                dayTimeProfiles2.Add(dtp);
            }
            
            // Initialize dayTimeProfiles3 (tariff 3 from 0 to 12; tariff 4 from 12 to 0)
            hours = 0;
            for (int i = 0; i < 96; i++)
            {
                var index = i % (minutes.Length);
                var dtp = new DayTimeProfile()
                {
                    StartTime = new TimeVarType() { Hour = (byte)hours, Minute = (byte)minutes[index] }
                };

                if (hours < 12)
                {
                    dtp.TariffNumber = 3;
                }
                else
                {
                    dtp.TariffNumber = 4;
                }

                hours = index == 3 ? hours + 1 : hours;
                dayTimeProfiles3.Add(dtp);
            }

            // Initialize dayTimeProfiles4 (tariff 1 from 0 to 12; tariff 4 from 12 to 0)
            hours = 0;
            for (int i = 0; i < 96; i++)
            {
                var index = i % (minutes.Length);
                var dtp = new DayTimeProfile()
                {
                    StartTime = new TimeVarType() { Hour = (byte)hours, Minute = (byte)minutes[index] }
                };

                if (hours < 12)
                {
                    dtp.TariffNumber = 1;
                }
                else
                {
                    dtp.TariffNumber = 4;
                }

                hours = index == 3 ? hours + 1 : hours;
                dayTimeProfiles4.Add(dtp);
            }


            // The MeterReading ObisIds
            var oc1 = new ObisId("0100010800FF");
            var oc2 = new ObisId("0100020800FF");

            // Create the list of DayProfiles
            var dayProfiles = new List<DayProfile>();
    
            var dp1 = new DayProfile() { DayId = 1, DayTimeProfiles = dayTimeProfiles1 };
            var dp2 = new DayProfile() { DayId = 2, DayTimeProfiles = dayTimeProfiles2 };
            var dp3 = new DayProfile() { DayId = 3, DayTimeProfiles = dayTimeProfiles3 };

            dayProfiles.Add(dp1);
            dayProfiles.Add(dp2);
            dayProfiles.Add(dp3);

            var tariffStages = new List<TariffStage>();

            var obisId181 = "0100010801FF";
            var obisId182 = "0100010802FF";
            var obisId281 = "0100020801FF";
            var obisId282 = "0100020802FF";

            var tariffStage1 = new TariffStage() { Description = "T1", ObisCode = obisId181, TariffNumber = 1 };
            var tariffStage2 = new TariffStage() { Description = "T2", ObisCode = obisId182, TariffNumber = 2 };
            var tariffStage3 = new TariffStage() { Description = "T3", ObisCode = obisId281, TariffNumber = 3 };
            var tariffStage4 = new TariffStage() { Description = "T4", ObisCode = obisId282, TariffNumber = 4 };

            tariffStages.Add(tariffStage1);
            tariffStages.Add(tariffStage2);
            tariffStages.Add(tariffStage3);
            tariffStages.Add(tariffStage4);

            var target = dayProfiles.GetValidDayProfilesForMeterReading(oc1, tariffStages);

            Assert.AreEqual(2, target.Count);
            Assert.AreEqual((ushort)1, target[0]);
            Assert.AreEqual((ushort)2, target[1]);

            target = dayProfiles.GetValidDayProfilesForMeterReading(oc2, tariffStages);

            Assert.AreEqual(1, target.Count);
            Assert.AreEqual((ushort)3, target[0]);

            var obisId381 = "0100030801FF";
            var obisId482 = "0100040802FF";

            var tariffStage5 = new TariffStage() { Description = "T5", ObisCode = obisId381, TariffNumber = 5 };
            var tariffStage6 = new TariffStage() { Description = "T6", ObisCode = obisId482, TariffNumber = 6 };

            var tariffStages2 = new List<TariffStage>();

            tariffStages2.Add(tariffStage5);
            tariffStages2.Add(tariffStage6);

            target = dayProfiles.GetValidDayProfilesForMeterReading(oc1, tariffStages2);

            Assert.AreEqual(0, target.Count);

            target = dayProfiles.GetValidDayProfilesForMeterReading(oc2, tariffStages2);

            Assert.AreEqual(0, target.Count);

            var dayProfiles2 = new List<DayProfile>();

            var dp4 = new DayProfile() { DayId = 4, DayTimeProfiles = dayTimeProfiles4 };

            dayProfiles2.Add(dp4);

            target = dayProfiles2.GetValidDayProfilesForMeterReading(oc1, tariffStages);

            Assert.AreEqual(0, target.Count);

            target = dayProfiles2.GetValidDayProfilesForMeterReading(oc2, tariffStages);

            Assert.AreEqual(0, target.Count);

            target = dayProfiles2.GetValidDayProfilesForMeterReading(oc1, tariffStages2);

            Assert.AreEqual(0, target.Count);

            target = dayProfiles2.GetValidDayProfilesForMeterReading(oc2, tariffStages2);

            Assert.AreEqual(0, target.Count);
            
            Assert.ThrowsException<ArgumentNullException>(() => dayProfiles.GetValidDayProfilesForMeterReading(null, null));
            
            Assert.ThrowsException<ArgumentNullException>(() => dayProfiles.GetValidDayProfilesForMeterReading(null, tariffStages));

            Assert.ThrowsException<ArgumentNullException>(() => dayProfiles.GetValidDayProfilesForMeterReading(oc1, null));

            Assert.ThrowsException<ArgumentException>(() => dayProfiles.GetValidDayProfilesForMeterReading(oc1, new List<TariffStage>()));

            var dayProfilesEmpty = new List<DayProfile>();

            Assert.ThrowsException<ArgumentException>(() => dayProfilesEmpty.GetValidDayProfilesForMeterReading(oc1, tariffStages));
        }
    }
}
