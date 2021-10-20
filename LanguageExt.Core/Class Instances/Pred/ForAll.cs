
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances.Pred
{
    /// <summary>
    /// Logical AND of the terms
    /// </summary>
    /// <typeparam name="A">Bound value to test</typeparam>
    /// <typeparam name="Term1">First term</typeparam>
    /// <typeparam name="Term2">Second term</typeparam>
    public struct ForAll<A, Term1, Term2> : Pred<A>
        where Term1 : struct, Pred<A>
        where Term2 : struct, Pred<A>
    {
        public static readonly ForAll<A, Term1, Term2> Is = default(ForAll<A, Term1, Term2>);

        [Pure]
        public bool True(A value) =>
            default(Term1).True(value) &&
            default(Term2).True(value);
    }

    /// <summary>
    /// Logical AND of the terms
    /// </summary>
    /// <typeparam name="A">Bound value to test</typeparam>
    /// <typeparam name="Term1">First term</typeparam>
    /// <typeparam name="Term2">Second term</typeparam>
    /// <typeparam name="Term3">Third term</typeparam>
    public struct ForAll<A, Term1, Term2, Term3> : Pred<A>
        where Term1 : struct, Pred<A>
        where Term2 : struct, Pred<A>
        where Term3 : struct, Pred<A>
    {
        public static readonly ForAll<A, Term1, Term2, Term3> Is = default(ForAll<A, Term1, Term2, Term3>);

        [Pure]
        public bool True(A value) =>
            default(Term1).True(value) &&
            default(Term2).True(value) &&
            default(Term3).True(value);
    }

    /// <summary>
    /// Logical AND of the terms
    /// </summary>
    /// <typeparam name="A">Bound value to test</typeparam>
    /// <typeparam name="Term1">First term</typeparam>
    /// <typeparam name="Term2">Second term</typeparam>
    /// <typeparam name="Term3">Third term</typeparam>
    /// <typeparam name="Term4">Fourth term</typeparam>
    public struct ForAll<A, Term1, Term2, Term3, Term4> : Pred<A>
        where Term1 : struct, Pred<A>
        where Term2 : struct, Pred<A>
        where Term3 : struct, Pred<A>
        where Term4 : struct, Pred<A>
    {
        public static readonly ForAll<A, Term1, Term2, Term3, Term4> Is = default(ForAll<A, Term1, Term2, Term3, Term4>);

        public bool True(A value) =>
            default(Term1).True(value) &&
            default(Term2).True(value) &&
            default(Term3).True(value) &&
            default(Term4).True(value);
    }

    /// <summary>
    /// Logical AND of the terms
    /// </summary>
    /// <typeparam name="A">Bound value to test</typeparam>
    /// <typeparam name="Term1">First term</typeparam>
    /// <typeparam name="Term2">Second term</typeparam>
    /// <typeparam name="Term3">Third term</typeparam>
    /// <typeparam name="Term4">Fourth term</typeparam>
    /// <typeparam name="Term5">Fifth term</typeparam>
    public struct ForAll<A, Term1, Term2, Term3, Term4, Term5> : Pred<A>
        where Term1 : struct, Pred<A>
        where Term2 : struct, Pred<A>
        where Term3 : struct, Pred<A>
        where Term4 : struct, Pred<A>
        where Term5 : struct, Pred<A>
    {
        public static readonly ForAll<A, Term1, Term2, Term3, Term4, Term5> Is = default(ForAll<A, Term1, Term2, Term3, Term4, Term5>);

        public bool True(A value) =>
            default(Term1).True(value) &&
            default(Term2).True(value) &&
            default(Term3).True(value) &&
            default(Term4).True(value) &&
            default(Term5).True(value);
    }
}
