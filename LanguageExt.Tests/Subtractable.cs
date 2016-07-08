// TODO: Restore when refactor of type-classes complete

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Xunit;
//using static LanguageExt.Prelude;
//using LanguageExt;

//namespace LanguageExtTests
//{
//    public class Subtractable
//    {
//        [Fact]
//        public void OptionalNumericSubtract()
//        {
//            var x = Some(20);
//            var y = Some(10);
//            var z = x - y;

//            Assert.True(z == 10);
//        }

//        [Fact]
//        public void OptionalListSubtract()
//        {
//            var x = Some(List(1, 2, 3));
//            var y = Some(List(1, 2));
//            var z = x - y;

//            match(z,
//                Some: list =>
//                {
//                    Assert.True(list.Count == 1);
//                    Assert.True(list[0] == 3);
//                },
//                None: () => Assert.True(false)
//            );
//        }

//        [Fact]
//        public void OptionalSetSubtract()
//        {
//            var x = Some(Set(1,2,3));
//            var y = Some(Set(2,3,4));
//            var z = x - y;

//            match(z,
//                Some: set =>
//                {
//                    Assert.True(set.Count == 1);
//                    Assert.True(set.Contains(1));
//                },
//                None: () => Assert.True(false)
//            );
//        }
//    }
//}
