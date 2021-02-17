/*      CLIENT                                  SERVER
    --> ClientHello
                                            <-- ServerHello
                                                ServerCertificate
                                                ServerKeyExchange
                                                ClientCertificateRequest
                                                ServerHelloDone
    --> ClientCertificate
        ClientKeyExchange
        CertificateVerify
    --> [ChangeCipherSpec]
    --> ClientFinishedMessage
                                            <-- [ChangeCipherSpec]
                                            <-- ServerFinishedMessage
        ------------------------------------------------------------------
                                    <-TLS DATA->                            */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using IVU.Common.Tls.Alerts;
using IVU.Common.Tls.HandshakeLayer;
using IVU.Common.Tls.HandshakeLayer.Protocol;
using IVU.Common.Tls.Logging;
using IVU.Common.Tls.RecordLayer;

namespace IVU.Common.Tls
{
    using System.Text;

    public class SecureSession
    {
        private ILog logger = LogProvider.For<SecureSession>();

        private SecurityParameters _securityParameters;
        private TlsRecordStream _recordStream;
        private TLSHandshakePacketizer _handshakePacketizer;

        private bool _isHandshaking;
        private bool _isAuthenticated;
        private RecordHandler _recordHandler;
        private HandshakeSession _handshakeSession;
        private List<Record> currentDataRecords;

        public SecureSession(Stream stream, SecurityParameters securityParameters)
        {
            _securityParameters = securityParameters;
            _recordStream = new TlsRecordStream(stream);
            _handshakePacketizer = new TLSHandshakePacketizer();
        }

        #region Client handshake methods


        public async Task PerformClientHandshake(CancellationToken token)
        {
            if (_isHandshaking)
            {
                throw new InvalidOperationException("Handshake already in progress");
            }

            if (_isAuthenticated)
            {
                throw new InvalidOperationException("Renegotiation not supported");
            }

            // Create record protocol handler
            _recordHandler = new RecordHandler(_securityParameters.MinimumVersion, isClient: true);

            // Create handshake protocol sub handler
            _handshakeSession = new ClientHandshakeSession(_securityParameters);
            _isHandshaking = true;

            await SendClientHello(token);
            await ReceiveServerHello(token);
            await SendClientKeyExchange(token);

            await Task.Delay(100);
            //await this.ReceiveAlert(token);

            await SendClientChangeCipherSpec(token);
            await SendClientFinished(token);
            await ReceiveChangeCipherSpecAndFinished(token);

            _isHandshaking = false;
            _isAuthenticated = true;
        }

        private async Task ReceiveAlert(CancellationToken token)
        {
            var ctsTimeout = new CancellationTokenSource(100);
            var ctsLinked = CancellationTokenSource.CreateLinkedTokenSource(token, ctsTimeout.Token);

            try
            {
                var records = await _recordStream.ReceiveAsync(ctsLinked.Token);
                if (records == null || records.Count == 0)
                {
                    return;
                }

                foreach (var record in records)
                {
                    if (record.Type == RecordType.Alert)
                    {
                        if (record.Fragment.Length == 2)
                        {
                            var alert = (AlertDescription)record.Fragment[1];
                            throw new AlertException(
                                (AlertDescription)record.Fragment[1],
                                $"Received TLS alert record: {alert}");
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async Task ReceiveChangeCipherSpecAndFinished(CancellationToken token)
        {
            var recordList = new List<Record>();
            var changeCipherSpecReceived = false;
            var finishedReceived = false;

            while (!token.IsCancellationRequested)
            {
                var records = await _recordStream.ReceiveAsync(token);
                if (records == null || records.Count == 0)
                {
                    continue;
                }

                // Check for alerts
                foreach (var record in records)
                {
                    if (record.Type == RecordType.Alert)
                    {
                        logger?.Error($"Alert received: { (AlertDescription)record.Fragment[1]}");
                        if (record.Fragment.Length == 2)
                        {
                            var alert = (AlertDescription)record.Fragment[1];
                            throw new AlertException((AlertDescription)record.Fragment[1], $"Received TLS alert record: {alert}");
                        }
                    }
                }

                recordList.AddRange(records);
                if (recordList.Count < 2)
                {
                    continue;
                }

                foreach (var record in recordList)
                {
                    _recordHandler.ProcessInputRecord(record);

                    if (record.Type == RecordType.ChangeCipherSpec)
                    {
                        changeCipherSpecReceived = true;

                        if (record.Fragment.Length != 1 || record.Fragment[0] != 0x01)
                        {
                            throw new AlertException(AlertDescription.IllegalParameter, "Received an invalid ChangeCipherSpec record");
                        }

                        // NOTE: keep this before recordHandler.ChangeLocalState since it may generate masterSecret
                        _handshakeSession.RemoteChangeCipherSpec();

                        // Change cipher suite in record handler and handle it in handshake session
                        _recordHandler.SetCipherSuite(_handshakeSession.CipherSuite, _handshakeSession.ConnectionState);
                        _recordHandler.ChangeRemoteState();

                        continue;
                    }

                    if (record.Type == RecordType.Handshake)
                    {
                        finishedReceived = true;
                        HandshakeMessage[] handshakeMessages = _handshakePacketizer.ProcessHandshakeRecord(_handshakeSession.NegotiatedVersion, record);
                        foreach (HandshakeMessage hsm in handshakeMessages)
                        {
                            if (hsm.Type != HandshakeMessageType.Finished)
                            {
                                throw new AlertException(AlertDescription.UnexpectedMessage, "Finished handshake message was expected");
                            }

                            logger?.Debug("Received Handshake message {0}", hsm.Type);
                            _handshakeSession.ProcessMessage(hsm);
                        }
                    }

                    if (changeCipherSpecReceived && finishedReceived)
                    {
                        return;
                    }
                }
            }

            throw new TimeoutException("ReceiveAsync change cipher spec timed out");

        }

        private async Task SendClientFinished(CancellationToken token)
        {
            // >>> IVU: create finished message by hand
            var finishedMsg = _handshakeSession.GetOutputMessages();
            if (finishedMsg.Length != 1 || finishedMsg.First().Type != HandshakeMessageType.Finished)
            {
                throw new InvalidOperationException("Oops, I thought the handshake session should create a finished message?!?");
            }

            foreach (var msg in finishedMsg)
            {
                logger?.Debug("Sending HandshakeMessage {0} back to server...", msg.Type);
            }

            // finished message resides in only one record
            var records =
                _handshakePacketizer.ProcessHandshakeMessages(
                    _handshakeSession.NegotiatedVersion,
                    finishedMsg,
                    _recordStream.MaximumFragmentLength);

            foreach (var record in records)
            {
                _recordHandler.ProcessOutputRecord(record);
            }

            await _recordStream.SendAsync(records, token);
            // <<< IVU
        }

        private async Task SendClientChangeCipherSpec(CancellationToken token)
        {
            logger?.Debug("Sending change cipher spec to server...");

            // Create the change cipher spec protocol packet
            // NOTE: this has to be before recordHandler.ChangeLocalState since it uses the old state
            var record = new Record(
                                    RecordType.ChangeCipherSpec,
                                    _handshakeSession.NegotiatedVersion,
                                    new byte[] { 0x01 });
            _recordHandler.ProcessOutputRecord(record);

            // NOTE: keep this before recordHandler.ChangeLocalState since it may generate masterSecret
            _handshakeSession.LocalChangeCipherSpec();

            // Change cipher suite in record handler and handle it in handshake session
            _recordHandler.SetCipherSuite(_handshakeSession.CipherSuite, _handshakeSession.ConnectionState);
            _recordHandler.ChangeLocalState();

            // Send the change cipher spec protocol packet 
            await _recordStream.SendAsync(new[] { record }, token);
        }

        private async Task SendClientKeyExchange(CancellationToken token)
        {
            HandshakeMessage[] messages = _handshakeSession.GetOutputMessages();
            foreach (var hsm in messages)
            {
                logger?.Debug("Sending HandshakeMessage {0} back to server...", hsm.Type);
            }

            Record[] records = _handshakePacketizer.ProcessHandshakeMessages(
                                    _handshakeSession.NegotiatedVersion,
                                    messages,
                                    _recordStream.MaximumFragmentLength);

            // Encrypt the handshake records
            foreach (Record record in records)
            {
                _recordHandler.ProcessOutputRecord(record);
            }

            await _recordStream.SendAsync(records, token);
        }

        private async Task ReceiveServerHello(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var records = await _recordStream.ReceiveAsync(token);
                if (records.Count == 0)
                {
                    continue;
                }

                foreach (var record in records)
                {
                    if (!_recordHandler.ProcessInputRecord(record))
                    {
                        throw new AlertException(AlertDescription.HandshakeFailure, "Process Input Record failed");
                    }

                    if (record.Type == RecordType.Alert)
                    {
                        ProcessAlertRecord(record);
                    }

                    if (record.Type != RecordType.Handshake)
                    {
                        throw new AlertException(AlertDescription.HandshakeFailure, "Handshake record expected");
                    }
                    
                    // Process each server handshake message included in this record fragment separately
                    HandshakeMessage[] msgs = _handshakePacketizer.ProcessHandshakeRecord(_handshakeSession.NegotiatedVersion, record);
                    foreach (HandshakeMessage handshakeMessage in msgs)
                    {
                        logger?.Debug("Received Handshake message {0}", handshakeMessage.Type);
                        _handshakeSession.ProcessMessage(handshakeMessage);

                        if (handshakeMessage.Type == HandshakeMessageType.ServerHelloDone)
                        {
                            return;
                        }
                    }
                }
            }

            throw new TimeoutException("ReceiveAsync Server Hello Done timed out");
        }

        private async Task SendClientHello(CancellationToken token)
        {
            HandshakeMessage[] messages = _handshakeSession.GetOutputMessages();
            foreach (var hsm in messages)
            {
                logger?.Debug("Sending Handshake message {0}...", hsm.Type);
            }

            Record[] records = _handshakePacketizer.ProcessHandshakeMessages(_handshakeSession.NegotiatedVersion, messages, _recordStream.MaximumFragmentLength);
            if (records.Length > 0)
            {
                // Encrypt the handshake records
                foreach (Record record in records)
                {
                    _recordHandler.ProcessOutputRecord(record);
                }

                // Transmit ClientHello record
                await _recordStream.SendAsync(records, token);
            }
        }

        #endregion

        #region Server handshake methods
        public async Task PerformServerHandshake(X509Certificate serverCertificate, CancellationToken token)
        {
            if (_isHandshaking)
            {
                throw new InvalidOperationException("Handshake already in progress");
            }

            if (_isAuthenticated)
            {
                throw new InvalidOperationException("Renegotiation not supported");
            }

            _recordHandler = new RecordHandler(_securityParameters.MinimumVersion, isClient: false);
            _handshakeSession = new ServerHandshakeSession(_securityParameters);
            _isHandshaking = true;

            var timoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var cts = CancellationTokenSource.CreateLinkedTokenSource(token, timoutCts.Token);

            await ReceiveClientHello(cts.Token);
            await SendServerHello(cts.Token);
            await ReceiveClientKeyExchangeChangeCipherSpecAndFinished(cts.Token);
            await SendServerChangeCipherSpec(cts.Token);
            await SendServerFinished(cts.Token);

            _isHandshaking = false;
            _isAuthenticated = true;
        }

        private async Task SendServerFinished(CancellationToken token)
        {
            // >>> IVU: create finished message by hand
            var finishedMsg = _handshakeSession.GetOutputMessages();
            if (finishedMsg.Length != 1 || finishedMsg.First().Type != HandshakeMessageType.Finished)
            {
                throw new InvalidOperationException(
                    "Oops, I thought the handshake session should create a finished message?!?");
            }

            // only one message in one record
            var records =
                _handshakePacketizer.ProcessHandshakeMessages(
                    _handshakeSession.NegotiatedVersion,
                    finishedMsg,
                    _recordStream.MaximumFragmentLength);

            foreach (var record in records)
            {
                _recordHandler.ProcessOutputRecord(record);
            }

            await _recordStream.SendAsync(records, token);
            // <<< IVU
        }

        private async Task SendServerChangeCipherSpec(CancellationToken token)
        {
            logger?.Debug("Sending change cipher spec to client...");

            // Create the change cipher spec protocol packet
            // NOTE: this has to be before recordHandler.ChangeLocalState since it uses the old state
            var record = new Record(
                                    RecordType.ChangeCipherSpec,
                                    _handshakeSession.NegotiatedVersion,
                                    new byte[] { 0x01 });
            _recordHandler.ProcessOutputRecord(record);

            // NOTE: keep this before recordHandler.ChangeLocalState since it may generate masterSecret
            _handshakeSession.LocalChangeCipherSpec();

            // Change cipher suite in record handler and handle it in handshake session
            _recordHandler.SetCipherSuite(_handshakeSession.CipherSuite, _handshakeSession.ConnectionState);
            _recordHandler.ChangeLocalState();

            // Send the change cipher spec protocol packet 
            await _recordStream.SendAsync(new[] { record }, token);
        }

        private async Task ReceiveClientKeyExchangeChangeCipherSpecAndFinished(CancellationToken token)
        {
            var receivedFinished = false;
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(50, token);
                var records = await _recordStream.ReceiveAsync(token);
                if (records.Count == 0)
                {
                    continue;
                }

                foreach (var record in records)
                {
                    _recordHandler.ProcessInputRecord(record);
                    if (record.Type == RecordType.ChangeCipherSpec)
                    {
                        if (record.Fragment.Length != 1 || record.Fragment[0] != 0x01)
                        {
                            throw new AlertException(AlertDescription.IllegalParameter, "Received an invalid ChangeCipherSpec record");
                        }

                        // NOTE: keep this before recordHandler.ChangeLocalState since it may generate masterSecret
                        _handshakeSession.RemoteChangeCipherSpec();

                        // Change cipher suite in record handler and handle it in handshake session
                        _recordHandler.SetCipherSuite(_handshakeSession.CipherSuite, _handshakeSession.ConnectionState);
                        _recordHandler.ChangeRemoteState();

                        continue;
                    }

                    HandshakeMessage[] handshakeMessages = _handshakePacketizer.ProcessHandshakeRecord(_handshakeSession.NegotiatedVersion, record);
                    foreach (HandshakeMessage hsm in handshakeMessages)
                    {
                        if (hsm.Type == HandshakeMessageType.Finished)
                        {
                            receivedFinished = true;
                        }

                        logger?.Debug("Received Handshake message {0}", hsm.Type);
                        _handshakeSession.ProcessMessage(hsm);
                    }
                }

                if (receivedFinished)
                {
                    return;
                }
            }

            throw new TimeoutException("ReceiveAsync client key exchange timed out");
        }

        private async Task SendServerHello(CancellationToken token)
        {
            HandshakeMessage[] messages = _handshakeSession.GetOutputMessages();
            foreach (var hsm in messages)
            {
                logger?.Debug("Sending HandshakeMessage {0} back to client...", hsm.Type);
            }

            Record[] records = _handshakePacketizer.ProcessHandshakeMessages(
                                    _handshakeSession.NegotiatedVersion,
                                    messages,
                                    _recordStream.MaximumFragmentLength);
            if (records.Length > 0)
            {
                // Encrypt the handshake records
                foreach (Record record in records)
                {
                    _recordHandler.ProcessOutputRecord(record);
                }

                await _recordStream.SendAsync(records, token);
            }
        }

        private async Task ReceiveClientHello(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(100, token);
                var records = await _recordStream.ReceiveAsync(token);
                if (records.Count == 0)
                {
                    continue;
                }

                if (records.Count > 1)
                {
                    throw new AlertException(AlertDescription.UnexpectedMessage,
                        "Too many records. Only one record with client hello expected");
                }

                var clientHello = records.First();
                if (!_recordHandler.ProcessInputRecord(clientHello))
                {
                    throw new AlertException(AlertDescription.HandshakeFailure, "Processing client hello failed");
                }

                HandshakeMessage[] msgs =
                    _handshakePacketizer.ProcessHandshakeRecord(_handshakeSession.NegotiatedVersion, clientHello);
                if (msgs.Length != 1)
                {
                    throw new AlertException(AlertDescription.UnexpectedMessage,
                        "Exactly one message of type client hello expected");
                }
                var clientHelloMsg = msgs[0];
                if (clientHelloMsg.Type != HandshakeMessageType.ClientHello)
                {
                    throw new AlertException(AlertDescription.UnexpectedMessage, "ClientHello expected");
                }

                _handshakeSession.ProcessMessage(clientHelloMsg);
                return;
            }

            throw new TimeoutException("ReceiveAsync ClientHello timed out");
        }

        #endregion

        #region Data receiving and sending methods

        public async Task<byte[]> Receive(CancellationToken token)
        {
            if (!_isAuthenticated)
            {
                throw new InvalidOperationException("Trying to receive on an unauthenticated session");
            }
            if (_isHandshaking)
            {
                // Queue requests to receive data during renegotiation
                // TODO: implement this when renegotiation is implemented
                throw new InvalidOperationException("Receiving data during renegotiation not implemented");
            }

            if (this.currentDataRecords == null || currentDataRecords.Count == 0)
            {
                this.currentDataRecords = await _recordStream.ReceiveAsync(token);
                if (this.currentDataRecords.Count == 0)
                {
                    throw new IOException("No TLS record received");
                }
            }

            var memStream = new MemoryStream();
            while(this.currentDataRecords.Count > 0)
            {
                var record = this.currentDataRecords[0];

                if (record.Type == RecordType.Alert)
                {
                    if (memStream.Length > 0)
                {
                        // Process the alert record with the next call to Receive(). 
                        // First the current application data will be returned to the caller.
                        break;
                    }

                    if (!_recordHandler.ProcessInputRecord(record))
                    {
                        logger?.Error("Processing input record failed!");
                    }

                    ProcessAlertRecord(record);
                }

                if (!_recordHandler.ProcessInputRecord(record))
                    {
                    logger?.Error("Processing input record failed!");
                    }

                if (record.Type == RecordType.Data)
                    {
                        memStream.Write(record.Fragment, 0, record.Fragment.Length);
                    }
                    else
                    {
                        ProcessUnknownRecord(record);
                    }

                this.currentDataRecords.RemoveAt(0);
            }
            return memStream.ToArray();
        }

        public async Task SendAsync(byte[] buffer, CancellationToken token)
        {
            await SendAsync(buffer, 0, buffer.Length, token);
        }

        public async Task SendAsync(byte[] buffer, int offset, int count, CancellationToken token)
        {
            if (!_isAuthenticated)
            {
                throw new InvalidOperationException("Trying to send on an unauthenticated session");
            }
            if (_isHandshaking)
            {
                // Queue requests to send data during renegotiation
                // TODO: implement this when renegotiation is implemented
                throw new InvalidOperationException("Sending data during renegotiation not implemented");
            }

            if (this.logger.IsTraceEnabled())
            {
                this.logger.Trace("TLS send: {0}", Encoding.ASCII.GetString(buffer, offset, count));
            }

            // Copy the bytes to send into own buffer
            byte[] outputBuffer = new byte[count];
            Buffer.BlockCopy(buffer, offset, outputBuffer, 0, count);

            // Create and encrypt the data output record
            var records = _handshakePacketizer.ProcessOutputData(
                _handshakeSession.NegotiatedVersion,
                (byte)RecordType.Data, outputBuffer,
                _recordStream.MaximumFragmentLength);
            foreach (Record t in records)
            {
                _recordHandler.ProcessOutputRecord(t);
            }

            // Send the data output record
            try
            {
                await _recordStream.SendAsync(records, token);
            }
            catch (AlertException ae)
            {
                await ProcessSendFatalAlert(new Alert(ae.AlertDescription, _handshakeSession.NegotiatedVersion), token);
                throw new Exception("Connection closed because of local alert", ae);
            }
            catch (IOException)
            {
                throw new EndOfStreamException("Connection closed unexpectedly");
            }
            catch (Exception e)
            {
                await ProcessSendFatalAlert(new Alert(AlertDescription.InternalError,
                    _handshakeSession.NegotiatedVersion), token);
                throw new Exception("Connection closed because of local error", e);
            }
        }

        public void Close()
        {
            _recordStream.Close();
        }
        #endregion





        private void ProcessAlertRecord(Record record)
        {
            Alert alert = new Alert(record.Fragment);
            logger?.Debug("Received an alert: " + alert.Description);
            if (alert.Description == AlertDescription.CloseNotify)
            {
                throw new AlertException(alert.Description);
            }

            if (alert.Level == AlertLevel.Fatal)
            {
                // Fatal alerts don't need close notify
                _recordStream.Close();
                throw new AlertException(AlertDescription.ReceivedFatalAlert, "Received a fatal alert");
            }
        }

        private async Task ProcessSendFatalAlert(Alert alert, CancellationToken token)
        {
            logger?.Warn("Processing fatal alert...");
            try
            {
                // Create and encrypt the alert record
                Record record = new Record(RecordType.Alert, _handshakeSession.NegotiatedVersion, alert.GetBytes());
                _recordHandler.ProcessOutputRecord(record);

                // Attempt sending the alert record
                await _recordStream.SendAsync(new[] { record }, token);
            }
            catch (Exception ex)
            {
                logger?.Error("Processing fatal alert failed: {0}", ex.Message);
            }
            finally
            {
                if (_recordStream != null)
                {
                    logger?.Info("Closing record stream...");
                    _recordStream.Flush();
                    _recordStream.Close();
                }
            }
        }


        private void ProcessUnknownRecord(Record record)
        {
            throw new InvalidOperationException("Unknown record");
        }

        public async Task SendCloseNotify(CancellationToken ct)
        {
            var record = new Record(RecordType.Alert, _handshakeSession.NegotiatedVersion, new[] { (byte)AlertLevel.None, (byte)AlertDescription.CloseNotify });
            _recordHandler.ProcessOutputRecord(record);
            await _recordStream.SendAsync(new[] { record }, ct);
        }
    }
}

