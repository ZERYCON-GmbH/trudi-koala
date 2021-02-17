namespace IVU.Common.Tls.RecordLayer
{
    using System.IO;

    using IVU.Common.Tls.Plugin;

    /// <summary>
    /// Parses TLS records from raw byte arrays.
    /// </summary>
    public class TlsRecordDecoder
    {
        private byte[] inputBuffer;
        private int inputPos;
        private int inputCount;
        private int recordPos;

        private readonly MemoryStream dataBuffer = new MemoryStream();

        private RecordType currentRecordType;
        private byte protocolVersionMajor;
        private byte protcolVersionMinor;
        private int currentRecordLength;

        /// <summary>
        /// Sets a new junk of data to parse.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="offset">The offset within the data array.</param>
        /// <param name="count">The count of bytes to process within the data array starting at the specifed offset.</param>
        public void SetData(byte[] data, int offset, int count)
        {
            this.inputBuffer = data;
            this.inputPos = offset;
            this.inputCount = offset + count;
        }

        /// <summary>
        /// Trys to get the next TLS record from the data.
        /// </summary>
        /// <returns>Returns a <see cref="Record"/> or <c>null</c> if there isn't a complete recored.</returns>
        public Record Decode()
        {
            for (; this.inputPos < this.inputCount; this.inputPos++, this.recordPos++)
            {
                var currentByte = this.inputBuffer[this.inputPos];

                switch (this.recordPos)
                {
                    case 0:
                        this.dataBuffer.SetLength(0);
                        this.currentRecordType = (RecordType)currentByte;
                        break;

                    case 1:
                        this.protocolVersionMajor = currentByte;
                        break;

                    case 2:
                        this.protcolVersionMinor = currentByte;
                        break;

                    case 3:
                        this.currentRecordLength = currentByte;
                        break;

                    case 4:
                        this.currentRecordLength = (this.currentRecordLength << 8) | currentByte;
                        break;
                        
                    default:
                        this.dataBuffer.WriteByte(currentByte);
                        if (this.dataBuffer.Length == this.currentRecordLength)
                        {
                            this.recordPos = 0;
                            this.inputPos++;

                            return new Record(
                                this.currentRecordType,
                                new ProtocolVersion(this.protocolVersionMajor, this.protcolVersionMinor),
                                this.dataBuffer.ToArray());
                        }

                        break;
                }
            }

            return null;
        }
    }
}
