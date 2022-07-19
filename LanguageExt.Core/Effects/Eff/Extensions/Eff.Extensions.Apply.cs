using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class EffExtensions
{
    // ------------ Eff<A> ---------------------------------------------------------------------------------------------
    
    public static Eff<B> Apply<A, B>(this Eff<Func<A, B>> mf, Eff<A> ma) =>
        EffMaybe<B>(() =>
        {
            var (f, a) = (mf.Run(), ma.Run());

            if (f.IsFail || a.IsFail)
            {
                var errs = Errors.None;
                if(f.IsFail) errs += f.Error;
                if(a.IsFail) errs += a.Error;
                return errs;
            }
            else
            {
                return f.Value(a.Value);
            }
        });
        
    public static Eff<C> Apply<A, B, C>(this Eff<Func<A, B, C>> mf, Eff<A> ma, Eff<B> mb) =>
        EffMaybe<C>(() =>
        {
            var (f, a, b) = (mf.Run(), ma.Run(), mb.Run());

            if (f.IsFail || a.IsFail|| b.IsFail)
            {
                var errs = Errors.None;
                if(f.IsFail) errs += f.Error;
                if(a.IsFail) errs += a.Error;
                if(b.IsFail) errs += b.Error;
                return errs;
            }
            else
            {
                return f.Value(a.Value, b.Value);
            }
        });
        
    public static Eff<Func<B, C>> Apply<A, B, C>(this Eff<Func<A, B, C>> mf, Eff<A> ma) =>
        EffMaybe(() =>
        {
            var (f, a) = (mf.Run(), ma.Run());

            if (f.IsFail || a.IsFail)
            {
                var errs = Errors.None;
                if(f.IsFail) errs += f.Error;
                if(a.IsFail) errs += a.Error;
                return errs;
            }
            else
            {
                return FinSucc((B b) => f.Value(a.Value, b));
            }
        });
        
    public static Eff<D> Apply<A, B, C, D>(this Eff<Func<A, B, C, D>> mf, Eff<A> ma, Eff<B> mb, Eff<C> mc) =>
        EffMaybe<D>(() =>
        {
            var (f, a, b, c) = (mf.Run(), ma.Run(), mb.Run(), mc.Run());

            if (f.IsFail || a.IsFail || b.IsFail || c.IsFail)
            {
                var errs = Errors.None;
                if(f.IsFail) errs += f.Error;
                if(a.IsFail) errs += a.Error;
                if(b.IsFail) errs += b.Error;
                if(c.IsFail) errs += c.Error;
                return errs;
            }
            else
            {
                return f.Value(a.Value, b.Value, c.Value);
            }
        });
        
    public static Eff<Func<C, D>> Apply<A, B, C, D>(this Eff<Func<A, B, C, D>> mf, Eff<A> ma, Eff<B> mb) =>
        EffMaybe(() =>
        {
            var (f, a, b) = (mf.Run(), ma.Run(), mb.Run());

            if (f.IsFail || a.IsFail || b.IsFail)
            {
                var errs = Errors.None;
                if(f.IsFail) errs += f.Error;
                if(a.IsFail) errs += a.Error;
                if(b.IsFail) errs += b.Error;
                return errs;
            }
            else
            {
                return FinSucc((C c) => f.Value(a.Value, b.Value, c));
            }
        });
        
    public static Eff<Func<B, C, D>> Apply<A, B, C, D>(this Eff<Func<A, B, C, D>> mf, Eff<A> ma) =>
        EffMaybe(() =>
        {
            var (f, a) = (mf.Run(), ma.Run());

            if (f.IsFail || a.IsFail)
            {
                var errs = Errors.None;
                if(f.IsFail) errs += f.Error;
                if(a.IsFail) errs += a.Error;
                return errs;
            }
            else
            {
                return FinSucc((B b, C c) => f.Value(a.Value, b, c));
            }
        });
        
    public static Eff<E> Apply<A, B, C, D, E>(this Eff<Func<A, B, C, D, E>> mf, Eff<A> ma, Eff<B> mb, Eff<C> mc, Eff<D> md) =>
        EffMaybe<E>(() =>
        {
            var (f, a, b, c, d) = (mf.Run(), ma.Run(), mb.Run(), mc.Run(), md.Run());

            if (f.IsFail || a.IsFail || b.IsFail || c.IsFail || d.IsFail)
            {
                var errs = Errors.None;
                if(f.IsFail) errs += f.Error;
                if(a.IsFail) errs += a.Error;
                if(b.IsFail) errs += b.Error;
                if(c.IsFail) errs += c.Error;
                if(d.IsFail) errs += d.Error;
                return errs;
            }
            else
            {
                return f.Value(a.Value, b.Value, c.Value, d.Value);
            }
        });
             
    public static Eff<Func<D, E>> Apply<A, B, C, D, E>(this Eff<Func<A, B, C, D, E>> mf, Eff<A> ma, Eff<B> mb, Eff<C> mc) =>
        EffMaybe(() =>
        {
            var (f, a, b, c) = (mf.Run(), ma.Run(), mb.Run(), mc.Run());

            if (f.IsFail || a.IsFail || b.IsFail || c.IsFail)
            {
                var errs = Errors.None;
                if(f.IsFail) errs += f.Error;
                if(a.IsFail) errs += a.Error;
                if(b.IsFail) errs += b.Error;
                if(c.IsFail) errs += c.Error;
                return errs;
            }
            else
            {
                return FinSucc((D d) => f.Value(a.Value, b.Value, c.Value, d));
            }
        });
             
    public static Eff<Func<C, D, E>> Apply<A, B, C, D, E>(this Eff<Func<A, B, C, D, E>> mf, Eff<A> ma, Eff<B> mb) =>
        EffMaybe(() =>
        {
            var (f, a, b) = (mf.Run(), ma.Run(), mb.Run());

            if (f.IsFail || a.IsFail || b.IsFail)
            {
                var errs = Errors.None;
                if(f.IsFail) errs += f.Error;
                if(a.IsFail) errs += a.Error;
                if(b.IsFail) errs += b.Error;
                return errs;
            }
            else
            {
                return FinSucc((C c, D d) => f.Value(a.Value, b.Value, c, d));
            }
        });
             
    public static Eff<Func<B, C, D, E>> Apply<A, B, C, D, E>(this Eff<Func<A, B, C, D, E>> mf, Eff<A> ma) =>
        EffMaybe(() =>
        {
            var (f, a) = (mf.Run(), ma.Run());

            if (f.IsFail || a.IsFail)
            {
                var errs = Errors.None;
                if(f.IsFail) errs += f.Error;
                if(a.IsFail) errs += a.Error;
                return errs;
            }
            else
            {
                return FinSucc((B b, C c, D d) => f.Value(a.Value, b, c, d));
            }
        });
    
    // ------------ Eff<RT, A> -----------------------------------------------------------------------------------------
    
    public static Eff<RT, B> Apply<RT, A, B>(this Eff<RT, Func<A, B>> mf, Eff<RT, A> ma) where RT : struct =>
        EffMaybe<RT, B>(rt =>
        {
            var (f, a) = (mf.Run(rt), ma.Run(rt));

            if (f.IsFail || a.IsFail)
            {
                var errs = Errors.None;
                if(f.IsFail) errs += f.Error;
                if(a.IsFail) errs += a.Error;
                return errs;
            }
            else
            {
                return f.Value(a.Value);
            }
        });
        
    public static Eff<RT, C> Apply<RT, A, B, C>(this Eff<RT, Func<A, B, C>> mf, Eff<RT, A> ma, Eff<RT, B> mb) where RT : struct =>
        EffMaybe<RT, C>(rt =>
        {
            var (f, a, b) = (mf.Run(rt), ma.Run(rt), mb.Run(rt));

            if (f.IsFail || a.IsFail|| b.IsFail)
            {
                var errs = Errors.None;
                if(f.IsFail) errs += f.Error;
                if(a.IsFail) errs += a.Error;
                if(b.IsFail) errs += b.Error;
                return errs;
            }
            else
            {
                return f.Value(a.Value, b.Value);
            }
        });
        
    public static Eff<RT, Func<B, C>> Apply<RT, A, B, C>(this Eff<RT, Func<A, B, C>> mf, Eff<RT, A> ma) where RT : struct  =>
        EffMaybe<RT, Func<B, C>>(rt =>
        {
            var (f, a) = (mf.Run(rt), ma.Run(rt));

            if (f.IsFail || a.IsFail)
            {
                var errs = Errors.None;
                if(f.IsFail) errs += f.Error;
                if(a.IsFail) errs += a.Error;
                return errs;
            }
            else
            {
                return FinSucc((B b) => f.Value(a.Value, b));
            }
        });
        
    public static Eff<RT, D> Apply<RT, A, B, C, D>(this Eff<RT, Func<A, B, C, D>> mf, Eff<RT, A> ma, Eff<RT, B> mb, Eff<RT, C> mc)  where RT : struct =>
        EffMaybe<RT, D>(rt =>
        {
            var (f, a, b, c) = (mf.Run(rt), ma.Run(rt), mb.Run(rt), mc.Run(rt));

            if (f.IsFail || a.IsFail || b.IsFail || c.IsFail)
            {
                var errs = Errors.None;
                if(f.IsFail) errs += f.Error;
                if(a.IsFail) errs += a.Error;
                if(b.IsFail) errs += b.Error;
                if(c.IsFail) errs += c.Error;
                return errs;
            }
            else
            {
                return f.Value(a.Value, b.Value, c.Value);
            }
        });
        
    public static Eff<RT, Func<C, D>> Apply<RT, A, B, C, D>(this Eff<RT, Func<A, B, C, D>> mf, Eff<RT, A> ma, Eff<RT, B> mb) where RT : struct  =>
        EffMaybe<RT, Func<C, D>>(rt =>
        {
            var (f, a, b) = (mf.Run(rt), ma.Run(rt), mb.Run(rt));

            if (f.IsFail || a.IsFail || b.IsFail)
            {
                var errs = Errors.None;
                if(f.IsFail) errs += f.Error;
                if(a.IsFail) errs += a.Error;
                if(b.IsFail) errs += b.Error;
                return errs;
            }
            else
            {
                return FinSucc((C c) => f.Value(a.Value, b.Value, c));
            }
        });
        
    public static Eff<RT, Func<B, C, D>> Apply<RT, A, B, C, D>(this Eff<RT, Func<A, B, C, D>> mf, Eff<RT, A> ma)  where RT : struct =>
        EffMaybe<RT, Func<B, C, D>>(rt =>
        {
            var (f, a) = (mf.Run(rt), ma.Run(rt));

            if (f.IsFail || a.IsFail)
            {
                var errs = Errors.None;
                if(f.IsFail) errs += f.Error;
                if(a.IsFail) errs += a.Error;
                return errs;
            }
            else
            {
                return FinSucc((B b, C c) => f.Value(a.Value, b, c));
            }
        });
            
    public static Eff<RT, E> Apply<RT, A, B, C, D, E>(this Eff<RT, Func<A, B, C, D, E>> mf, Eff<RT, A> ma, Eff<RT, B> mb, Eff<RT, C> mc, Eff<RT, D> md) where RT : struct  =>
        EffMaybe<RT, E>(rt =>
        {
            var (f, a, b, c, d) = (mf.Run(rt), ma.Run(rt), mb.Run(rt), mc.Run(rt), md.Run(rt));

            if (f.IsFail || a.IsFail || b.IsFail || c.IsFail || d.IsFail)
            {
                var errs = Errors.None;
                if(f.IsFail) errs += f.Error;
                if(a.IsFail) errs += a.Error;
                if(b.IsFail) errs += b.Error;
                if(c.IsFail) errs += c.Error;
                if(d.IsFail) errs += d.Error;
                return errs;
            }
            else
            {
                return f.Value(a.Value, b.Value, c.Value, d.Value);
            }
        });
             
    public static Eff<RT, Func<D, E>> Apply<RT, A, B, C, D, E>(this Eff<RT, Func<A, B, C, D, E>> mf, Eff<RT, A> ma, Eff<RT, B> mb, Eff<RT, C> mc) where RT : struct  =>
        EffMaybe<RT, Func<D, E>>(rt =>
        {
            var (f, a, b, c) = (mf.Run(rt), ma.Run(rt), mb.Run(rt), mc.Run(rt));

            if (f.IsFail || a.IsFail || b.IsFail || c.IsFail)
            {
                var errs = Errors.None;
                if(f.IsFail) errs += f.Error;
                if(a.IsFail) errs += a.Error;
                if(b.IsFail) errs += b.Error;
                if(c.IsFail) errs += c.Error;
                return errs;
            }
            else
            {
                return FinSucc((D d) => f.Value(a.Value, b.Value, c.Value, d));
            }
        });
             
    public static Eff<RT, Func<C, D, E>> Apply<RT, A, B, C, D, E>(this Eff<RT, Func<A, B, C, D, E>> mf, Eff<RT, A> ma, Eff<RT, B> mb) where RT : struct  =>
        EffMaybe<RT, Func<C, D, E>>(rt =>
        {
            var (f, a, b) = (mf.Run(rt), ma.Run(rt), mb.Run(rt));

            if (f.IsFail || a.IsFail || b.IsFail)
            {
                var errs = Errors.None;
                if(f.IsFail) errs += f.Error;
                if(a.IsFail) errs += a.Error;
                if(b.IsFail) errs += b.Error;
                return errs;
            }
            else
            {
                return FinSucc((C c, D d) => f.Value(a.Value, b.Value, c, d));
            }
        });
             
    public static Eff<RT, Func<B, C, D, E>> Apply<RT, A, B, C, D, E>(this Eff<RT, Func<A, B, C, D, E>> mf, Eff<RT, A> ma) where RT : struct  =>
        EffMaybe<RT, Func<B, C, D, E>>(rt =>
        {
            var (f, a) = (mf.Run(rt), ma.Run(rt));

            if (f.IsFail || a.IsFail)
            {
                var errs = Errors.None;
                if(f.IsFail) errs += f.Error;
                if(a.IsFail) errs += a.Error;
                return errs;
            }
            else
            {
                return FinSucc((B b, C c, D d) => f.Value(a.Value, b, c, d));
            }
        });
    
    // ------------ Non Eff functions ----------------------------------------------------------------------------------
    
    public static Eff<RT, B> Apply<RT, A, B>(this Func<A, B> f, Eff<RT, A> ma) where RT : struct  =>
        EffMaybe<RT, B>(rt =>
        {
            var a = ma.Run(rt);

            if (a.IsFail)
            {
                return a.Error;
            }
            else
            {
                return f(a.Value);
            }
        });
        
    public static Eff<RT, C> Apply<RT, A, B, C>(this Func<A, B, C> f, Eff<RT, A> ma, Eff<RT, B> mb)  where RT : struct =>
        EffMaybe<RT, C>(rt =>
        {
            var (a, b) = (ma.Run(rt), mb.Run(rt));

            if (a.IsFail|| b.IsFail)
            {
                var errs = Errors.None;
                if(a.IsFail) errs += a.Error;
                if(b.IsFail) errs += b.Error;
                return errs;
            }
            else
            {
                return f(a.Value, b.Value);
            }
        });
        
    public static Eff<RT, Func<B, C>> Apply<RT, A, B, C>(this Func<A, B, C> f, Eff<RT, A> ma) where RT : struct  =>
        EffMaybe<RT, Func<B, C>>(rt =>
        {
            var a = ma.Run(rt);

            if (a.IsFail)
            {
                return a.Error;
            }
            else
            {
                return FinSucc((B b) => f(a.Value, b));
            }
        });
        
    public static Eff<RT, D> Apply<RT, A, B, C, D>(this Func<A, B, C, D> f, Eff<RT, A> ma, Eff<RT, B> mb, Eff<RT, C> mc) where RT : struct  =>
        EffMaybe<RT, D>(rt =>
        {
            var (a, b, c) = (ma.Run(rt), mb.Run(rt), mc.Run(rt));

            if (a.IsFail || b.IsFail || c.IsFail)
            {
                var errs = Errors.None;
                if(a.IsFail) errs += a.Error;
                if(b.IsFail) errs += b.Error;
                if(c.IsFail) errs += c.Error;
                return errs;
            }
            else
            {
                return f(a.Value, b.Value, c.Value);
            }
        });
        
    public static Eff<RT, Func<C, D>> Apply<RT, A, B, C, D>(this Func<A, B, C, D> f, Eff<RT, A> ma, Eff<RT, B> mb) where RT : struct  =>
        EffMaybe<RT, Func<C, D>>(rt =>
        {
            var (a, b) = (ma.Run(rt), mb.Run(rt));
            
            if (a.IsFail || b.IsFail)
            {
                var errs = Errors.None;
                if(a.IsFail) errs += a.Error;
                if(b.IsFail) errs += b.Error;
                return errs;
            }
            else
            {
                return FinSucc((C c) => f(a.Value, b.Value, c));
            }
        });
        
    public static Eff<RT, Func<B, C, D>> Apply<RT, A, B, C, D>(this Func<A, B, C, D> f, Eff<RT, A> ma) where RT : struct  =>
        EffMaybe<RT, Func<B, C, D>>(rt =>
        {
            var a = ma.Run(rt);

            if (a.IsFail)
            {
                return a.Error;
            }
            else
            {
                return FinSucc((B b, C c) => f(a.Value, b, c));
            }
        });
            
    public static Eff<RT, E> Apply<RT, A, B, C, D, E>(this Func<A, B, C, D, E> f, Eff<RT, A> ma, Eff<RT, B> mb, Eff<RT, C> mc, Eff<RT, D> md) where RT : struct  =>
        EffMaybe<RT, E>(rt =>
        {
            var (a, b, c, d) = (ma.Run(rt), mb.Run(rt), mc.Run(rt), md.Run(rt));
            
            if (a.IsFail || b.IsFail || c.IsFail || d.IsFail)
            {
                var errs = Errors.None;
                if(a.IsFail) errs += a.Error;
                if(b.IsFail) errs += b.Error;
                if(c.IsFail) errs += c.Error;
                if(d.IsFail) errs += d.Error;
                return errs;
            }
            else
            {
                return f(a.Value, b.Value, c.Value, d.Value);
            }
        });
             
    public static Eff<RT, Func<D, E>> Apply<RT, A, B, C, D, E>(this Func<A, B, C, D, E> f, Eff<RT, A> ma, Eff<RT, B> mb, Eff<RT, C> mc) where RT : struct  =>
        EffMaybe<RT, Func<D, E>>(rt =>
        {
            var (a, b, c) = (ma.Run(rt), mb.Run(rt), mc.Run(rt));
            
            if (a.IsFail || b.IsFail || c.IsFail)
            {
                var errs = Errors.None;
                if(a.IsFail) errs += a.Error;
                if(b.IsFail) errs += b.Error;
                if(c.IsFail) errs += c.Error;
                return errs;
            }
            else
            {
                return FinSucc((D d) => f(a.Value, b.Value, c.Value, d));
            }
        });
             
    public static Eff<RT, Func<C, D, E>> Apply<RT, A, B, C, D, E>(this Func<A, B, C, D, E> f, Eff<RT, A> ma, Eff<RT, B> mb) where RT : struct  =>
        EffMaybe<RT, Func<C, D, E>>(rt =>
        {
            var (a, b) = (ma.Run(rt), mb.Run(rt));

            if (a.IsFail || b.IsFail)
            {
                var errs = Errors.None;
                if(a.IsFail) errs += a.Error;
                if(b.IsFail) errs += b.Error;
                return errs;
            }
            else
            {
                return FinSucc((C c, D d) => f(a.Value, b.Value, c, d));
            }
        });
             
    public static Eff<RT, Func<B, C, D, E>> Apply<RT, A, B, C, D, E>(this Func<A, B, C, D, E> f, Eff<RT, A> ma) where RT : struct  =>
        EffMaybe<RT, Func<B, C, D, E>>(rt =>
        {
            var a = ma.Run(rt);

            if (a.IsFail)
            {
                return a.Error;
            }
            else
            {
                return FinSucc((B b, C c, D d) => f(a.Value, b, c, d));
            }
        });
}
