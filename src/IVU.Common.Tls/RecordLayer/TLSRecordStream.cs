namespace IVU.Common.Tls.RecordLayer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using IVU.Common.Tls.Logging;

    public class TlsRecordStream
    {
        private static readonly ILog Log = LogProvider.For<TlsRecordStream>();

        private const int MAX_FRAGMENT_SIZE = 16384;
        private const int MAX_RECORD_SIZE = 5 + 16384 + 2048;

        private readonly Stream innerStream;

        private readonly List<byte> rcvBuffer = new List<byte>();
        
        public TlsRecordStream(Stream innerStream)
        {
            this.innerStream = innerStream;
        }
        
        public int MaximumFragmentLength => MAX_FRAGMENT_SIZE;

        public async Task SendAsync(Record[] records, CancellationToken token)
        {
            if (records == null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            // Make sure fragments are not too long
            foreach (Record record in records)
            {
                token.ThrowIfCancellationRequested();

                if (record.Fragment.Length > MAX_RECORD_SIZE)
                {
                    throw new RecordTooLargeException("Trying to send a too large fragment: " + record.Fragment.Length);
                }

                Log.Debug(">> RECORD TX, Type {0}, Len {1}.", record.Type, record.FragmentLength);

                var buffer = record.GetBytes();

                try
                {
                    // hook to combine token.cancel with stream.close, otherwise WriteAsync possibly never returns
                    using (token.Register(this.innerStream.Close))
                    {
                        await this.innerStream.WriteAsync(buffer, 0, buffer.Length, token);
                    }
                    
                    break;
                }
                catch (Exception ex)
                {
                    Log.Error("Send failed: {0}", ex.Message);
                    throw;
                }
            }
        }

        public async Task<List<Record>> ReceiveAsync(CancellationToken token)
        {
            var records = new List<Record>();
            var readBuffer = new byte[MAX_RECORD_SIZE];
            var needsMoreData = true;

            while (!token.IsCancellationRequested)
            {
                if (needsMoreData)
                {
                    // hook to combine token.cancel with stream.close, otherwise ReadAsync never returns
                    using (token.Register(this.innerStream.Close))
                    {
                        try
                        {
                            var readBytes = await this.innerStream.ReadAsync(readBuffer, 0, readBuffer.Length, token);
                            if (readBytes == 0)
                            {
                                return records;
                            }
                            this.rcvBuffer.AddRange(readBuffer.Take(readBytes));
                        }
                        catch (Exception e)
                        {
                            if (token.IsCancellationRequested)
                            {
                                throw new OperationCanceledException("Read was cancelled");
                            }

                            Log.Error("Read failed: {0}", e.Message);
                            throw e;
                        }
                    }                   
                }

                // do not inspect buffer if there is not even a packet header
                if (this.rcvBuffer.Count < 5)
                {
                    await Task.Delay(5, token);
                    needsMoreData = true;
                    continue;
                }

                
                // We require the fragment bytes
                int fragmentLength = (this.rcvBuffer[3] << 8) | this.rcvBuffer[4];
                if (fragmentLength + 5 > this.rcvBuffer.Count)
                {
                    // read more data
                    needsMoreData = true;
                    continue;
                }

                needsMoreData = false;
                // Construct the TLSRecord returned as result
                var recordBuffer = this.rcvBuffer.Take(5 + fragmentLength).ToArray();
                var record = new Record(recordBuffer);
                Log.Debug("<< RECORD RX, Type {0}, Fragment Len {1} Seq {2}.", record.Type, record.FragmentLength, record.SequenceNumber);

                Buffer.BlockCopy(recordBuffer, 5, record.Fragment, 0, fragmentLength);
                records.Add(record);

                // remove record from input buffer
                this.rcvBuffer.RemoveRange(0, 5 + fragmentLength);

                if (this.rcvBuffer.Count == 0)
                {
                    // return records
                    break;
                }
            }

            return records;
        }


        public void Flush()
        {
            innerStream.Flush();
        }

        public void Close()
        {
            try
            {
                innerStream.Close();
            }
            catch (Exception)
            {
            }
        }
    }
}
