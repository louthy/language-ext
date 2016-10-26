using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.ClassInstances.Pred
{
    public struct GreaterThan<A, ORD, CONST> : Pred<A>
        where ORD : struct, Ord<A>
        where CONST : struct, Const<A>
    {
        public static readonly GreaterThan<A, ORD, CONST> Is = default(GreaterThan<A, ORD, CONST>);

        public bool True(A value) =>
            default(ORD).Compare(value, default(CONST).Value) > 0;
    }

    public struct LessThan<A, ORD, CONST> : Pred<A>
        where ORD : struct, Ord<A>
        where CONST : struct, Const<A>
    {
        public static readonly LessThan<A, ORD, CONST> Is = default(LessThan<A, ORD, CONST>);

        public bool True(A value) =>
            default(ORD).Compare(value, default(CONST).Value) < 0;
    }

    public struct GreaterOrEq<A, ORD, CONST> : Pred<A>
        where ORD : struct, Ord<A>
        where CONST : struct, Const<A>
    {
        public static readonly GreaterOrEq<A, ORD, CONST> Is = default(GreaterOrEq<A, ORD, CONST>);

        public bool True(A value) =>
            default(ORD).Compare(value, default(CONST).Value) >= 0;
    }

    public struct LessOrEq<A, ORD, CONST> : Pred<A>
        where ORD   : struct, Ord<A>
        where CONST : struct, Const<A>
    {
        public static readonly LessOrEq<A, ORD, CONST> Is = default(LessOrEq<A, ORD, CONST>);

        public bool True(A value) =>
            default(ORD).Compare(value, default(CONST).Value) <= 0;
    }
}
