using System;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes
{
    internal static class PipesInternal
    {
        public static Producer<RT, B, R> Use<H, RT, B, R>(Aff<RT, H> acq, Func<H, Producer<RT, B, R>> f, Func<H, Unit> dispose)
            where RT : struct, HasCancel<RT>
            where H : IDisposable =>
            Use(acq, h => f(h).ToProxy(), dispose).ToProducer(); 
 
        public static Producer<RT, B, R> Use<H, RT, B, R>(Aff<H> acq, Func<H, Producer<RT, B, R>> f, Func<H, Unit> dispose)
            where RT : struct, HasCancel<RT> =>
            Use(acq, h => f(h).ToProxy(), dispose).ToProducer();
        
        public static Consumer<RT, A, R> Use<H, RT, A, R>(Aff<RT, H> acq, Func<H, Consumer<RT, A, R>> f, Func<H, Unit> dispose)
            where RT : struct, HasCancel<RT>
            where H : IDisposable =>
            Use(acq, h => f(h).ToProxy(), dispose).ToConsumer(); 
 
        public static Consumer<RT, A, R> Use<H, RT, A, R>(Aff<H> acq, Func<H, Consumer<RT, A, R>> f, Func<H, Unit> dispose)
            where RT : struct, HasCancel<RT> =>
            Use(acq, h => f(h).ToProxy(), dispose).ToConsumer();
        
        public static Pipe<RT, A, B, R> Use<H, RT, A, B, R>(Aff<RT, H> acq, Func<H, Pipe<RT, A, B, R>> f, Func<H, Unit> dispose)
            where RT : struct, HasCancel<RT>
            where H : IDisposable =>
            Use(acq, h => f(h).ToProxy(), dispose).ToPipe(); 
 
        public static Pipe<RT, A, B, R> Use<H, RT, A, B, R>(Aff<H> acq, Func<H, Pipe<RT, A, B, R>> f, Func<H, Unit> dispose)
            where RT : struct, HasCancel<RT> =>
            Use(acq, h => f(h).ToProxy(), dispose).ToPipe();

        public static Proxy<RT, A1, A, B1, B, R> Use<H, RT, A1, A, B1, B, R>(Aff<H> acq, Func<H, Proxy<RT, A1, A, B1, B, R>> f, Func<H, Unit> dispose)
            where RT : struct, HasCancel<RT> =>
            new M<RT, A1, A, B1, B, R>(AffMaybe<RT, Proxy<RT, A1, A, B1, B, R>>(
                                           async env =>
                                           {
                                               // Acquire the resource
                                               var maybeResource = await acq.Run().ConfigureAwait(false);
                                               if (maybeResource.IsFail) return maybeResource.Cast<Proxy<RT, A1, A, B1, B, R>>();
                                               var resource = (H)maybeResource;

                                               return Fin<Proxy<RT, A1, A, B1, B, R>>.Succ(f(resource).Map(a =>
                                                                                                           {
                                                                                                               dispose?.Invoke(resource);
                                                                                                               return a;
                                                                                                           }));
                                           }));

        public static Proxy<RT, A1, A, B1, B, R> Use<H, RT, A1, A, B1, B, R>(Aff<RT, H> acq, Func<H, Proxy<RT, A1, A, B1, B, R>> f, Func<H, Unit> dispose)
            where RT : struct, HasCancel<RT> =>
            new M<RT, A1, A, B1, B, R>(AffMaybe<RT, Proxy<RT, A1, A, B1, B, R>>(
                                           async env =>
                                           {

                                               // Acquire the resource
                                               var maybeResource = await acq.Run(env).ConfigureAwait(false);
                                               if (maybeResource.IsFail) return maybeResource.Cast<Proxy<RT, A1, A, B1, B, R>>();
                                               var resource = (H)maybeResource;

                                               return Fin<Proxy<RT, A1, A, B1, B, R>>.Succ(f(resource).Map(a =>
                                                                                                           {
                                                                                                               dispose?.Invoke(resource);
                                                                                                               return a;
                                                                                                           }));
                                           }));

        public static Unit Dispose<H>(H x) where H : IDisposable
        {
            x?.Dispose();
            return unit;
        }
    }
}
