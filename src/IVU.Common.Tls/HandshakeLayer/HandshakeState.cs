namespace IVU.Common.Tls.HandshakeLayer
{
	public enum HandshakeState
	{
		// Initial handshake state
		Initial,

		// Server handshake states
		ReceivedClientHello,
		ReceivedClientKeyExchange,
		ReceivedCertificateVerify,

		// Client handshake states
		ReceivedServerHello,
		ReceivedServerKeyExchange,
		ReceivedCertificateRequest,
		WaitForChangeCipherSpec,

		// Common handshake states
		ReceivedCertificate,
		SendChangeCipherSpec,
		ReceivedChangeCipherSpec,
		Finished
	}
}