namespace GsmGadgetry.Micro.Hardware
{
    using Microsoft.SPOT;

    public class DataReceivedEventArgs : EventArgs
    {
        private readonly string _data;

        public DataReceivedEventArgs(string data)
        {
            _data = data;
        }

        public string Data
        {
            get { return _data; }
        }
    }
}
