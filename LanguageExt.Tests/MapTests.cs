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

            var vs = toSeq(m.Values);

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

            var vs = toSeq(m.Keys);

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
            Assert.False(Map((1, 2)).Equals(Map((1, 3))));
            Assert.False(Map((1, 2), (3, 4)).Equals(Map((1, 2))));
            Assert.False(Map((1, 2)).Equals(Map((1, 2), (3, 4))));
            Assert.True(Map((1, 2), (3, 4)).Equals(Map((1, 2), (3, 4))));
            Assert.True(Map((3, 4), (1, 2)).Equals(Map((1, 2), (3, 4))));
            Assert.True(Map((3, 4), (1, 2)).Equals(Map((3, 4), (1, 2))));
        }

        [Fact]
        public void SliceTest()
        {
            var m = Map((1, 1), (2, 2), (3, 3), (4, 4), (5, 5));

            var x = m.Slice(1, 2);

            Assert.True(x.Count == 2);
            Assert.True(x.ContainsKey(1));
            Assert.True(x.ContainsKey(2));

            var y = m.Slice(2, 4);

            Assert.True(y.Count == 3);
            Assert.True(y.ContainsKey(2));
            Assert.True(y.ContainsKey(3));
            Assert.True(y.ContainsKey(4));

            var z = m.Slice(4, 5);

            Assert.True(z.Count == 2);
            Assert.True(z.ContainsKey(4));
            Assert.True(z.ContainsKey(5));
        }

        [Fact]
        public void MinMaxTest()
        {
            var m = Map((1, 1), (2, 2), (3, 3), (4, 4), (5, 5));

            Assert.True(m.Min == (1, 1));
            Assert.True(m.Max == (5, 5));

            var me = Map<int, int>();

            Assert.True(me.Min == None);
            Assert.True(me.Max == None);
        }



        [Fact]
        public void FindPredecessorWhenKeyExistsTest()
        {
            var m = Map((1, 1), (2, 2), (3, 3), (4, 4), (5, 5), (6, 6), (7, 7), (8, 8), (9, 9), (10, 10), (11, 11), (12, 12), (13, 13), (14, 14), (15, 15));

            Assert.True(m.FindPredecessor(1) == None);
            Assert.True(m.FindPredecessor(2) == (1, 1));
            Assert.True(m.FindPredecessor(3) == (2, 2));
            Assert.True(m.FindPredecessor(4) == (3, 3));
            Assert.True(m.FindPredecessor(5) == (4, 4));
            Assert.True(m.FindPredecessor(6) == (5, 5));
            Assert.True(m.FindPredecessor(7) == (6, 6));
            Assert.True(m.FindPredecessor(8) == (7, 7));
            Assert.True(m.FindPredecessor(9) == (8, 8));
            Assert.True(m.FindPredecessor(10) == (9, 9));
            Assert.True(m.FindPredecessor(11) == (10, 10));
            Assert.True(m.FindPredecessor(12) == (11, 11));
            Assert.True(m.FindPredecessor(13) == (12, 12));
            Assert.True(m.FindPredecessor(14) == (13, 13));
            Assert.True(m.FindPredecessor(15) == (14, 14));
        }

        [Fact]
        public void FindPredecessorWhenKeyNotExistsTest()
        {
            var m = Map((1, 1), (3, 3), (5, 5), (7, 7), (9, 9), (11, 11), (13, 13), (15, 15));

            Assert.True(m.FindPredecessor(1) == None);
            Assert.True(m.FindPredecessor(2) == (1, 1));
            Assert.True(m.FindPredecessor(3) == (1, 1));
            Assert.True(m.FindPredecessor(4) == (3, 3));
            Assert.True(m.FindPredecessor(5) == (3, 3));
            Assert.True(m.FindPredecessor(6) == (5, 5));
            Assert.True(m.FindPredecessor(7) == (5, 5));
            Assert.True(m.FindPredecessor(8) == (7, 7));
            Assert.True(m.FindPredecessor(9) == (7, 7));
            Assert.True(m.FindPredecessor(10) == (9, 9));
            Assert.True(m.FindPredecessor(11) == (9, 9));
            Assert.True(m.FindPredecessor(12) == (11, 11));
            Assert.True(m.FindPredecessor(13) == (11, 11));
            Assert.True(m.FindPredecessor(14) == (13, 13));
            Assert.True(m.FindPredecessor(15) == (13, 13));
        }

        [Fact]
        public void FindExactOrPredecessorWhenKeyExistsTest()
        {
            var m = Map((1, 1), (2, 2), (3, 3), (4, 4), (5, 5), (6, 6), (7, 7), (8, 8), (9, 9), (10, 10), (11, 11), (12, 12), (13, 13), (14, 14), (15, 15));

            Assert.True(m.FindExactOrPredecessor(1) == (1, 1));
            Assert.True(m.FindExactOrPredecessor(2) == (2, 2));
            Assert.True(m.FindExactOrPredecessor(3) == (3, 3));
            Assert.True(m.FindExactOrPredecessor(4) == (4, 4));
            Assert.True(m.FindExactOrPredecessor(5) == (5, 5));
            Assert.True(m.FindExactOrPredecessor(6) == (6, 6));
            Assert.True(m.FindExactOrPredecessor(7) == (7, 7));
            Assert.True(m.FindExactOrPredecessor(8) == (8, 8));
            Assert.True(m.FindExactOrPredecessor(9) == (9, 9));
            Assert.True(m.FindExactOrPredecessor(10) == (10, 10));
            Assert.True(m.FindExactOrPredecessor(11) == (11, 11));
            Assert.True(m.FindExactOrPredecessor(12) == (12, 12));
            Assert.True(m.FindExactOrPredecessor(13) == (13, 13));
            Assert.True(m.FindExactOrPredecessor(14) == (14, 14));
            Assert.True(m.FindExactOrPredecessor(15) == (15, 15));
        }

        [Fact]
        public void FindExactOrPredecessorWhenKeySometimesExistsTest()
        {
            var m = Map((1, 1), (3, 3), (5, 5), (7, 7), (9, 9), (11, 11), (13, 13), (15, 15));

            Assert.True(m.FindExactOrPredecessor(1) == (1, 1));
            Assert.True(m.FindExactOrPredecessor(2) == (1, 1));
            Assert.True(m.FindExactOrPredecessor(3) == (3, 3));
            Assert.True(m.FindExactOrPredecessor(4) == (3, 3));
            Assert.True(m.FindExactOrPredecessor(5) == (5, 5));
            Assert.True(m.FindExactOrPredecessor(6) == (5, 5));
            Assert.True(m.FindExactOrPredecessor(7) == (7, 7));
            Assert.True(m.FindExactOrPredecessor(8) == (7, 7));
            Assert.True(m.FindExactOrPredecessor(9) == (9, 9));
            Assert.True(m.FindExactOrPredecessor(10) == (9, 9));
            Assert.True(m.FindExactOrPredecessor(11) == (11, 11));
            Assert.True(m.FindExactOrPredecessor(12) == (11, 11));
            Assert.True(m.FindExactOrPredecessor(13) == (13, 13));
            Assert.True(m.FindExactOrPredecessor(14) == (13, 13));
            Assert.True(m.FindExactOrPredecessor(15) == (15, 15));
        }

        [Fact]
        public void FindSuccessorWhenKeyExistsTest()
        {
            var m = Map((1, 1), (2, 2), (3, 3), (4, 4), (5, 5), (6, 6), (7, 7), (8, 8), (9, 9), (10, 10), (11, 11), (12, 12), (13, 13), (14, 14), (15, 15));

            Assert.True(m.FindSuccessor(1) == (2, 2));
            Assert.True(m.FindSuccessor(2) == (3, 3));
            Assert.True(m.FindSuccessor(3) == (4, 4));
            Assert.True(m.FindSuccessor(4) == (5, 5));
            Assert.True(m.FindSuccessor(5) == (6, 6));
            Assert.True(m.FindSuccessor(6) == (7, 7));
            Assert.True(m.FindSuccessor(7) == (8, 8));
            Assert.True(m.FindSuccessor(8) == (9, 9));
            Assert.True(m.FindSuccessor(9) == (10, 10));
            Assert.True(m.FindSuccessor(10) == (11, 11));
            Assert.True(m.FindSuccessor(11) == (12, 12));
            Assert.True(m.FindSuccessor(12) == (13, 13));
            Assert.True(m.FindSuccessor(13) == (14, 14));
            Assert.True(m.FindSuccessor(14) == (15, 15));
            Assert.True(m.FindSuccessor(15) == None);
        }

        [Fact]
        public void FindSuccessorWhenKeyNotExistsTest()
        {
            var m = Map((1, 1), (3, 3), (5, 5), (7, 7), (9, 9), (11, 11), (13, 13), (15, 15));

            Assert.True(m.FindSuccessor(1) == (3, 3));
            Assert.True(m.FindSuccessor(2) == (3, 3));
            Assert.True(m.FindSuccessor(3) == (5, 5));
            Assert.True(m.FindSuccessor(4) == (5, 5));
            Assert.True(m.FindSuccessor(5) == (7, 7));
            Assert.True(m.FindSuccessor(6) == (7, 7));
            Assert.True(m.FindSuccessor(7) == (9, 9));
            Assert.True(m.FindSuccessor(8) == (9, 9));
            Assert.True(m.FindSuccessor(9) == (11, 11));
            Assert.True(m.FindSuccessor(10) == (11, 11));
            Assert.True(m.FindSuccessor(11) == (13, 13));
            Assert.True(m.FindSuccessor(12) == (13, 13));
            Assert.True(m.FindSuccessor(13) == (15, 15));
            Assert.True(m.FindSuccessor(14) == (15, 15));
            Assert.True(m.FindSuccessor(15) == None);
        }

        [Fact]
        public void FindExactOrSuccessorWhenKeyExistsTest()
        {
            var m = Map((1, 1), (2, 2), (3, 3), (4, 4), (5, 5), (6, 6), (7, 7), (8, 8), (9, 9), (10, 10), (11, 11), (12, 12), (13, 13), (14, 14), (15, 15));

            Assert.True(m.FindExactOrSuccessor(1) == (1, 1));
            Assert.True(m.FindExactOrSuccessor(2) == (2, 2));
            Assert.True(m.FindExactOrSuccessor(3) == (3, 3));
            Assert.True(m.FindExactOrSuccessor(4) == (4, 4));
            Assert.True(m.FindExactOrSuccessor(5) == (5, 5));
            Assert.True(m.FindExactOrSuccessor(6) == (6, 6));
            Assert.True(m.FindExactOrSuccessor(7) == (7, 7));
            Assert.True(m.FindExactOrSuccessor(8) == (8, 8));
            Assert.True(m.FindExactOrSuccessor(9) == (9, 9));
            Assert.True(m.FindExactOrSuccessor(10) == (10, 10));
            Assert.True(m.FindExactOrSuccessor(11) == (11, 11));
            Assert.True(m.FindExactOrSuccessor(12) == (12, 12));
            Assert.True(m.FindExactOrSuccessor(13) == (13, 13));
            Assert.True(m.FindExactOrSuccessor(14) == (14, 14));
            Assert.True(m.FindExactOrSuccessor(15) == (15, 15));
        }

        [Fact]
        public void FindExactOrSuccessorWhenKeySometimesExistsTest()
        {
            var m = Map((1, 1), (3, 3), (5, 5), (7, 7), (9, 9), (11, 11), (13, 13), (15, 15));

            Assert.True(m.FindExactOrSuccessor(1) == (1, 1));
            Assert.True(m.FindExactOrSuccessor(2) == (3, 3));
            Assert.True(m.FindExactOrSuccessor(3) == (3, 3));
            Assert.True(m.FindExactOrSuccessor(4) == (5, 5));
            Assert.True(m.FindExactOrSuccessor(5) == (5, 5));
            Assert.True(m.FindExactOrSuccessor(6) == (7, 7));
            Assert.True(m.FindExactOrSuccessor(7) == (7, 7));
            Assert.True(m.FindExactOrSuccessor(8) == (9, 9));
            Assert.True(m.FindExactOrSuccessor(9) == (9, 9));
            Assert.True(m.FindExactOrSuccessor(10) == (11, 11));
            Assert.True(m.FindExactOrSuccessor(11) == (11, 11));
            Assert.True(m.FindExactOrSuccessor(12) == (13, 13));
            Assert.True(m.FindExactOrSuccessor(13) == (13, 13));
            Assert.True(m.FindExactOrSuccessor(14) == (15, 15));
            Assert.True(m.FindExactOrSuccessor(15) == (15, 15));
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

        [Fact]
        public void itemLensGetShouldGetExistingValue()
        {
            var expected = "3";
            var map = Map((1, "1"), (2, "2"), (3, "3"), (4, "4"), (5, "5"));
            var actual = Map<int, string>.item(3).Get(map);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void itemLensGetShouldThrowExceptionForNonExistingValue()
        {
            Assert.Throws<Exception>(() =>
            {
                var map = Map((1, "1"), (2, "2"), (3, "3"), (4, "4"), (5, "5"));
                var actual = Map<int, string>.item(10).Get(map);
            });
        }

        [Fact]
        public void itemOrNoneLensGetShouldGetExistingValue()
        {
            var expected = "3";
            var map = Map((1, "1"), (2, "2"), (3, "3"), (4, "4"), (5, "5"));
            var actual = Map<int, string>.itemOrNone(3).Get(map);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void itemOrNoneLensGetShouldReturnNoneForNonExistingValue()
        {
            var expected = Option<string>.None;
            var map = Map((1, "1"), (2, "2"), (3, "3"), (4, "4"), (5, "5"));
            var actual = Map<int, string>.itemOrNone(10).Get(map);

            Assert.Equal(expected, actual);
        }
    }
}
