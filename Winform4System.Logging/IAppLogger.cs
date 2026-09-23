using System;

namespace Winform4System.Logging
{
    public interface IAppLogger
    {
        void Info(string source, string message);
        void Error(string source, string message, Exception exception = null);
    }
}
