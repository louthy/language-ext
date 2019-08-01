//using Xunit;
//using System;
//using System.Collections.Generic;
//using LanguageExt;
//using static LanguageExt.Prelude;

//namespace LanguageExt.Tests
//{
//    public class OptionLazyTests
//    {
//        [Fact]
//        public void OptionLazyTest1()
//        {
//            var items = List<int>();

//            var option = from w in Some(_ => { items = items.Add(5); return 5; })
//                         from x in Some(_ => { items = items.Add(10); return 10; })
//                         from y in Some(_ => { items = items.Add(15); return 15; })
//                         from z in Some(_ => { items = items.Add(20); return 20; })
//                         select w + x + y + z;

//            Assert.True(items.Count == 0);

//            var value = option.IfNone(0);

//            Assert.True(items.Count == 4);
//            Assert.True(value == 50);

//            var value2 = option.IfNone(0);
//            var value3 = option.IfNone(0);
//            var value4 = option.IfNone(0);
//            var value5 = option.IfNone(0);
//            var value6 = option.IfNone(0);

//            Assert.True(items.Count == 4);
//            Assert.True(value6 == 50);
//        }

//        [Fact]
//        public void OptionLazyStrictCombinedTest1()
//        {
//            var items = List<int>();

//            var option = from w in Some(_ => { items = items.Add(5); return 5; })
//                         from x in Some(_ => { items = items.Add(10); return 10; })
//                         from y in Some(_ => { items = items.Add(15); return 15; })
//                         from z in Some(_ => { items = items.Add(20); return 20; })
//                         from i in Some(30)
//                         select w + x + y + z + i;

//            Assert.True(items.Count == 0);

//            var value = option.IfNone(0);

//            Assert.True(items.Count == 4);
//            Assert.True(value == 80);

//            var value2 = option.IfNone(0);
//            var value3 = option.IfNone(0);
//            var value4 = option.IfNone(0);
//            var value5 = option.IfNone(0);
//            var value6 = option.IfNone(0);

//            Assert.True(items.Count == 4);
//            Assert.True(value6 == 80);
//        }

//        [Fact]
//        public void OptionLazyStrictCombinedTest2()
//        {
//            var items = List<int>();

//            var option = from i in Some(30)
//                         from w in Some(_ => { items = items.Add(5); return 5; })
//                         from x in Some(_ => { items = items.Add(10); return 10; })
//                         from y in Some(_ => { items = items.Add(15); return 15; })
//                         from z in Some(_ => { items = items.Add(20); return 20; })
//                         select w + x + y + z + i;

//            Assert.True(items.Count == 0);

//            var value = option.IfNone(0);

//            Assert.True(items.Count == 4);
//            Assert.True(value == 80);

//            var value2 = option.IfNone(0);
//            var value3 = option.IfNone(0);
//            var value4 = option.IfNone(0);
//            var value5 = option.IfNone(0);
//            var value6 = option.IfNone(0);

//            Assert.True(items.Count == 4);
//            Assert.True(value6 == 80);
//        }

//        [Fact]
//        public void OptionStrictOnlyTest()
//        {
//            var items = List<int>();

//            var option = from t in Some(30)
//                         let _a = items = items.Add(5)
//                         from u in Some(30)
//                         let _b = items = items.Add(10)
//                         from v in Some(30)
//                         let _c = items = items.Add(15)
//                         select t * u * v;

//            Assert.True(!option.IsLazy);
//            Assert.True(items.Count == 3);

//            var res = option.IfNone(0);

//            Assert.True(res == 27000);
//        }

//        [Fact]
//        public void OptionLazyMapTest()
//        {
//            var n = 0;

//            var x = Some(_ => ++n);

//            Assert.True(x.IsLazy);
//            Assert.True(n == 0);

//            var y = x.Map(v => v * 10);

//            Assert.True(x.IsLazy);
//            Assert.True(n == 0);

//            Assert.True(y == 10);
//            Assert.True(n == 1);
//        }

//        [Fact]
//        public void OptionLazyFilter()
//        {
//            var items = List<int>();

//            var option = from i in Some(30)
//                         where i > 10
//                         from w in Some(_ => { items = items.Add(5); return 5; })
//                         where w > 4
//                         from x in Some(_ => { items = items.Add(10); return 10; })
//                         where x > 9
//                         from y in Some(_ => { items = items.Add(15); return 15; })
//                         where y > 14
//                         from z in Some(_ => { items = items.Add(20); return 20; })
//                         where z > 19
//                         select w + x + y + z + i;

//            Assert.True(items.Count == 0);

//            var value = option.IfNone(0);

//            Assert.True(items.Count == 4);
//            Assert.True(value == 80);

//            var value2 = option.IfNone(0);
//            var value3 = option.IfNone(0);
//            var value4 = option.IfNone(0);
//            var value5 = option.IfNone(0);
//            var value6 = option.IfNone(0);

//            Assert.True(items.Count == 4);
//            Assert.True(value6 == 80);
//        }
//    }
//}
