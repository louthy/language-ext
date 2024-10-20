using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// A value of type `IO` is a computation which, when performed, does some I/O before returning
/// a value of type `A`.
///
/// There is really only one way you should _"perform"_ an I/O action: bind it to `Main` in your
/// program:  When your program is run, the I/O will be performed. It shouldn't be possible to
/// perform I/O from an arbitrary function, unless that function is itself in the `IO` monad and
/// called at some point, directly or indirectly, from `Main`.
///
/// Obviously, as this is C#, the above restrictions are for you to enforce. It would be reasonable
/// to relax that approach and have I/O invoked from, say, web-request handlers - or any other 'edges'
/// of your application.
/// 
/// `IO` is a monad, so `IO` actions can be combined using either the LINQ-notation or the `bind` 
/// operations from the `Monad` class.
/// </summary>
/// <param name="runIO">The lifted thunk that is the IO operation</param>
/// <typeparam name="A">Bound value</typeparam>
record IOPure<A>(A Value) : IO<A>
{
    public IO<A> ToSync() =>
        new IOSync<A>(_ => IOResponse.Complete(Value));
    
    public IO<A> ToAsync() =>
        new IOAsync<A>(_ => Task.FromResult(IOResponse.Complete(Value)));

    public override IO<B> Map<B>(Func<A, B> f) =>
        new IOSync<B>(_ => IOResponse.Complete(f(Value)));

    public override IO<S> FoldUntil<S>(
        Schedule schedule, 
        S initialState, 
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        ToAsync().FoldUntil(schedule, initialState, folder, predicate);

    public override IO<A> Post() => 
        this;

    public override IO<B> Bind<B>(Func<A, IO<B>> f) =>
        f(Value);

    public override IO<A> Bracket() => 
        this;

    public override IO<C> Bracket<B, C>(Func<A, IO<C>> Use, Func<Error, IO<C>> Catch, Func<A, IO<B>> Finally) => 
        ToSync().Bracket(Use, Catch, Finally);

    public override IO<A> Local() => 
        this;

    public override IO<ForkIO<A>> Fork(Option<TimeSpan> timeout = default) => 
        new IOPure<ForkIO<A>>(new ForkIO<A>(Prelude.unitIO, this));

    public override ValueTask<A> RunAsync(EnvIO? envIO = null) => 
        ValueTask.FromResult(Value);

    public override A Run(EnvIO? envIO = null) => 
        Value;

    public override IO<A> RepeatUntil(Func<A, bool> predicate) =>
        predicate(Value)
            ? this
            : throw new InvalidOperationException("non-terminating and non-effectful IO repeat");

    public override IO<A> RepeatUntil(Schedule schedule, Func<A, bool> predicate) => 
        predicate(Value)
            ? this
            : throw new InvalidOperationException("non-terminating and non-effectful IO repeat");

    public override IO<A> RetryUntil(Func<Error, bool> predicate) => 
        this;

    public override IO<A> RetryUntil(Schedule schedule, Func<Error, bool> predicate) => 
        this;

    /// <summary>
    /// Catches any error thrown by invoking this IO computation, passes it through a predicate,
    /// and if that returns true, returns the result of invoking the Fail function, otherwise
    /// this is returned.
    /// </summary>
    /// <param name="Predicate">Predicate</param>
    /// <param name="Fail">Fail functions</param>
    public override IO<A> Catch(Func<Error, bool> Predicate, Func<Error, K<IO, A>> Fail) =>
        this;
}
