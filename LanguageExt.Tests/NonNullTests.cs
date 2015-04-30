using System;
using LanguageExt;
using static LanguageExt.Prelude;
using NUnit.Framework;

namespace LanguageExtTests
{
    [TestFixture]
    public class NonNullTests
    {
        [Test]
        public void ValueCastTest1()
        {
            Assert.Throws(
                typeof(ValueIsNullException),
                () =>
                {
                    Foo(null);
                }
            );

            Assert.Throws(
                typeof(ValueIsNullException),
                () =>
                {
                    string isnull = null;
                    Foo(isnull);
                }
            );
        }

        [Test]
        public void ValueCastTest2()
        {
            // These should pass
            Foo("Hello");
            string world = "World";
            Foo(world);
        }

        [Test]
        public void NotNullReferenceTypeTest()
        {
            Assert.Throws(
                typeof(ValueIsNullException),
                () =>
                {
                    Some<string> str = null;
                });
        }

        [Test]
        public void SomeCastsToOptionTest()
        {
            Some<string> some = "Hello";
            Option<string> opt = some;

            Assert.IsTrue(opt.IsSome && opt.Failure("") == "Hello");
        }


        private Option<string> GetValue(bool select) =>
            select
                ? Some("Hello")
                : None;

        public void Foo( Some<string> value )
        {
            if (value.Value == null)
            {
                failwith<Unit>("Value should never be null");
            }

            string doesItImplicitlyCastBackToAString = value;
        }

        public void Greet(Some<string> arg)
        {
            Console.WriteLine(arg);
        }

        [Test]
        public void AssignToSomeAfterDeclaration()
        {
            Some<string> val;
            val = "Hello";
            Assert.IsTrue(val.Value != null);
            Greet(val);
        }

        [Test]
        public void AssignToSomeMemberAfterDeclaration()
        {
            var obj = new SomeClass();

            obj.SomeOtherValue = "123";
            Console.WriteLine(obj.SomeOtherValue);
            Assert.IsTrue(obj.SomeValue == "Hello");
            Assert.IsTrue(obj.SomeOtherValue != null);
            Greet(obj.SomeOtherValue);
        }

        [Test]
        public void AccessUninitialisedEitherMember()
        {
            var obj = new EitherClass();

            match(obj.EitherValue,
                Right: r => Console.WriteLine(r),
                Left: l => Console.WriteLine(l)
            );

            Assert.Throws(
                typeof(EitherNotInitialisedException),
                () => {

                    match(obj.EitherOtherValue,
                        Right: r => Console.WriteLine(r),
                        Left: l => Console.WriteLine(l)
                    );
                }
            );
        }


        [Test]
        public void AccessUninitialisedSomeMember()
        {
            var obj = new SomeClass();
            Assert.Throws(
                typeof(SomeNotInitialisedException),
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

    //#Disable Warning CS0649

    class EitherClass
    {
        public Either<string, int> EitherValue = "Hello";
        public Either<string, int> EitherOtherValue;
    }

}
