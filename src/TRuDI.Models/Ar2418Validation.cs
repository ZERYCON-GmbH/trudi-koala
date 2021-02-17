namespace TRuDI.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;

    using TRuDI.Models.Logging;

    public static class Ar2418Validation
    {
        private static ILog logger = LogProvider.GetCurrentClassLogger();

        public static void ValidateSchema(XDocument document)
        {
            var schemas = new XmlSchemaSet();
            schemas.XmlResolver = new XmlResolver();
            schemas.Add("http://vde.de/AR_2418-6.xsd", XmlReader.Create(Assembly.GetExecutingAssembly().GetManifestResourceStream($"TRuDI.Models.Schemata.AR_2418-6.xsd"), new XmlReaderSettings() { DtdProcessing = DtdProcessing.Parse }));
            var exceptions = new List<Exception>();

            document.Validate(
                schemas,
                (sender, eventArgs) =>
                {
                    logger.Error("Validation failed: {0}", eventArgs.Message);
                    exceptions.Add(new ApplicationException($"{eventArgs.Message}"));
                });

            if (exceptions.Any())
            {
                throw new AggregateException("Schema error:>", exceptions);
            }
        }
    }
}
