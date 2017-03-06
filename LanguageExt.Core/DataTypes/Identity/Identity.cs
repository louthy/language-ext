using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// Identity monad
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    public struct Identity<A>
    {
        public static readonly Identity<A> Bottom = default(Identity<A>);

        public readonly A value;
        public readonly bool IsBottom;

        public Identity(A value)
        {
            this.value = value;
            IsBottom = false;
        }

        [Pure]
        public A Value
        {
            get
            {
                if (IsBottom) throw new BottomException();
                return value;
            }
        }
    }
}
