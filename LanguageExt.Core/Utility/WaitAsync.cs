using System;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt
{
    internal static class WaitAsync
    {
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
