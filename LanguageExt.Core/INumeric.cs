using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    interface INumeric<T> :
        IMultiplicable<T>,
        IDivisible<T>,
        IAppendable<T>,
        ISubtractable<T>,
        IComparable<T>,
        IEquatable<T>
    {
    }
}
