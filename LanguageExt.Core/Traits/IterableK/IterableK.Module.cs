namespace LanguageExt.Traits;

public static class IterableK
{
    /// <summary>
    /// Low-level interface for iterating using stack-based primitives.
    /// </summary>
    public static FS stepSetup<F, FS, A>(K<F, A> ta)
        where F : IterableK<F, FS> 
        where FS : allows ref struct =>
        F.StepSetup(ta);

    /// <summary>
    /// Low-level interface for iterating using stack-based primitives.
    /// </summary>
    public static bool step<T, FS, A>(K<T, A> ta, ref FS refState, out A value)
        where T : IterableK<T, FS> 
        where FS : allows ref struct =>
        T.Step(ta, ref refState, out value);
}
