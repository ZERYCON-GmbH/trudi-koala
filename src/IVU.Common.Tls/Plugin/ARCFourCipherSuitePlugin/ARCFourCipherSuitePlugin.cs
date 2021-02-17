namespace IVU.Common.Tls.Plugin.ARCFourCipherSuitePlugin
{
    using IVU.Common.Tls.Logging;
    using IVU.Common.Tls.Plugin.CipherSuitePluginInterface;

    public class ARCFourCipherSuitePlugin : CipherSuitePlugin
	{
        public override string PluginName
		{
			get { return "ARCFour cipher plugin"; }
		}

		public override string[] SupportedBulkCipherAlgorithms
		{
			get { return new string[] { "RC4_128" }; }
		}

		public override BulkCipherAlgorithm GetBulkCipherAlgorithm(string name)
		{
			if (name.Equals("RC4_128")) {
				return new BulkCipherAlgorithmARCFour(128);
			} else {
				return null;
			}
		}
	}
}
