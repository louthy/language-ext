using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Map;
using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace LanguageExtTests
{
    [TestFixture]
    public class MapTests
    {
        [Test]
        public void MapGeneratorTest()
        {
            var m1 = map<int, string>();
            m1 = add(m1, 100, "hello");
            Assert.IsTrue(m1.Count == 1 && containsKey(m1,100));
        }

        [Test]
        public void MapGeneratorAndMatchTest()
        {
            var m2 = map( tuple(1, "a"),
                          tuple(2, "b"),
                          tuple(3, "c") );

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
            var m1 = map( tuple(1, "a"),
                          tuple(2, "b"),
                          tuple(3, "c") );

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
            var m = map(tuple(1, 1));
            m.Find(1).IfNone(() => failwith<int>("Broken"));

            m = map(tuple(1, 1), tuple(2, 2));
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));

            m = map(tuple(1, 1), tuple(2, 2), tuple(3, 3));
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));
            m.Find(3).IfNone(() => failwith<int>("Broken"));

            m = map(tuple(1, 1), tuple(2, 2), tuple(3, 3), tuple(4, 4));
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));
            m.Find(3).IfNone(() => failwith<int>("Broken"));
            m.Find(4).IfNone(() => failwith<int>("Broken"));

            m = map(tuple(1, 1), tuple(2, 2), tuple(3, 3), tuple(4, 4), tuple(5, 5));
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));
            m.Find(3).IfNone(() => failwith<int>("Broken"));
            m.Find(4).IfNone(() => failwith<int>("Broken"));
            m.Find(5).IfNone(() => failwith<int>("Broken"));
        }

        [Test]
        public void MapAddInReverseOrderTest()
        {
            var m = map(tuple(2, 2), tuple(1, 1));
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));

            m = map(tuple(3, 3), tuple(2, 2), tuple(1, 1));
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));
            m.Find(3).IfNone(() => failwith<int>("Broken"));

            m = map(tuple(4, 4), tuple(3, 3), tuple(2, 2), tuple(1, 1));
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));
            m.Find(3).IfNone(() => failwith<int>("Broken"));
            m.Find(4).IfNone(() => failwith<int>("Broken"));

            m = map(tuple(5, 5), tuple(4, 4), tuple(3, 3), tuple(2, 2), tuple(1, 1));
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));
            m.Find(3).IfNone(() => failwith<int>("Broken"));
            m.Find(4).IfNone(() => failwith<int>("Broken"));
            m.Find(5).IfNone(() => failwith<int>("Broken"));
        }

        [Test]
        public void MapAddInMixedOrderTest()
        {
            var m = map(tuple(5, 5), tuple(1, 1), tuple(3, 3), tuple(2, 2), tuple(4, 4));
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));
            m.Find(3).IfNone(() => failwith<int>("Broken"));
            m.Find(4).IfNone(() => failwith<int>("Broken"));
            m.Find(5).IfNone(() => failwith<int>("Broken"));

            m = map(tuple(1, 1), tuple(3, 3), tuple(5, 5), tuple(2, 2), tuple(4, 4));
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));
            m.Find(3).IfNone(() => failwith<int>("Broken"));
            m.Find(4).IfNone(() => failwith<int>("Broken"));
            m.Find(5).IfNone(() => failwith<int>("Broken"));
        }


        [Test]
        public void MapRemoveTest()
        {
            var m = map(tuple(1, "a"),
                        tuple(2, "b"),
                        tuple(3, "c"),
                        tuple(4, "d"),
                        tuple(5, "e"));

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

            var items = LanguageExt.List.map(range(1, max), _ => tuple(Guid.NewGuid(), Guid.NewGuid()))
                                        .ToDictionary(kv => kv.Item1, kv => kv.Item2);

            var m = map<Guid, Guid>().AddRange(items);
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
    }
}
