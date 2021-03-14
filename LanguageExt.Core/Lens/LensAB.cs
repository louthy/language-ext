using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Primitive lens type for creating well-behaved bidirectional transformations
    /// </summary>
    public readonly struct Lens<A, B>
    {
        public readonly Func<A, B> Get;
        public readonly Func<B, Func<A, A>> SetF;

        Lens(Func<A, B> get, Func<B, Func<A, A>> set)
        {
            Get = get;
            SetF = set;
        }

        public A Set(B value, A cont) =>
            SetF(value)(cont);

        public static Lens<A, B> New(Func<A, B> Get, Func<B, Func<A, A>> Set) =>
            new Lens<A, B>(Get, Set);

        public Func<A, A> Update(Func<B, B> f)
        {
            var self = this;
            return a => self.Set(f(self.Get(a)), a);
        }

        public A Update(Func<B, B> f, A value) =>
            Set(f(Get(value)), value);
    }
}
