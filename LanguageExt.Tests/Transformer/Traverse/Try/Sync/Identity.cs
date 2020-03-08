using System;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryT.Sync
{
    public class IdentityTry
    {
        [Fact]
        public void IdentityFailIsFail()
        {
            var ma = new Identity<Try<int>>(TryFail<int>(new Exception("fail")));
            var mb = ma.Traverse(Prelude.identity);
            var mc = TryFail<Identity<int>>(new Exception("fail"));

            Assert.True(default(EqTry<Identity<int>>).Equals(mb, mc));
        }
        
        [Fact]
        public void IdentitySuccIsSuccIdentity()
        {
            var ma = new Identity<Try<int>>(TrySucc(1234));
            var mb = ma.Traverse(Prelude.identity);
            var mc = TrySucc(new Identity<int>(1234));

            Assert.True(default(EqTry<Identity<int>>).Equals(mb, mc));
        }
    }
}
