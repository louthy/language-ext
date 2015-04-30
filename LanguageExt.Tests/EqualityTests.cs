using NUnit.Framework;
using LanguageExt;
using static LanguageExt.Prelude;
using System;


namespace LanguageExtTests
{
    [TestFixture]
    public class EqualityTests
    {
        [Test]
        public void EqualityTest1()
        {
            var optional = Some(123);

            if (optional == 123)
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void EqualityTest2()
        {
            Option<int> optional = None;

            if (optional == None)
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.Fail();
            }
        }

        [Test]
        public void EqualityTest3()
        {
            var optional = Some(123);

            if (optional == None)
            {
                Assert.Fail();
            }
            else
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void EqualityTest4()
        {
            Option<int> optional = None;

            if (optional == Some(123))
            {
                Assert.Fail();
            }
            else
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void NonEqualityTest1()
        {
            var optional = Some(123);

            if (optional != 123)
            {
                Assert.Fail();
            }
            else
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void NonEqualityTest2()
        {
            Option<int> optional = None;

            if (optional != None)
            {
                Assert.Fail();
            }
            else
            {
                Assert.IsTrue(true);
            }
        }
    }
}
