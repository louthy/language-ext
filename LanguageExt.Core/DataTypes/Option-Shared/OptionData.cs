using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageExt
{
    internal enum OptionState : byte
    {
        Lazy,
        None,
        Some,
    }

    internal static class OptionData
    {
        public static OptionData<A> Lazy<A>(OptionLazy<A> f) =>
            new OptionData<A>(OptionState.Lazy, default(A), f);

        public static OptionData<A> Some<A>(A value) =>
            new OptionData<A>(OptionState.Some, value, null);

        public static OptionData<A> None<A>() =>
            OptionData<A>.None;

        public static OptionData<A> Optional<A>(A value) =>
            value.IsNull()
                ? OptionData<A>.None
                : Some(value);
    }

    internal class OptionData<A>
    {
        public readonly static OptionData<A> None = new OptionData<A>(OptionState.None, default(A), null);

        OptionState state;
        A value;
        readonly OptionLazy<A> lazy;

        public A Value
        {
            get
            {
                CheckLazy();
                return value;
            }
        }

        public bool IsLazy => 
            state == OptionState.Lazy;

        public bool IsSome
        {
            get
            {
                CheckLazy();
                return state == OptionState.Some;
            }
        }

        public bool IsNone => !IsSome;

        public OptionData(OptionState state, A value, OptionLazy<A> lazy)
        {
            this.state = state;
            this.value = value;
            this.lazy = lazy;
        }

        void CheckLazy()
        {
            if (state == OptionState.Lazy)
            {
                var (isSome, a) = lazy();
                value = a;
                state = isSome
                    ? OptionState.Some
                    : OptionState.None;
            }
        }
    }
}
