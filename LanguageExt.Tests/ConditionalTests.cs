using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt.DataTypes.Conditional;
using Xunit;

namespace LanguageExtTests
{
    public class ConditionalTests
    {
        [Fact]
        public void SynchronousTests()
        {
            Assert.True(4.Cond(x => x == 4, (_) => true).Else((_) => false));
            Assert.True(5.Cond(x => x == 4, (_) => true).Cond(x => x > 4, (_) => true).Else((_) => false));
            Assert.False(3.Cond(x => x == 4, (_) => true).Cond(x => x > 4, (_) => true).Else((_) => false));
        }

        [Fact]
        public async Task AsynchronousTests()
        {
            Assert.True(await (Task.FromResult(4).Cond(x => x == 4, (_) => true).Else((_) => false)));            
            Assert.False(await (Task.FromResult(3).Cond(x => x == 4, (_) => true).Cond(x => x > 4, (_) => true).Else((_) => false)));
            Assert.True(await (4.Cond(x => Task.FromResult(x == 4), (_) => true).Else((_) => false)));
            Assert.True(await (4.Cond(x => x == 4, (_) => Task.FromResult(true)).Else((_) => false)));
            Assert.False(await 3.Cond(x => x == 4, (_) => true).Cond(x => x > 4, (_) => true).Else((_) => Task.FromResult(false)));
            Assert.True(await (4.Cond(x => Task.FromResult(x == 4), (_) => Task.FromResult(true)).Else((_) => false)));
            Assert.True(await (4.Cond(x => Task.FromResult(x == 4), (_) => Task.FromResult(true)).Else((_) => Task.FromResult(false))));
            Assert.True(await (3.Cond(x => x == 4, (_) => true).Cond(x => Task.FromResult(x < 4), (_) => true).Else((_) => false)));
            Assert.True(await (3.Cond(x => x == 4, (_) => true).Cond(x => Task.FromResult(x < 4), (_) => Task.FromResult(true)).Else((_) => false)));
            Assert.True(await (3.Cond(x => x == 4, (_) => true).Cond(x => x < 4, (_) => Task.FromResult(true)).Else((_) => false)));
        }
    }
}