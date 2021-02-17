namespace IVU.Common.Tls
{
	public struct ConnectionState
	{
		// Session identifier used for resumption
		public byte[] SessionID;

		// Session data and seed for key generation
		public byte[] ClientRandom;
		public byte[] ServerRandom;
		public byte[] MasterSecret;

		// Used for safe renegotiation
		public bool SecureRenegotiation;
		public byte[] ClientVerifyData;
		public byte[] ServerVerifyData;
		
		public ConnectionState(byte[] clientRandom, byte[] serverRandom, byte[] masterSecret)
		{
			SessionID = new byte[0];
			
			ClientRandom = clientRandom;
			ServerRandom = serverRandom;
			MasterSecret = masterSecret;
			
			SecureRenegotiation = false;
			ClientVerifyData = null;
			ServerVerifyData = null;
		}
	}
}


