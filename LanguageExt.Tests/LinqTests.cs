using System;
using System.Linq;
using NUnit.Framework;
using LanguageExt;
using static LanguageExt.Prelude;

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
            var res = (from v in match( 
                                     GetTryOptionValue(true).AsEnumerable(), 
                                     Right: r => list(r),
                                     Left:  l => list<int>()
                                 )
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
            var res = (from v in GetTryOptionValue(false).AsEnumerable().Failure( list<int>() )
                       from r in range(1, 10)
                       select v * r)
                      .ToList();

            Assert.IsTrue(res.Count() == 0);
        }

        [Test]
        public void WithTryOptionErrorList()
        {
            match( GetTryOptionError().AsEnumerable().First(),
                Right: r => Assert.Fail(),
                Left:  e => Assert.IsTrue(true)
            );

            var res = (from v in GetTryOptionError().AsEnumerable().FailWithEmpty()
                       from r in range(1, 10)
                       select v * r)
                      .ToList();

            Assert.IsTrue(res.Count() == 0);
        }

        [Test]
        public void WhereOptionTest()
        {
            var res1 = from v in GetOptionValue(true)
                       where v == 10
                       select v;

            Assert.IsTrue(res1.Failure(0) == 10);

            var res2 = from v in GetOptionValue(false)
                       where v == 10
                       select v;

            Assert.IsTrue(res2.Failure(0) == 0);
        }

        [Test]
        public void WhereOptionUnsafeTest()
        {
            var res1 = from v in GetOptionUnsafeValue(true)
                       where v == 10
                       select v;

            Assert.IsTrue(res1.FailureUnsafe(0) == 10);

            var res2 = from v in GetOptionUnsafeValue(false)
                       where v == 10
                       select v;

            Assert.IsTrue(res2.FailureUnsafe(0) == 0);
        }


        [Test]
        public void WhereTryOptionTest()
        {
            var res1 = from v in GetTryOptionValue(0)
                       where v == 10
                       select v;

            Assert.IsTrue(res1.Failure(0) == 10);

            var res2 = from v in GetTryOptionValue(1)
                       where v == 10
                       select v;

            Assert.IsTrue(res2.Failure(0) == 0);

            var res3 = match(from v in GetTryOptionValue(2)
                             where v == 10
                             select v,
                             Some: x  => 1,
                             None: () => 2,
                             Fail: ex => 3);

            Assert.IsTrue(res3 == 3);
        }

        [Test]
        public void OptionAndOrTest()
        {
            Option<int> optional1 = None;
            Option<int> optional2 = Some(10);
            Option<int> optional3 = Some(20);

            var res = from x in optional1 || optional2
                      from y in optional3
                      select x + y;

            Assert.IsTrue(res == Some(30));
        }

        private TryOption<int> GetTryOptionValue(int state) => () =>
        {
            switch (state)
            {
                case 0: return Some(10);
                case 1: return None;
                default: throw new Exception("eerrr");
            }
        };

        private Option<int> GetOptionValue(bool select) =>
            select
                ? Some(10)
                : None;

        private OptionUnsafe<int> GetOptionUnsafeValue(bool select) =>
            select
                ? SomeUnsafe(10)
                : None;

        private Either<string, int> GetEitherValue(bool select)
        {
            if (select)
                return 10;
            else
                return "left";
        }

        private EitherUnsafe<string, int> GetEitherUnsafeValue(bool select)
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
