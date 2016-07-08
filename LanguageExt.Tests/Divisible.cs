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
//    public class Divisible
//    {
//        [Fact]
//        public void OptionalNumericDivide()
//        {
//            var x = Some(20);
//            var y = Some(10);
//            var z = x / y;

//            Assert.True(z == 2);
//        }

//        [Fact]
//        public void OptionalListDivide()
//        {
//            var x = Some(List(15, 30, 60));
//            var y = Some(List(3, 5));
//            var z = x / y;

//            match(z,
//                Some: list =>
//                {
//                    Assert.True(list.Count == 6);
//                    Assert.True(list[0] == 5);
//                    Assert.True(list[1] == 10);
//                    Assert.True(list[2] == 20);
//                    Assert.True(list[3] == 3);
//                    Assert.True(list[4] == 6);
//                    Assert.True(list[5] == 12);
//                },
//                None: () => Assert.True(false)
//            );
//        }

//        [Fact]
//        public void OptionalSetDivide()
//        {
//            var x = Some(Set(15, 30, 60));
//            var y = Some(Set(3, 5));
//            var z = x / y;

//            match(z,
//                Some: set =>
//                {
//                    Assert.True(set.Count == 6);
//                    Assert.True(set.Contains(5));
//                    Assert.True(set.Contains(10));
//                    Assert.True(set.Contains(20));
//                    Assert.True(set.Contains(3));
//                    Assert.True(set.Contains(6));
//                    Assert.True(set.Contains(12));
//                },
//                None: () => Assert.True(false)
//            );
//        }
//    }
//}
