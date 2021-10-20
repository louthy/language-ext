using LanguageExt;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;
using System;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using System.Collections;

namespace LanguageExt
{
    /// <summary>
    /// A unit type that represents `Option.None`.  This type can be implicitly
    /// converted to Option or OptionUnsafe.
    /// </summary>
    public readonly struct OptionNone : 
        IOptional,
        IEnumerable<Unit>,
        IEquatable<OptionNone>,
        IComparable<OptionNone>
    {
        public readonly static OptionNone Default = new OptionNone();

        public bool IsSome => false;
        public bool IsNone => true;

        [Pure]
        public OptionNone Select(Func<Unit, Unit> f) =>
            Default;

        [Pure]
        public Option<C> SelectMany<B, C>(Func<Unit, Option<B>> bind, Func<Unit, B, C> project) =>
            Option<C>.None;

        [Pure]
        public OptionNone Where(Func<Unit, bool> f) =>
            Default;

        /// <summary>
        /// Truth operator
        /// </summary>
        [Pure]
        public static bool operator true(OptionNone value) =>
            false;

        /// <summary>
        /// Falsity operator
        /// </summary>
        [Pure]
        public static bool operator false(OptionNone value) =>
            true;

        [Pure]
        public override int GetHashCode() =>
            0;

        [Pure]
        public R MatchUntyped<R>(Func<object, R> Some, Func<R> None) =>
            Check.NullReturn(None());

        [Pure]
        public R MatchUntypedUnsafe<R>(Func<object, R> Some, Func<R> None) =>
            None();

        [Pure]
        public Type GetUnderlyingType() =>
            typeof(Unit);

        [Pure]
        public IEnumerator<Unit> GetEnumerator()
        {
            yield break;
        }

        [Pure]
        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        [Pure]
        public bool Equals(OptionNone other) =>
            true;

        [Pure]
        public int CompareTo(OptionNone other) =>
            0;

        [Pure]
        public Option<A> Bind<A>() =>
            Option<A>.None;
    }
}

public static class OptionNoneExt
{
    public static Option<C> SelectMany<A, C>(this Option<A> ma, Func<A, OptionNone> bind, Func<A, Unit, C> project)
    {
        ma.IfSome(x => bind(x));
        return Option<C>.None;
    }
}
