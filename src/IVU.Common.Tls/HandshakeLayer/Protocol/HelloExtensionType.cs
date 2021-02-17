namespace IVU.Common.Tls.HandshakeLayer.Protocol
{
	/* http://www.iana.org/assignments/tls-extensiontype-values/tls-extensiontype-values.xml */
	public enum HelloExtensionType
	{
		ServerName            = 0,
		MaxFragmentLength     = 1,
		ClientCertificateUrl  = 2,
		TrustedCaKeys         = 3,
		TruncatedHmac         = 4,
		StatusRequest         = 5,
		UserMapping           = 6,
		ClientAuthz           = 7,
		ServerAuthz           = 8,
		CertType              = 9,
		EllipticCurves        = 10,
		EcPointFormats        = 11,
		Srp                   = 12,
		SignatureAlgorithms   = 13,
		UseSrtp               = 14,
		SessionTicket         = 35,
		RenegotiationInfo     = 65281
	}
}
