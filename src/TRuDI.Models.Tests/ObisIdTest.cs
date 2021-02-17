namespace TRuDI.Models.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TRuDI.Models;

    [TestClass]
    public class ObisIdTests
    {
        [TestMethod]
        public void TestObisId()
        {
            var target = new ObisId("0100010800ff");
            Assert.AreEqual(1, target.A);
            Assert.AreEqual(0, target.B);
            Assert.AreEqual(1, target.C);
            Assert.AreEqual(8, target.D);
            Assert.AreEqual(0, target.E);
            Assert.AreEqual(255, target.F);

            Assert.AreEqual("1-0:1.8.0*255", target.ToString());
        }

        [TestMethod]
        public void TestObisId2()
        {
            var target = new ObisId("1-0:1.8.0*255");
            Assert.AreEqual(1, target.A);
            Assert.AreEqual(0, target.B);
            Assert.AreEqual(1, target.C);
            Assert.AreEqual(8, target.D);
            Assert.AreEqual(0, target.E);
            Assert.AreEqual(255, target.F);

            Assert.AreEqual("1-0:1.8.0*255", target.ToString());
        }

        [TestMethod]
        public void TestObisId3()
        {
            var target = new ObisId("1-0:1.8.0");
            Assert.AreEqual(1, target.A);
            Assert.AreEqual(0, target.B);
            Assert.AreEqual(1, target.C);
            Assert.AreEqual(8, target.D);
            Assert.AreEqual(0, target.E);
            Assert.AreEqual(255, target.F);

            Assert.AreEqual("1-0:1.8.0*255", target.ToString());
        }

        [TestMethod]
        public void TestObisId4()
        {
            var a = new ObisId("1-0:1.8.0*254");
            var b = new ObisId(a);

            Assert.AreEqual(1, b.A);
            Assert.AreEqual(0, b.B);
            Assert.AreEqual(1, b.C);
            Assert.AreEqual(8, b.D);
            Assert.AreEqual(0, b.E);
            Assert.AreEqual(254, b.F);

            Assert.AreEqual("1-0:1.8.0*254", b.ToString());
        }

        [TestMethod]
        public void TestObisId5()
        {
            Assert.ThrowsException<ArgumentException>(() => new ObisId("xxxx"));
        }

        [TestMethod]
        public void TestObisIdEquals()
        {
            var a = new ObisId("1-0:1.8.0*255");
            var b = new ObisId("1-0:1.8.0*255");
            var c = new ObisId("1-0:1.8.1*255");

            var d = a;

            Assert.IsTrue(a == b);
            Assert.IsTrue(a == d);
            Assert.IsFalse(a == c);
            Assert.IsTrue(a != c);
            Assert.IsFalse(a != b);

            Assert.IsTrue(a == "1-0:1.8.0*255");
            Assert.IsFalse(a == "1-0:1.8.1*255");
            Assert.IsFalse(a == "xxxxx");
            Assert.IsTrue(a != "xxxxx");

            Assert.IsTrue("1-0:1.8.0*255" == a);
            Assert.IsFalse("1-0:1.8.1*255" == a);

            Assert.IsFalse(a.Equals("test"));
            Assert.IsFalse(a.Equals(null));
            Assert.IsTrue(a.Equals(b));
            Assert.IsFalse(a.Equals(c));
            Assert.IsTrue(a.Equals(a));
            Assert.IsTrue(a.Equals(d));
        }
    }
}
