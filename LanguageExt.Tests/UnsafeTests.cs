using NUnit.Framework;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExtTests
{
    [TestFixture]
    public class UnsafeTests
    {
        [Test]
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
