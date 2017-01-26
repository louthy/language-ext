using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances.Pred
{
    /// <summary>
    /// Always true predicate
    /// </summary>
    /// <typeparam name="A">Bound value to test</typeparam>
    public struct True<A> : Pred<A>
    {
        bool Pred<A>.True(A value) => true;
    }

    /// <summary>
    /// Always false predicate
    /// </summary>
    /// <typeparam name="A">Bound value to test</typeparam>
    public struct False<A> : Pred<A>
    {
        bool Pred<A>.True(A value) => false;
   }
}
