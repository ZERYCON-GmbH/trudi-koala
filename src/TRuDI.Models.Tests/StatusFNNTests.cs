namespace TRuDI.Models.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TRuDI.Models.BasicData;

    [TestClass]
    public class StatusFNNTests
    {
        [TestMethod]
        public void TestMapToStatusPtb()
        {
            var target = new StatusFNN(0x210500000004);
            Assert.AreEqual(BzStatusWord.BzStatusWordIdentification, target.BzStatusWord);
            Assert.AreEqual(SmgwStatusWord.SmgwStatusWordIdentification | SmgwStatusWord.Fatal_Error | SmgwStatusWord.PTB_Temp_Error_signed_invalid, target.SmgwStatusWord);
            Assert.AreEqual("A084000020000000", target.Status);
            Assert.AreEqual(StatusPTB.FatalError, target.MapToStatusPtb());

            target = new StatusFNN(0x200500000004);
            Assert.AreEqual(BzStatusWord.BzStatusWordIdentification, target.BzStatusWord);
            Assert.AreEqual(SmgwStatusWord.SmgwStatusWordIdentification | SmgwStatusWord.PTB_Temp_Error_signed_invalid, target.SmgwStatusWord);
            Assert.AreEqual("A004000020000000", target.Status);
            Assert.AreEqual(StatusPTB.TemporaryError, target.MapToStatusPtb());

            target = new StatusFNN(0x0500000004);
            Assert.AreEqual(BzStatusWord.BzStatusWordIdentification, target.BzStatusWord);
            Assert.AreEqual(SmgwStatusWord.SmgwStatusWordIdentification, target.SmgwStatusWord);
            Assert.AreEqual("A000000020000000", target.Status);
            Assert.AreEqual(StatusPTB.NoError, target.MapToStatusPtb());

            target = new StatusFNN(0x100500000004);
            Assert.AreEqual(BzStatusWord.BzStatusWordIdentification, target.BzStatusWord);
            Assert.AreEqual(SmgwStatusWord.SmgwStatusWordIdentification | SmgwStatusWord.PTB_Warning, target.SmgwStatusWord);
            Assert.AreEqual("A008000020000000", target.Status);
            Assert.AreEqual(StatusPTB.Warning, target.MapToStatusPtb());

            target = new StatusFNN(0x400500000004);
            Assert.AreEqual(BzStatusWord.BzStatusWordIdentification, target.BzStatusWord);
            Assert.AreEqual(SmgwStatusWord.SmgwStatusWordIdentification | SmgwStatusWord.PTB_Temp_Error_is_invalid, target.SmgwStatusWord);
            Assert.AreEqual("A002000020000000", target.Status);
            Assert.AreEqual(StatusPTB.CriticalTemporaryError, target.MapToStatusPtb());
        }

        [TestMethod]
        public void TestMapToStatusPtb2()
        {
            var target = new StatusFNN("A084000020000000");
            Assert.AreEqual(BzStatusWord.BzStatusWordIdentification, target.BzStatusWord);
            Assert.AreEqual(SmgwStatusWord.SmgwStatusWordIdentification | SmgwStatusWord.Fatal_Error | SmgwStatusWord.PTB_Temp_Error_signed_invalid, target.SmgwStatusWord);
            Assert.AreEqual("A084000020000000", target.Status);
            Assert.AreEqual(StatusPTB.FatalError, target.MapToStatusPtb());

            target = new StatusFNN("A004000020000000");
            Assert.AreEqual(BzStatusWord.BzStatusWordIdentification, target.BzStatusWord);
            Assert.AreEqual(SmgwStatusWord.SmgwStatusWordIdentification | SmgwStatusWord.PTB_Temp_Error_signed_invalid, target.SmgwStatusWord);
            Assert.AreEqual("A004000020000000", target.Status);
            Assert.AreEqual(StatusPTB.TemporaryError, target.MapToStatusPtb());

            target = new StatusFNN("A000000020000000");
            Assert.AreEqual(BzStatusWord.BzStatusWordIdentification, target.BzStatusWord);
            Assert.AreEqual(SmgwStatusWord.SmgwStatusWordIdentification, target.SmgwStatusWord);
            Assert.AreEqual("A000000020000000", target.Status);
            Assert.AreEqual(StatusPTB.NoError, target.MapToStatusPtb());

            target = new StatusFNN("A008000020000000");
            Assert.AreEqual(BzStatusWord.BzStatusWordIdentification, target.BzStatusWord);
            Assert.AreEqual(SmgwStatusWord.SmgwStatusWordIdentification | SmgwStatusWord.PTB_Warning, target.SmgwStatusWord);
            Assert.AreEqual("A008000020000000", target.Status);
            Assert.AreEqual(StatusPTB.Warning, target.MapToStatusPtb());

            target = new StatusFNN("A002000020000000");
            Assert.AreEqual(BzStatusWord.BzStatusWordIdentification, target.BzStatusWord);
            Assert.AreEqual(SmgwStatusWord.SmgwStatusWordIdentification | SmgwStatusWord.PTB_Temp_Error_is_invalid, target.SmgwStatusWord);
            Assert.AreEqual("A002000020000000", target.Status);
            Assert.AreEqual(StatusPTB.CriticalTemporaryError, target.MapToStatusPtb());
        }
    }
}
