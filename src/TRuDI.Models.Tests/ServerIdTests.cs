namespace TRuDI.Models.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TRuDI.HanAdapter.Interface;
    using TRuDI.Models;

    [TestClass]
    public class ServerIdTests
    {
        [TestMethod]
        public void TestServerId1()
        {
            var target = new ServerId("EPPC0210486901");
            Assert.AreEqual(ObisMedium.Communication, target.Medium);
            Assert.AreEqual("PPC", target.FlagId);
            Assert.AreEqual(2, target.ProductionBlock);
            Assert.AreEqual(10486901u, target.Number);

            Assert.AreEqual("E PPC 02 10486901", target.ToString());
            Assert.AreEqual("EPPC0210486901", target.ToStringWithoutSpace());
            Assert.AreEqual("0A0E5050430200A00475", target.ToHexString());
        }

        [TestMethod]
        public void TestServerId2()
        {
            var target = new ServerId("0A01454D48000051971E");
            Assert.AreEqual(ObisMedium.Electricity, target.Medium);
            Assert.AreEqual("EMH", target.FlagId);
            Assert.AreEqual(0, target.ProductionBlock);
            Assert.AreEqual(5347102u, target.Number);

            Assert.AreEqual("1 EMH 00 05347102", target.ToString());
            Assert.AreEqual("1EMH0005347102", target.ToStringWithoutSpace());
            Assert.AreEqual("0A01454D48000051971E", target.ToHexString());
        }

        [TestMethod]
        public void TestServerId3()
        {
            var target = new ServerId("E PPC 02 10486901");
            Assert.AreEqual(ObisMedium.Communication, target.Medium);
            Assert.AreEqual("PPC", target.FlagId);
            Assert.AreEqual(2, target.ProductionBlock);
            Assert.AreEqual(10486901u, target.Number);

            Assert.AreEqual("E PPC 02 10486901", target.ToString());
            Assert.AreEqual("EPPC0210486901", target.ToStringWithoutSpace());
            Assert.AreEqual("0A0E5050430200A00475", target.ToHexString());
        }

        [TestMethod]
        public void TestServerIdLowerCase()
        {
            var target = new ServerId("e ppc 02 10486901");
            Assert.AreEqual(ObisMedium.Communication, target.Medium);
            Assert.AreEqual("PPC", target.FlagId);
            Assert.AreEqual(2, target.ProductionBlock);
            Assert.AreEqual(10486901u, target.Number);

            Assert.AreEqual("E PPC 02 10486901", target.ToString());
            Assert.AreEqual("EPPC0210486901", target.ToStringWithoutSpace());
            Assert.AreEqual("0A0E5050430200A00475", target.ToHexString());
        }

        [TestMethod]
        public void TestServerIdInvalid()
        {
            var target = new ServerId("irgendwas");
            Assert.IsFalse(target.IsValid);
            Assert.AreEqual("irgendwas", target.ToString());
            Assert.AreEqual("irgendwas", target.ToHexString());
            Assert.AreEqual("irgendwas", target.ToStringWithoutSpace());
        }

        [TestMethod]
        public void TestServerIdWiredMBus()
        {
            var target = new ServerId("0224010523AC484104");
            Assert.IsTrue(target.IsValid);
            Assert.AreEqual(ObisMedium.Heat, target.Medium);
            Assert.AreEqual("REL", target.FlagId);
            Assert.AreEqual(23050124u, target.Number);
            Assert.AreEqual("REL 23050124", target.ToString());
            Assert.AreEqual("0224010523AC484104", target.ToHexString());
        }

        [TestMethod]
        public void TestServerIdWirelessMBus()
        {
            var target = new ServerId("01E230197600150003");
            Assert.IsTrue(target.IsValid);
            Assert.AreEqual(ObisMedium.Gas, target.Medium);
            Assert.AreEqual("LGB", target.FlagId);
            Assert.AreEqual(15007619u, target.Number);
            Assert.AreEqual("LGB 15007619", target.ToString());
            Assert.AreEqual("01E230197600150003", target.ToHexString());
        }

        [TestMethod]
        public void TestServerIdDin_43863_5_2010_7()
        {
            var target = new ServerId("090E4d5543AB00bc614e");
            Assert.IsTrue(target.IsValid);
            Assert.AreEqual(ObisMedium.Communication, target.Medium);
            Assert.AreEqual("MUC", target.FlagId);
            Assert.AreEqual(12345678u, target.Number);
            Assert.AreEqual("E MUC AB 12345678", target.ToString());
            Assert.AreEqual("090E4D5543AB00BC614E", target.ToHexString());
        }

        [TestMethod]
        public void TestServerIdDin_43863_5_2010_2()
        {
            var target = new ServerId("064d554301001dd8594E");
            Assert.IsTrue(target.IsValid);
            Assert.AreEqual(ObisMedium.Electricity, target.Medium);
            Assert.AreEqual("MUC", target.FlagId);
            Assert.AreEqual(12345678u, target.Number);
            Assert.AreEqual("1 MUC 10 00 12345678", target.ToString());
            Assert.AreEqual("064D554301001DD8594E", target.ToHexString());
        }


        [TestMethod]
        public void TestServerWMBusWithoutTypeLgb()
        {
            var target = new ServerId("e230197600150003");
            Assert.IsTrue(target.IsValid);
            Assert.AreEqual(ObisMedium.Gas, target.Medium);
            Assert.AreEqual("LGB", target.FlagId);
            Assert.AreEqual(15007619u, target.Number);
            Assert.AreEqual("LGB 15007619", target.ToString());
            Assert.AreEqual("E230197600150003", target.ToHexString());
            Assert.AreEqual(ServerId.ServerIdType.WirelessMBusAddress, target.Type);
        }

        [TestMethod]
        public void TestServerWMBusWithoutTypeEsy()
        {
            var target = new ServerId("7916802852601001");
            Assert.IsTrue(target.IsValid);
            Assert.AreEqual(ObisMedium.Abstract, target.Medium);
            Assert.AreEqual("ESY", target.FlagId);
            Assert.AreEqual(60522880u, target.Number);
            Assert.AreEqual("ESY 60522880", target.ToString());
            Assert.AreEqual("7916802852601001", target.ToHexString());
            Assert.AreEqual(ServerId.ServerIdType.WirelessMBusAddress, target.Type);
        }
    }
}
