namespace GsmGadgetry.Micro
{
    using System;

    public class TimeoutException : SystemException
    {
        public TimeoutException()
            : base(null, null)
        { }

        public TimeoutException(string message)
            : base(message, null)
        { }

        public TimeoutException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
