using System.Collections;
using System.Linq;
using LanguageExt;
//using LanguageExt.Trans;
using static LanguageExt.Prelude;
using Xunit;

namespace LanguageExt.Tests
{
    public class EnumerableTTests
    {
        [Fact]
        public void WrappedListTest()
        {
            var lst = List(List(1, 2, 3, 4, 5), List(1, 2, 3, 4, 5), List(1, 2, 3, 4, 5));

            var res = lst.FoldT(0, (s, v) => s + v);
            var mlst = lst.MapT(x => x * 2);
            var mres = mlst.FoldT(0, (s, v) => s + v);

            Assert.True(res == 45, "Expected 45 got "+ res);
            Assert.True(mres == 90, "Expected 90 got " + res);
            Assert.True(lst.CountT() == 15, "(lst) Expected 15 got " + lst.CountT());
            Assert.True(mlst.CountT() == 15, "(mlst) Expected 15 got " + mlst.CountT());

            lst = List<Lst<int>>();
            res = lst.FoldT(0, (s, v) => s + v);

            Assert.True(res == 0, "Fold results, expected 0 got " + res);
            Assert.True(lst.CountT() == 0, "Empty count, expected 0 got " + res);
        }

        [Fact]
        public void ChooseTest()
        {
            var input = List(
                Some(1),
                Some(2),
                Some(3),
                None,
                Some(4),
                None,
                Some(5));

            var actual = input.Choose(x => x).ToList();

            var expected = List(1, 2, 3, 4, 5);

            var toString = fun((IEnumerable items) => string.Join(", ", items));

            Assert.True(Enumerable.SequenceEqual(actual, expected), $"Expected {toString(expected)} but was {toString(actual)}");
        }

        //[Fact]
        //public void WrappedMapTest()
        //{
        //    var lst = List(
        //                  Map(
        //                      Tuple(1, "A"), 
        //                      Tuple(2, "B"), 
        //                      Tuple(3, "C"), 
        //                      Tuple(4, "D"), 
        //                      Tuple(5, "E")
        //                  ),
        //                  Map(
        //                      Tuple(1, "A"),
        //                      Tuple(2, "B"),
        //                      Tuple(3, "C"),
        //                      Tuple(4, "D"),
        //                      Tuple(5, "E")
        //                  ),
        //                  Map(
        //                      Tuple(1, "A"),
        //                      Tuple(2, "B"),
        //                      Tuple(3, "C"),
        //                      Tuple(4, "D"),
        //                      Tuple(5, "E")
        //                  )
        //              );
        //    var res = lst.FoldT("", (s, v) => s + v);
        //    var mlst = lst.MapT(x => x.ToLower());
        //    var mres = mlst.FoldT("", (s, v) => s + v);

        //    Assert.True(res == "ABCDEABCDEABCDE", "Expected ABCDEABCDEABCDE, got " + res);
        //    Assert.True(lst.CountT() == 15, "(lst) Expected 15, got " + lst.CountT());
        //    Assert.True(mlst.CountT() == 15, "(mlst) Expected 15, got " + mlst.CountT());

        //    //List.head(mopt)

        //    mlst.Match(
        //        ()      => failwith<Unit>("no items"),
        //        a       => failwith<Unit>("one item"),
        //        (a,b)   => failwith<Unit>("two items"),
        //        (a,b,c) =>
        //        {
        //            Assert.True(a[1] == "a");
        //            Assert.True(a[2] == "b");
        //            Assert.True(a[3] == "c");
        //            Assert.True(a[4] == "d");
        //            Assert.True(a[5] == "e");

        //            Assert.True(b[1] == "a");
        //            Assert.True(b[2] == "b");
        //            Assert.True(b[3] == "c");
        //            Assert.True(b[4] == "d");
        //            Assert.True(b[5] == "e");

        //            Assert.True(c[1] == "a");
        //            Assert.True(c[2] == "b");
        //            Assert.True(c[3] == "c");
        //            Assert.True(c[4] == "d");
        //            Assert.True(c[5] == "e");
        //            return unit;
        //        },
        //        (a,b,c,xs) => failwith<Unit>("more!")
        //    );
        //}
    }
}
