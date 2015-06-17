using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    /// <summary>
    /// Pattern matching for exceptions.  This is to aid expression based error handling.
    /// </summary>
    public class ExceptionMatch<R>
    {
        Exception exception;
        bool valueSet;
        R value;

        public ExceptionMatch(Exception e)
        {
            exception = e;
        }

        public ExceptionMatch<R> With<EXCEPTION>(Func<EXCEPTION, R> handler) where EXCEPTION : Exception
        {
            if (typeof(EXCEPTION).IsAssignableFrom(exception.GetType()))
            {
                value = handler(exception as EXCEPTION);
                valueSet = true;
            }
            return this;
        }

        public R Otherwise(R defaultValue) =>
            valueSet
                ? value
                : defaultValue;

        public R Otherwise(Func<R> defaultHandler) =>
            valueSet
                ? value
                : defaultHandler();

        public R Otherwise(Func<Exception,R> defaultHandler) =>
            valueSet
                ? value
                : defaultHandler(exception);
    }
}

public static class ExceptionExt
{
    /// <summary>
    /// Pattern matching for exceptions.  This is to aid expression based error handling.
    /// </summary>
    public static LanguageExt.ExceptionMatch<R> Match<R>(this Exception self)
    {
        return new LanguageExt.ExceptionMatch<R>(self);
    }
}

