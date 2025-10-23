using System;

namespace SmartShift.Core.Model.Source
{
    public enum ExceptionLevel
    {
        Info,
        Warning,
        Error
    }

    public class CustomException : Exception
    {
        public ExceptionLevel Level { get; }

        public CustomException(string message, ExceptionLevel level)
            : base(message)
        {
            Level = level;
        }
    }
}
