namespace IVU.Http
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using IVU.Common.Tls;

    public class TlsStream : Stream
    {
        private SecureSession secureSession;
        private byte[] lastReceivedBytes;

        public TlsStream(SecureSession secureSession)
        {
            this.secureSession = secureSession;
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.ReadAsync(buffer, offset, count, CancellationToken.None).Result;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (this.lastReceivedBytes == null)
            {
                this.lastReceivedBytes = await this.secureSession.Receive(cancellationToken);
            }

            if (this.lastReceivedBytes != null)
            {
                var maxRead = Math.Min(count, this.lastReceivedBytes.Length);
                Array.Copy(this.lastReceivedBytes, 0, buffer, offset, maxRead);

                if (this.lastReceivedBytes.Length > maxRead)
                {
                    var old = this.lastReceivedBytes;
                    this.lastReceivedBytes = new byte[this.lastReceivedBytes.Length - maxRead];
                    Array.Copy(old, maxRead, this.lastReceivedBytes, 0, this.lastReceivedBytes.Length);
                }
                else
                {
                    this.lastReceivedBytes = null;
                }

                return maxRead;
            }

            return 0;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new System.NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new System.NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.secureSession.SendAsync(buffer, offset, count, CancellationToken.None).Wait();
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return this.secureSession.SendAsync(buffer, offset, count, cancellationToken);
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length { get; }

        public override long Position { get; set; }

        public override void Close()
        {
            this.secureSession?.Close();
            this.secureSession = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.secureSession?.Close();
                this.secureSession = null;
            }

            base.Dispose(disposing);
        }
    }
}
