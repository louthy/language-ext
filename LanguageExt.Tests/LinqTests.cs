using System;
using System.Linq;
using Xunit;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExtTests
{
    
    public class LinqTests
    {
        [Fact]
        public void WithOptionSomeList()
        {
            var res = from v in GetOptionValue(true)
                      from r in Range(1, 10)
                      select v * r;

            var res2 = res.ToList();

            Assert.True(res2.Count() == 10);
            Assert.True(res2[0] == 10);
            Assert.True(res2[9] == 100);
        }

        [Fact]
        public void WithOptionNoneList()
        {
            var res = from v in GetOptionValue(false)
                      from r in Range(1, 10)
                      select v * r;

            Assert.True(res.Count() == 0);
        }

        [Fact]
        public void WithOptionUnsafeSomeList()
        {
            var res = (from v in GetOptionUnsafeValue(true)
                       from r in Range(1, 10)
                       select v * r)
                      .ToList();

            Assert.True(res.Count() == 10);
            Assert.True(res[0] == 10);
            Assert.True(res[9] == 100);
        }

        [Fact]
        public void WithOptionUnsafeNoneList()
        {
            var res = from v in GetOptionUnsafeValue(false)
                      from r in Range(1, 10)
                      select v * r;

            Assert.True(res.Count() == 0);
        }

        [Fact]
        public void WithEitherRightList()
        {
            var res = from v in seq(GetEitherValue(true))
                      from r in Range(1, 10)
                      select v * r;

            var res2 = res.ToList();

            Assert.True(res.Count() == 10);
            Assert.True(res2[0] == 10);
            Assert.True(res2[9] == 100);
        }

        [Fact]
        public void WithEitherLeftList()
        {
            var res = from v in seq(GetEitherValue(false))
                      from r in Range(1, 10)
                      select v * r;

            Assert.True(res.Count() == 0);
        }

        [Fact]
        public void WithEitherUnsafeRightList()
        {
            var res = from v in seq(GetEitherUnsafeValue(true))
                      from r in Range(1, 10)
                      select v * r;

            var res2 = res.ToList();

            Assert.True(res.Count() == 10);
            Assert.True(res2[0] == 10);
            Assert.True(res2[9] == 100);
        }

        [Fact]
        public void WithEitherUnsafeLeftList()
        {
            var res = from v in seq(GetEitherUnsafeValue(false))
                      from r in Range(1, 10)
                      select v * r;

            Assert.True(res.Count() == 0);
        }

        [Fact]
        public void WithTryOptionSomeList()
        {
            var res = from v in seq(GetTryOptionValue(true))
                      from r in Range(1, 10)
                      select v * r;

            var res2 = res.ToList();

            Assert.True(res.Count() == 10);
            Assert.True(res2[0] == 10);
            Assert.True(res2[9] == 100);
        }

        [Fact]
        public void WithTryOptionNoneList()
        {
            var res = from v in seq(GetTryOptionValue(false))
                      from r in Range(1, 10)
                      select v * r;

            Assert.True(res.Count() == 0);
        }

        [Fact]
        public void WhereOptionTest()
        {
            var res1 = from v in GetOptionValue(true)
                       where v == 10
                       select v;

            Assert.True(res1.IfNone(0) == 10);

            var res2 = from v in GetOptionValue(false)
                       where v == 10
                       select v;

            Assert.True(res2.IfNone(0) == 0);
        }

        [Fact]
        public void WhereOptionUnsafeTest()
        {
            var res1 = from v in GetOptionUnsafeValue(true)
                       where v == 10
                       select v;

            Assert.True(res1.IfNoneUnsafe(0) == 10);

            var res2 = from v in GetOptionUnsafeValue(false)
                       where v == 10
                       select v;

            Assert.True(res2.IfNoneUnsafe(0) == 0);
        }


        [Fact]
        public void WhereTryOptionTest()
        {
            var res1 = from v in GetTryOptionValue(0)
                       where v == 10
                       select v;

            Assert.True(res1.IfNoneOrFail(0) == 10);

            var res2 = from v in GetTryOptionValue(1)
                       where v == 10
                       select v;

            Assert.True(res2.IfNoneOrFail(0) == 0);

            var res3 = match(from v in GetTryOptionValue(2)
                             where v == 10
                             select v,
                             Some: x  => 1,
                             None: () => 2,
                             Fail: ex => 3);

            Assert.True(res3 == 3);
        }

        [Fact]
        public void OptionAndOrTest()
        {
            Option<int> optional1 = None;
            Option<int> optional2 = Some(10);
            Option<int> optional3 = Some(20);

            var res = from x in optional1 || optional2
                      from y in optional3
                      select x + y;

            Assert.True(res == Some(30));
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
            failwith<int>("failed!");

        [Fact]
        public void OptionLst1()
        {
            var list = List(1, 2, 3, 4);
            var opt = Some(5);

            var res = from y in opt
                      from x in list
                      select x + y;
        }

    }
}
