using LanguageExt;
using L = LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Map;
using Xunit;
using System;
using System.Linq;
using LanguageExt.ClassInstances;

namespace LanguageExt.Tests
{
    public class SetTests
    {
        [Fact]
        public void SetKeyTypeTests()
        {
            var set = Set<OrdStringOrdinalIgnoreCase, string>("one", "two", "three");

            Assert.True(set.Contains("one"));
            Assert.True(set.Contains("ONE"));

            Assert.True(set.Contains("two"));
            Assert.True(set.Contains("Two"));

            Assert.True(set.Contains("three"));
            Assert.True(set.Contains("thREE"));
        }


        [Fact]
        public void EqualsTest()
        {
            var a = Set(1, 2);
            var b = Set(1, 2, 3);

            Assert.False(Set(1, 2, 3).Equals(Set<int>()));
            Assert.False(Set<int>().Equals(Set(1, 2, 3)));
            Assert.True(Set<int>().Equals(Set<int>()));
            Assert.True(Set(1).Equals(Set(1)));
            Assert.True(Set(1, 2).Equals(Set(1, 2)));
            Assert.False(Set(1, 2).Equals(Set(1, 2, 3)));
            Assert.False(Set(1, 2, 3).Equals(Set(1, 2)));
        }

        [Fact]
        public void SetGeneratorTest()
        {
            var m1 = Set<int>();
            m1 = m1.Add(100);
            Assert.True(m1.Count == 1 && m1.Contains(100));
        }

        [Fact]
        public void SetAddInOrderTest()
        {
            var m = Set(1);
            m.Find(1).IfNone(() => failwith<int>("Broken"));

            m = Set(1, 2);
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));

            m = Set(1, 2, 3);
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));
            m.Find(3).IfNone(() => failwith<int>("Broken"));

            m = Set(1, 2, 3, 4);
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));
            m.Find(3).IfNone(() => failwith<int>("Broken"));
            m.Find(4).IfNone(() => failwith<int>("Broken"));

            m = Set(1, 2, 3, 4, 5);
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));
            m.Find(3).IfNone(() => failwith<int>("Broken"));
            m.Find(4).IfNone(() => failwith<int>("Broken"));
            m.Find(5).IfNone(() => failwith<int>("Broken"));
        }

        [Fact]
        public void SetAddInReverseOrderTest()
        {
            var m = Set(2, 1);
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));

            m = Set(3, 2, 1);
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));
            m.Find(3).IfNone(() => failwith<int>("Broken"));

            m = Set(4, 3, 2, 1);
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));
            m.Find(3).IfNone(() => failwith<int>("Broken"));
            m.Find(4).IfNone(() => failwith<int>("Broken"));

            m = Set(5, 4, 3, 2, 1);
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));
            m.Find(3).IfNone(() => failwith<int>("Broken"));
            m.Find(4).IfNone(() => failwith<int>("Broken"));
            m.Find(5).IfNone(() => failwith<int>("Broken"));
        }

        [Fact]
        public void MapAddInMixedOrderTest()
        {
            var m = Set(5, 1, 3, 2, 4);
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));
            m.Find(3).IfNone(() => failwith<int>("Broken"));
            m.Find(4).IfNone(() => failwith<int>("Broken"));
            m.Find(5).IfNone(() => failwith<int>("Broken"));

            m = Set(1, 3, 5, 2, 4);
            m.Find(1).IfNone(() => failwith<int>("Broken"));
            m.Find(2).IfNone(() => failwith<int>("Broken"));
            m.Find(3).IfNone(() => failwith<int>("Broken"));
            m.Find(4).IfNone(() => failwith<int>("Broken"));
            m.Find(5).IfNone(() => failwith<int>("Broken"));
        }


        [Fact]
        public void SetRemoveTest()
        {
            var m = Set(1, 3, 5, 2, 4);

            m.Find(1).IfNone(() => failwith<int>("Broken 1"));
            m.Find(2).IfNone(() => failwith<int>("Broken 2"));
            m.Find(3).IfNone(() => failwith<int>("Broken 3"));
            m.Find(4).IfNone(() => failwith<int>("Broken 4"));
            m.Find(5).IfNone(() => failwith<int>("Broken 5"));

            Assert.True(m.Count == 5);

            m = m.Remove(4);
            Assert.True(m.Count == 4);
            Assert.True(m.Find(4).IsNone);
            m.Find(1).IfNone(() => failwith<int>("Broken 1"));
            m.Find(2).IfNone(() => failwith<int>("Broken 2"));
            m.Find(3).IfNone(() => failwith<int>("Broken 3"));
            m.Find(5).IfNone(() => failwith<int>("Broken 5"));

            m = m.Remove(1);
            Assert.True(m.Count == 3);
            Assert.True(m.Find(1).IsNone);
            m.Find(2).IfNone(() => failwith<int>("Broken 2"));
            m.Find(3).IfNone(() => failwith<int>("Broken 3"));
            m.Find(5).IfNone(() => failwith<int>("Broken 5"));

            m = m.Remove(2);
            Assert.True(m.Count == 2);
            Assert.True(m.Find(2).IsNone);
            m.Find(3).IfNone(() => failwith<int>("Broken 3"));
            m.Find(5).IfNone(() => failwith<int>("Broken 5"));

            m = m.Remove(3);
            Assert.True(m.Count == 1);
            Assert.True(m.Find(3).IsNone);
            m.Find(5).IfNone(() => failwith<int>("Broken 5"));

            m = m.Remove(5);
            Assert.True(m.Count == 0);
            Assert.True(m.Find(5).IsNone);
        }

        [Fact]
        public void MassAddRemoveTest()
        {
            int max = 100000;

            var items = Range(1, max).Map(_ => Guid.NewGuid()).ToList();

            var m = toSet(items);
            Assert.True(m.Count == max);
            foreach (var item in items)
            {
                Assert.True(m.Contains(item));
                m = m.Remove(item);
                Assert.False(m.Contains(item));
                max--;
                Assert.True(m.Count == max);
            }
        }

        [Fact]
        public void SetUnionTest1()
        {
            var x = Set((1, 1), (2, 2), (3, 3));
            var y = Set((1, 1), (2, 2), (3, 3));

            var z = Set.union(x, y);

            Assert.True(z == Set((1, 1), (2, 2), (3, 3)));
        }

        [Fact]
        public void SetUnionTest2()
        {
            var x = Set((1, 1), (2, 2), (3, 3));
            var y = Set((4, 4), (5, 5), (6, 6));

            var z = Set.union(x, y);

            Assert.True(z == Set((1, 1), (2, 2), (3, 3), (4, 4), (5, 5), (6, 6)));
        }

        [Fact]
        public void SetIntesectTest1()
        {
            var x = Set((2, 2), (3, 3));
            var y = Set((1, 1), (2, 2));

            var z = Set.intersect(x, y);

            Assert.True(z == Set((2, 2)));
        }

        [Fact]
        public void SetExceptTest()
        {
            var x = Set((1, 1), (2, 2), (3, 3));
            var y = Set((1, 1));

            var z = Set.except(x, y);

            Assert.True(z == Set((2, 2), (3, 3)));
        }

        [Fact]
        public void SetSymmetricExceptTest()
        {
            var x = Set((1, 1), (2, 2), (3, 3));
            var y = Set((1, 1), (3, 3));

            var z = Set.symmetricExcept(x, y);

            Assert.True(z == Set((2, 2)));
        }


        [Fact]
        public void SliceTest()
        {
            var m = Set(1, 2, 3, 4, 5);

            var x = m.Slice(1, 2);

            Assert.True(x.Count == 2);
            Assert.True(x.Contains(1));
            Assert.True(x.Contains(2));

            var y = m.Slice(2, 4);

            Assert.True(y.Count == 3);
            Assert.True(y.Contains(2));
            Assert.True(y.Contains(3));
            Assert.True(y.Contains(4));

            var z = m.Slice(4, 5);

            Assert.True(z.Count == 2);
            Assert.True(z.Contains(4));
            Assert.True(z.Contains(5));
        }

        [Fact]
        public void MinMaxTest()
        {
            var m = Set(1, 2, 3, 4, 5);

            Assert.True(m.Min == 1);
            Assert.True(m.Max == 5);

            var me = Set<int>();

            Assert.True(me.Min == None);
            Assert.True(me.Max == None);
        }

        [Fact]
        public void FindPredecessorWhenKeyExistsTest()
        {
            var m = Set(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);

            Assert.True(m.FindPredecessor(1) == None);
            Assert.True(m.FindPredecessor(2) == 1);
            Assert.True(m.FindPredecessor(3) == 2);
            Assert.True(m.FindPredecessor(4) == 3);
            Assert.True(m.FindPredecessor(5) == 4);
            Assert.True(m.FindPredecessor(6) == 5);
            Assert.True(m.FindPredecessor(7) == 6);
            Assert.True(m.FindPredecessor(8) == 7);
            Assert.True(m.FindPredecessor(9) == 8);
            Assert.True(m.FindPredecessor(10) == 9);
            Assert.True(m.FindPredecessor(11) == 10);
            Assert.True(m.FindPredecessor(12) == 11);
            Assert.True(m.FindPredecessor(13) == 12);
            Assert.True(m.FindPredecessor(14) == 13);
            Assert.True(m.FindPredecessor(15) == 14);
        }

        [Fact]
        public void FindPredecessorWhenKeyNotExistsTest()
        {
            var m = Set(1, 3, 5, 7, 9, 11, 13, 15);

            Assert.True(m.FindPredecessor(1) == None);
            Assert.True(m.FindPredecessor(2) == 1);
            Assert.True(m.FindPredecessor(3) == 1);
            Assert.True(m.FindPredecessor(4) == 3);
            Assert.True(m.FindPredecessor(5) == 3);
            Assert.True(m.FindPredecessor(6) == 5);
            Assert.True(m.FindPredecessor(7) == 5);
            Assert.True(m.FindPredecessor(8) == 7);
            Assert.True(m.FindPredecessor(9) == 7);
            Assert.True(m.FindPredecessor(10) == 9);
            Assert.True(m.FindPredecessor(11) == 9);
            Assert.True(m.FindPredecessor(12) == 11);
            Assert.True(m.FindPredecessor(13) == 11);
            Assert.True(m.FindPredecessor(14) == 13);
            Assert.True(m.FindPredecessor(15) == 13);
        }

        [Fact]
        public void FindExactOrPredecessorWhenKeyExistsTest()
        {
            var m = Set(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);

            Assert.True(m.FindExactOrPredecessor(1) == 1);
            Assert.True(m.FindExactOrPredecessor(2) == 2);
            Assert.True(m.FindExactOrPredecessor(3) == 3);
            Assert.True(m.FindExactOrPredecessor(4) == 4);
            Assert.True(m.FindExactOrPredecessor(5) == 5);
            Assert.True(m.FindExactOrPredecessor(6) == 6);
            Assert.True(m.FindExactOrPredecessor(7) == 7);
            Assert.True(m.FindExactOrPredecessor(8) == 8);
            Assert.True(m.FindExactOrPredecessor(9) == 9);
            Assert.True(m.FindExactOrPredecessor(10) == 10);
            Assert.True(m.FindExactOrPredecessor(11) == 11);
            Assert.True(m.FindExactOrPredecessor(12) == 12);
            Assert.True(m.FindExactOrPredecessor(13) == 13);
            Assert.True(m.FindExactOrPredecessor(14) == 14);
            Assert.True(m.FindExactOrPredecessor(15) == 15);
        }

        [Fact]
        public void FindExactOrPredecessorWhenKeySometimesExistsTest()
        {
            var m = Set(1, 3, 5, 7, 9, 11, 13, 15);

            Assert.True(m.FindExactOrPredecessor(1) == 1);
            Assert.True(m.FindExactOrPredecessor(2) == 1);
            Assert.True(m.FindExactOrPredecessor(3) == 3);
            Assert.True(m.FindExactOrPredecessor(4) == 3);
            Assert.True(m.FindExactOrPredecessor(5) == 5);
            Assert.True(m.FindExactOrPredecessor(6) == 5);
            Assert.True(m.FindExactOrPredecessor(7) == 7);
            Assert.True(m.FindExactOrPredecessor(8) == 7);
            Assert.True(m.FindExactOrPredecessor(9) == 9);
            Assert.True(m.FindExactOrPredecessor(10) == 9);
            Assert.True(m.FindExactOrPredecessor(11) == 11);
            Assert.True(m.FindExactOrPredecessor(12) == 11);
            Assert.True(m.FindExactOrPredecessor(13) == 13);
            Assert.True(m.FindExactOrPredecessor(14) == 13);
            Assert.True(m.FindExactOrPredecessor(15) == 15);
        }

        [Fact]
        public void FindSuccessorWhenKeyExistsTest()
        {
            var m = Set(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);

            Assert.True(m.FindSuccessor(1) == 2);
            Assert.True(m.FindSuccessor(2) == 3);
            Assert.True(m.FindSuccessor(3) == 4);
            Assert.True(m.FindSuccessor(4) == 5);
            Assert.True(m.FindSuccessor(5) == 6);
            Assert.True(m.FindSuccessor(6) == 7);
            Assert.True(m.FindSuccessor(7) == 8);
            Assert.True(m.FindSuccessor(8) == 9);
            Assert.True(m.FindSuccessor(9) == 10);
            Assert.True(m.FindSuccessor(10) == 11);
            Assert.True(m.FindSuccessor(11) == 12);
            Assert.True(m.FindSuccessor(12) == 13);
            Assert.True(m.FindSuccessor(13) == 14);
            Assert.True(m.FindSuccessor(14) == 15);
            Assert.True(m.FindSuccessor(15) == None);
        }

        [Fact]
        public void FindSuccessorWhenKeyNotExistsTest()
        {
            var m = Set(1, 3, 5, 7, 9, 11, 13, 15);

            Assert.True(m.FindSuccessor(1) == 3);
            Assert.True(m.FindSuccessor(2) == 3);
            Assert.True(m.FindSuccessor(3) == 5);
            Assert.True(m.FindSuccessor(4) == 5);
            Assert.True(m.FindSuccessor(5) == 7);
            Assert.True(m.FindSuccessor(6) == 7);
            Assert.True(m.FindSuccessor(7) == 9);
            Assert.True(m.FindSuccessor(8) == 9);
            Assert.True(m.FindSuccessor(9) == 11);
            Assert.True(m.FindSuccessor(10) == 11);
            Assert.True(m.FindSuccessor(11) == 13);
            Assert.True(m.FindSuccessor(12) == 13);
            Assert.True(m.FindSuccessor(13) == 15);
            Assert.True(m.FindSuccessor(14) == 15);
            Assert.True(m.FindSuccessor(15) == None);
        }

        [Fact]
        public void FindExactOrSuccessorWhenKeyExistsTest()
        {
            var m = Set(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);

            Assert.True(m.FindExactOrSuccessor(1) == 1);
            Assert.True(m.FindExactOrSuccessor(2) == 2);
            Assert.True(m.FindExactOrSuccessor(3) == 3);
            Assert.True(m.FindExactOrSuccessor(4) == 4);
            Assert.True(m.FindExactOrSuccessor(5) == 5);
            Assert.True(m.FindExactOrSuccessor(6) == 6);
            Assert.True(m.FindExactOrSuccessor(7) == 7);
            Assert.True(m.FindExactOrSuccessor(8) == 8);
            Assert.True(m.FindExactOrSuccessor(9) == 9);
            Assert.True(m.FindExactOrSuccessor(10) == 10);
            Assert.True(m.FindExactOrSuccessor(11) == 11);
            Assert.True(m.FindExactOrSuccessor(12) == 12);
            Assert.True(m.FindExactOrSuccessor(13) == 13);
            Assert.True(m.FindExactOrSuccessor(14) == 14);
            Assert.True(m.FindExactOrSuccessor(15) == 15);
        }

        [Fact]
        public void FindExactOrSuccessorWhenKeySometimesExistsTest()
        {
            var m = Set(1, 3, 5, 7, 9, 11, 13, 15);

            Assert.True(m.FindExactOrSuccessor(1) == 1);
            Assert.True(m.FindExactOrSuccessor(2) == 3);
            Assert.True(m.FindExactOrSuccessor(3) == 3);
            Assert.True(m.FindExactOrSuccessor(4) == 5);
            Assert.True(m.FindExactOrSuccessor(5) == 5);
            Assert.True(m.FindExactOrSuccessor(6) == 7);
            Assert.True(m.FindExactOrSuccessor(7) == 7);
            Assert.True(m.FindExactOrSuccessor(8) == 9);
            Assert.True(m.FindExactOrSuccessor(9) == 9);
            Assert.True(m.FindExactOrSuccessor(10) == 11);
            Assert.True(m.FindExactOrSuccessor(11) == 11);
            Assert.True(m.FindExactOrSuccessor(12) == 13);
            Assert.True(m.FindExactOrSuccessor(13) == 13);
            Assert.True(m.FindExactOrSuccessor(14) == 15);
            Assert.True(m.FindExactOrSuccessor(15) == 15);
        }

        [Fact]
        public void CaseTest()
        {
            // seq1 tests here just for reference
            { Assert.True(Seq<int>().Case is not var (_, _) and not {} ); }
            { Assert.True(Seq1<int>(1).Case is not var (_, _) and 1); }
            { Assert.True(Seq<int>(1, 2).Case is (1, Seq<int> xs) && xs == Seq1(2)); }

            { Assert.True(Set<int>().Case is not var (_, _) and not {} ); }
            { Assert.True(Set<int>(1).Case is not var (_, _) and 1); }
            { Assert.True(Set<int>(1, 2).Case is (1, Seq<int> xs) && xs == Seq1(2)); }

            { Assert.True(Set<OrdInt, int>().Case is not var (_, _) and not {} ); }
            { Assert.True(Set<OrdInt, int>(1).Case is not var (_, _) and 1); }
            { Assert.True(Set<OrdInt, int>(1, 2).Case is (1, Seq<int> xs) && xs == Seq1(2)); }
        }
    }
}
