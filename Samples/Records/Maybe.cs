using LanguageExt;
using System;
using System.Text;

namespace Records
{
    /// <summary>
    /// Maybe discriminated union example
    /// </summary>
    public interface Maybe<A>
    { }

    /// <summary>
    /// Just case
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public sealed class Just<A> : Record<Just<A>>, Maybe<A>
    {
        public readonly A Value;
        public Just(A value) => Value = value;
    }

    /// <summary>
    /// Nothing case
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public sealed class Nothing<A> : Record<Nothing<A>>, Maybe<A>
    {
    }

    /// <summary>
    /// Extensions and construction
    /// </summary>
    public static class Maybe
    {
        public static Maybe<A> Just<A>(A value) => 
            new Just<A>(value);

        public static Maybe<A> Nothing<A>() =>
            new Nothing<A>();

        public static R Match<A, R>(this Maybe<A> ma, Func<A, R> Just, Func<R> Nothing) =>
            ma is null         ? Nothing()
          : ma is Nothing<A> _ ? Nothing()
          : ma is Just<A> just ? Just(just.Value)
          : throw new InvalidOperationException();
    }
}
