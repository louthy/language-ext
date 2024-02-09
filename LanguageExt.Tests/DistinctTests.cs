using System;
using LanguageExt.ClassInstances;
using Xunit;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.Tests
{
    public class DistinctTests
    {
        [Fact]
        void SeqDistinctIgnoreCase()
        {
            var items = Seq("Test", "other", "test");
            
            Assert.Equal(items, items.Distinct());
            Assert.Equal(items.Take(3), items.Distinct(_ => _, fun<string, string, bool>(EqStringOrdinal.Inst.Equals)));
            Assert.Equal(items.Take(2), items.Distinct(_ => _, fun<string, string, bool>(EqStringOrdinalIgnoreCase.Inst.Equals)));
        }
    }
}
