using System;
using System.Linq;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Range : Foldable<Range>
{
    public static Iterator<A> ForwardIterator<A>(K<Range, A> fa) => 
        throw new NotImplementedException();
}
