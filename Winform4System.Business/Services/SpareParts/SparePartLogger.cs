using System.Diagnostics;

namespace Winform4System.Business.Services.SpareParts
{
    public sealed class SparePartLogger
    {
        private readonly string _source;

        public SparePartLogger(string source)
        {
            _source = source;
        }

        public void Error(string operation, string message)
        {
            Trace.TraceError("[{0}] {1}: {2}", _source, operation, message);
        }
    }
}
