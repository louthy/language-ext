using System;
using LanguageExt;
using static LanguageExt.Prelude;
using Xunit;
using System.Diagnostics;

namespace LanguageExt.Tests
{
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
    }
}
