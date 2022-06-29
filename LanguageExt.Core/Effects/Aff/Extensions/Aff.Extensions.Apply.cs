using System;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class AffExtensions
{
    // ------------ Aff<A> ----------------------------------------------------------------------------------
    
    public static Aff<B> Apply<A, B>(this Aff<Func<A, B>> mf, Aff<A> ma) =>
        AffMaybe<B>(async () =>
        {
            var (f, a) = await WaitAsync.WaitAll(mf.Run(), ma.Run()).ConfigureAwait(false);

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
        
    public static Aff<C> Apply<A, B, C>(this Aff<Func<A, B, C>> mf, Aff<A> ma, Aff<B> mb) =>
        AffMaybe<C>(async () =>
        {
            var (f, a, b) = await WaitAsync.WaitAll(mf.Run(), ma.Run(), mb.Run()).ConfigureAwait(false);

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
        
    public static Aff<Func<B, C>> Apply<A, B, C>(this Aff<Func<A, B, C>> mf, Aff<A> ma) =>
        AffMaybe(async () =>
        {
            var (f, a) = await WaitAsync.WaitAll(mf.Run(), ma.Run()).ConfigureAwait(false);

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

        
    public static Aff<D> Apply<A, B, C, D>(this Aff<Func<A, B, C, D>> mf, Aff<A> ma, Aff<B> mb, Aff<C> mc) =>
        AffMaybe<D>(async () =>
        {
            var (f, a, b, c) = await WaitAsync.WaitAll(mf.Run(), ma.Run(), mb.Run(), mc.Run()).ConfigureAwait(false);

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
        
    public static Aff<Func<C, D>> Apply<A, B, C, D>(this Aff<Func<A, B, C, D>> mf, Aff<A> ma, Aff<B> mb) =>
        AffMaybe(async () =>
        {
            var (f, a, b) = await WaitAsync.WaitAll(mf.Run(), ma.Run(), mb.Run()).ConfigureAwait(false);

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
        
    public static Aff<Func<B, C, D>> Apply<A, B, C, D>(this Aff<Func<A, B, C, D>> mf, Aff<A> ma) =>
        AffMaybe(async () =>
        {
            var (f, a) = await WaitAsync.WaitAll(mf.Run(), ma.Run()).ConfigureAwait(false);

            if (f.IsFail || a.IsFail)
            {
                var errs = Seq<Error>();
                if(f.IsFail) errs = errs.Add(f.Error);
                if(a.IsFail) errs = errs.Add(a.Error);
                return Error.Many(errs);
            }
            else
            {
                return FinSucc((B b, C c) => f.Value(a.Value, b, c));
            }
        });
            
    public static Aff<E> Apply<A, B, C, D, E>(this Aff<Func<A, B, C, D, E>> mf, Aff<A> ma, Aff<B> mb, Aff<C> mc, Aff<D> md) =>
        AffMaybe<E>(async () =>
        {
            var (f, a, b, c, d) = await WaitAsync.WaitAll(mf.Run(), ma.Run(), mb.Run(), mc.Run(), md.Run()).ConfigureAwait(false);

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
             
    public static Aff<Func<D, E>> Apply<A, B, C, D, E>(this Aff<Func<A, B, C, D, E>> mf, Aff<A> ma, Aff<B> mb, Aff<C> mc) =>
        AffMaybe(async () =>
        {
            var (f, a, b, c) = await WaitAsync.WaitAll(mf.Run(), ma.Run(), mb.Run(), mc.Run()).ConfigureAwait(false);

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
             
    public static Aff<Func<C, D, E>> Apply<A, B, C, D, E>(this Aff<Func<A, B, C, D, E>> mf, Aff<A> ma, Aff<B> mb) =>
        AffMaybe(async () =>
        {
            var (f, a, b) = await WaitAsync.WaitAll(mf.Run(), ma.Run(), mb.Run()).ConfigureAwait(false);

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
             
    public static Aff<Func<B, C, D, E>> Apply<A, B, C, D, E>(this Aff<Func<A, B, C, D, E>> mf, Aff<A> ma) =>
        AffMaybe(async () =>
        {
            var (f, a) = await WaitAsync.WaitAll(mf.Run(), ma.Run()).ConfigureAwait(false);

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

    
    // ------------ Aff<RT, A> -----------------------------------------------------------------------------------------
    
    public static Aff<RT, B> Apply<RT, A, B>(this Aff<RT, Func<A, B>> mf, Aff<RT, A> ma) where RT : struct, HasCancel<RT> =>
        AffMaybe<RT, B>(async rt =>
        {
            var (f, a) = await WaitAsync.WaitAll(mf.Run(rt), ma.Run(rt)).ConfigureAwait(false);

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
        
    public static Aff<RT, C> Apply<RT, A, B, C>(this Aff<RT, Func<A, B, C>> mf, Aff<RT, A> ma, Aff<RT, B> mb) where RT : struct, HasCancel<RT> =>
        AffMaybe<RT, C>(async rt =>
        {
            var (f, a, b) = await WaitAsync.WaitAll(mf.Run(rt), ma.Run(rt), mb.Run(rt)).ConfigureAwait(false);

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
        
    public static Aff<RT, Func<B, C>> Apply<RT, A, B, C>(this Aff<RT, Func<A, B, C>> mf, Aff<RT, A> ma) where RT : struct, HasCancel<RT> =>
        AffMaybe<RT, Func<B, C>>(async rt =>
        {
            var (f, a) = await WaitAsync.WaitAll(mf.Run(rt), ma.Run(rt)).ConfigureAwait(false);

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
        
    public static Aff<RT, D> Apply<RT, A, B, C, D>(this Aff<RT, Func<A, B, C, D>> mf, Aff<RT, A> ma, Aff<RT, B> mb, Aff<RT, C> mc) where RT : struct, HasCancel<RT> =>
        AffMaybe<RT, D>(async rt =>
        {
            var (f, a, b, c) = await WaitAsync.WaitAll(mf.Run(rt), ma.Run(rt), mb.Run(rt), mc.Run(rt)).ConfigureAwait(false);

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
        
    public static Aff<RT, Func<C, D>> Apply<RT, A, B, C, D>(this Aff<RT, Func<A, B, C, D>> mf, Aff<RT, A> ma, Aff<RT, B> mb) where RT : struct, HasCancel<RT> =>
        AffMaybe<RT, Func<C, D>>(async rt =>
        {
            var (f, a, b) = await WaitAsync.WaitAll(mf.Run(rt), ma.Run(rt), mb.Run(rt)).ConfigureAwait(false);

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
        
    public static Aff<RT, Func<B, C, D>> Apply<RT, A, B, C, D>(this Aff<RT, Func<A, B, C, D>> mf, Aff<RT, A> ma) where RT : struct, HasCancel<RT> =>
        AffMaybe<RT, Func<B, C, D>>(async rt =>
        {
            var (f, a) = await WaitAsync.WaitAll(mf.Run(rt), ma.Run(rt)).ConfigureAwait(false);

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
            
    public static Aff<RT, E> Apply<RT, A, B, C, D, E>(this Aff<RT, Func<A, B, C, D, E>> mf, Aff<RT, A> ma, Aff<RT, B> mb, Aff<RT, C> mc, Aff<RT, D> md) where RT : struct, HasCancel<RT> =>
        AffMaybe<RT, E>(async rt =>
        {
            var (f, a, b, c, d) = await WaitAsync.WaitAll(mf.Run(rt), ma.Run(rt), mb.Run(rt), mc.Run(rt), md.Run(rt)).ConfigureAwait(false);

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
             
    public static Aff<RT, Func<D, E>> Apply<RT, A, B, C, D, E>(this Aff<RT, Func<A, B, C, D, E>> mf, Aff<RT, A> ma, Aff<RT, B> mb, Aff<RT, C> mc) where RT : struct, HasCancel<RT> =>
        AffMaybe<RT, Func<D, E>>(async rt =>
        {
            var (f, a, b, c) = await WaitAsync.WaitAll(mf.Run(rt), ma.Run(rt), mb.Run(rt), mc.Run(rt)).ConfigureAwait(false);

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
             
    public static Aff<RT, Func<C, D, E>> Apply<RT, A, B, C, D, E>(this Aff<RT, Func<A, B, C, D, E>> mf, Aff<RT, A> ma, Aff<RT, B> mb) where RT : struct, HasCancel<RT> =>
        AffMaybe<RT, Func<C, D, E>>(async rt =>
        {
            var (f, a, b) = await WaitAsync.WaitAll(mf.Run(rt), ma.Run(rt), mb.Run(rt)).ConfigureAwait(false);

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
             
    public static Aff<RT, Func<B, C, D, E>> Apply<RT, A, B, C, D, E>(this Aff<RT, Func<A, B, C, D, E>> mf, Aff<RT, A> ma) where RT : struct, HasCancel<RT> =>
        AffMaybe<RT, Func<B, C, D, E>>(async rt =>
        {
            var (f, a) = await WaitAsync.WaitAll(mf.Run(rt), ma.Run(rt)).ConfigureAwait(false);

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
    
    // ------------ Non Aff functions ----------------------------------------------------------------------------------
    
    public static Aff<RT, B> Apply<RT, A, B>(this Func<A, B> f, Aff<RT, A> ma) where RT : struct, HasCancel<RT> =>
        AffMaybe<RT, B>(async rt =>
        {
            var a = await ma.Run(rt).ConfigureAwait(false);

            if (a.IsFail)
            {
                return a.Error;
            }
            else
            {
                return f(a.Value);
            }
        });
        
    public static Aff<RT, C> Apply<RT, A, B, C>(this Func<A, B, C> f, Aff<RT, A> ma, Aff<RT, B> mb) where RT : struct, HasCancel<RT> =>
        AffMaybe<RT, C>(async rt =>
        {
            var (a, b) = await WaitAsync.WaitAll(ma.Run(rt), mb.Run(rt)).ConfigureAwait(false);

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
        
    public static Aff<RT, Func<B, C>> Apply<RT, A, B, C>(this Func<A, B, C> f, Aff<RT, A> ma) where RT : struct, HasCancel<RT> =>
        AffMaybe<RT, Func<B, C>>(async rt =>
        {
            var a = await ma.Run(rt).ConfigureAwait(false);

            if (a.IsFail)
            {
                return a.Error;
            }
            else
            {
                return FinSucc((B b) => f(a.Value, b));
            }
        });
        
    public static Aff<RT, D> Apply<RT, A, B, C, D>(this Func<A, B, C, D> f, Aff<RT, A> ma, Aff<RT, B> mb, Aff<RT, C> mc) where RT : struct, HasCancel<RT> =>
        AffMaybe<RT, D>(async rt =>
        {
            var (a, b, c) = await WaitAsync.WaitAll(ma.Run(rt), mb.Run(rt), mc.Run(rt)).ConfigureAwait(false);

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
        
    public static Aff<RT, Func<C, D>> Apply<RT, A, B, C, D>(this Func<A, B, C, D> f, Aff<RT, A> ma, Aff<RT, B> mb) where RT : struct, HasCancel<RT> =>
        AffMaybe<RT, Func<C, D>>(async rt =>
        {
            var (a, b) = await WaitAsync.WaitAll(ma.Run(rt), mb.Run(rt)).ConfigureAwait(false);
            
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
        
    public static Aff<RT, Func<B, C, D>> Apply<RT, A, B, C, D>(this Func<A, B, C, D> f, Aff<RT, A> ma) where RT : struct, HasCancel<RT> =>
        AffMaybe<RT, Func<B, C, D>>(async rt =>
        {
            var a = await ma.Run(rt).ConfigureAwait(false);

            if (a.IsFail)
            {
                return a.Error;
            }
            else
            {
                return FinSucc((B b, C c) => f(a.Value, b, c));
            }
        });
            
    public static Aff<RT, E> Apply<RT, A, B, C, D, E>(this Func<A, B, C, D, E> f, Aff<RT, A> ma, Aff<RT, B> mb, Aff<RT, C> mc, Aff<RT, D> md) where RT : struct, HasCancel<RT> =>
        AffMaybe<RT, E>(async rt =>
        {
            var (a, b, c, d) = await WaitAsync.WaitAll(ma.Run(rt), mb.Run(rt), mc.Run(rt), md.Run(rt)).ConfigureAwait(false);
            
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
             
    public static Aff<RT, Func<D, E>> Apply<RT, A, B, C, D, E>(this Func<A, B, C, D, E> f, Aff<RT, A> ma, Aff<RT, B> mb, Aff<RT, C> mc) where RT : struct, HasCancel<RT> =>
        AffMaybe<RT, Func<D, E>>(async rt =>
        {
            var (a, b, c) = await WaitAsync.WaitAll(ma.Run(rt), mb.Run(rt), mc.Run(rt)).ConfigureAwait(false);
            
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
             
    public static Aff<RT, Func<C, D, E>> Apply<RT, A, B, C, D, E>(this Func<A, B, C, D, E> f, Aff<RT, A> ma, Aff<RT, B> mb) where RT : struct, HasCancel<RT> =>
        AffMaybe<RT, Func<C, D, E>>(async rt =>
        {
            var (a, b) = await WaitAsync.WaitAll(ma.Run(rt), mb.Run(rt)).ConfigureAwait(false);

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
             
    public static Aff<RT, Func<B, C, D, E>> Apply<RT, A, B, C, D, E>(this Func<A, B, C, D, E> f, Aff<RT, A> ma) where RT : struct, HasCancel<RT> =>
        AffMaybe<RT, Func<B, C, D, E>>(async rt =>
        {
            var a = await ma.Run(rt).ConfigureAwait(false);

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
