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
        public void HashSetKeyTypeTests()
        {
            var set = HashSet<EqStringOrdinalIgnoreCase, string>("one", "two", "three");

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
    }
}
