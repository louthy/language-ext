using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static class WaitAsync
    {
        public static async ValueTask<(A A, B B)> All<A, B>(ValueTask<A> va, ValueTask<B> vb)
        {
            var ta = va.AsTask();
            var tb = vb.AsTask();
            await Task.WhenAll(ta, tb).ConfigureAwait(false);
            return (ta.Result, tb.Result);
        }
        
        public static async ValueTask<(A A, B B, C C)> All<A, B, C>(ValueTask<A> va, ValueTask<B> vb, ValueTask<C> vc)
        {
            var ta = va.AsTask();
            var tb = vb.AsTask();
            var tc = vc.AsTask();
            await Task.WhenAll(ta, tb, tc).ConfigureAwait(false);
            return (ta.Result, tb.Result, tc.Result);
        }
        
        public static async ValueTask<(A A, B B, C C, D D)> All<A, B, C, D>(ValueTask<A> va, ValueTask<B> vb, ValueTask<C> vc, ValueTask<D> vd)
        {
            var ta = va.AsTask();
            var tb = vb.AsTask();
            var tc = vc.AsTask();
            var td = vd.AsTask();
            await Task.WhenAll(ta, tb, tc, td).ConfigureAwait(false);
            return (ta.Result, tb.Result, tc.Result, td.Result);
        }
        
        public static async ValueTask<(A A, B B, C C, D D, E E)> All<A, B, C, D, E>(ValueTask<A> va, ValueTask<B> vb, ValueTask<C> vc, ValueTask<D> vd, ValueTask<E> ve)
        {
            var ta = va.AsTask();
            var tb = vb.AsTask();
            var tc = vc.AsTask();
            var td = vd.AsTask();
            var te = ve.AsTask();
            await Task.WhenAll(ta, tb, tc, td, te).ConfigureAwait(false);
            return (ta.Result, tb.Result, tc.Result, td.Result, te.Result);
        }
        
        public static async ValueTask<(A A, B B, C C, D D, E E, F F)> All<A, B, C, D, E, F>(ValueTask<A> va, ValueTask<B> vb, ValueTask<C> vc, ValueTask<D> vd, ValueTask<E> ve, ValueTask<F> vf)
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
        
        public static async Task<(A A, B B)> All<A, B>(Task<A> ta, Task<B> tb)
        {
            await Task.WhenAll(ta, tb).ConfigureAwait(false);
            return (ta.Result, tb.Result);
        }
        
        public static async Task<(A A, B B, C C)> All<A, B, C>(Task<A> ta, Task<B> tb, Task<C> tc)
        {
            await Task.WhenAll(ta, tb, tc).ConfigureAwait(false);
            return (ta.Result, tb.Result, tc.Result);
        }
        
        public static async Task<(A A, B B, C C, D D)> All<A, B, C, D>(Task<A> ta, Task<B> tb, Task<C> tc, Task<D> td)
        {
            await Task.WhenAll(ta, tb, tc, td).ConfigureAwait(false);
            return (ta.Result, tb.Result, tc.Result, td.Result);
        }
        
        public static async Task<(A A, B B, C C, D D, E E)> All<A, B, C, D, E>(Task<A> ta, Task<B> tb, Task<C> tc, Task<D> td, Task<E> te)
        {
            await Task.WhenAll(ta, tb, tc, td, te).ConfigureAwait(false);
            return (ta.Result, tb.Result, tc.Result, td.Result, te.Result);
        }
        
        public static async Task<(A A, B B, C C, D D, E E, F F)> All<A, B, C, D, E, F>(Task<A> ta, Task<B> tb, Task<C> tc, Task<D> td, Task<E> te, Task<F> tf)
        {
            await Task.WhenAll(ta, tb, tc, td, te, tf).ConfigureAwait(false);
            return (ta.Result, tb.Result, tc.Result, td.Result, te.Result, tf.Result);
        }


        public static async ValueTask<A[]> All<A>(params ValueTask<A>[] vts)
        {
            var ts = vts.Map(static t => t.AsTask()).ToArray();
            await Task.WhenAll(ts).ConfigureAwait(false);
            return ts.Map(t => t.Result).ToArray();
        }

        public static async ValueTask<Seq<A>> All<A>(Seq<ValueTask<A>> vts)
        {
            var ts = vts.Map(static t => t.AsTask());
            await Task.WhenAll(ts).ConfigureAwait(false);
            return ts.Map(t => t.Result).ToSeq();
        }

        public static ValueTask<IEnumerable<A>> All<A>(IEnumerable<ValueTask<A>> vts) =>
            All(vts.ToArray()).Map(static ts => (IEnumerable<A>)ts);

        public static async Task<A[]> All<A>(params Task<A>[] ts)
        {
            await Task.WhenAll(ts).ConfigureAwait(false);
            return ts.Map(t => t.Result).ToArray();
        }

        public static async Task<Seq<A>> All<A>(Seq<Task<A>> ts)
        {
            await Task.WhenAll(ts).ConfigureAwait(false);
            return ts.Map(t => t.Result).ToSeq();
        }

        public static Task<IEnumerable<A>> All<A>(IEnumerable<Task<A>> vts) =>
            All(vts.ToArray()).Map(static ts => (IEnumerable<A>)ts);        
        
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
