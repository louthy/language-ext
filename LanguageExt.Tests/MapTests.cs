using LanguageExt;
using LanguageExt.Trans;
using static LanguageExt.Prelude;
using static LanguageExt.Map;
using NUnit.Framework;
using System;
using System.Linq;

namespace LanguageExtTests
{
    [TestFixture]
    public class MapTests
    {
        [Test]
        public void MapGeneratorTest()
        {
            var m1 = Map<int, string>();
            m1 = add(m1, 100, "hello");
            Assert.IsTrue(m1.Count == 1 && containsKey(m1,100));
        }

        [Test]
        public void MapGeneratorAndMatchTest()
        {
            var m2 = Map( Tuple(1, "a"),
                          Tuple(2, "b"),
                          Tuple(3, "c") );

            m2 = add(m2, 100, "world");

            var res = match(
                m2, 100,
                v  => v,
                () => "failed"
            );

            Assert.IsTrue(res == "world");
        }

        [Test]
        public void MapSetTest()
        {
            var m1 = Map( Tuple(1, "a"),
                          Tuple(2, "b"),
                          Tuple(3, "c") );

            var m2 = setItem(m1, 1, "x");

            match( 
                m1, 1, 
                Some: v => Assert.IsTrue(v == "a"), 
                None: () => Assert.Fail() 
                );

            match(
                find(m2, 1),
                Some: v => Assert.IsTrue(v == "x"),
                None: () => Assert.Fail()
                );
        }

        [Test]
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

        [Test]
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

        [Test]
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


        [Test]
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

            Assert.IsTrue(m.Count == 5);

            m = remove(m,4);
            Assert.IsTrue(m.Count == 4);
            Assert.IsTrue(m.Find(4).IsNone);
            m.Find(1).IfNone(() => failwith<string>("Broken 1"));
            m.Find(2).IfNone(() => failwith<string>("Broken 2"));
            m.Find(3).IfNone(() => failwith<string>("Broken 3"));
            m.Find(5).IfNone(() => failwith<string>("Broken 5"));

            m = remove(m, 1);
            Assert.IsTrue(m.Count == 3);
            Assert.IsTrue(m.Find(1).IsNone);
            m.Find(2).IfNone(() => failwith<string>("Broken 2"));
            m.Find(3).IfNone(() => failwith<string>("Broken 3"));
            m.Find(5).IfNone(() => failwith<string>("Broken 5"));

            m = remove(m, 2);
            Assert.IsTrue(m.Count == 2);
            Assert.IsTrue(m.Find(2).IsNone);
            m.Find(3).IfNone(() => failwith<string>("Broken 3"));
            m.Find(5).IfNone(() => failwith<string>("Broken 5"));

            m = remove(m, 3);
            Assert.IsTrue(m.Count == 1);
            Assert.IsTrue(m.Find(3).IsNone);
            m.Find(5).IfNone(() => failwith<string>("Broken 5"));

            m = remove(m, 5);
            Assert.IsTrue(m.Count == 0);
            Assert.IsTrue(m.Find(5).IsNone);
        }

        [Test]
        public void MassAddRemoveTest()
        {
            int max = 100000;

            var items = LanguageExt.List.map(Range(1, max), _ => Tuple(Guid.NewGuid(), Guid.NewGuid()))
                                        .ToDictionary(kv => kv.Item1, kv => kv.Item2);

            var m = Map<Guid, Guid>().AddRange(items);
            Assert.IsTrue(m.Count == max);

            foreach (var item in items)
            {
                Assert.IsTrue(m.ContainsKey(item.Key));
                m = m.Remove(item.Key);
                Assert.IsFalse(m.ContainsKey(item.Key));
                max--;
                Assert.IsTrue(m.Count == max);
            }
        }

        [Test]
        public void MapOptionTest()
        {
            var m = Map<Option<int>, Map<Option<int>, string>>();

            m = m.AddOrUpdate(Some(1), Some(1), "Some Some");
            m = m.AddOrUpdate(None, Some(1), "None Some");
            m = m.AddOrUpdate(Some(1), None, "Some None");
            m = m.AddOrUpdate(None, None, "None None");

            Assert.IsTrue(m[Some(1)][Some(1)] == "Some Some");
            Assert.IsTrue(m[None][Some(1)] == "None Some");
            Assert.IsTrue(m[Some(1)][None] == "Some None");
            Assert.IsTrue(m[None][None] == "None None");

            Assert.IsTrue(m.CountT() == 4);

            m = m.FilterT(v => v.EndsWith("None"));

            Assert.IsTrue(m.CountT() == 2);
        }
    }
}
