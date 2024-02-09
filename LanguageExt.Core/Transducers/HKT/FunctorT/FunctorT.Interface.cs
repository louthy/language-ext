namespace LanguageExt.HKT;

/// <summary>
/// Functor 
/// </summary>
/// <typeparam name="G">FunctorT trait type</typeparam>
/// <typeparam name="F">Functor trait type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public interface FunctorT<F, G, A>
    where F : FunctorT<F, G>
    where G : Functor<G>
{
    public FunctorT<F, G, A> AsFunctorT() => this;
}
