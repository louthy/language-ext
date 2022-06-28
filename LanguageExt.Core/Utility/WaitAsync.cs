using System;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt
{
    internal static class WaitAsync
    {
        public static async ValueTask<(A A, B B)> WaitAll<A, B>(ValueTask<A> va, ValueTask<B> vb)
        {
            var ta = va.AsTask();
            var tb = vb.AsTask();
            await Task.WhenAll(ta, tb).ConfigureAwait(false);
            return (ta.Result, tb.Result);
        }
        
        public static async ValueTask<(A A, B B, C C)> WaitAll<A, B, C>(ValueTask<A> va, ValueTask<B> vb, ValueTask<C> vc)
        {
            var ta = va.AsTask();
            var tb = vb.AsTask();
            var tc = vc.AsTask();
            await Task.WhenAll(ta, tb, tc).ConfigureAwait(false);
            return (ta.Result, tb.Result, tc.Result);
        }
        
        public static async ValueTask<(A A, B B, C C, D D)> WaitAll<A, B, C, D>(ValueTask<A> va, ValueTask<B> vb, ValueTask<C> vc, ValueTask<D> vd)
        {
            var ta = va.AsTask();
            var tb = vb.AsTask();
            var tc = vc.AsTask();
            var td = vd.AsTask();
            await Task.WhenAll(ta, tb, tc, td).ConfigureAwait(false);
            return (ta.Result, tb.Result, tc.Result, td.Result);
        }
        
        public static async ValueTask<(A A, B B, C C, D D, E E)> WaitAll<A, B, C, D, E>(ValueTask<A> va, ValueTask<B> vb, ValueTask<C> vc, ValueTask<D> vd, ValueTask<E> ve)
        {
            var ta = va.AsTask();
            var tb = vb.AsTask();
            var tc = vc.AsTask();
            var td = vd.AsTask();
            var te = ve.AsTask();
            await Task.WhenAll(ta, tb, tc, td, te).ConfigureAwait(false);
            return (ta.Result, tb.Result, tc.Result, td.Result, te.Result);
        }
        
        public static async ValueTask<(A A, B B, C C, D D, E E, F F)> WaitAll<A, B, C, D, E, F>(ValueTask<A> va, ValueTask<B> vb, ValueTask<C> vc, ValueTask<D> vd, ValueTask<E> ve, ValueTask<F> vf)
        {
            var ta = va.AsTask();
            var tb = vb.AsTask();
            var tc = vc.AsTask();
            var td = vd.AsTask();
            var te = ve.AsTask();
            var tf = vf.AsTask();
            await Task.WhenAll(ta, tb, tc, td, te, tf).ConfigureAwait(false);
            return (ta.Result, tb.Result, tc.Result, td.Result, te.Result, tf.Result);
        }
        
        public static async Task<bool> WaitOneAsync(this WaitHandle handle, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            RegisteredWaitHandle registeredHandle  = null;
            var tokenRegistration = default(CancellationTokenRegistration);
            try
            {
                var tcs = new TaskCompletionSource<bool>();
                
                registeredHandle = ThreadPool.RegisterWaitForSingleObject(
                    handle,
                    static (state, timedOut) => ((TaskCompletionSource<bool>)state).TrySetResult(!timedOut),
                    tcs,
                    millisecondsTimeout,
                    true);
                
                tokenRegistration = cancellationToken.Register(
                    static state => ((TaskCompletionSource<bool>)state).TrySetCanceled(),
                    tcs);
                return await tcs.Task.ConfigureAwait(false);
            }
            finally
            {
                registeredHandle?.Unregister(null);
                tokenRegistration.Dispose();
            }
        }

        public static Task<bool> WaitOneAsync(this WaitHandle handle, TimeSpan timeout, CancellationToken cancellationToken) =>
            handle.WaitOneAsync((int)timeout.TotalMilliseconds, cancellationToken);

        public static Task<bool> WaitOneAsync(this WaitHandle handle, CancellationToken cancellationToken) =>
            handle.WaitOneAsync(Timeout.Infinite, cancellationToken);
    }
}
