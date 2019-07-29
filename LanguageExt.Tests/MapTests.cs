using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Map;
using Xunit;
using System;
using System.Linq;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests
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

            Assert.Throws<ArgumentException>(() => setItem(m1, 4, "y"));
        }

        
        [Fact]
        public void MapOrdSetTest()
        {
            var m1 = Map<OrdStringOrdinalIgnoreCase, string, int>(("one", 1), ("two",2), ("three", 3));
            var m2 = m1.SetItem("One", -1);
            
            Assert.Equal(3, m2.Count);
            Assert.Equal(-1, m2["one"]);
            Assert.DoesNotContain("one", m2.Keys); // make sure key got replaced, too
            Assert.Contains("One", m2.Keys); // make sure key got replaced, too

            Assert.Throws<ArgumentException>(() => m1.SetItem("four", identity));

            var m3 = m1.TrySetItem("four", 0).Add("five", 0).TrySetItem("Five", 5);
            Assert.Equal(5, m3["fiVe"]);
            Assert.DoesNotContain("four", m3.Keys);
            Assert.DoesNotContain("five", m3.Keys);
            Assert.Contains("Five", m3.Keys);
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

        [Fact]
        public void MapUnionTest1()
        {
            var x = Map((1, 1), (2, 2), (3, 3));
            var y = Map((1, 1), (2, 2), (3, 3));

            var z = union(x, y, (k, l, r) => l + r);

            Assert.True(z == Map((1, 2), (2, 4), (3, 6)));
        }

        [Fact]
        public void MapUnionTest2()
        {
            var x = Map((1, 1), (2, 2), (3, 3));
            var y = Map((4, 4), (5, 5), (6, 6));

            var z = union(x, y, (k, l, r) => l + r);

            Assert.True(z == Map((1, 1), (2, 2), (3, 3), (4, 4), (5, 5), (6, 6)));
        }

        [Fact]
        public void MapIntesectTest1()
        {
            var x = Map(        (2, 2), (3, 3));
            var y = Map((1, 1), (2, 2)        );

            var z = intersect(x, y, (k, l, r) => l + r);

            Assert.True(z == Map((2, 4)));
        }

        [Fact]
        public void MapExceptTest()
        {
            var x = Map((1, 1), (2, 2), (3, 3));
            var y = Map((1, 1));

            var z = except(x, y);

            Assert.True(z == Map((2, 2), (3, 3)));
        }

        [Fact]
        public void MapSymmetricExceptTest()
        {
            var x = Map((1, 1), (2, 2), (3, 3));
            var y = Map((1, 1),         (3, 3));

            var z = symmetricExcept(x, y);

            Assert.True(z == Map((2, 2)));
        }

        [Fact]
        public void EqualsTest()
        {
            var emp = Map<int, int>();

            Assert.True(emp.Equals(emp));
            Assert.False(Map((1, 2)).Equals(emp));
            Assert.False(emp.Equals(Map((1, 2))));
            Assert.True(Map((1, 2)).Equals(Map((1, 2))));
            Assert.False(Map((1, 2), (3, 4)).Equals(Map((1, 2))));
            Assert.False(Map((1, 2)).Equals(Map((1, 2), (3, 4))));
            Assert.True(Map((1, 2), (3, 4)).Equals(Map((1, 2), (3, 4))));
            Assert.True(Map((3, 4), (1, 2)).Equals(Map((1, 2), (3, 4))));
            Assert.True(Map((3, 4), (1, 2)).Equals(Map((3, 4), (1, 2))));
        }

        // Exponential test - takes too long to run
        //[Fact]
        //public void Issue_454()
        //{
        //    var tmp = "".PadLeft(30000, 'x'); // something big enough (one Referral object = 20-40kb)
        //    var map = Map<int, string>();

        //    for (int i = 0; i < 30000; i++) // for our real system it is only 3000 items, but with string it needs more
        //    {
        //        map = map.AddOrUpdate(i, tmp);
        //        map = map.Filter(_ => true);
        //    }

        //    map.Filter(_ => false);
        //}
    }
}
