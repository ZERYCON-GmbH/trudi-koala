namespace IVU.Common.Tls.HandshakeLayer.Protocol
{
	public enum HandshakeMessageType
	{
		HelloRequest        = 0,
		ClientHello         = 1,
		ServerHello         = 2,
		HelloVerifyRequest  = 3, // RFC 4347
		NewSessionTicket    = 4, // RFC 4507
		Certificate         = 11,
		ServerKeyExchange   = 12,
		CertificateRequest  = 13,
		ServerHelloDone     = 14,
		CertificateVerify   = 15,
		ClientKeyExchange   = 16,
		Finished            = 20
	}
}
