using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct OptionAsyncMethodBuilder<A>
    {
        IAsyncStateMachine stateMachine;
        
        public static OptionAsyncMethodBuilder<A> Create() =>
            new OptionAsyncMethodBuilder<A>();

        public void Start<TStateMachine>(ref TStateMachine machine)
            where TStateMachine : IAsyncStateMachine =>
            machine.MoveNext();

        public void SetStateMachine(IAsyncStateMachine machine) =>
            this.stateMachine = machine;

        public void SetException(Exception _) =>
            Task = OptionAsync<A>.None;

        public void SetResult(A result) =>
            Task = OptionAsync<A>.Some(result);

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

        public OptionAsync<A> Task { get; private set; }
    }
}
