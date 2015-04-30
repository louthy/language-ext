using NUnit.Framework;
using static LanguageExt.Prelude;

namespace LanguageExtTests
{
    [TestFixture]
    public class TryOutTests
    {
        [Test] public void OutTest()
        {
            int value1 = parseInt("123").Failure(() => 0);

            int value2 = failure(parseInt("123"), () => 0);

            int value3 = parseInt("123").Failure(0);

            int value4 = failure(parseInt("123"), 0);

            Assert.IsTrue(value1 == 123);
            Assert.IsTrue(value2 == 123);
            Assert.IsTrue(value3 == 123);
            Assert.IsTrue(value4 == 123);

            parseInt("123").Match(
                Some: UseTheInteger,
                None: () => failwith<int>("Not an integer")
                );

            match( parseInt("123"),
                Some: UseTheInteger,
                None: () => failwith<int>("Not an integer")
                );

            int value5 = failure(parseInt("XXX"), 0);
            Assert.IsTrue(value5 == 0);
        }

        private int UseTheInteger(int v)
        {
            return 0;
        }
    }
}
