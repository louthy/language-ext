using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    /// <summary>
    /// Discriminated union type.  Can be in one of two states:
    /// 
    ///     Some(a)
    ///     
    ///     None
    ///     
    /// All instance methods are provided as extension methods.  This is to defend
    /// against null references, and to equate null with None.  Therefore you do not
    /// need to check for null when using IOption<A> or Option<A>
    /// </summary>
    /// <typeparam name="A">Bound value</typeparam>
    public interface IOption<out A>
    {
    }
}
