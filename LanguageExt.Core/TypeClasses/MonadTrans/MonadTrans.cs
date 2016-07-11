using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.TypeClasses
{
    public interface MonadTrans<MA, A> where MA : Monad<A>
    {
        TMA Lift<TMA>(MA ma) where TMA : MonadT<MA, A>;
    }
}
