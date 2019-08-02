using System;
using LanguageExt;
using static LanguageExt.Prelude;
using Xunit;

namespace LanguageExt.Tests
{
    public class NonNullTests
    {
        [Fact]
        public void ValueCastTest1()
        {
            Assert.Throws<ValueIsNullException>(
                () =>
                {
                    Foo(null);
                }
            );

            Assert.Throws<ValueIsNullException>(
                () =>
                {
                    string isnull = null;
                    Foo(isnull);
                }
            );
        }

        [Fact]
        public void ValueCastTest2()
        {
            // These should pass
            Foo("Hello");
            string world = "World";
            Foo(world);
        }

        [Fact]
        public void NotNullReferenceTypeTest()
        {
            Assert.Throws<ValueIsNullException>(
                () =>
                {
                    Some<string> str = null;
                });
        }

        [Fact]
        public void SomeCastsToOptionTest()
        {
            Some<string> some = "Hello";
            Option<string> opt = some;

            Assert.True(opt.IsSome && opt.IfNone("") == "Hello");
        }


        private Option<string> GetValue(bool select) =>
            select
                ? Some("Hello")
                : None;

        void Foo( Some<string> value )
        {
            if (value.Value == null)
            {
                failwith<Unit>("Value should never be null");
            }

            string doesItImplicitlyCastBackToAString = value;
        }

        void Greet(Some<string> arg)
        {
            Console.WriteLine(arg);
        }

        [Fact]
        public void AssignToSomeAfterDeclaration()
        {
            Some<string> val;
            val = "Hello";
            Assert.True(val.Value != null);
            Greet(val);
        }

        [Fact]
        public void AssignToSomeMemberAfterDeclaration()
        {
            var obj = new SomeClass();

            obj.SomeOtherValue = "123";
            Console.WriteLine(obj.SomeOtherValue);
            Assert.True(obj.SomeValue == "Hello");
            Assert.True(obj.SomeOtherValue.IsSome);
            Greet(obj.SomeOtherValue);
        }

        [Fact]
        public void AccessUninitialisedEitherMember()
        {
            var obj = new EitherClass();

            match(obj.EitherValue,
                Right: r => Console.WriteLine(r),
                Left: l => Console.WriteLine(l)
            );

            Assert.Throws<BottomException>(
                () => {

                    match(obj.EitherOtherValue,
                        Right: r => Console.WriteLine(r),
                        Left: l => Console.WriteLine(l)
                    );
                }
            );
        }


        [Fact]
        public void AccessUninitialisedSomeMember()
        {
            var obj = new SomeClass();
            Assert.Throws<SomeNotInitialisedException>(
                () => {
                    Greet(obj.SomeOtherValue);
                }
            );
        }
    }

    class SomeClass
    {
        public Some<string> SomeValue = "Hello";
        public Some<string> SomeOtherValue;
    }

    #pragma warning disable CS0649
    class EitherClass
    {
        public Either<string, int> EitherValue = "Hello";
        public Either<string, int> EitherOtherValue;
    }

}
