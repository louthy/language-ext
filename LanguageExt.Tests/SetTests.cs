using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

using LanguageExt;
using static LanguageExt.Prelude;
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
            Assert.False(Set(1, 2, 3).Equals(Set<int>()));
            Assert.False(Set<int>().Equals(Set<int>(1, 2, 3)));
            Assert.True(Set<int>().Equals(Set<int>()));
            Assert.True(Set<int>(1).Equals(Set<int>(1)));
            Assert.True(Set<int>(1, 2).Equals(Set<int>(1, 2)));
            Assert.False(Set<int>(1, 2).Equals(Set<int>(1, 2, 3)));
            Assert.False(Set<int>(1, 2, 3).Equals(Set<int>(1, 2)));
        }
    }
}
