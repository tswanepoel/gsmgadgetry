namespace GsmGadgetry.Micro.Hardware
{
    using System;
    using System.IO.Ports;
    using System.Text;

    public abstract class SerialClientBase : IDisposable
    {
        public event DataReceivedEventHandler DataReceived;
        public event DataReceivedEventHandler MessageDataReceived;

        private readonly SerialPort _port;
        private readonly object _syncDataReceived = new object();
        private string _incompleteMessageBuffer = string.Empty;
        private bool _messageProcessingSuspended;
        private bool _disposed;

        protected SerialClientBase(string portName, int baudRate)
        {
            _port = new SerialPort(portName, baudRate);
            _port.DataReceived += SerialPortDataReceived;
            _port.Open();
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            ReadData();
        }

        private void ReadData()
        {
            var buffer = new byte[_port.BytesToRead];
            int length = _port.Read(buffer, 0, buffer.Length);
            var data = new string(Encoding.UTF8.GetChars(buffer), 0, length);
            OnDataReceived(new DataReceivedEventArgs(data));
        }

        protected virtual void OnDataReceived(DataReceivedEventArgs e)
        {
            if (DataReceived != null)
            {
                DataReceived(this, e);
            }

            ProcessDataReceived(e.Data);
        }

        public virtual void SuspendMessageProcessing()
        {
            lock (_syncDataReceived)
            {
                _messageProcessingSuspended = true;
            }
        }

        public virtual void ResumeMessageProcessing()
        {
            lock (_syncDataReceived)
            {
                _messageProcessingSuspended = false;
            }
        }

        private void ProcessDataReceived(string data)
        {
            lock (_syncDataReceived)
            {
                if (_messageProcessingSuspended)
                {
                    return;
                }

                string buffer = _incompleteMessageBuffer + data;
                int startIndex = 0;

                for (int i = 0; i < buffer.Length; i++)
                {
                    if (buffer[i] == '\r')
                    {
                        string messageData = buffer.Substring(startIndex, i - startIndex).Trim('\r', '\n');

                        if (messageData.Length != 0)
                        {
                            OnMessageDataReceived(new DataReceivedEventArgs(messageData));
                        }

                        startIndex = i + 1;
                    }
                }

                _incompleteMessageBuffer = buffer.Substring(startIndex);
            }
        }

        protected virtual void OnMessageDataReceived(DataReceivedEventArgs e)
        {
            if (MessageDataReceived != null)
            {
                MessageDataReceived(this, e);
            }

            ProcessMessageDataReceived(e.Data);
        }

        protected abstract void ProcessMessageDataReceived(string messageData);

        public void SendCommand(string command)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(typeof(SerialClientBase).Name);
            }

            var data = Encoding.UTF8.GetBytes(command + "\r");
            SendBytes(data);
        }

        protected void SendBytes(byte[] data)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(typeof(SerialClientBase).Name);
            }

            _port.Write(data, 0, data.Length);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _port.Dispose();
                _disposed = true;
            }
        }
    }
}
