using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExtTests
{
    
    public class TryOutTests
    {
        [Fact] public void OutTest()
        {
            int value1 = parseInt("123").IfNone(() => 0);

            int value2 = ifNone(parseInt("123"), () => 0);

            int value3 = parseInt("123").IfNone(0);

            int value4 = ifNone(parseInt("123"), 0);

            Assert.True(value1 == 123);
            Assert.True(value2 == 123);
            Assert.True(value3 == 123);
            Assert.True(value4 == 123);

            parseInt("123").Match(
                Some: UseTheInteger,
                None: () => failwith<int>("Not an integer")
                );

            match( parseInt("123"),
                Some: UseTheInteger,
                None: () => failwith<int>("Not an integer")
                );

            int value5 = ifNone(parseInt("XXX"), 0);
            Assert.True(value5 == 0);
        }

        private int UseTheInteger(int v)
        {
            return 0;
        }
    }
}
