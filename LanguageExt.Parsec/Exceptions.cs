using System;

namespace LanguageExt.Parsec
{
#if !COREFX13
    [Serializable]
#endif
    public class ParserException : Exception
    {
        public ParserException()
        {
        }

        public ParserException(string message) : base(message)
        {
        }

        public ParserException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}