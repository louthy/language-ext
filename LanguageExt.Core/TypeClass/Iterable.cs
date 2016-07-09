using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.TypeClass
{
    public interface Iterable<A>
    {
        /// <summary>
        /// Iterate the value(s) within the structure
        /// </summary>
        /// <param name="f">Operation to perform on the value(s)</param>
        Unit Iter(Iterable<A> ia, Action<A> f);
    }
}
