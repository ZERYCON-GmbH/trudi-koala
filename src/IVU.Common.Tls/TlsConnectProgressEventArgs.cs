namespace IVU.Common.Tls
{
    using System;

    public class TlsConnectProgressEventArgs : EventArgs
    {
        public TlsConnectProgressEventArgs(TlsConnectProgressState state)
        {
            this.State = state;
        }

        public TlsConnectProgressState State { get; set; }
    }
}