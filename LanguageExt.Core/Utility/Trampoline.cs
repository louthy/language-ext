#nullable enable
using System;
using System.Threading.Tasks;

namespace LanguageExt;

public class Trampoline
{
    public static TrampolineF<A> Continue<A>() =>
        new TrampolineContinueF<A>();

    public static TrampolineF<A, B> Continue<A, B>(A arg1) =>
        new TrampolineContinueF<A, B>(arg1);

    public static TrampolineF<A, B, C> Continue<A, B, C>(A arg1, B arg2) =>
        new TrampolineContinueF<A, B, C>(arg1, arg2);

    public static TrampolineF<A, B, C, D> Continue<A, B, C, D>(A arg1, B arg2, C arg3) =>
        new TrampolineContinueF<A, B, C, D>(arg1, arg2, arg3);

    public static TrampolineF<A> Complete<A>(A result) =>
        new TrampolineCompleteF<A>(result);

    public static TrampolineF<A, B> Complete<A, B>(B result) =>
        new TrampolineCompleteF<A, B>(result);

    public static TrampolineF<A, B, C> Complete<A, B, C>(C result) =>
        new TrampolineCompleteF<A, B, C>(result);

    public static TrampolineF<A, B, C, D> Complete<A, B, C, D>(D result) =>
        new TrampolineCompleteF<A, B, C, D>(result);

    public static A Start<A>(Func<TrampolineF<A>> f)
    {
        var bounce = TrampolineF<A>.Continue();
        while (true)
        {
            switch (bounce)
            {
                case TrampolineContinueF<A>:
                    bounce = f();
                    break;
                
                case TrampolineCompleteF<A> c:
                    return c.Result;
            }
        }
    }

    public static async ValueTask<A> StartAsync<A>(Func<ValueTask<TrampolineF<A>>> f)
    {
        var bounce = TrampolineF<A>.Continue();
        while (true)
        {
            switch (bounce)
            {
                case TrampolineContinueF<A>:
                    bounce = await f().ConfigureAwait(false);
                    break;
                
                case TrampolineCompleteF<A> c:
                    return c.Result;
            }
        }
    }

    public static B Start<A, B>(Func<A, TrampolineF<A, B>> f, A arg1)
    {
        var bounce = TrampolineF<A, B>.Continue(arg1);
        while (true)
        {
            switch (bounce)
            {
                case TrampolineContinueF<A, B> c:
                    bounce = f(c.Arg1);
                    break;
                
                case TrampolineCompleteF<A, B> c:
                    return c.Result;
            }
        }
    }

    public static async ValueTask<B> StartAsync<A, B>(Func<A, ValueTask<TrampolineF<A, B>>> f, A arg1)
    {
        var bounce = TrampolineF<A, B>.Continue(arg1);
        while (true)
        {
            switch (bounce)
            {
                case TrampolineContinueF<A, B> c:
                    bounce = await f(c.Arg1).ConfigureAwait(false);
                    break;
                
                case TrampolineCompleteF<A, B> c:
                    return c.Result;
            }
        }
    }

    public static C Start<A, B, C>(Func<A, B, TrampolineF<A, B, C>> f, A arg1, B arg2)
    {
        var bounce = TrampolineF<A, B, C>.Continue(arg1, arg2);
        while (true)
        {
            switch (bounce)
            {
                case TrampolineContinueF<A, B, C> c:
                    bounce = f(c.Arg1, c.Arg2);
                    break;
                
                case TrampolineCompleteF<A, B, C> c:
                    return c.Result;
            }
        }
    }
    
    public static async ValueTask<C> StartAsync<A, B, C>(Func<A, B, ValueTask<TrampolineF<A, B, C>>> f, A arg1, B arg2)
    {
        var bounce = TrampolineF<A, B, C>.Continue(arg1, arg2);
        while (true)
        {
            switch (bounce)
            {
                case TrampolineContinueF<A, B, C> c:
                    bounce = await f(c.Arg1, c.Arg2).ConfigureAwait(false);
                    break;
                
                case TrampolineCompleteF<A, B, C> c:
                    return c.Result;
            }
        }
    }

    public static D Start<A, B, C, D>(Func<A, B, C, TrampolineF<A, B, C, D>> f, A arg1, B arg2, C arg3)
    {
        var bounce = TrampolineF<A, B, C, D>.Continue(arg1, arg2, arg3);
        while (true)
        {
            switch (bounce)
            {
                case TrampolineContinueF<A, B, C, D> c:
                    bounce = f(c.Arg1, c.Arg2, c.Arg3);
                    break;
                
                case TrampolineCompleteF<A, B, C, D> c:
                    return c.Result;
            }
        }
    }
    
    public static async ValueTask<D> StartAsync<A, B, C, D>(
        Func<A, B, C, ValueTask<TrampolineF<A, B, C, D>>> f, 
        A arg1, B arg2, C arg3)
    {
        var bounce = TrampolineF<A, B, C, D>.Continue(arg1, arg2, arg3);
        while (true)
        {
            switch (bounce)
            {
                case TrampolineContinueF<A, B, C, D> c:
                    bounce = await f(c.Arg1, c.Arg2, c.Arg3).ConfigureAwait(false);
                    break;
                
                case TrampolineCompleteF<A, B, C, D> c:
                    return c.Result;
            }
        }
    }
}

/// <summary>
/// Zero argument trampoline 
/// </summary>
public abstract record TrampolineF<A>
{
    public static TrampolineF<A> Continue() =>
        new TrampolineContinueF<A>();
    
    public static TrampolineF<A> Complete(A result) =>
        new TrampolineCompleteF<A>(result);
}

/// <summary>
/// Zero argument trampoline continue 
/// </summary>
public record TrampolineContinueF<A> : TrampolineF<A>;

/// <summary>
/// Zero argument trampoline complete 
/// </summary>
public record TrampolineCompleteF<A>(A Result) : TrampolineF<A>;

/// <summary>
/// One argument trampoline 
/// </summary>
public abstract record TrampolineF<A, B>
{
    public static TrampolineF<A, B> Continue(A arg1) =>
        new TrampolineContinueF<A, B>(arg1);
    
    public static TrampolineF<A, B> Complete(B result) =>
        new TrampolineCompleteF<A, B>(result);
}

/// <summary>
/// One argument trampoline continue 
/// </summary>
public record TrampolineContinueF<A, B>(A Arg1) : TrampolineF<A, B>;

/// <summary>
/// One argument trampoline complete 
/// </summary>
public record TrampolineCompleteF<A, B>(B Result) : TrampolineF<A, B>;

/// <summary>
/// Two argument trampoline 
/// </summary>
public abstract record TrampolineF<A, B, C>
{
    public static TrampolineF<A, B, C> Continue(A arg1, B arg2) =>
        new TrampolineContinueF<A, B, C>(arg1, arg2);
    
    public static TrampolineF<A, B, C> Complete(C result) =>
        new TrampolineCompleteF<A, B, C>(result);
}

/// <summary>
/// Two argument trampoline continue 
/// </summary>
public record TrampolineContinueF<A, B, C>(A Arg1, B Arg2) : TrampolineF<A, B, C>;

/// <summary>
/// Two argument trampoline complete 
/// </summary>
public record TrampolineCompleteF<A, B, C>(C Result) : TrampolineF<A, B, C>;

/// <summary>
/// Three argument trampoline 
/// </summary>
public abstract record TrampolineF<A, B, C, D>
{
    public static TrampolineF<A, B, C, D> Continue(A arg1, B arg2, C arg3) =>
        new TrampolineContinueF<A, B, C, D>(arg1, arg2, arg3);
    
    public static TrampolineF<A, B, C, D> Complete(D result) =>
        new TrampolineCompleteF<A, B, C, D>(result);
}

/// <summary>
/// Three argument trampoline continue 
/// </summary>
public record TrampolineContinueF<A, B, C, D>(A Arg1, B Arg2, C Arg3) : TrampolineF<A, B, C, D>;

/// <summary>
/// Three argument trampoline complete 
/// </summary>
public record TrampolineCompleteF<A, B, C, D>(D Result) : TrampolineF<A, B, C, D>;
