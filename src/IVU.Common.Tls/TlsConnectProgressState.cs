namespace IVU.Common.Tls
{
    public enum TlsConnectProgressState
    {
        SendClientHello,
        ReceiveServerHello,
        SendClientKeyExchange,
        SendClientChangeCipherSpec,
        SendClientFinished,
        ReceiveChangeCipherSpecAndFinished,
    }
}