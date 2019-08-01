using Xunit;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;


namespace LanguageExt.Tests
{
    public class Divisible
    {
        [Fact]
        public void OptionalNumericDivide()
        {
            var x = Some(20);
            var y = Some(10);
            var z = divide<TInt, int>(x, y);

            Assert.True(z == 2);
        }
    }
}
