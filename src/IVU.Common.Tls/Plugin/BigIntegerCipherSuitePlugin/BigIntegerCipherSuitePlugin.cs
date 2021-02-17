using IVU.Common.Tls.Plugin.CipherSuitePluginInterface;

namespace IVU.Common.Tls.Plugin.BigIntegerCipherSuitePlugin
{
    using IVU.Common.Tls.Plugin.CipherSuitePluginInterface;

    public class BigIntegerCipherSuitePlugin : CipherSuitePlugin
	{
        public override string PluginName
		{
			get { return "BigInteger cipher suite plugin (DH and RSA signature)"; }
		}

		public override string[] SupportedKeyExchangeAlgorithms
		{
			get { return new string[] { "DHE" }; }
		}

		public override string[] SupportedSignatureAlgorithms
		{
			get { return new string[] { "RSA" }; }
		}

		public override KeyExchangeAlgorithm GetKeyExchangeAlgorithm(string name)
		{
			if (name.Equals("DHE")) {
				return new KeyExchangeAlgorithmDHE();
			} else {
				return null;
			}
		}

		public override SignatureAlgorithm GetSignatureAlgorithm(string name)
		{
			if (name.Equals("RSA")) {
				return new SignatureAlgorithmRSA();
			} else {
				return null;
			}
		}
	}
}
