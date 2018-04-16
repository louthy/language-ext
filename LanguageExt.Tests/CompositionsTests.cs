using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using static LanguageExt.Compositions;
using LanguageExt.ClassInstances;
using Xunit;

namespace LanguageExt.Tests
{
    public class CompositionsTests
    {
        [Fact]
        public void ComposedIsMonoidMorphism1()
        {
            var m = default(MCompositions<TInt, int>);
            var t = default(TInt);
            var empty = Compositions<int>.Empty;

            var a = 123.Cons<TInt, int>(empty);
            var b = 456.Cons<TInt, int>(empty);
            var c = 789.Cons<TInt, int>(empty);

            Assert.True(composed<TInt, int>(m.Append(a, b)) == t.Append(composed<TInt, int>(a), composed<TInt, int>(b)));
        }

        [Fact]
        public void ComposedIsMonoidMorphism2()
        {
            var empty = Compositions<int>.Empty;

            var a = 123.Cons<TInt, int>(empty);
            var b = 456.Cons<TInt, int>(empty);
            var c = 789.Cons<TInt, int>(empty);

            var l = fromList<TInt, int>(List(123, 456, 789));

            Assert.True(composed<TInt, int>(l) == mconcat<TInt, int>(List(123, 456, 789)));
        }

        [Fact]
        public void CompositionsEquality1()
        {
            var empty = Compositions<int>.Empty;

            var ca = 123.Cons<TInt, int>(empty);
            var cb = 456.Cons<TInt, int>(empty);
            var cc = 123.Cons<TInt, int>(empty);

            Assert.True(ca != cb);
            Assert.True(ca == cc);
        }

        [Fact]
        public void CompositionsEquality2()
        {
            var empty = Compositions<int>.Empty;

            var ca = 123.Cons<TInt, int>(empty);
            ca = 123.Cons<TInt, int>(ca);

            var cb = 456.Cons<TInt, int>(empty);
            cb = 456.Cons<TInt, int>(cb);

            var cc = 123.Cons<TInt, int>(empty);
            cc = 123.Cons<TInt, int>(cc);

            Assert.True(ca != cb);
            Assert.True(ca == cc);
        }

        [Fact]
        public void CompositionsToListIsMorphism()
        {
            var m = default(MCompositions<TInt, int>);

            var empty = Compositions<int>.Empty;
            var ca = 123.Cons<TInt, int>(empty);
            var cb = 456.Cons<TInt, int>(empty);

            var cd = m.Append(ca, cb);
            var ce = fromList<TInt, int>(new[] { 123, 456 });

            Assert.True(cd == ce);

        }

        [Fact]
        public void CompositionsMonoidLaws()
        {
            var m = default(MCompositions<TInt, int>);
            var empty = Compositions<int>.Empty;

            var ca = 123.Cons<TInt, int>(empty);

            var cb = m.Append(m.Empty(), ca);
            var cc = m.Append(ca, m.Empty());

            var cd = 456.Cons<TInt, int>(empty);
            var ce = 789.Cons<TInt, int>(empty);

            var cf = m.Append(ca, m.Append(cd, ce));
            var cg = m.Append(m.Append(ca, cd), ce);

            Assert.True(ca == cb);
            Assert.True(ca == cc);
            Assert.True(cf == cg);
        }

    }
}
