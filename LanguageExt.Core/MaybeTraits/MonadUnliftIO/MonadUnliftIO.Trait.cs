using System;
using LanguageExt.Common;

namespace LanguageExt.Traits;

public static partial class Maybe
{
    /// <summary>
    /// Monad that is either the IO monad or a transformer with the IO monad in its stack
    /// </summary>
    /// <typeparam name="M">Self-referring trait</typeparam>
    public interface MonadUnliftIO<M> : Maybe.MonadIO<M>
        where M : MonadUnliftIO<M>, Traits.Monad<M>
    {
        /// <summary>
        /// Extract the IO monad from within the M monad (usually as part of a monad-transformer stack).
        /// </summary>
        /// <remarks>
        /// IMPLEMENTATION REQUIRED: If this method isn't overloaded in this monad
        /// or any monad in the stack on the way to the inner-monad, then it will throw
        /// an exception.
        ///
        /// This isn't ideal, it appears to be the only way to achieve this
        /// kind of functionality in C# without resorting to magic. 
        /// </remarks>
        /// <exception cref="ExceptionalException">If this method isn't overloaded in
        /// the inner monad or any monad in the stack on the way to the inner-monad,
        /// then it will throw an exception.</exception>
        public static virtual K<M, IO<A>> ToIO<A>(K<M, A> ma) =>
            throw new ExceptionalException(Errors.ToIONotSupported);

        /// <summary>
        /// Extract the IO monad from within the `M` monad (usually as part of a monad-transformer stack).  Then perform
        /// a mapping operation on the IO action before lifting the IO back into the `M` monad.
        /// </summary>
        public static virtual K<M, B> MapIO<A, B>(K<M, A> ma, Func<IO<A>, IO<B>> f) =>
            M.ToIO(ma).Bind(io => M.LiftIO(f(io)));
        
        /// <summary>
        /// Queue this IO operation to run on the thread-pool. 
        /// </summary>
        /// <param name="timeout">Maximum time that the forked IO operation can run for. `None` for no timeout.</param>
        /// <returns>Returns a `ForkIO` data-structure that contains two IO effects that can be used to either cancel
        /// the forked IO operation or to await the result of it.
        /// </returns>
        public static virtual K<M, ForkIO<A>> ForkIO<A>(K<M, A> ma, Option<TimeSpan> timeout = default) =>
            M.MapIO(ma, io => io.Fork(timeout));
    }
}
