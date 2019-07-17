using Xunit;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Linq;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests
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


        [Fact]
        public static void OptionMonadEqualityTests1()
        {
            var optionx = Some(123);
            var optiony = Some(123);

            var optionr = IsEqual<EqInt, MOption<int>, Option<int>, int>(optionx, optiony);

            Assert.True(optionr);
            Assert.True(optionx == optiony);
        }

        [Fact]
        public static void OptionMonadEqualityTests2()
        {
            var optionx = Some("ABC");
            var optiony = Some("abc");

            var optionr = IsEqual<EqStringCurrentCultureIgnoreCase, MOption<string>, Option<string>, string>(optionx, optiony);

            Assert.True(optionr);

            Assert.True(optionx != optiony);
        }

        [Fact]
        public static void EitherMonadEqualityTests1()
        {
            var optionx = Right<Exception, int>(123);
            var optiony = Right<Exception, int>(123);

            var optionr = IsEqual<EqInt, MEither<Exception, int>, Either<Exception, int>, int>(optionx, optiony);
            Assert.True(optionr);

            Assert.True(optionx == optiony);
        }

        [Fact]
        public static void EitherMonadEqualityTests2()
        {
            var optionx = Right<Exception, string>("ABC");
            var optiony = Right<Exception, string>("abc");

            var optionr = IsEqual<EqStringCurrentCultureIgnoreCase, MEither<Exception, string>, Either<Exception, string>, string>(optionx, optiony);

            Assert.True(optionr);
            Assert.True(optionx != optiony);
        }

        public static bool IsEqual<EqA, MonadA, MA, A>(MA mx, MA my)
            where EqA : struct, Eq<A>
            where MonadA : struct, Monad<MA, A> =>
            default(MonadA).Fold(mx, false, (s1, x) =>
                default(MonadA).Fold(my, false, (s2, y) =>
                    default(EqA).Equals(x, y))(unit))(unit);
    }


    public class EqualityTestsWithStaticProperties
    {
        public class Foo : Record<Foo>
        {
            public static Foo Default { get; } 

            static Foo()
            {
                Default = new Foo( 10,20,List(1,2) );
            }

            public Foo(int age, int s, Lst<int> numbers)
            {
                Age = age;
                String = s;
                Numbers = numbers;
            }
            public int Age { get; }
            public int String { get; }
            public Lst<int> Numbers { get; }
        }

        [Fact]
        public void EqualRecordsShouldBeEqual()
        {

            var a = new Foo( 10, 20, List( 0, 1, 2 ) );
            var b = new Foo( 10, 20, List( 0, 1, 2 ) );

            Assert.Equal( b, a );

        }

        [Fact]
        public void NonEqualRecordsShouldBeNotEqual()
        {

            var a = new Foo( 10, 20, List( 0, 1, 2 ) );
            var b = new Foo( 10, 20, List( 0, -1, 2 ) );

            Assert.NotEqual( b, a );

        }
    }
}
