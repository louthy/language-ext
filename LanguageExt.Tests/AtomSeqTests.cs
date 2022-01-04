using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    public class AtomSeqTests
    {
        [Fact]
        public void SwapInvokesChange()
        {
            var seq = AtomSeq(1, 2, 3, 4, 5);
            Seq<int> state = default;
            seq.Change += v => state = v;

            seq.Swap(s => 0.Cons(s));

            Assert.Equal(seq.ToSeq(), state);
        }

        [Fact]
        public void AddInvokesChange()
        {
            var seq = AtomSeq(1, 2, 3, 4, 5);
            var toConcat = Seq(6, 7, 8);
            Seq<int> state = default;
            seq.Change += v => state = v;

            seq.Concat(toConcat);

            Assert.Equal(seq.ToSeq(), state);
        }

        [Fact]
        public void MapInPlaceInvokesChange()
        {
            var seq = AtomSeq(1, 2, 3, 4, 5);
            Seq<int> state = default;
            seq.Change += v => state = v;

            seq.MapInPlace(i => i * 2);

            Assert.Equal(seq.ToSeq(), state);
        }

        [Fact]
        public void BindInPlaceInvokesChange()
        {
            var seq = AtomSeq(1, 2, 3, 4, 5);
            Seq<int> state = default;
            seq.Change += v => state = v;

            seq.BindInPlace<int>(i => Range(0, i).ToSeq());

            Assert.Equal(seq.ToSeq(), state);
        }

        [Fact]
        public void FilterInPlaceInvokesChange()
        {
            var seq = AtomSeq(1, 2, 3, 4, 5);
            Seq<int> state = default;
            seq.Change += v => state = v;

            seq.FilterInPlace(i => i % 2 == 0);

            Assert.Equal(seq.ToSeq(), state);
        }

        [Fact]
        public void IntersperseInvokesChange()
        {
            var seq = AtomSeq(1, 2, 3, 4, 5);
            Seq<int> state = default;
            seq.Change += v => state = v;

            seq.Intersperse(9);

            Assert.Equal(seq.ToSeq(), state);
        }

        [Fact]
        public void SkipInvokesChange()
        {
            var seq = AtomSeq(1, 2, 3, 4, 5);
            Seq<int> state = default;
            seq.Change += v => state = v;

            seq.Skip(2);

            Assert.Equal(seq.ToSeq(), state);
        }

        [Fact]
        public void TakeInvokesChange()
        {
            var seq = AtomSeq(1, 2, 3, 4, 5);
            Seq<int> state = default;
            seq.Change += v => state = v;

            seq.Take(2);

            Assert.Equal(seq.ToSeq(), state);
        }

        [Fact]
        public void TakeWhileInvokesChange()
        {
            var seq = AtomSeq(1, 2, 3, 4, 5);
            Seq<int> state = default;
            seq.Change += v => state = v;

            seq.TakeWhile(i => i < 3);

            Assert.Equal(seq.ToSeq(), state);
        }

        [Fact]
        public void TakeWhileIndexInvokesChange()
        {
            var seq = AtomSeq(1, 2, 3, 4, 5);
            Seq<int> state = default;
            seq.Change += v => state = v;

            seq.TakeWhile((i, ix) => ix < 3);

            Assert.Equal(seq.ToSeq(), state);
        }
    }
}
