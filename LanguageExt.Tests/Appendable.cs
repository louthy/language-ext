using Xunit;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;
using static LanguageExt.TypeClass;

namespace LanguageExt.Tests
{
    public class Appendable
    {
        [Fact]
        public void OptionalNumericAppend()
        {
            var x = Some(10);
            var y = Some(20);
            var z = append<TInt, int>(x, y);

            Assert.True(z == 30);
        }

        //[Fact]
        //public void OptionalListAppend()
        //{
        //    var x = Some(List(1, 2, 3));
        //    var y = Some(List(4, 5, 6));
        //    var z = append<TInteger, int>(x, y);

        //    match(z,
        //        Some: list =>
        //        {
        //            Assert.True(list.Count == 6);
        //            Assert.True(list[0] == 1);
        //            Assert.True(list[1] == 2);
        //            Assert.True(list[2] == 3);
        //            Assert.True(list[3] == 4);
        //            Assert.True(list[4] == 5);
        //            Assert.True(list[5] == 6);
        //        },
        //        None: () => Assert.True(false)
        //    );
        //}

        //[Fact]
        //public void OptionalSetAppend()
        //{
        //    var x = Some(Set(1,2,3));
        //    var y = Some(Set(2,3,4));
        //    var z = append<TInteger, int>(x, y);

        //    match(z,
        //        Some: set =>
        //        {
        //            Assert.True(set.Count == 4);
        //            Assert.True(set.Contains(1));
        //            Assert.True(set.Contains(2));
        //            Assert.True(set.Contains(3));
        //            Assert.True(set.Contains(4));
        //        },
        //        None: () => Assert.True(false)
        //    );
        //}
    }
}
