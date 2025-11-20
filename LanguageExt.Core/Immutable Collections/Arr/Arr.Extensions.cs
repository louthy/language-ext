using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class ArrExtensions
{
    public static Arr<A> As<A>(this K<Arr, A> xs) =>
        (Arr<A>)xs;
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static A[] Flatten<A>(this A[][] ma) =>
        ma.Bind(identity).ToArray();

    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Arr<A> Flatten<A>(this Arr<Arr<A>> ma) =>
        ma.Bind(identity);

    [Pure]
    public static Arr<A> Filter<A>(this Arr<A> ma, Func<A, bool> f) =>
        Where(ma, f);

    [Pure]
    public static Arr<A> Where<A>(this Arr<A> ma, Func<A, bool> f)
    {
        var mb = new List<A>();
        foreach (var a in ma)
        {
            if (f(a))
            {
                mb.Add(a);
            }
        }
        return new Arr<A>(mb);
    }

    [Pure]
    public static Arr<B> Map<A, B>(this Arr<A> ma, Func<A, B> f) =>
        Select(ma, f);

    [Pure]
    public static Arr<B> Select<A, B>(this Arr<A> ma, Func<A, B> f)
    {
        var mb = new B[ma.Count];
        var index = 0;
        foreach (var a in ma)
        {
            mb[index] = f(a);
            index++;
        }
        return new Arr<B>(mb);
    }

    [Pure]
    public static Arr<B> Bind<A, B>(this Arr<A> ma, Func<A, Arr<B>> f) =>
        SelectMany(ma, f);

    [Pure]
    public static Arr<B> SelectMany<A, B>(this Arr<A> ma, Func<A, Arr<B>> f)
    {
        var mb = new List<B>();
        foreach (var a in ma)
        {
            foreach (var b in f(a))
            {
                mb.Add(b);
            }
        }
        return new Arr<B>(mb.ToArray());
    }

    [Pure]
    public static Arr<C> SelectMany<A, B, C>(this Arr<A> ma, Func<A, Arr<B>> bind, Func<A, B, C> project)
    {
        var mc = new List<C>();
        foreach (var a in ma)
        {
            foreach (var b in bind(a))
            {
                mc.Add(project(a, b));
            }
        }
        return new Arr<C>(mc.ToArray());
    }
    
    /// <summary>
    /// Convert to a queryable 
    /// </summary>
    [Pure]
    public static IQueryable<A> AsQueryable<A>(this Arr<A> source) =>
        // NOTE TO FUTURE ME: Don't delete this thinking it's not needed!
        // NOTE FROM FUTURE ME: Next time you leave a message for your future self, explain your reasoning.
        source.Value.AsQueryable();
}
