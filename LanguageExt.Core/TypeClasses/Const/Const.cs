using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.TypeClasses
{
    /// <summary>
    /// Constant value type-class
    /// </summary>
    /// <typeparam name="TYPE"></typeparam>
    public interface Const<TYPE>
    {
        TYPE Value { get; }
    }
}
