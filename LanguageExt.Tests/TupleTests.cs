using NUnit.Framework;
using static LanguageExt.Prelude;

namespace LanguageExtTests
{
    [TestFixture]
    public class TupleTests
    {
        [Test] public void TupleGeneratorTests()
        {
            var t2 = tuple("a", "b");
            var t3 = tuple("a", "b", "c");
            var t4 = tuple("a", "b", "c", "d");
            var t5 = tuple("a", "b", "c", "d", "e");
            var t6 = tuple("a", "b", "c", "d", "e", "f");
            var t7 = tuple("a", "b", "c", "d", "e", "f", "g");

            Assert.IsTrue(t2.Item1 == "a" && t2.Item2 == "b");
            Assert.IsTrue(t3.Item1 == "a" && t3.Item2 == "b" && t3.Item3 == "c");
            Assert.IsTrue(t4.Item1 == "a" && t4.Item2 == "b" && t4.Item3 == "c" && t4.Item4 == "d");
            Assert.IsTrue(t5.Item1 == "a" && t5.Item2 == "b" && t5.Item3 == "c" && t5.Item4 == "d" && t5.Item5 == "e");
            Assert.IsTrue(t6.Item1 == "a" && t6.Item2 == "b" && t6.Item3 == "c" && t6.Item4 == "d" && t6.Item5 == "e" && t6.Item6 == "f");
            Assert.IsTrue(t6.Item1 == "a" && t6.Item2 == "b" && t6.Item3 == "c" && t6.Item4 == "d" && t6.Item5 == "e" && t6.Item6 == "f");
            Assert.IsTrue(t7.Item1 == "a" && t7.Item2 == "b" && t7.Item3 == "c" && t7.Item4 == "d" && t7.Item5 == "e" && t7.Item6 == "f" && t7.Item7 == "g");
        }

        [Test] public void WithApplicationTests1()
        {
            tuple("a", "b").Map((a, b) => Assert.IsTrue(a == "a" && b == "b"));
            tuple("a", "b", "c").Map((a, b, c) => Assert.IsTrue(a == "a" && b == "b" && c == "c"));
            tuple("a", "b", "c", "d").Map((a, b, c, d) => Assert.IsTrue(a == "a" && b == "b" && c == "c" && d == "d"));
            tuple("a", "b", "c", "d", "e").Map((a, b, c, d, e) => Assert.IsTrue(a == "a" && b == "b" && c == "c" && d == "d" && e == "e"));
            tuple("a", "b", "c", "d", "e", "f").Map((a, b, c, d, e, f) => Assert.IsTrue(a == "a" && b == "b" && c == "c" && d == "d" && e == "e" && f == "f"));
            tuple("a", "b", "c", "d", "e", "f", "g").Map((a, b, c, d, e, f, g) => Assert.IsTrue(a == "a" && b == "b" && c == "c" && d == "d" && e == "e" && f == "f" && g == "g"));
        }

        [Test] public void WithApplicationTests2()
        {
            map( tuple("a", "b"), (a, b) => Assert.IsTrue(a == "a" && b == "b"));
            map( tuple("a", "b", "c"), (a, b, c) => Assert.IsTrue(a == "a" && b == "b" && c == "c"));
            map( tuple("a", "b", "c", "d"), (a, b, c, d) => Assert.IsTrue(a == "a" && b == "b" && c == "c" && d == "d"));
            map( tuple("a", "b", "c", "d", "e"), (a, b, c, d, e) => Assert.IsTrue(a == "a" && b == "b" && c == "c" && d == "d" && e == "e"));
            map( tuple("a", "b", "c", "d", "e", "f"), (a, b, c, d, e, f) => Assert.IsTrue(a == "a" && b == "b" && c == "c" && d == "d" && e == "e" && f == "f"));
            map( tuple("a", "b", "c", "d", "e", "f", "g"), (a, b, c, d, e, f, g) => Assert.IsTrue(a == "a" && b == "b" && c == "c" && d == "d" && e == "e" && f == "f" && g == "g"));
        }

    }
}
