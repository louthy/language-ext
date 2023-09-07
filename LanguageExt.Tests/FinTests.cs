using System;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    
    public class FinTests
    {
        [Fact] public void Equality()
        {
            var fin1 = FinSucc(1);
            Assert.Equal(1, fin1);
        }
        
        [Fact] public void Enumerability()
        {
            var fin1 = FinSucc(1);
            Assert.Equal(1, fin1.HeadOrNone());
        }
    }
}
