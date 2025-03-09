using System.Threading.Tasks;

namespace LanguageExt;

/// <summary>
/// Reducer delegate
/// </summary>
/// <typeparam name="S">State</typeparam>
/// <typeparam name="A">Value</typeparam>
public delegate ValueTask<Reduced<S>> Reducer<in A, S>(S state, A input);

public static class Reduced
{
    static Reduced()
    {
        Unit = Continue<Unit>(default);
        UnitAsync = new ValueTask<Reduced<Unit>>(Unit);
    }
    
    public static Reduced<A> Continue<A>(A value) =>
        new(true, value);
    
    public static Reduced<A> Done<A>(A value) =>
        new(false, value);

    public static readonly Reduced<Unit> Unit;
    
    public static ValueTask<Reduced<A>> ContinueAsync<A>(A value) =>
        new(new Reduced<A>(true, value));
    
    public static ValueTask<Reduced<A>> DoneAsync<A>(A value) =>
        new(new Reduced<A>(false, value));

    public static readonly ValueTask<Reduced<Unit>> UnitAsync;
}

public readonly record struct Reduced<A>(bool Continue, A Value);
