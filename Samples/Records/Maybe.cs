using LanguageExt;
using System;
using System.Text;

namespace Records
{
    /// <summary>
    /// Maybe type
    /// </summary>
    public interface Maybe<A>
    { }

    public sealed class Just<A> : Record<Just<A>>, Maybe<A>
    {
        public readonly A Value;
        public Just(A value) => Value = value;
    }

    public sealed class Nothing<A> : Record<Nothing<A>>, Maybe<A>
    {
    }

    public static class Maybe
    {
        public static Maybe<A> Just<A>(A value) => 
            new Just<A>(value);

        public static Maybe<A> Nothing<A>() =>
            new Nothing<A>();

        public static R Match<A, R>(this Maybe<A> ma, Func<A, R> Just, Func<R> Nothing) =>
            ma is Just<A> just ? Just(just.Value)
          : ma is Nothing<A> _ ? Nothing()
          : throw new InvalidOperationException();
    }
}
