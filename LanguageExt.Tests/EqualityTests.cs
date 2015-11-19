using Xunit;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Linq;
using System.Collections.Generic;

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

        /// <summary>
        /// Test for issue #64
        /// It just needs to complete without throwing an exception to be tested
        /// https://github.com/louthy/language-ext/issues/64
        /// </summary>
        [Fact]
        public void EitherEqualityComparerTest()
        {
            var results = List<Either<Exception, int>>();

            var firsterror = results.FirstOrDefault(i => i.IsLeft);
            if (IsDefault(firsterror)) // <-- here i get exception
            {
            }
        }

        /// <summary>
        /// Test for issue #64
        /// It just needs to complete without throwing an exception to be tested
        /// https://github.com/louthy/language-ext/issues/64
        /// </summary>
        [Fact]
        public void EitherUnsafeEqualityComparerTest()
        {
            var results = List<EitherUnsafe<Exception, int>>();

            var firsterror = results.FirstOrDefault(i => i.IsLeft);
            if (IsDefault(firsterror)) // <-- here i get exception
            {
            }
        }

        public static bool IsDefault<T>(T obj) =>
            EqualityComparer<T>.Default.Equals(obj, default(T));
    }
}
