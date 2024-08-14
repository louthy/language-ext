using System.Linq;
using static LanguageExt.Prelude;
using Xunit;
using System.Diagnostics;

namespace LanguageExt.Tests;

public class AtomTests
{
    [Fact]
    public void ConstructAndSwap()
    {
        var atom = Atom(Set("A", "B", "C"));

        atom.Swap(old => old.Add("D"));
        atom.Swap(old => old.Add("E"));
        atom.Swap(old => old.Add("F"));

        Debug.Assert(atom == Set("A", "B", "C", "D", "E", "F"));
    }
    [Fact]
    public void AtomSeqEnumeration()
    {
        var xs   = Seq(1,2,3,4);
        var atom = AtomSeq(xs);
            
        Assert.Equal(atom.Sum(), xs.Sum());
    }
}
