namespace LanguageExt;

public static class LensExtensions
{
    extension<A, B, C>(Lens<A, B> _)
    {
        public static Lens<A, C> operator |(Lens<A, B> lhs, Lens<B, C> rhs) =>
            Lens<A, C>.New(
                Get: a => rhs.Get(lhs.Get(a)),
                Set: v => lhs.Update(rhs.SetF(v)));
    }
}
