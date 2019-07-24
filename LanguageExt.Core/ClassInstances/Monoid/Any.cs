using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Booleans form a monoid under conjunction.
    /// </summary>
    public struct Any : Monoid<bool>, Bool<bool>
    {
        public static readonly Any Inst = default(Any);

        [Pure]
        public bool Append(bool x, bool y) => x || y;

        [Pure]
        public bool Empty() => false;

        [Pure]
        public bool And(bool a, bool b) =>
            TBool.Inst.And(a, b);

        [Pure]
        public bool BiCondition(bool a, bool b) =>
            TBool.Inst.BiCondition(a, b);

        [Pure]
        public bool False() => false;

        [Pure]
        public bool Implies(bool a, bool b) =>
            TBool.Inst.Implies(a, b);

        [Pure]
        public bool Not(bool a) =>
            TBool.Inst.Not(a);

        [Pure]
        public bool Or(bool a, bool b) =>
            TBool.Inst.Or(a, b);

        [Pure]
        public bool True() =>
            true;

        [Pure]
        public bool XOr(bool a, bool b) =>
            TBool.Inst.XOr(a, b);
    }
}
