using System;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// No change to the collection
    /// </summary>
    /// <typeparam name="A">Value type</typeparam>
    public sealed class NoChange<A> : 
        Change<A>, 
        IEquatable<NoChange<A>>
    {
        /// <summary>
        /// Singleton value of `NoChange`
        /// </summary>
        public static readonly Change<A> Default = new NoChange<A>();

        private NoChange()
        { }

        public override bool Equals(Change<A> obj) =>
            obj is NoChange<A>;

        public bool Equals(NoChange<A> rhs) =>
            rhs != null;

        public override int GetHashCode() => 
            FNV32.OffsetBasis;

        public override string ToString() => $"No Change";
    }
}
