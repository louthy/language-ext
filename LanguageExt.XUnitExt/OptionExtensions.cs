using Xunit;

namespace LanguageExt;

public static class OptionExtensions
{
    extension<A>(Option<A> ma)
    {
        public void AssertNone() => 
            ma.AssertNone("Expected to be in a None state");

        public void AssertNone(string userMessage) =>
            Assert.True(ma.IsNone, userMessage);

        public void AssertSome() => 
            ma.AssertSome("Expected to be in a Some state");

        public void AssertSome(string userMessage) =>
            Assert.True(ma.IsSome, userMessage);

        public void AssertSome(A expected) => 
            ma.AssertSome(expected, $"Expected to be in a Some state with a value of {expected}");

        public void AssertSome(A expected, string userMessage) =>
            Assert.True(ma.IsSome && expected is not null && expected.Equals((A)ma), userMessage);

        public void AssertSome(Func<A, bool> predicate) => 
            ma.AssertSome(predicate, "Expected to be in a Some state with a predicate that returns true");

        public void AssertSome(Func<A, bool> predicate, string userMessage) =>
            Assert.True(ma.IsSome && predicate((A)ma), userMessage);
    }
}
