namespace TRuDI.Models.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TRuDI.Models;

    [TestClass]
    public class ObisIdExtensionsTests
    {
        [TestMethod]
        public void TestGetLabel()
        {
            Assert.AreEqual("Elektrische Wirkarbeit Bezug Gesamt", new ObisId("1-0:1.8.0").GetLabel());
            Assert.AreEqual("Elektrische Wirkarbeit Lieferung Gesamt", new ObisId("1-0:2.8.0").GetLabel());
            Assert.AreEqual("Frequenz", new ObisId("1-0:14.7.0").GetLabel());
            Assert.AreEqual("Spannung L1", new ObisId("1-0:32.7.0").GetLabel());
            Assert.AreEqual("Spannung L2", new ObisId("1-0:52.7.0").GetLabel());
            Assert.AreEqual("Spannung L3", new ObisId("1-0:72.7.0").GetLabel());
            Assert.AreEqual("Strom L1", new ObisId("1-0:31.7.0").GetLabel());
            Assert.AreEqual("Strom L2", new ObisId("1-0:51.7.0").GetLabel());
            Assert.AreEqual("Strom L3", new ObisId("1-0:71.7.0").GetLabel());
            Assert.AreEqual("Wirkleistung Verbrauch", new ObisId("1-0:16.7.0").GetLabel());
        }
    }
}
