// ReSharper disable once CheckNamespace
namespace System
{
    public static class ExceptionExtensions
    {
        #region returns string

        public static string InnerExceptionMessage(this Exception source)
        {
            return source == null ? string.Empty : source.InnerException == null ? source.Message : source.InnerException.Message;
        }

        #endregion
    }
}
