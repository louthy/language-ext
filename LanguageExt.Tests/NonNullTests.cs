using System;
using LanguageExt;
using LanguageExt.Prelude;
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
    }
}
