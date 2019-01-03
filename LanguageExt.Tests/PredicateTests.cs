using LanguageExt.ClassInstances.Pred;
using static LanguageExt.Prelude;
using Xunit;
using System;
using LanguageExt.ClassInstances.Const;
using LanguageExt.TypeClasses;

namespace LanguageExt.Tests
{
    public class PredicateTests
    {
        public struct NumbersPattern : Const<string>
        {
            public string Value => "^[0-9]*$";
        }

        [Fact]
        public void StringMatchesPattern() =>
            Assert.True(Matches<NumbersPattern>.Is.True("12345"));

        [Fact]
        public void StringDoesNotMatchPattern() =>
            Assert.False(Matches<NumbersPattern>.Is.True("1ABC5"));
    }
}
