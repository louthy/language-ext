using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using LanguageExt;
using LanguageExt.Prelude;

namespace LanguageExtTests
{
    [TestFixture]
    public class LinqTests
    {
        [Test]
        public void WithOptionSomeList()
        {
            var res = (from v in GetOptionValue(true)
                       from r in range(1, 10)
                       select v * r)
                      .ToList();

            Assert.IsTrue(res.Count() == 10);
            Assert.IsTrue(res[0] == 10);
            Assert.IsTrue(res[9] == 100);
        }

        [Test]
        public void WithOptionNoneList()
        {
            var res = (from v in GetOptionValue(false)
                       from r in range(1, 10)
                       select v * r)
                      .ToList();

            Assert.IsTrue(res.Count() == 0);
        }

        [Test]
        public void WithOptionUnsafeSomeList()
        {
            var res = (from v in GetOptionUnsafeValue(true)
                       from r in range(1, 10)
                       select v * r)
                      .ToList();

            Assert.IsTrue(res.Count() == 10);
            Assert.IsTrue(res[0] == 10);
            Assert.IsTrue(res[9] == 100);
        }

        [Test]
        public void WithOptionUnsafeNoneList()
        {
            var res = (from v in GetOptionUnsafeValue(false)
                       from r in range(1, 10)
                       select v * r)
                      .ToList();

            Assert.IsTrue(res.Count() == 0);
        }

        [Test]
        public void WithEitherRightList()
        {
            var res = (from v in GetEitherValue(true)
                       from r in range(1, 10)
                       select v * r)
                      .ToList();

            Assert.IsTrue(res.Count() == 10);
            Assert.IsTrue(res[0] == 10);
            Assert.IsTrue(res[9] == 100);
        }

        [Test]
        public void WithEitherLeftList()
        {
            var res = (from v in GetEitherValue(false)
                       from r in range(1, 10)
                       select v * r)
                      .ToList();

            Assert.IsTrue(res.Count() == 0);
        }

        [Test]
        public void WithEitherUnsafeRightList()
        {
            var res = (from v in GetEitherUnsafeValue(true)
                       from r in range(1, 10)
                       select v * r)
                      .ToList();

            Assert.IsTrue(res.Count() == 10);
            Assert.IsTrue(res[0] == 10);
            Assert.IsTrue(res[9] == 100);
        }

        [Test]
        public void WithEitherUnsafeLeftList()
        {
            var res = (from v in GetEitherUnsafeValue(false)
                       from r in range(1, 10)
                       select v * r)
                      .ToList();

            Assert.IsTrue(res.Count() == 0);
        }

        [Test]
        public void WithTryOptionSomeList()
        {
            var res = (from v in GetTryOptionValue(true).AsEnumerable()
                       from r in range(1, 10)
                       select v * r)
                      .ToList();

            Assert.IsTrue(res.Count() == 10);
            Assert.IsTrue(res[0] == 10);
            Assert.IsTrue(res[9] == 100);
        }

        [Test]
        public void WithTryOptionNoneList()
        {
            var res = (from v in GetTryOptionValue(false).AsEnumerable()
                       from r in range(1, 10)
                       select v * r)
                      .ToList();

            Assert.IsTrue(res.Count() == 0);
        }

        [Test]
        public void WithTryOptionErrorList()
        {
            var res = (from v in GetTryOptionError().AsEnumerable()
                       from r in range(1, 10)
                       select v * r)
                      .ToList();

            Assert.IsTrue(res.Count() == 0);
        }

        private Option<int> GetOptionValue(bool select) =>
            select
                ? Some(10)
                : None;

        private OptionUnsafe<int> GetOptionUnsafeValue(bool select) =>
            select
                ? SomeUnsafe(10)
                : None;

        private Either<int, string> GetEitherValue(bool select)
        {
            if (select)
                return 10;
            else
                return "left";
        }

        private EitherUnsafe<int, string> GetEitherUnsafeValue(bool select)
        {
            if (select)
                return 10;
            else
                return "left";
        }

        private TryOption<int> GetTryOptionValue(bool select) => () =>
            select
                ? Some(10)
                : None;

        private TryOption<int> GetTryOptionError() => () =>
            failwith<TryOptionResult<int>>("failed!");

    }
}
