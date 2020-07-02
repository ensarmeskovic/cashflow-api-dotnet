using System.Linq;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class StringExtensions
    {
        #region returns string
        public static string Format(this string format, params object[] args) => string.Format(format, args);
        public static string Remove(this string source, string value) => source.Replace(value, string.Empty);
        public static bool IsSet(this string value) => !string.IsNullOrEmpty(value);
        #endregion

        #region returns int[]
        public static int[] CsvToIntArray(this string csv) => string.IsNullOrWhiteSpace(csv) ? null : Array.ConvertAll(csv.Split(','), int.Parse);
        public static int[] CsvToUniqueIntArray(this string csv) => string.IsNullOrWhiteSpace(csv) ? null : csv.Split(',').Select(int.Parse).Distinct().ToArray();
        #endregion
    }
}
