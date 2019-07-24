using System;

namespace LanguageExt
{
    /// <summary>
    /// Parameters for injecting the default behaviour for
    /// building patches 
    /// </summary>
    internal class PatchParams<V, O, C>
    {
        public readonly Func<V, V, bool> equivalent;
        public readonly Func<int, V, O> delete;
        public readonly Func<int, V, O> insert;
        public readonly Func<int, V, V, O> substitute;
        public readonly Func<O, C> cost;
        public readonly Func<O, int> positionOffset;

        public PatchParams(
            Func<V, V, bool> equivalent,
            Func<int, V, O> delete,
            Func<int, V, O> insert,
            Func<int, V, V, O> substitute,
            Func<O, C> cost,
            Func<O, int> positionOffset
            )
        {
            this.equivalent = equivalent;
            this.delete = delete;
            this.insert = insert;
            this.substitute = substitute;
            this.cost = cost;
            this.positionOffset = positionOffset;
        }
    }
}
