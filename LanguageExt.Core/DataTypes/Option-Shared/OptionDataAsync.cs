using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    internal static class OptionDataAsync
    {
        public static OptionDataAsync<A> Lazy<A>(OptionLazyAsync<A> f) =>
            new OptionDataAsync<A>(OptionState.Lazy, default(A), f);

        public static OptionDataAsync<A> Some<A>(A value) =>
            new OptionDataAsync<A>(OptionState.Some, value, null);

        public static OptionDataAsync<A> None<A>() =>
            OptionDataAsync<A>.None;

        public static OptionDataAsync<A> Optional<A>(A value) =>
            value.IsNull()
                ? OptionDataAsync<A>.None
                : Some(value);
    }

    internal class OptionDataAsync<A>
    {
        public readonly static OptionDataAsync<A> None = new OptionDataAsync<A>(OptionState.None, default(A), null);

        OptionState state;
        A value;
        readonly OptionLazyAsync<A> lazy;

        public async Task<A> Value()
        {
            await CheckLazy();
            return value;
        }

        public async Task<Result<A>> AsTask()
        {
            await CheckLazy();
            return state == OptionState.Some
                ? new Result<A>(value)
                : Result<A>.None;
        }

        public bool IsLazy => 
            state == OptionState.Lazy;

        public async Task<bool> IsSome()
        {
            await CheckLazy();
            return state == OptionState.Some;
        }

        public async Task<bool> IsNone()
        {
            await CheckLazy();
            return state == OptionState.None;
        }

        public OptionDataAsync(OptionState state, A value, OptionLazyAsync<A> lazy)
        {
            this.state = state;
            this.value = value;
            this.lazy = lazy;
        }

        async Task CheckLazy()
        {
            if (state == OptionState.Lazy)
            {
                var result = await lazy();
                value = result.Value;
                state = result.IsFaulted
                    ? OptionState.None
                    : OptionState.Some;
            }
        }
    }
}
