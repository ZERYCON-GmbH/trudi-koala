namespace TRuDI.Models.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TRuDI.Models;
    using TRuDI.Models.BasicData;

    [TestClass]
    public class UnitExtensionsTests
    {
        [TestMethod]
        public void TestGetSiPrefix()
        {
            Assert.AreEqual("G", PowerOfTenMultiplier.Giga.GetSiPrefix());
            Assert.AreEqual("M", PowerOfTenMultiplier.Mega.GetSiPrefix());
            Assert.AreEqual("k", PowerOfTenMultiplier.kilo.GetSiPrefix());
        }

        [TestMethod]
        public void TestGetUnitSymbol()
        {
            Assert.AreEqual("Wh", Uom.Real_energy.GetUnitSymbol());
            Assert.AreEqual("m³", Uom.Cubic_meter.GetUnitSymbol());
        }

        [TestMethod]
        public void TestGetDisplayUnit()
        {
            Assert.AreEqual(string.Empty, Uom.Not_Applicable.GetDisplayUnit(PowerOfTenMultiplier.None));
            Assert.AreEqual("kWh", Uom.Real_energy.GetDisplayUnit(PowerOfTenMultiplier.None));
            Assert.AreEqual("kWh", Uom.Real_energy.GetDisplayUnit(PowerOfTenMultiplier.kilo));
            Assert.AreEqual("MWh", Uom.Real_energy.GetDisplayUnit(PowerOfTenMultiplier.Mega));
            Assert.AreEqual("m³", Uom.Cubic_meter.GetDisplayUnit(PowerOfTenMultiplier.None));
        }

        [TestMethod]
        public void TestGetDisplayValue()
        {
            long value = 123456789;

            Assert.AreEqual("12.345,6789", value.GetDisplayValue(Uom.Real_energy, PowerOfTenMultiplier.None, -1));
            Assert.AreEqual("12.345.678,9", value.GetDisplayValue(Uom.Real_energy, PowerOfTenMultiplier.kilo, -1));
            Assert.AreEqual("12.345,6789", value.GetDisplayValue(Uom.Real_energy, PowerOfTenMultiplier.kilo, -4));
            Assert.AreEqual("12.345.678,9", value.GetDisplayValue(Uom.Cubic_meter, PowerOfTenMultiplier.None, -1));

            Assert.AreEqual("12.345.678.900,00", value.GetDisplayValue(Uom.Real_energy, PowerOfTenMultiplier.kilo, 2));
        }
    }
}
