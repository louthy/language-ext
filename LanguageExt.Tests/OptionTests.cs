using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExtTests
{
    
    public class OptionTests
    {
        [Fact]
        public void SomeGeneratorTestsObject()
        {
            var optional = Some(123);

            optional.Match(Some: i => Assert.True(i == 123),
                           None: () => Assert.False(true,"Shouldn't get here"));

            int c = optional.Match(Some: i => i + 1,
                                   None: () => 0);

            Assert.True(c == 124);
        }

        [Fact]
        public void SomeGeneratorTestsFunction()
        {
            var optional = Some(123);

            match(optional, Some: i => Assert.True(i == 123),
                            None: () => Assert.False(true,"Shouldn't get here"));

            int c = match(optional, Some: i => i + 1,
                                    None: () => 0);

            Assert.True(c == 124);
        }

        [Fact]
        public void NoneGeneratorTestsObject()
        {
            Option<int> optional = None;

            optional.Match(Some: i => Assert.False(true,"Shouldn't get here"),
                           None: () => Assert.True(true));

            int c = optional.Match(Some: i => i + 1,
                                   None: () => 0);

            Assert.True(c == 0);
        }

        [Fact]
        public void NoneGeneratorTestsFunction()
        {
            Option<int> optional = None;

            match(optional, Some: i => Assert.False(true,"Shouldn't get here"),
                            None: () => Assert.True(true));

            int c = match(optional, Some: i => i + 1,
                                    None: () => 0);

            Assert.True(c == 0);
        }

        [Fact]
        public void SomeLinqTest()
        {
            var two = Some(2);
            var four = Some(4);
            var six = Some(6);

            var expr = from x in two
                       from y in four
                       from z in six
                       select x + y + z;

            match(expr,
                Some: v => Assert.True(v == 12),
                None: failwith("Shouldn't get here"));
        }

        [Fact]
        public void NoneLinqTest()
        {
            var two = Some(2);
            var four = Some(4);
            var six = Some(6);
            Option<int> none = None;

            match(from x in two
                  from y in four
                  from _ in none
                  from z in six
                  select x + y + z,
                   Some: v => failwith<int>("Shouldn't get here"),
                   None: () => Assert.True(true));
        }

        [Fact]
        public void NullIsNotSomeTest()
        {
            Assert.Throws<ValueIsNullException>(
                () =>
                {
                    GetStringNone();
                }
            );
        }

        [Fact]
        public void NullIsNoneTest()
        {
            Assert.True(GetStringNone2().IsNone);
        }

        [Fact]
        public void OptionFluentSomeNoneTest()
        {
            int res1 = GetValue(true)
                        .Some(x => x + 10)
                        .None(0);

            int res2 = GetValue(false)
                        .Some(x => x + 10)
                        .None(() => 0);

            Assert.True(res1 == 1010);
            Assert.True(res2 == 0);
        }

        [Fact]
        public void NullInSomeOrNoneTest()
        {
            Assert.Throws<ResultIsNullException>(
                () =>
                {
                    GetValue(true)
                       .Some(x => (string)null)
                       .None((string)null);
                }
            );

            Assert.Throws<ResultIsNullException>(
                () =>
                {
                    GetValue(false)
                       .Some(x => (string)null)
                       .None((string)null);
                }
            );
        }

        [Fact]
        public void NullableTest()
        {
            var res = GetNullable(true)
                        .Some(v => v)
                        .None(() => 0);

            Assert.True(res == 1000);
        }

        [Fact]
        public void NullableDenySomeNullTest()
        {
            Assert.Throws<ValueIsNullException>(
                    () =>
                    {
                        var res = GetNullable(false)
                                    .Some(v => v)
                                    .None(() => 0);
                    }
                );
        }

        [Fact]
        public void BiIterSomeTest()
        {
            var x = Some(3);
            int way = 0;
            var dummy = x.BiIter(_ => way = 1, () => way = 2);
            Assert.Equal(1, way);
        }

        [Fact]
        public void BiIterNoneTest()
        {
            var x = Option<int>.None;
            int way = 0;
            var dummy = x.BiIter(_ => way = 1, () => way = 2);
            Assert.Equal(2, way);
        }

        [Fact]
        public void ToArrayTest()
        {
            var x = Option<int>.None;
            var arr1 = x.ToArray();
            Assert.Equal(0, arr1.Count);
#pragma warning disable CS0183 // 'is' expression's given expression is always of the provided type
            Assert.True(arr1 is Arr<int>);
#pragma warning restore CS0183 // 'is' expression's given expression is always of the provided type

            var y = Option<int>.Some(10);
            var arr2 = y.ToArray();
            Assert.Equal(1, arr2.Count);
            Assert.Equal(10, arr2[0]);
        }

        [Fact]
        public void ToListTest()
        {
            var x = Option<int>.None;
            var lst1 = x.ToList();
            Assert.Equal(0, lst1.Count);
#pragma warning disable CS0183 // 'is' expression's given expression is always of the provided type
            Assert.True(lst1 is Lst<int>);
#pragma warning restore CS0183 // 'is' expression's given expression is always of the provided type

            var y = Option<int>.Some(10);
            var lst2 = y.ToList();
            Assert.Equal(1, lst2.Count);
            Assert.Equal(10, lst2[0]);
        }

        [Fact]
        public void ToSeqTest()
        {
            var x = Option<int>.None;
            var seq1 = x.ToSeq();
            Assert.Equal(0, seq1.Count);
            Assert.True(seq1 is Seq<int>);

            var y = Option<int>.Some(10);
            var seq2 = y.ToSeq();
            Assert.Equal(1, seq2.Count);
            Assert.Equal(10, seq2.First());
        }

        [Fact]
        public void AsEnumerableTest()
        {
            var x = Option<int>.None;
            var enmrbl1 = x.AsEnumerable();
            Assert.False(enmrbl1.Any());
            Assert.True(enmrbl1 is IEnumerable<int>);

            var y = Option<int>.Some(10);
            var enmrbl2 = y.AsEnumerable();
            Assert.True(enmrbl2.Any());
            Assert.Equal(10, enmrbl2.First());
        }

        private Option<string> GetStringNone()
        {
            // This should fail
            string nullStr = null;
            return Some(nullStr);
        }

        private Option<string> GetStringNone2()
        {
            // This should be coerced to None
            string nullStr = null;
            return nullStr;
        }

        private Option<int> GetNullable(bool select) =>
            select
                ? Some((int?)1000)
                : Some((int?)null);

        private Option<int> GetValue(bool select) =>
            select
                ? Some(1000)
                : None;

        private Option<Option<int>> GetSomeOptionValue(bool select) =>
            select
                ? Some(Some(1000))
                : Some(Option<int>.None);

        private Option<int> ImplicitConversion() => 1000;

        private Option<int> ImplicitNoneConversion() => None;

        private void InferenceTest1()
        {
            Action<int> actionint = v => v = v * 2;
            Option<int> optional1 = Some(123);
            optional1.Some(actionint) //Compiler tries to call:  public static Option<T> Some(T value)
                     .None(() => { });
        }
    }
}
