using System;

namespace Winform4System.Helpers
{
    public static class StoredFileNameGenerator
    {
        public static string Create()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
