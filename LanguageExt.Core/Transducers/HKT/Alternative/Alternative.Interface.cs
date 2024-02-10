namespace LanguageExt.HKT;

public interface Alternative<F, A> : Applicative<F, A>
    where F : Alternative<F>
{
    public static Alternative<F, A> operator |(Alternative<F, A> ma, Alternative<F, A> mb) =>
        F.Or(ma, mb);
    
    public static Alternative<F, A> operator |(Alternative<F, A> ma, Applicative<F, A> mb) =>
        F.Or(ma, mb);
    
    public static Alternative<F, A> operator |(Applicative<F, A> ma, Alternative<F, A> mb) =>
        F.Or(ma, mb);
}
