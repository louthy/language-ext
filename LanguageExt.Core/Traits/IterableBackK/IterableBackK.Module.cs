namespace LanguageExt.Traits;

public static class IterableBackK
{
    /// <summary>
    /// Low-level interface for iterating using stack-based primitives.
    /// </summary>
    public static FS stepBackSetup<F, FS, A>(K<F, A> ta)
        where F : IterableBackK<F, FS> 
        where FS : allows ref struct =>
        F.StepBackSetup(ta);

    /// <summary>
    /// Low-level interface for iterating using stack-based primitives.
    /// </summary>
    public static bool stepBack<T, FS, A>(K<T, A> ta, ref FS refState, out A value)
        where T : IterableBackK<T, FS> 
        where FS : allows ref struct =>
        T.StepBack(ta, ref refState, out value);
}
