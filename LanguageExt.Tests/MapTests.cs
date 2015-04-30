using static LanguageExt.Prelude;
using static LanguageExt.Map;
using NUnit.Framework;

namespace LanguageExtTests
{
    [TestFixture]
    public class MapTests
    {
        [Test]
        public void MapGeneratorTest()
        {
            var m1 = map<int, string>();
            m1 = add(m1, 100, "hello");
            Assert.IsTrue(m1.Count == 1 && containsKey(m1,100));
        }

        [Test]
        public void MapGeneratorAndMatchTest()
        {
            var m2 = map( tuple(1, "a"),
                          tuple(2, "b"),
                          tuple(3, "c") );

            m2 = add(m2, 100, "world");

            var res = match(
                m2, 100,
                v  => v,
                () => "failed"
            );

            Assert.IsTrue(res == "world");
        }

        [Test]
        public void MapSetTest()
        {
            var m1 = map( tuple(1, "a"),
                          tuple(2, "b"),
                          tuple(3, "c") );

            var m2 = setItem(m1, 1, "x");

            match( 
                m1, 1, 
                Some: v => Assert.IsTrue(v == "a"), 
                None: () => Assert.Fail() 
                );

            match(
                find(m2, 1),
                Some: v => Assert.IsTrue(v == "x"),
                None: () => Assert.Fail()
                );
        }
    }
}
