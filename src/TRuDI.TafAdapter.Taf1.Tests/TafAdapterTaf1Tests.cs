using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Linq;


using TRuDI.Models;
using TRuDI.Models.CheckData;
using TRuDI.TafAdapter.Interface;

namespace TRuDI.TafAdapter.Taf1.Tests
{
    [TestClass]
    public class TafAdapterTaf1Tests
    {

        /// <summary>
        /// Calculates a billing period of 1 month.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month.xml")]
        [DeploymentItem(@"Data\supplier_1_month.xml")]
        public void Test1Month()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_month.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf1();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-10-01T00:00:00+02:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf1Data;

            Assert.AreEqual(1, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(29800, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);
        }


        /// <summary>
        /// Calculates a billing period of 2 month.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_2_month.xml")]
        [DeploymentItem(@"Data\supplier_2_month.xml")]
        public void Test2Month()
        {
            var deviceXml = XDocument.Load(@"Data\result_2_month.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_2_month.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf1();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-09-01T00:00:00+02:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf1Data;

            Assert.AreEqual(1, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(58600, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);
        }

        /// <summary>
        /// Calculates a billing perid of 3 month.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_3_month.xml")]
        [DeploymentItem(@"Data\supplier_3_month.xml")]
        public void Test3Month()
        {
            var deviceXml = XDocument.Load(@"Data\result_3_month.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_3_month.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf1();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-08-01T00:00:00+02:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf1Data;

            Assert.AreEqual(1, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(88360, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);
        }

        /// <summary>
        /// Calculates a billing period of 6 month.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_6_month.xml")]
        [DeploymentItem(@"Data\supplier_6_month.xml")]
        public void Test6Month()
        {
            var deviceXml = XDocument.Load(@"Data\result_6_month.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_6_month.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf1();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-05-01T00:00:00+02:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf1Data;

            Assert.AreEqual(1, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(176680, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);
        }

        /// <summary>
        /// Calculates a billing period of 12 month.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_12_month.xml")]
        [DeploymentItem(@"Data\supplier_12_month.xml")]
        public void Test12Month()
        {
            var deviceXml = XDocument.Load(@"Data\result_12_month.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_12_month.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf1();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2016-11-01T00:00:00+01:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf1Data;

            Assert.AreEqual(1, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(350400, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);
        }

        /// <summary>
        /// Calculates a billing period of 1 month with 2 original value lists. 
        /// Every original value list has its own Register due to different obis ids.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month_2_ovl.xml")]
        [DeploymentItem(@"Data\supplier_1_month_2_DayProfiles.xml")]
        public void Test1Month_2_Ovl()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month_2_ovl.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_month_2_DayProfiles.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf1();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-10-01T00:00:00+02:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf1Data;

            Assert.AreEqual(2, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(29800, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual("1-0:2.8.1*255", data.SummaryRegister[1].ObisCode.ToString());
            Assert.AreEqual(27800, data.SummaryRegister[1].Amount);
            Assert.AreEqual(2, data.SummaryRegister[1].TariffId);
        }

        /// <summary>
        /// Calculates a billing period of 1 month with 2 original value lists.
        /// There is just one Obis Id. So the result is
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month_2_ovl_sameObisId.xml")]
        [DeploymentItem(@"Data\supplier_1_month.xml")]
        public void Test1Month_2_Ovl_sameObisId()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month_2_ovl_sameObisId.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_month.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf1();
            var result = target.Calculate(deviceModel, supplierModel);

            Assert.AreEqual("2017-10-01T00:00:00+02:00", result.Data.Begin.ToString("yyyy-MM-ddTHH:mm:ssK"));
            Assert.AreEqual("2017-11-01T00:00:00+01:00", result.Data.End.ToString("yyyy-MM-ddTHH:mm:ssK"));

            var data = result.Data as Taf1Data;

            Assert.AreEqual(1, data.SummaryRegister.Count);
            Assert.AreEqual("1-0:1.8.1*255", data.SummaryRegister[0].ObisCode.ToString());
            Assert.AreEqual(57600, data.SummaryRegister[0].Amount);
            Assert.AreEqual(1, data.SummaryRegister[0].TariffId);

            Assert.AreEqual(2, data.AccountingSections.Count);
            Assert.AreEqual(29800, data.AccountingSections[0].MeasuringRanges[0].Amount);
            Assert.AreEqual(27800, data.AccountingSections[1].MeasuringRanges[0].Amount);
        }

        /// <summary>
        /// This exception is thrown if no original value list is available.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month_no_ovl.xml")]
        [DeploymentItem(@"Data\supplier_1_month.xml")]
        public void TestTaf1ExceptionNoOVL()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month_no_ovl.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_month.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf1();

            var ex = Assert.ThrowsException<InvalidOperationException>(() => target.Calculate(deviceModel, supplierModel));
            Assert.AreEqual("Es ist keine originäre Messwertliste verfügbar.", ex.Message);
        }

        /// <summary>
        /// This exception is thrown if more then 3 original value lists are in the xml result.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month_4_ovl.xml")]
        [DeploymentItem(@"Data\supplier_1_month.xml")]
        public void TestTaf1ExceptionTooManyOVL()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month_4_ovl.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_month.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf1();

            var ex = Assert.ThrowsException<InvalidOperationException>(() => target.Calculate(deviceModel, supplierModel));
            Assert.AreEqual("Es werden maximal 3 originäre Messwertlisten unterstützt.", ex.Message);
        }

        /// <summary>
        /// This exception is thrown if one of the meter reading objects are not an original value list.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month_1_ovl_1_meterReading.xml")]
        [DeploymentItem(@"Data\supplier_1_month.xml")]
        public void TestTaf1ExceptionAMeterReadingIsNotAnOVL()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month_1_ovl_1_meterReading.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_month.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf1();

            var ex = Assert.ThrowsException<InvalidOperationException>(() => target.Calculate(deviceModel, supplierModel));
            Assert.AreEqual("Es sind nur originäre Messwertlisten zulässig.", ex.Message);
        }

        /// <summary>
        /// This exceptin is thrown if too much tariff stages are in the supplier file.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month.xml")]
        [DeploymentItem(@"Data\supplier_1_month_2_DayProfiles.xml")]
        public void TestTaf1ExceptionToMuchTariffStages()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_month_2_DayProfiles.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf1();

            var ex = Assert.ThrowsException<InvalidOperationException>(() => target.Calculate(deviceModel, supplierModel));
            Assert.AreEqual("Die Anzahl der Tarifstufen darf die Anzahl der originären Messwertlisten nicht überschreiten.", ex.Message);
        }

        /// <summary>
        /// This exception is thrown if the start of the billing period is not at the beginning of a month.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month.xml")]
        [DeploymentItem(@"Data\supplier_1_month_start_at_10_2.xml")]
        public void TestTaf1ExceptionInvalidBillingPeriodStart()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_month_start_at_10_2.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf1();

            var start = supplierModel.AnalysisProfile.BillingPeriod.Start;

            var ex = Assert.ThrowsException<InvalidOperationException>(() => target.Calculate(deviceModel, supplierModel));
            Assert.AreEqual($"Die Abrechnungsperiode {start} startet nicht am Monatsanfang.", ex.Message);
        }

        /// <summary>
        /// This exception is thrown if the duration of the billing period are invalid.
        /// Allowed billing periods are 1, 2, 3, 6, or 12 months.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month.xml")]
        [DeploymentItem(@"Data\supplier_InvalidBillingPeriod1.xml")]
        public void TestTaf1ExceptionInvalidBillingPeriod1()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_InvalidBillingPeriod1.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf1();

            var start = supplierModel.AnalysisProfile.BillingPeriod.Start;
            var end = supplierModel.AnalysisProfile.BillingPeriod.GetEnd();

            var ex = Assert.ThrowsException<InvalidOperationException>(() => target.Calculate(deviceModel, supplierModel));
            Assert.AreEqual($"Die angegebene Abrechnungsperiode von {(end - start).TotalDays} Tagen ist ungültigt. Unterstütz werden 1, 2, 3, 6 oder 12 Monate."
                , ex.Message);
        }

        /// <summary>
        /// This exception is thrown if the duration of the billing period are invalid.
        /// Allowed billing periods are 1, 2, 3, 6, or 12 months.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month.xml")]
        [DeploymentItem(@"Data\supplier_InvalidBillingPeriod2.xml")]
        public void TestTaf1ExceptionInvalidBillingPeriod2()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_InvalidBillingPeriod2.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf1();

            var start = supplierModel.AnalysisProfile.BillingPeriod.Start;
            var end = supplierModel.AnalysisProfile.BillingPeriod.GetEnd();

            var ex = Assert.ThrowsException<InvalidOperationException>(() => target.Calculate(deviceModel, supplierModel));
            Assert.AreEqual($"Die angegebene Abrechnungsperiode von {(end - start).TotalDays} Tagen ist ungültigt. Unterstütz werden 1, 2, 3, 6 oder 12 Monate."
                , ex.Message);
        }

        /// <summary>
        /// This exception is thrown if no start IntervalReading was found.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month_without_start.xml")]
        [DeploymentItem(@"Data\supplier_1_month.xml")]
        public void TestTaf1ExceptionNoStartValueFound()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month_without_start.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_month.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf1();

            var start = supplierModel.AnalysisProfile.BillingPeriod.Start;

            var ex = Assert.ThrowsException<InvalidOperationException>(() => target.Calculate(deviceModel, supplierModel));
            Assert.AreEqual($"Zu dem Zeitpunkt {start} ist kein Wert vorhanden oder der Status kritisch oder fatal.", ex.Message);
        }

        /// <summary>
        /// This exception is thrown if no end IntervalReading was found.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month_without_end.xml")]
        [DeploymentItem(@"Data\supplier_1_month.xml")]
        public void TestTaf1ExceptionNoEndValueFound()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month_without_end.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_month.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf1();

            var end = supplierModel.AnalysisProfile.BillingPeriod.GetEnd();

            var ex = Assert.ThrowsException<InvalidOperationException>(() => target.Calculate(deviceModel, supplierModel));
            Assert.AreEqual($"Zu dem Zeitpunkt {end} ist kein Wert vorhanden oder der Status kritisch oder fatal.", ex.Message);
        }

        /// <summary>
        /// This exception is thrown if the PTB status if the start value is 3(time critcal) or 4(fatal).
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month_start_statusPTB4.xml")]
        [DeploymentItem(@"Data\supplier_1_month.xml")]
        public void TestTaf1ExceptionStartValuePTBStatus4()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month_start_statusPTB4.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_month.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf1();

            var start = supplierModel.AnalysisProfile.BillingPeriod.Start;

            var ex = Assert.ThrowsException<InvalidOperationException>(() => target.Calculate(deviceModel, supplierModel));
            Assert.AreEqual($"Zu dem Zeitpunkt {start} ist kein Wert vorhanden oder der Status kritisch oder fatal.", ex.Message);
        }

        /// <summary>
        /// This exception is thrown if the PTB status if the end value is 3(time critcal) or 4(fatal).
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month_end_statusPTB4.xml")]
        [DeploymentItem(@"Data\supplier_1_month.xml")]
        public void TestTaf1ExceptionEndValuePTBStatus4()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month_end_statusPTB4.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_month.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf1();

            var end = supplierModel.AnalysisProfile.BillingPeriod.GetEnd();

            var ex = Assert.ThrowsException<InvalidOperationException>(() => target.Calculate(deviceModel, supplierModel));
            Assert.AreEqual($"Zu dem Zeitpunkt {end} ist kein Wert vorhanden oder der Status kritisch oder fatal.", ex.Message);
        }



        /// <summary>
        /// This exception is thrown if the number of the special day profiles are invalid.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Data\result_1_month.xml")]
        [DeploymentItem(@"Data\supplier_1_month_plus_1_day.xml")]
        public void TestTaf1ExceptionInvalidSpecialDayProfilesCount()
        {
            var deviceXml = XDocument.Load(@"Data\result_1_month.xml");
            var deviceModel = XmlModelParser.ParseHanAdapterModel(deviceXml.Root.Descendants());

            var supplierXml = XDocument.Load(@"Data\supplier_1_month_plus_1_day.xml");
            var supplierModel = XmlModelParser.ParseSupplierModel(supplierXml.Root.Descendants());

            var target = new TafAdapterTaf1();

            var start = supplierModel.AnalysisProfile.BillingPeriod.Start;
            var end = supplierModel.AnalysisProfile.BillingPeriod.GetEnd();
            var days = (int)(end - start).TotalDays;

            var ex = Assert.ThrowsException<InvalidOperationException>(() => target.Calculate(deviceModel, supplierModel));
            Assert.AreEqual($"Die Anzahl der SpecialDayProfile Objekte muss einem vielfachen von { days} entsprechen.", ex.Message);
        }
    }
}
