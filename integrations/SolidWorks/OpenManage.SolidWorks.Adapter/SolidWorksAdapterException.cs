using System;

namespace OpenManage.SolidWorks.Adapter
{
    public sealed class SolidWorksAdapterException : Exception
    {
        public SolidWorksAdapterException(string message)
            : base(message)
        {
        }

        public SolidWorksAdapterException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
