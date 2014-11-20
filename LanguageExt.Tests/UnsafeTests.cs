using NUnit.Framework;
using LanguageExt;
using LanguageExt.Prelude;

namespace LanguageExtTests
{
    [TestFixture]
    public class UnsafeTests
    {
        [Test]
        public void AssignmentTests()
        {
            Option<string> str = SomeUnsafe((string)null);

            string res = match(
                            str,
                            Some: v => v,
                            None: () => "failed"
                         );
        }
    }
}
