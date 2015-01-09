namespace GsmGadgetry.Micro.Hardware.Gsm
{
    using System;
    using System.Threading;
    using Text;
    using Microsoft.SPOT;

    public class Sim900Client : SerialClientBase
    {
        private const int DefaultTimeoutMilliseconds = 5000;
        private const int Sim900BaudRate = 19200;

        private readonly int _timeoutMilliseconds;
        private readonly object _syncRoot = new object();
        private readonly AutoResetEvent _syncEvent = new AutoResetEvent(false);
        private bool _disposed;

        public event EventHandler PowerOnReceived;
        public event DataReceivedEventHandler PhoneFunctionalityReceived;
        public event DataReceivedEventHandler EnterPinReceived;
        public event EventHandler CallReadyReceived;
        public event DataReceivedEventHandler GprsStateReceived;
        public event DataReceivedEventHandler TextEncodingCharacterSetReceived;
        public event DataReceivedEventHandler SmsMessageFormatReceived;
        public event DataReceivedEventHandler DataHeaderStateReceived;
        public event EventHandler PowerOffReceived;

        public Sim900Client(string serialPortName)
            : this(serialPortName, DefaultTimeoutMilliseconds)
        { }

        public Sim900Client(string serialPortName, int timeoutMilliseconds)
            : base(serialPortName, Sim900BaudRate)
        {
            _timeoutMilliseconds = timeoutMilliseconds;
        }

        public int GetGprsState()
        {
            return GetGprsState(_timeoutMilliseconds);
        }

        public int GetGprsState(int timeoutMilliseconds)
        {
            int state;
            bool timeout;

            if (!TryGetGprsState(out state, timeoutMilliseconds, out timeout))
            {
                if (timeout)
                {
                    throw new TimeoutException();
                }

                throw new InvalidOperationException();
            }

            return state;
        }

        public bool TryGetGprsState(out int state)
        {
            return TryGetGprsState(out state, _timeoutMilliseconds);
        }

        public bool TryGetGprsState(out int state, int timeoutMilliseconds)
        {
            bool timeout;
            return TryGetGprsState(out state, timeoutMilliseconds, out timeout);
        }

        public bool TryGetGprsState(out int state, int timeoutMilliseconds, out bool timeout)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }

            lock (_syncRoot)
            {
                string data = null;

                var callback = new DataReceivedEventHandler((sender, e) =>
                                                                {
                                                                    data = e.Data;
                                                                    _syncEvent.Set();
                                                                });

                GprsStateReceived += callback;

                SendCommand("AT+CGATT?");
                timeout = !_syncEvent.WaitOne(timeoutMilliseconds, false);

                GprsStateReceived -= callback;

                state = int.Parse(data);
                return !timeout;
            }
        }

        public void SetGprsState(int state)
        {
            SetGprsState(state, _timeoutMilliseconds);
        }

        public void SetGprsState(int state, int timeoutMilliseconds)
        {
            bool timeout;

            if (!TrySetGprsState(state, timeoutMilliseconds, out timeout))
            {
                if (timeout)
                {
                    throw new TimeoutException();
                }

                throw new InvalidOperationException();
            }
        }

        public bool TrySetGprsState(int state)
        {
            return TrySetGprsState(state, _timeoutMilliseconds);
        }

        public bool TrySetGprsState(int state, int timeoutMilliseconds)
        {
            bool timeout;

            return TrySetGprsState(state, timeoutMilliseconds, out timeout);
        }

        public bool TrySetGprsState(int state, int timeoutMilliseconds, out bool timeout)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }

            lock (_syncRoot)
            {
                bool success = false;

                var callback = new DataReceivedEventHandler((sender, e) =>
                                                                {
                                                                    if ((success = e.Data == "OK") || e.Data == "ERROR")
                                                                    {
                                                                        _syncEvent.Set();
                                                                    }
                                                                });

                MessageDataReceived += callback;

                SendCommand("AT+CGATT=" + state);
                timeout = !_syncEvent.WaitOne(timeoutMilliseconds, false);

                MessageDataReceived -= callback;
                return success && !timeout;
            }
        }

        public void StartDataState(string protocol)
        {
            StartDataState(protocol, _timeoutMilliseconds);
        }

        public void StartDataState(string protocol, int timeoutMilliseconds)
        {
            bool timeout;

            if (!TryStartDataState(protocol, timeoutMilliseconds, out timeout))
            {
                if (timeout)
                {
                    throw new TimeoutException();
                }

                throw new InvalidOperationException();
            }
        }

        public bool TryStartDataState(string protocol)
        {
            return TryStartDataState(protocol, _timeoutMilliseconds);
        }

        public bool TryStartDataState(string protocol, int timeoutMilliseconds)
        {
            bool timeout;
            return TryStartDataState(protocol, timeoutMilliseconds, out timeout);
        }

        public bool TryStartDataState(string protocol, int timeoutMilliseconds, out bool timeout)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }

            lock (_syncRoot)
            {
                bool success = false;

                var callback = new DataReceivedEventHandler((sender, e) =>
                                                                {
                                                                    if ((success = e.Data == "OK") || e.Data == "ERROR")
                                                                    {
                                                                        _syncEvent.Set();
                                                                    }
                                                                });

                MessageDataReceived += callback;

                SendCommand("AT+CGDATA=\"" + protocol + "\"");
                timeout = !_syncEvent.WaitOne(timeoutMilliseconds, false);

                MessageDataReceived -= callback;
                return success && !timeout;
            }
        }

        public int GetDataHeaderState()
        {
            return GetDataHeaderState(_timeoutMilliseconds);
        }

        public int GetDataHeaderState(int timeoutMilliseconds)
        {
            int state;
            bool timeout;

            if (!TryGetDataHeaderState(out state, timeoutMilliseconds, out timeout))
            {
                if (timeout)
                {
                    throw new TimeoutException();
                }

                throw new InvalidOperationException();
            }

            return state;
        }

        public bool TryGetDataHeaderState(out int state)
        {
            return TryGetDataHeaderState(out state, _timeoutMilliseconds);
        }

        public bool TryGetDataHeaderState(out int state, int timeoutMilliseconds)
        {
            bool timeout;
            return TryGetDataHeaderState(out state, timeoutMilliseconds, out timeout);
        }

        public bool TryGetDataHeaderState(out int state, int timeoutMilliseconds, out bool timeout)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }

            lock (_syncRoot)
            {
                string data = null;

                var callback = new DataReceivedEventHandler((sender, e) =>
                                                                {
                                                                    data = e.Data;
                                                                    _syncEvent.Set();
                                                                });

                DataHeaderStateReceived += callback;

                SendCommand("AT+CIPHEAD?");
                timeout = !_syncEvent.WaitOne(timeoutMilliseconds, false);

                DataHeaderStateReceived -= callback;

                state = int.Parse(data);
                return !timeout;
            }
        }

        public void SetDataHeaderState(int header)
        {
            SetDataHeaderState(header, _timeoutMilliseconds);
        }

        public void SetDataHeaderState(int header, int timeoutMilliseconds)
        {
            bool timeout;

            if (!TrySetDataHeaderState(header, timeoutMilliseconds, out timeout))
            {
                if (timeout)
                {
                    throw new TimeoutException();
                }

                throw new InvalidOperationException();
            }
        }

        public bool TrySetDataHeaderState(int header)
        {
            return TrySetDataHeaderState(header, _timeoutMilliseconds);
        }

        public bool TrySetDataHeaderState(int header, int timeoutMilliseconds)
        {
            bool timeout;
            return TrySetDataHeaderState(header, timeoutMilliseconds, out timeout);
        }

        public bool TrySetDataHeaderState(int header, int timeoutMilliseconds, out bool timeout)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }

            lock (_syncRoot)
            {
                bool success = false;

                var callback = new DataReceivedEventHandler((sender, e) =>
                                                                {
                                                                    if ((success = e.Data == "OK") || e.Data == "ERROR")
                                                                    {
                                                                        _syncEvent.Set();
                                                                    }
                                                                });

                MessageDataReceived += callback;

                SendCommand("AT+CIPHEAD=" + header);
                timeout = !_syncEvent.WaitOne(timeoutMilliseconds, false);

                MessageDataReceived -= callback;
                return success && !timeout;
            }
        }

        public void StartDataConnection(string connectionType, string host, int port)
        {
            StartDataConnection(connectionType, host, port, _timeoutMilliseconds);
        }

        public void StartDataConnection(string connectionType, string host, int port, int timeoutMilliseconds)
        {
            bool timeout;

            if (!TryStartDataConnection(connectionType, host, port, timeoutMilliseconds, out timeout))
            {
                if (timeout)
                {
                    throw new TimeoutException();
                }

                throw new InvalidOperationException();
            }
        }

        public bool TryStartDataConnection(string connectionType, string host, int port)
        {
            return TryStartDataConnection(connectionType, host, port, _timeoutMilliseconds);
        }

        public bool TryStartDataConnection(string connectionType, string host, int port, int timeoutMilliseconds)
        {
            bool timeout;
            return TryStartDataConnection(connectionType, host, port, timeoutMilliseconds, out timeout);
        }

        public bool TryStartDataConnection(string connectionType, string host, int port, int timeoutMilliseconds, out bool timeout)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }

            int gprsState;

            if (!TryGetGprsState(out gprsState, timeoutMilliseconds, out timeout))
            {
                return false;
            }

            if (gprsState != (int)GprsState.Attached)
            {
                if (!TrySetGprsState((int)GprsState.Attached, timeoutMilliseconds, out timeout))
                {
                    return false;
                }
            }

            int dataHeaderState;

            if (!TryGetDataHeaderState(out dataHeaderState, timeoutMilliseconds, out timeout))
            {
                return false;
            }

            if (dataHeaderState != (int)DataHeaderState.IP)
            {
                if (!TrySetDataHeaderState((int)DataHeaderState.IP, timeoutMilliseconds, out timeout))
                {
                    return false;
                }
            }

            lock (_syncRoot)
            {
                bool firstResponseReceived = false;
                bool success = false;

                var callback = new DataReceivedEventHandler((sender, e) =>
                                                                {
                                                                    if (!firstResponseReceived)
                                                                    {
                                                                        if ((success = e.Data == "OK") || e.Data == "ERROR")
                                                                        {
                                                                            firstResponseReceived = true;
                                                                        }

                                                                        return;
                                                                    }

                                                                    _syncEvent.Set();
                                                                });

                MessageDataReceived += callback;

                SendCommand("AT+CIPSTART=\"" + connectionType + "\",\"" + host + "\"," + port);
                timeout = !_syncEvent.WaitOne(timeoutMilliseconds, false);

                MessageDataReceived -= callback;
                return success && !timeout;
            }
        }

        public void SendData(byte[] data)
        {
            SendData(data, _timeoutMilliseconds);
        }

        public void SendData(byte[] data, int timeoutMilliseconds)
        {
            bool timeout;

            if (!TrySendData(data, timeoutMilliseconds, out timeout))
            {
                if (timeout)
                {
                    throw new TimeoutException();
                }

                throw new InvalidOperationException();
            }
        }

        public bool TrySendData(byte[] data)
        {
            return TrySendData(data, _timeoutMilliseconds);
        }

        public bool TrySendData(byte[] data, int timeoutMilliseconds)
        {
            bool timeout;
            return TrySendData(data, timeoutMilliseconds, out timeout);
        }

        public bool TrySendData(byte[] data, int timeoutMilliseconds, out bool timeout)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }

            lock (_syncRoot)
            {
                var dataCallback = new DataReceivedEventHandler((sender, e) =>
                                                                    {
                                                                        if (e.Data.Length >= 2 &&
                                                                            e.Data.Substring(e.Data.Length - 2, 2) == "> ")
                                                                        {
                                                                            _syncEvent.Set();
                                                                        }
                                                                    });

                DataReceived += dataCallback;

                SendCommand("AT+CIPSEND=" + data.Length);
                timeout = !_syncEvent.WaitOne(timeoutMilliseconds, false);

                DataReceived -= dataCallback;

                if (timeout)
                {
                    return false;
                }
                
                bool success = false;

                var messageCallback = new DataReceivedEventHandler((sender, e) =>
                                                                       {
                                                                           if ((success = e.Data == "SEND OK") || e.Data == "SEND FAIL")
                                                                           {
                                                                               _syncEvent.Set();
                                                                           }
                                                                       });

                MessageDataReceived += messageCallback;

                SendBytes(data);
                SendBytes(new byte[] { 26 });

                timeout = !_syncEvent.WaitOne(timeoutMilliseconds, false);
                MessageDataReceived -= messageCallback;
                
                return success && !timeout;
            }
        }

        public int GetSmsMessageFormat()
        {
            return GetSmsMessageFormat(_timeoutMilliseconds);
        }

        public int GetSmsMessageFormat(int timeoutMilliseconds)
        {
            int format;
            bool timeout;

            if (!TryGetSmsMessageFormat(out format, timeoutMilliseconds, out timeout))
            {
                if (timeout)
                {
                    throw new TimeoutException();
                }

                throw new InvalidOperationException();
            }

            return format;
        }

        public bool TryGetSmsMessageFormat(out int format)
        {
            return TryGetSmsMessageFormat(out format, _timeoutMilliseconds);
        }

        public bool TryGetSmsMessageFormat(out int format, int timeoutMilliseconds)
        {
            bool timeout;
            return TryGetSmsMessageFormat(out format, timeoutMilliseconds, out timeout);
        }

        public bool TryGetSmsMessageFormat(out int format, int timeoutMilliseconds, out bool timeout)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }

            lock (_syncRoot)
            {
                string data = null;

                var callback = new DataReceivedEventHandler((sender, e) =>
                                                                {
                                                                    data = e.Data;
                                                                    _syncEvent.Set();
                                                                });

                SmsMessageFormatReceived += callback;

                SendCommand("AT+CMGF?");
                timeout = !_syncEvent.WaitOne(timeoutMilliseconds, false);

                SmsMessageFormatReceived -= callback;

                format = int.Parse(data);
                return !timeout;
            }
        }

        public void SetSmsMessageFormat(int format)
        {
            SetSmsMessageFormat(format, _timeoutMilliseconds);
        }

        public void SetSmsMessageFormat(int format, int timeoutMilliseconds)
        {
            bool timeout;

            if (!TrySetSmsMessageFormat(format, timeoutMilliseconds, out timeout))
            {
                if (timeout)
                {
                    throw new TimeoutException();
                }

                throw new InvalidOperationException();
            }
        }

        public bool TrySetSmsMessageFormat(int format)
        {
            return TrySetSmsMessageFormat(format, _timeoutMilliseconds);
        }

        public bool TrySetSmsMessageFormat(int format, int timeoutMilliseconds)
        {
            bool timeout;
            return TrySetSmsMessageFormat(format, timeoutMilliseconds, out timeout);
        }

        public bool TrySetSmsMessageFormat(int format, int timeoutMilliseconds, out bool timeout)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }

            lock (_syncRoot)
            {
                bool success = false;

                var callback = new DataReceivedEventHandler((sender, e) =>
                                                                {
                                                                    if ((success = e.Data == "OK") || e.Data == "ERROR")
                                                                    {
                                                                        _syncEvent.Set();
                                                                    }
                                                                });

                MessageDataReceived += callback;

                SendCommand("AT+CMGF=" + format);
                timeout = !_syncEvent.WaitOne(timeoutMilliseconds, false);

                MessageDataReceived -= callback;
                return success && !timeout;
            }
        }

        public string GetTextEncodingCharacterSet()
        {
            return GetTextEncodingCharacterSet(_timeoutMilliseconds);
        }

        public string GetTextEncodingCharacterSet(int timeoutMilliseconds)
        {
            string characterSet;
            bool timeout;

            if (!TryGetTextEncodingCharacterSet(out characterSet, timeoutMilliseconds, out timeout))
            {
                if (timeout)
                {
                    throw new TimeoutException();
                }

                throw new InvalidOperationException();
            }

            return characterSet;
        }

        public bool TryGetTextEncodingCharacterSet(out string characterSet)
        {
            return TryGetTextEncodingCharacterSet(out characterSet, _timeoutMilliseconds);
        }

        public bool TryGetTextEncodingCharacterSet(out string characterSet, int timeoutMilliseconds)
        {
            bool timeout;
            return TryGetTextEncodingCharacterSet(out characterSet, timeoutMilliseconds, out timeout);
        }

        public bool TryGetTextEncodingCharacterSet(out string characterSet, int timeoutMilliseconds, out bool timeout)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }

            lock (_syncRoot)
            {
                string data = null;

                var callback = new DataReceivedEventHandler((sender, e) =>
                                                                {
                                                                    data = e.Data;
                                                                    _syncEvent.Set();
                                                                });

                TextEncodingCharacterSetReceived += callback;

                SendCommand("AT+CSCS?");
                timeout = !_syncEvent.WaitOne(timeoutMilliseconds, false);

                TextEncodingCharacterSetReceived -= callback;

                characterSet = data.Trim('\"');
                return !timeout;
            }
        }

        public void SetTextEncodingCharacterSet(string characterSet)
        {
            SetTextEncodingCharacterSet(characterSet, _timeoutMilliseconds);
        }

        public void SetTextEncodingCharacterSet(string characterSet, int timeoutMilliseconds)
        {
            bool timeout;

            if (!TrySetTextEncodingCharacterSet(characterSet, timeoutMilliseconds, out timeout))
            {
                if (timeout)
                {
                    throw new TimeoutException();
                }

                throw new InvalidOperationException();
            }
        }

        public bool TrySetTextEncodingCharacterSet(string characterSet)
        {
            return TrySetTextEncodingCharacterSet(characterSet, _timeoutMilliseconds);
        }

        public bool TrySetTextEncodingCharacterSet(string characterSet, int timeoutMilliseconds)
        {
            bool timeout;
            return TrySetTextEncodingCharacterSet(characterSet, timeoutMilliseconds, out timeout);
        }

        public bool TrySetTextEncodingCharacterSet(string characterSet, int timeoutMilliseconds, out bool timeout)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }

            lock (_syncRoot)
            {
                bool success = false;

                var callback = new DataReceivedEventHandler((sender, e) =>
                                                                {
                                                                    if ((success = e.Data == "OK") || e.Data == "ERROR")
                                                                    {
                                                                        _syncEvent.Set();
                                                                    }
                                                                });

                MessageDataReceived += callback;

                SendCommand("AT+CSCS=\"" + characterSet + "\"");
                timeout = !_syncEvent.WaitOne(timeoutMilliseconds, false);

                MessageDataReceived -= callback;
                return success && !timeout;
            }
        }

        public void SendSmsMessage(string text, string destination)
        {
            SendSmsMessage(text, destination, _timeoutMilliseconds);
        }

        public void SendSmsMessage(string text, string destination, int timeoutMilliseconds)
        {
            bool timeout;

            if (!TrySendSmsMessage(text, destination, timeoutMilliseconds, out timeout))
            {
                if (timeout)
                {
                    throw new TimeoutException();
                }

                throw new InvalidOperationException();
            }
        }

        public bool TrySendSmsMessage(string text, string destination)
        {
            return TrySendSmsMessage(text, destination, _timeoutMilliseconds);
        }

        public bool TrySendSmsMessage(string text, string destination, int timeoutMilliseconds)
        {
            bool timeout;
            return TrySendSmsMessage(text, destination, timeoutMilliseconds, out timeout);
        }

        public bool TrySendSmsMessage(string text, string destination, int timeoutMilliseconds, out bool timeout)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }

            int format;

            if (!TryGetSmsMessageFormat(out format, timeoutMilliseconds, out timeout))
            {
                return false;
            }

            if (format != (int)SmsMessageFormat.TextMode)
            {
                if (!TrySetSmsMessageFormat((int)SmsMessageFormat.TextMode, timeoutMilliseconds, out timeout))
                {
                    return false;
                }
            }

            lock (_syncRoot)
            {
                var dataCallback = new DataReceivedEventHandler((sender, e) =>
                                                                    {
                                                                        if (e.Data.Length >= 2 &&
                                                                            e.Data.Substring(e.Data.Length - 2, 2) == "> ")
                                                                        {
                                                                            _syncEvent.Set();
                                                                        }
                                                                    });

                DataReceived += dataCallback;

                SendCommand("AT+CMGS=\"" + destination + "\"");
                timeout = !_syncEvent.WaitOne(timeoutMilliseconds, false);

                DataReceived -= dataCallback;

                if (timeout)
                {
                    return false;
                }

                bool success = false;

                var messageCallback = new DataReceivedEventHandler((sender, e) =>
                                                                       {
                                                                           if ((success = e.Data == "OK") || e.Data == "ERROR")
                                                                           {
                                                                               _syncEvent.Set();
                                                                           }
                                                                       });

                MessageDataReceived += messageCallback;

                SendCommand(text + (char)26);
                timeout = !_syncEvent.WaitOne(timeoutMilliseconds, false);

                MessageDataReceived -= messageCallback;
                return success && !timeout;
            }
        }

        public void SendSmsMessage(string text, string destination, bool requestStatusReport, bool rejectDuplicate)
        {
            SendSmsMessage(text, destination, requestStatusReport, rejectDuplicate, _timeoutMilliseconds);
        }

        public void SendSmsMessage(string text, string destination, bool requestStatusReport, bool rejectDuplicate, int timeoutMilliseconds)
        {
            bool timeout;

            if (!TrySendSmsMessage(text, destination, requestStatusReport, rejectDuplicate, timeoutMilliseconds, out timeout))
            {
                if (timeout)
                {
                    throw new TimeoutException();
                }

                throw new InvalidOperationException();
            }
        }

        public bool TrySendSmsMessage(string text, string destination, bool requestStatusReport, bool rejectDuplicate)
        {
            return TrySendSmsMessage(text, destination, requestStatusReport, rejectDuplicate, _timeoutMilliseconds);
        }

        public bool TrySendSmsMessage(string text, string destination, bool requestStatusReport, bool rejectDuplicate, int timeoutMilliseconds)
        {
            bool timeout;
            return TrySendSmsMessage(text, destination, requestStatusReport, rejectDuplicate, timeoutMilliseconds, out timeout);
        }

        public bool TrySendSmsMessage(string text, string destination, bool requestStatusReport, bool rejectDuplicate, int timeoutMilliseconds, out bool timeout)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException();
            }

            int format;

            if (!TryGetSmsMessageFormat(out format, timeoutMilliseconds, out timeout))
            {
                return false;
            }

            if (format != (int)SmsMessageFormat.PduMode)
            {
                if (!TrySetSmsMessageFormat((int)SmsMessageFormat.PduMode, timeoutMilliseconds, out timeout))
                {
                    return false;
                }
            }

            string characterSet;

            if (!TryGetTextEncodingCharacterSet(out characterSet, timeoutMilliseconds, out timeout))
            {
                return false;
            }

            if (characterSet != TextEncodingCharacterSet.Ira)
            {
                if (!TrySetTextEncodingCharacterSet(TextEncodingCharacterSet.Ira, timeoutMilliseconds, out timeout))
                {
                    return false;
                }
            }

            lock (_syncRoot)
            {
                const string smscInfo = "00"/*device default*/;
                const bool replyPathExists = false;
                const bool userDataHeader = false;
                const int validityPeriodFormat = 0x00/*not applicable*/;
                const int messageTypeIndicator = 0x01/*SMS-submit*/;
                const byte messageReference = 0/*none*/;

                string message = smscInfo;

                int smsSubmitPdu =
                    (replyPathExists ? 0x80 : 0x00) |
                    (userDataHeader ? 0x40 : 0x00) |
                    (requestStatusReport ? 0x20 : 0x00) |
                    ((validityPeriodFormat << 3) & 0x18) |
                    (rejectDuplicate ? 0x04 : 0x00) |
                    (messageTypeIndicator & 0x03);

                message += new string(HexEncoding.Hex.GetChars(new[] { (byte)smsSubmitPdu }));
                message += new string(HexEncoding.Hex.GetChars(new[] { messageReference }));
                message += new string(HexEncoding.Hex.GetChars(new[] { (byte)destination.Length }));

                int typeOfNumber = 0x00/*unknown*/;

                if (destination[0] == '0')
                {
                    typeOfNumber = 0x02/*national*/;
                }
                else if (destination.Substring(0, 2) == "27")
                {
                    typeOfNumber = 0x01/*international*/;
                }

                const int numberingPlan = 0x01/*unknown*/;

                int typeOfAddress =
                    0x80 |
                    ((typeOfNumber << 4) & 0x30) |
                    (numberingPlan & 0x0F);

                message += new string(HexEncoding.Hex.GetChars(new[] { (byte)typeOfAddress }));

                for (int i = 1; i < destination.Length; i += 2)
                {
                    message += string.Concat(destination[i], destination[i - 1]);
                }

                if (destination.Length % 2 != 0)
                {
                    message += "F" + destination[destination.Length - 1];
                }

                const int protocolIdentifier = 0x00;
                message += new string(HexEncoding.Hex.GetChars(new[] { (byte)protocolIdentifier }));

                const int dataEncodingGroup = 0x00/*general*/;
                const bool compressText = false;
                const int alphabet = 0x00/*7-bit encoding*/;

                const int dataEncoding =
                    ((dataEncodingGroup << 6) & 0xC0) |
                    (compressText ? 0x20 : 0x00) |
                    ((alphabet << 2) & 0x0C);

                byte[] encodedBytes = Gsm7BitEncoding.Gsm7Bit.GetBytes(text);

                message += new string(HexEncoding.Hex.GetChars(new[] { (byte)dataEncoding }));
                message += new string(HexEncoding.Hex.GetChars(new[] { (byte)text.Length }));
                message += new string(HexEncoding.Hex.GetChars(encodedBytes));

                var dataCallback = new DataReceivedEventHandler((sender, e) =>
                                                                    {
                                                                        if (e.Data.Length >= 2 &&
                                                                            e.Data.Substring(e.Data.Length - 2, 2) == "> ")
                                                                        {
                                                                            _syncEvent.Set();
                                                                        }
                                                                    });

                DataReceived += dataCallback;

                SendCommand("AT+CMGS=" + (message.Length / 2 - 1));
                timeout = !_syncEvent.WaitOne(timeoutMilliseconds, false);

                DataReceived -= dataCallback;

                if (timeout)
                {
                    return false;
                }

                bool success = false;

                var messageCallback = new DataReceivedEventHandler((sender, e) =>
                                                                       {
                                                                           if ((success = e.Data == "OK") || e.Data == "ERROR")
                                                                           {
                                                                               _syncEvent.Set();
                                                                           }
                                                                       });

                MessageDataReceived += messageCallback;

                SendCommand(message + (char)26);
                timeout = !_syncEvent.WaitOne(timeoutMilliseconds, false);

                MessageDataReceived -= messageCallback;
                return success && !timeout;
            }
        }

        protected override void ProcessMessageDataReceived(string message)
        {
            switch (message)
            {
                case "RDY":
                    OnPowerOnReceived(new EventArgs());
                    break;

                case "NORMAL POWER DOWN":
                    OnPowerOffReceived(new EventArgs());
                    break;

                case "Call Ready":
                    OnCallReadyReceived(new EventArgs());
                    break;

                default:
                    if (message.Length >= 6 && message.Substring(0, 6) == "+CFUN:")
                    {
                        string data = message.Substring(7).Trim();
                        OnPhoneFunctionalityReceived(new DataReceivedEventArgs(data));
                    }
                    else if (message.Length >= 6 && message.Substring(0, 6) == "+CPIN:")
                    {
                        string data = message.Substring(7).Trim();
                        OnEnterPinReceived(new DataReceivedEventArgs(data));
                    }
                    else if (message.Length >= 7 && message.Substring(0, 7) == "+CGATT:")
                    {
                        string data = message.Substring(8).Trim();
                        OnGprsStateReceived(new DataReceivedEventArgs(data));
                    }
                    else if (message.Length >= 9 && message.Substring(0, 9) == "+CIPHEAD:")
                    {
                        string data = message.Substring(10).Trim();
                        OnDataHeaderStateReceived(new DataReceivedEventArgs(data));
                    }
                    else if (message.Length >= 6 && message.Substring(0, 6) == "+CMGF:")
                    {
                        string data = message.Substring(7).Trim();
                        OnSmsMessageFormatReceived(new DataReceivedEventArgs(data));
                    }
                    else if (message.Length >= 6 && message.Substring(0, 6) == "+CSCS:")
                    {
                        string data = message.Substring(7).Trim();
                        OnTextEncodingCharacterSetReceived(new DataReceivedEventArgs(data));
                    }
                    break;
            }
        }

        protected virtual void OnPowerOnReceived(EventArgs e)
        {
            if (PowerOnReceived != null)
            {
                PowerOnReceived(this, e);
            }
        }

        protected virtual void OnPhoneFunctionalityReceived(DataReceivedEventArgs e)
        {
            if (PhoneFunctionalityReceived != null)
            {
                PhoneFunctionalityReceived(this, e);
            }
        }

        protected virtual void OnEnterPinReceived(DataReceivedEventArgs e)
        {
            if (EnterPinReceived != null)
            {
                EnterPinReceived(this, e);
            }
        }

        protected virtual void OnCallReadyReceived(EventArgs e)
        {
            if (CallReadyReceived != null)
            {
                CallReadyReceived(this, e);
            }
        }

        protected virtual void OnGprsStateReceived(DataReceivedEventArgs e)
        {
            if (GprsStateReceived != null)
            {
                GprsStateReceived(this, e);
            }
        }

        protected virtual void OnDataHeaderStateReceived(DataReceivedEventArgs e)
        {
            if (DataHeaderStateReceived != null)
            {
                DataHeaderStateReceived(this, e);
            }
        }

        protected virtual void OnTextEncodingCharacterSetReceived(DataReceivedEventArgs e)
        {
            if (TextEncodingCharacterSetReceived != null)
            {
                TextEncodingCharacterSetReceived(this, e);
            }
        }

        protected virtual void OnSmsMessageFormatReceived(DataReceivedEventArgs e)
        {
            if (SmsMessageFormatReceived != null)
            {
                SmsMessageFormatReceived(this, e);
            }
        }

        protected virtual void OnPowerOffReceived(EventArgs e)
        {
            if (PowerOffReceived != null)
            {
                PowerOffReceived(this, e);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                base.Dispose(true);
                _disposed = true;
            }
        }
    }
}
