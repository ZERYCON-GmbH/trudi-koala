namespace TRuDI.Models
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Xml;

    using TRuDI.Models.Logging;

    public class XmlResolver : XmlUrlResolver
    {
        private readonly ILog logger = LogProvider.For<XmlResolver>();

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            var xsd = Path.GetFileName(absoluteUri.LocalPath);

            switch (xsd)
            {
                case "xml.xsd":
                    return this.ReadFromResource("xml.xsd");

                case "atom.xsd":
                    return this.ReadFromResource("atom.xsd");

                case "espi_derived.xsd":
                    return this.ReadFromResource("espi_derived.xsd");

                default:
                    if (absoluteUri.AbsolutePath.Contains("AR_2418-6_V0.96"))
                    {
                        return this.ReadFromResource("AR_2418-6.xsd");
                    }

                    this.logger.Error("XSD file not found: {0}" + absoluteUri);
                    break;
            }

            return base.GetEntity(absoluteUri, role, ofObjectToReturn);
        }

        private Stream ReadFromResource(string filename)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream($"TRuDI.Models.Schemata.{filename}");
        }
    }
}
