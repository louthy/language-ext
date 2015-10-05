using Xunit;
using LanguageExt;
using static LanguageExt.Prelude;
using System;


namespace LanguageExtTests
{
    
    public class EqualityTests
    {
        [Fact]
        public void EqualityTest1()
        {
            var optional = Some(123);

            if (optional == 123)
            {
                Assert.True(true);
            }
            else
            {
                Assert.False(true);
            }
        }

        [Fact]
        public void EqualityTest2()
        {
            Option<int> optional = None;

            if (optional == None)
            {
                Assert.True(true);
            }
            else
            {
                Assert.False(true);
            }
        }

        [Fact]
        public void EqualityTest3()
        {
            var optional = Some(123);

            if (optional == None)
            {
                Assert.False(true);
            }
            else
            {
                Assert.True(true);
            }
        }

        [Fact]
        public void EqualityTest4()
        {
            Option<int> optional = None;

            if (optional == Some(123))
            {
                Assert.False(true);
            }
            else
            {
                Assert.True(true);
            }
        }

        [Fact]
        public void NonEqualityTest1()
        {
            var optional = Some(123);

            if (optional != 123)
            {
                Assert.False(true);
            }
            else
            {
                Assert.True(true);
            }
        }

        [Fact]
        public void NonEqualityTest2()
        {
            Option<int> optional = None;

            if (optional != None)
            {
                Assert.False(true);
            }
            else
            {
                Assert.True(true);
            }
        }
    }
}
