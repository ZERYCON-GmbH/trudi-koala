namespace TRuDI.Models.Tests
{
    using System.Xml.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class XmlModelParserTests
    {
        [TestMethod]
        [DeploymentItem(@"Data\UsagePoint_with_AnalysisProfile.xml")]
        public void TestUsagePointWithAnalysisProfile()
        {
            var xml = XDocument.Load(@"Data\UsagePoint_with_AnalysisProfile.xml");
            var model = XmlModelParser.ParseHanAdapterModel(xml.Root.Descendants());

            Assert.IsNotNull(model.AnalysisProfile);
        }

        [TestMethod]
        [DeploymentItem(@"Data\UsagePoint_without_AnalysisProfile.xml")]
        public void TestUsagePointWithoutAnalysisProfile()
        {
            var xml = XDocument.Load(@"Data\UsagePoint_without_AnalysisProfile.xml");
            var model = XmlModelParser.ParseHanAdapterModel(xml.Root.Descendants());

            Assert.IsNull(model.AnalysisProfile);
        }
    }
}
