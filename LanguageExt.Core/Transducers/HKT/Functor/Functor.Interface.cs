namespace LanguageExt.HKT;

/// <summary>
/// Functor 
/// </summary>
/// <typeparam name="F">Functor trait type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public interface Functor<F, A> where F : Functor<F>
{
    public Functor<F, A> AsFunctor() => this;
}
