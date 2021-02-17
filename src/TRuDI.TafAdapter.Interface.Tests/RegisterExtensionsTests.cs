namespace TRuDI.TafAdapter.Interface.Tests
{
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TRuDI.Models;
    using TRuDI.TafAdapter.Interface.Taf2;

    [TestClass]
    public class RegisterExtensionsTests
    {
        [TestMethod]
        public void TestGetAccountingRegistersWithTotal()
        {
            var target = new List<Register>
                             {
                                 new Register
                                     {
                                         Amount = 10,
                                         ObisCode = new ObisId("1-0:1.8.1*255"),
                                         TariffId = 1
                                     },
                                 new Register
                                     {
                                         Amount = 15,
                                         ObisCode = new ObisId("1-0:1.8.2*255"),
                                         TariffId = 1
                                     }
                             };

            var res = target.GetAccountingRegistersWithTotal();
            Assert.AreEqual(3, res.Count);
            Assert.AreEqual(25, res[2].Amount);
        }
    }
}
