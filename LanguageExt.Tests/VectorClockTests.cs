using Newtonsoft.Json;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    public class VectorClockTests
    {
        [Fact]
        public void SerialisationTest1()
        {
            var vc1 = VectorClock<char>.fromList(Seq(('a', 1L), ('b', 2L)));

            var jvc = JsonConvert.SerializeObject(vc1);
            var vc2 = JsonConvert.DeserializeObject<VectorClock<char>>(jvc);

            Assert.Equal(vc1, vc2);
        }

        [Fact]
        public void MaxTest()
        {
            var vc1 = VectorClock.fromList(Seq(('a', 1L), ('b', 2L)));
            var vc2 = VectorClock.fromList(Seq(('c', 3L), ('b', 1L)));

            var vc3 = VectorClock.max(vc1, vc2);
            
            var exp = VectorClock.fromList(Seq(('a', 1L), ('b', 2L), ('c', 3L)));

            Assert.Equal(vc3, exp);
        }

        [Fact]
        public void CausesTest()
        {
            var vc1 = VectorClock.fromList(Seq(('a', 1L), ('b', 2L)));
            var vc2 = VectorClock.fromList(Seq(('a', 2L), ('b', 2L)));

            var rel = VectorClock.relation(vc1, vc2);
            var flg = VectorClock.causes(vc1, vc2);
            
            Assert.Equal(Relation.Causes, rel);
            Assert.True(flg);
        }

        [Fact]
        public void CausedByTest()
        {
            var vc1 = VectorClock.fromList(Seq(('a', 2L), ('b', 2L)));
            var vc2 = VectorClock.fromList(Seq(('a', 1L), ('b', 2L)));

            var rel = VectorClock.relation(vc1, vc2);
            var flg = VectorClock.causes(vc1, vc2);
            
            Assert.Equal(Relation.CausedBy, rel);
            Assert.False(flg);
        }

        [Fact]
        public void ConcurrentTest()
        {
            var vc1 = VectorClock.fromList(Seq(('a', 2L), ('b', 2L)));
            var vc2 = VectorClock.fromList(Seq(('a', 1L), ('b', 3L)));

            var rel = VectorClock.relation(vc1, vc2);
            var flg = VectorClock.causes(vc1, vc2);
            
            Assert.Equal(Relation.Concurrent, rel);
            Assert.False(flg);
        }
    }
}
