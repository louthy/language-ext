using LanguageExt;
//using LanguageExt.Trans;
using static LanguageExt.Prelude;
using static LanguageExt.Map;
using Xunit;
using System;
using System.Linq;

namespace LanguageExtTests
{
    
    public class MapTests
    {
        [Fact]
        public void MapGeneratorTest()
        {
            var m1 = Map<int, string>();
            m1 = add(m1, 100, "hello");
            Assert.True(m1.Count == 1 && containsKey(m1,100));
        }

        [Fact]
        public void MapGeneratorAndMatchTest()
        {
            Map<int, string> m2 = ((1, "a"), (2, "b"), (3, "c"));

            m2 = add(m2, 100, "world");

            var res = match(
                m2, 100,
                v  => v,
                () => "failed"
            );

            Assert.True(res == "world");
        }

        [Fact]
        public void MapSetTest()
        {
            var m1 = Map( Tuple(1, "a"),
                          Tuple(2, "b"),
                          Tuple(3, "c") );

            var m2 = setItem(m1, 1, "x");

            match( 
                m1, 1, 
                Some: v => Assert.True(v == "a"), 
                None: () => Assert.False(true) 
                );

            match(
                find(m2, 1),
                Some: v => Assert.True(v == "x"),
                None: () => Assert.False(true)
                );
        }

        [Fact]
        public void MapAddInOrderTest()
        {
            var m = Map(Tuple(1, 1));
            m.Find(1).IfNone(() => failwith<int>("Broken"));

            m = Map(Tuple(1, 1), Tuple(2, 2));
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));

            m = Map(Tuple(1, 1), Tuple(2, 2), Tuple(3, 3));
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));
            m.Find(3).IfNone(() => failwith<int>("Broken"));

            m = Map(Tuple(1, 1), Tuple(2, 2), Tuple(3, 3), Tuple(4, 4));
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));
            m.Find(3).IfNone(() => failwith<int>("Broken"));
            m.Find(4).IfNone(() => failwith<int>("Broken"));

            m = Map(Tuple(1, 1), Tuple(2, 2), Tuple(3, 3), Tuple(4, 4), Tuple(5, 5));
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));
            m.Find(3).IfNone(() => failwith<int>("Broken"));
            m.Find(4).IfNone(() => failwith<int>("Broken"));
            m.Find(5).IfNone(() => failwith<int>("Broken"));
        }

        [Fact]
        public void MapAddInReverseOrderTest()
        {
            var m = Map(Tuple(2, 2), Tuple(1, 1));
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));

            m = Map(Tuple(3, 3), Tuple(2, 2), Tuple(1, 1));
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));
            m.Find(3).IfNone(() => failwith<int>("Broken"));

            m = Map(Tuple(4, 4), Tuple(3, 3), Tuple(2, 2), Tuple(1, 1));
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));
            m.Find(3).IfNone(() => failwith<int>("Broken"));
            m.Find(4).IfNone(() => failwith<int>("Broken"));

            m = Map(Tuple(5, 5), Tuple(4, 4), Tuple(3, 3), Tuple(2, 2), Tuple(1, 1));
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));
            m.Find(3).IfNone(() => failwith<int>("Broken"));
            m.Find(4).IfNone(() => failwith<int>("Broken"));
            m.Find(5).IfNone(() => failwith<int>("Broken"));
        }

        [Fact]
        public void MapAddInMixedOrderTest()
        {
            var m = Map(Tuple(5, 5), Tuple(1, 1), Tuple(3, 3), Tuple(2, 2), Tuple(4, 4));
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));
            m.Find(3).IfNone(() => failwith<int>("Broken"));
            m.Find(4).IfNone(() => failwith<int>("Broken"));
            m.Find(5).IfNone(() => failwith<int>("Broken"));

            m = Map(Tuple(1, 1), Tuple(3, 3), Tuple(5, 5), Tuple(2, 2), Tuple(4, 4));
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));
            m.Find(3).IfNone(() => failwith<int>("Broken"));
            m.Find(4).IfNone(() => failwith<int>("Broken"));
            m.Find(5).IfNone(() => failwith<int>("Broken"));
        }


        [Fact]
        public void MapRemoveTest()
        {
            var m = Map(Tuple(1, "a"),
                        Tuple(2, "b"),
                        Tuple(3, "c"),
                        Tuple(4, "d"),
                        Tuple(5, "e"));

            m.Find(1).IfNone(() => failwith<string>("Broken 1"));
            m.Find(2).IfNone(() => failwith<string>("Broken 2"));
            m.Find(3).IfNone(() => failwith<string>("Broken 3"));
            m.Find(4).IfNone(() => failwith<string>("Broken 4"));
            m.Find(5).IfNone(() => failwith<string>("Broken 5"));

            Assert.True(m.Count == 5);

            m = remove(m,4);
            Assert.True(m.Count == 4);
            Assert.True(m.Find(4).IsNone);
            m.Find(1).IfNone(() => failwith<string>("Broken 1"));
            m.Find(2).IfNone(() => failwith<string>("Broken 2"));
            m.Find(3).IfNone(() => failwith<string>("Broken 3"));
            m.Find(5).IfNone(() => failwith<string>("Broken 5"));

            m = remove(m, 1);
            Assert.True(m.Count == 3);
            Assert.True(m.Find(1).IsNone);
            m.Find(2).IfNone(() => failwith<string>("Broken 2"));
            m.Find(3).IfNone(() => failwith<string>("Broken 3"));
            m.Find(5).IfNone(() => failwith<string>("Broken 5"));

            m = remove(m, 2);
            Assert.True(m.Count == 2);
            Assert.True(m.Find(2).IsNone);
            m.Find(3).IfNone(() => failwith<string>("Broken 3"));
            m.Find(5).IfNone(() => failwith<string>("Broken 5"));

            m = remove(m, 3);
            Assert.True(m.Count == 1);
            Assert.True(m.Find(3).IsNone);
            m.Find(5).IfNone(() => failwith<string>("Broken 5"));

            m = remove(m, 5);
            Assert.True(m.Count == 0);
            Assert.True(m.Find(5).IsNone);
        }

        [Fact]
        public void MassAddRemoveTest()
        {
            int max = 100000;

            var items = LanguageExt.List.map(Range(1, max), _ => Tuple(Guid.NewGuid(), Guid.NewGuid()))
                                        .ToDictionary(kv => kv.Item1, kv => kv.Item2);

            var m = Map<Guid, Guid>().AddRange(items);
            Assert.True(m.Count == max);

            foreach (var item in items)
            {
                Assert.True(m.ContainsKey(item.Key));
                m = m.Remove(item.Key);
                Assert.False(m.ContainsKey(item.Key));
                max--;
                Assert.True(m.Count == max);
            }
        }

        [Fact]
        public void MapOptionTest()
        {
            var m = Map<Option<int>, Map<Option<int>, string>>();

            m = m.AddOrUpdate(Some(1), Some(1), "Some Some");
            m = m.AddOrUpdate(None, Some(1), "None Some");
            m = m.AddOrUpdate(Some(1), None, "Some None");
            m = m.AddOrUpdate(None, None, "None None");

            Assert.True(m[Some(1)][Some(1)] == "Some Some");
            Assert.True(m[None][Some(1)] == "None Some");
            Assert.True(m[Some(1)][None] == "Some None");
            Assert.True(m[None][None] == "None None");

            Assert.True(m.CountT() == 4);

            m = m.FilterT(v => v.EndsWith("None", StringComparison.Ordinal));

            Assert.True(m.CountT() == 2);
        }

        [Fact]
        public void MapValuesTest()
        {
            var m = Map((1, 1), (2, 2), (3, 3), (4, 4), (5, 5));

            var vs = Seq(m.Values);

            Assert.True(vs.Head == 1);
            Assert.True(vs.Tail.Head == 2);
            Assert.True(vs.Tail.Tail.Head == 3);
            Assert.True(vs.Tail.Tail.Tail.Head == 4);
            Assert.True(vs.Tail.Tail.Tail.Tail.Head == 5);
            Assert.True(vs.Count == 5);
        }

        [Fact]
        public void MapKeysTest()
        {
            var m = Map((1, 1), (2, 2), (3, 3), (4, 4), (5, 5));

            var vs = Seq(m.Keys);

            Assert.True(vs.Head == 1);
            Assert.True(vs.Tail.Head == 2);
            Assert.True(vs.Tail.Tail.Head == 3);
            Assert.True(vs.Tail.Tail.Tail.Head == 4);
            Assert.True(vs.Tail.Tail.Tail.Tail.Head == 5);
            Assert.True(vs.Count == 5);
        }
    }
}
