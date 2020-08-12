using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Thunks;

namespace LanguageExt
{
    /// <summary>
    /// Method builder for the Aff<A> monad
    /// Works with AffAwaiter to allow Aff<A> to be used with async/await 
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct AffMethodBuilder<A>
    {
        IAsyncStateMachine stateMachine;
        
        public static AffMethodBuilder<A> Create() =>
            new AffMethodBuilder<A>();

        public void Start<TStateMachine>(ref TStateMachine machine)
            where TStateMachine : IAsyncStateMachine =>
            machine.MoveNext();

        public void SetStateMachine(IAsyncStateMachine machine) =>
            this.stateMachine = machine;

        public void SetException(Exception ex) =>
            Task = new AffPure<A>(ThunkAsync<A>.Fail(
                ex is AffException ioex
                    ? ioex.Error
                    : (Error)ex));

        public void SetResult(A result) =>
            Task = new AffPure<A>(ThunkAsync<A>.Success(result));

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine machine)
            where TAwaiter : INotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine machine)
            where TAwaiter : ICriticalNotifyCompletion
            where TStateMachine : IAsyncStateMachine
        {
        }

        public AffPure<A> Task { get; private set; }
    }
}
