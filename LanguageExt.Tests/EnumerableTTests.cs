using LanguageExt;
using LanguageExt.Trans;
using static LanguageExt.Prelude;

using NU = NUnit.Framework;

namespace LanguageExtTests
{
    [NU.TestFixture]
    public class EnumerableTTests
    {
        [NU.Test]
        public void WrappedListTest()
        {
            var lst = list(list(1, 2, 3, 4, 5), list(1, 2, 3, 4, 5), list(1, 2, 3, 4, 5));
            var res = lst.FoldT(0, (s, v) => s + v);
            var mlst = lst.MapT(x => x * 2);
            var mres = mlst.FoldT(0, (s, v) => s + v);

            NU.Assert.IsTrue(res == 45, "Expected 45 got "+ res);
            NU.Assert.IsTrue(mres == 90, "Expected 90 got " + res);
            NU.Assert.IsTrue(lst.CountT() == 15, "(lst) Expected 15 got " + lst.CountT());
            NU.Assert.IsTrue(mlst.CountT() == 15, "(mlst) Expected 15 got " + mlst.CountT());

            lst = list<Lst<int>>();
            res = lst.FoldT(0, (s, v) => s + v);

            NU.Assert.IsTrue(res == 0, "Fold results, expected 0 got " + res);
            NU.Assert.IsTrue(lst.CountT() == 0, "Empty count, expected 0 got " + res);
        }

        [NU.Test]
        public void WrappedMapTest()
        {
            var lst = list(
                          map(
                              tuple(1, "A"), 
                              tuple(2, "B"), 
                              tuple(3, "C"), 
                              tuple(4, "D"), 
                              tuple(5, "E")
                          ),
                          map(
                              tuple(1, "A"),
                              tuple(2, "B"),
                              tuple(3, "C"),
                              tuple(4, "D"),
                              tuple(5, "E")
                          ),
                          map(
                              tuple(1, "A"),
                              tuple(2, "B"),
                              tuple(3, "C"),
                              tuple(4, "D"),
                              tuple(5, "E")
                          )
                      );
            var res = lst.FoldT("", (s, v) => s + v);
            var mlst = lst.MapT(x => x.ToLower());
            var mres = mlst.FoldT("", (s, v) => s + v);

            NU.Assert.IsTrue(res == "ABCDEABCDEABCDE", "Expected ABCDEABCDEABCDE, got " + res);
            NU.Assert.IsTrue(lst.CountT() == 15, "(lst) Expected 15, got " + lst.CountT());
            NU.Assert.IsTrue(mlst.CountT() == 15, "(mlst) Expected 15, got " + mlst.CountT());

            //List.head(mopt)

            mlst.Match(
                ()      => failwith<Unit>("no items"),
                a       => failwith<Unit>("one item"),
                (a,b)   => failwith<Unit>("two items"),
                (a,b,c) =>
                {
                    NU.Assert.IsTrue(a[1] == "a");
                    NU.Assert.IsTrue(a[2] == "b");
                    NU.Assert.IsTrue(a[3] == "c");
                    NU.Assert.IsTrue(a[4] == "d");
                    NU.Assert.IsTrue(a[5] == "e");

                    NU.Assert.IsTrue(b[1] == "a");
                    NU.Assert.IsTrue(b[2] == "b");
                    NU.Assert.IsTrue(b[3] == "c");
                    NU.Assert.IsTrue(b[4] == "d");
                    NU.Assert.IsTrue(b[5] == "e");

                    NU.Assert.IsTrue(c[1] == "a");
                    NU.Assert.IsTrue(c[2] == "b");
                    NU.Assert.IsTrue(c[3] == "c");
                    NU.Assert.IsTrue(c[4] == "d");
                    NU.Assert.IsTrue(c[5] == "e");
                    return unit;
                },
                (x,xs) => failwith<Unit>("more!")
            );
        }
    }
}
