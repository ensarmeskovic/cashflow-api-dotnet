using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Cashflow.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsSet(this byte[] source)
        {
            return source != null && source.Length > 0;
        }
    }
}
