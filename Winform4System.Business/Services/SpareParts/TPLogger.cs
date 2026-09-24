using System.Diagnostics;

namespace Logger
{
    public sealed class TPLogger
    {
        private readonly string _source;

        public TPLogger(string source)
        {
            _source = source;
        }

        public void Error(string operation, string message)
        {
            Trace.TraceError("[{0}] {1}: {2}", _source, operation, message);
        }
    }
}
