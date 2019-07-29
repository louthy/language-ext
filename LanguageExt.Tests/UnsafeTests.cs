using Xunit;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    
    public class UnsafeTests
    {
        [Fact]
        public void AssignmentTests()
        {
            OptionUnsafe<string> str = SomeUnsafe((string)null);

            string res = matchUnsafe(
                            str,
                            Some: v => v,
                            None: () => "failed"
                         );
        }
    }
}
