using System;
using LanguageExt.ClassInstances;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.TryOptionT.Sync
{
    public class IdentityTryOption
    {
        [Fact]
        public void IdentityFailIsFail()
        {
            var ma = new Identity<TryOption<int>>(TryOptionFail<int>(new Exception("fail")));
            var mb = ma.Traverse(identity);
            var mc = TryOptionFail<Identity<int>>(new Exception("fail"));

            Assert.True(default(EqTryOption<Identity<int>>).Equals(mb, mc));
        }

        [Fact]
        public void IdentitySuccIsSuccIdentity()
        {
            var ma = new Identity<TryOption<int>>(TryOptionSucc(1234));
            var mb = ma.Traverse(identity);
            var mc = TryOptionSucc(new Identity<int>(1234));

            Assert.True(default(EqTryOption<Identity<int>>).Equals(mb, mc));
        }
    }
}
