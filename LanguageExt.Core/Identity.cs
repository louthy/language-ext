using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    /// <summary>
    /// Identity monad
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    public struct Identity<A>
    {
        internal static Identity<A> Default = new Identity<A>(default(A));

        public readonly A value;
        public readonly bool IsBottom;

        Identity(A value)
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
