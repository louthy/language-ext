using System;
using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt;

public class AtomQue : Foldable<AtomQue>
{
    public static Iterator<A> ForwardIterator<A>(K<AtomQue, A> fa) => 
        fa.As().Snapshot().ForwardIterator();
}
