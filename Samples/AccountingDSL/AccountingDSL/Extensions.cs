using AccountingDSL.Data;
using AccountingDSL.DSL;
using static AccountingDSL.DSL.Transform;
using static LanguageExt.Prelude;
using LanguageExt;

namespace AccountingDSL
{
    public static class Extensions
    {
        public static Transform<Unit> ToTransform(this Seq<IOperation> operations) =>
            operations.IsEmpty
                ? Return(unit)
                : from head in
                      operations.Head is ComputeOperation c ? Compute(c)
                    : operations.Head is PrintOperation p ? Print(p)
                    : Fail<Unit>("Invalid operation")
                  from tail in ToTransform(operations.Tail)
                  select tail;
    }
}
