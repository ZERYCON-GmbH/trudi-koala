namespace TRuDI.Models.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using TRuDI.HanAdapter.Interface;

    [TestClass]
    public class BillingPeriodExtensionsTests
    {
        [TestMethod]
        public void TestToFormatedString()
        {
            var target = new BillingPeriod()
            {
                Begin = new DateTime(2017, 1, 2, 3, 4, 5, DateTimeKind.Local),
                End = null
            };

            Assert.IsFalse(target.IsCompleted());

            target.End = DateTime.Now.AddYears(1);
            Assert.IsFalse(target.IsCompleted());

            target.End = DateTime.Now.AddDays(-3);
            Assert.IsTrue(target.IsCompleted());
        }
    }
}
