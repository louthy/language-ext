using System;
using LanguageExt.Interfaces;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes
{
    internal static class PipesInternal
    {
        public static Producer<Env, B, R> Use<H, Env, B, R>(Aff<Env, H> acq, Func<H, Producer<Env, B, R>> f, Func<H, Unit> dispose)
            where Env : struct, HasCancel<Env>
            where H : IDisposable =>
            Use(acq, h => f(h).ToProxy(), dispose).ToProducer(); 
 
        public static Producer<Env, B, R> Use<H, Env, B, R>(Aff<H> acq, Func<H, Producer<Env, B, R>> f, Func<H, Unit> dispose)
            where Env : struct, HasCancel<Env> =>
            Use(acq, h => f(h).ToProxy(), dispose).ToProducer();
        
        public static Consumer<Env, A, R> Use<H, Env, A, R>(Aff<Env, H> acq, Func<H, Consumer<Env, A, R>> f, Func<H, Unit> dispose)
            where Env : struct, HasCancel<Env>
            where H : IDisposable =>
            Use(acq, h => f(h).ToProxy(), dispose).ToConsumer(); 
 
        public static Consumer<Env, A, R> Use<H, Env, A, R>(Aff<H> acq, Func<H, Consumer<Env, A, R>> f, Func<H, Unit> dispose)
            where Env : struct, HasCancel<Env> =>
            Use(acq, h => f(h).ToProxy(), dispose).ToConsumer();
        
        public static Pipe<Env, A, B, R> Use<H, Env, A, B, R>(Aff<Env, H> acq, Func<H, Pipe<Env, A, B, R>> f, Func<H, Unit> dispose)
            where Env : struct, HasCancel<Env>
            where H : IDisposable =>
            Use(acq, h => f(h).ToProxy(), dispose).ToPipe(); 
 
        public static Pipe<Env, A, B, R> Use<H, Env, A, B, R>(Aff<H> acq, Func<H, Pipe<Env, A, B, R>> f, Func<H, Unit> dispose)
            where Env : struct, HasCancel<Env> =>
            Use(acq, h => f(h).ToProxy(), dispose).ToPipe(); 
 
        public static Proxy<Env, A1, A, B1, B, R> Use<H, Env, A1, A, B1, B, R>(Aff<H> acq, Func<H, Proxy<Env, A1, A, B1, B, R>> f, Func<H, Unit> dispose)
            where Env : struct, HasCancel<Env>
        {
            return new M<Env, A1, A, B1, B, R>(AffMaybe<Env, Proxy<Env, A1, A, B1, B, R>>(async env => {

                // Acquire the resource
                var maybeResource = await acq.RunIO().ConfigureAwait(false);
                if (maybeResource.IsFail) return maybeResource.Cast<Proxy<Env, A1, A, B1, B, R>>();
                var resource = (H) maybeResource;
                
                return Fin<Proxy<Env, A1, A, B1, B, R>>.Succ(f(resource).Map(a => {
                    dispose?.Invoke(resource);
                    return a;
                }));
            }));
        }        
 
        public static Proxy<Env, A1, A, B1, B, R> Use<H, Env, A1, A, B1, B, R>(Aff<Env, H> acq, Func<H, Proxy<Env, A1, A, B1, B, R>> f, Func<H, Unit> dispose)
            where Env : struct, HasCancel<Env>
        {
            return new M<Env, A1, A, B1, B, R>(AffMaybe<Env, Proxy<Env, A1, A, B1, B, R>>(async env => {

                // Acquire the resource
                var maybeResource = await acq.RunIO(env).ConfigureAwait(false);
                if (maybeResource.IsFail) return maybeResource.Cast<Proxy<Env, A1, A, B1, B, R>>();
                var resource = (H) maybeResource;
                
                return Fin<Proxy<Env, A1, A, B1, B, R>>.Succ(f(resource).Map(a => {
                    dispose?.Invoke(resource);
                    return a;
                }));
            }));
        }

        public static Unit Dispose<H>(H x) where H : IDisposable
        {
            x?.Dispose();
            return unit;
        }
    }
}
