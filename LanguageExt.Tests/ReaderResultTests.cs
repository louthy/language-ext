using Xunit;

namespace LanguageExt.Tests
{
    public class ValidationResultTests
    {
        [Fact]
        public void MatchWhenNotFaulted()
        {
            // This will throw an InvalidCastException
            var expected = "Some value";
            var actual = ReaderResult<string>.New(expected).Match(s => s, error => error.Message);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void MatchWhenFaulted()
        {
            var expected = "Some error";
            var actual = ReaderResult<string>.New(Common.Error.New(expected)).Match(s => s, error => error.Message);
            Assert.Equal(expected, actual);
        }

    }
}
