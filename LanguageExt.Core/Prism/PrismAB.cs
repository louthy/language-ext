using System;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// Primitive prism type for creating transformations through Options
    /// </summary>
    public readonly struct Prism<A, B>
    {
        public readonly Func<A, Option<B>> Get;
        public readonly Func<B, Func<A, A>> SetF;

        private Prism(Func<A, Option<B>> get, Func<B, Func<A, A>> set)
        {
            Get = get;
            SetF = set;
        }

        public A Set(B value, A cont)
            => SetF(value)(cont);

        public static Prism<A, B> New(Func<A, Option<B>> Get,
                                      Func<B, Func<A, A>> Set)
            => new Prism<A, B>(Get, Set);

        public static Prism<A, B> New(Lens<A, B> lens) =>
            Prism<A, B>.New(
                Get: a => lens.Get(a),
                Set: v => lens.SetF(v)
            );

        public static Prism<A, B> New(Lens<A, Option<B>> lens) =>
            Prism<A, B>.New(
                Get: a => lens.Get(a),
                Set: v => lens.SetF(v)
            );

        public Func<A, A> Update(Func<B, B> f)
        {
            var self = this;
            return a => self.Get(a)
                            .Map(v => self.Set(f(v), a))
                            .IfNone(a);
        }

        public A Update(Func<B, B> f, A value)
        {
            var self = this;
            return Get(value).Map(v => self.Set(f(v), value))
                             .IfNone(value);
        }

        /// <summary>
        /// Implicit conversion operator from Lens〈A, B〉 to Prism〈A, B〉
        /// </summary>
        /// <param name="value">Value</param>
        [Pure]
        public static implicit operator Prism<A, B>(Lens<A, B> value) => 
            New(value);

        /// <summary>
        /// Implicit conversion operator from Option〈A〉 to Result〈A〉
        /// </summary>
        /// <param name="value">Value</param>
        [Pure]
        public static implicit operator Prism<A, B>(Lens<A, Option<B>> value) => 
            New(value);

    }
}
