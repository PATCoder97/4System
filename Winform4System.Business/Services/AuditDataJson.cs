using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winform4System.Business.Services
{
    internal static class AuditDataJson
    {
        public static string Change(IDictionary<string, string> before, IDictionary<string, string> after)
        {
            return "{\"before\":" + Object(before) + ",\"after\":" + Object(after) + "}";
        }

        public static string CollectionChange(string name, IEnumerable<string> before, IEnumerable<string> after)
        {
            return "{\"before\":{" + Quote(name) + ":" + Array(before) + "},\"after\":{" + Quote(name) + ":" + Array(after) + "}}";
        }

        private static string Object(IEnumerable<KeyValuePair<string, string>> values)
        {
            if (values == null) return "null";
            return "{" + string.Join(",", values.Select(x => Quote(x.Key) + ":" + (x.Value == null ? "null" : Quote(x.Value)))) + "}";
        }

        private static string Array(IEnumerable<string> values)
        {
            return "[" + string.Join(",", (values ?? Enumerable.Empty<string>()).OrderBy(x => x).Select(Quote)) + "]";
        }

        private static string Quote(string value)
        {
            if (value == null) return "null";
            var builder = new StringBuilder(value.Length + 2).Append('"');
            foreach (char character in value)
            {
                switch (character)
                {
                    case '"': builder.Append("\\\""); break;
                    case '\\': builder.Append("\\\\"); break;
                    case '\b': builder.Append("\\b"); break;
                    case '\f': builder.Append("\\f"); break;
                    case '\n': builder.Append("\\n"); break;
                    case '\r': builder.Append("\\r"); break;
                    case '\t': builder.Append("\\t"); break;
                    default:
                        if (character < 32) builder.Append("\\u").Append(((int)character).ToString("x4"));
                        else builder.Append(character);
                        break;
                }
            }
            return builder.Append('"').ToString();
        }
    }
}
