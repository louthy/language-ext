using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageExt
{
    internal enum OptionState : byte
    {
        None,
        Some,
        Lazy
    }

    internal static class OptionData
    {
        public static OptionData<A> Lazy<A>(OptionLazy<A> f)
        {
            object sync = new object();
            (bool, A) value = default((bool, A));
            bool hasValue = false;

            return new OptionData<A>(OptionState.Lazy, default(A), () =>
            {
                if (hasValue) return value;
                lock (sync)
                {
                    if (!hasValue)
                    {
                        value = f();
                        hasValue = true;
                    }
                }
                return value;
            });
        }

        public static OptionData<A> Some<A>(A value) =>
            new OptionData<A>(OptionState.Some, value, null);

        public static OptionData<A> None<A>() =>
            OptionData<A>.None;

        public static OptionData<A> Optional<A>(A value) =>
            value.IsNull()
                ? OptionData<A>.None
                : Some(value);
    }

    internal struct OptionData<A>
    {
        public readonly static OptionData<A> None = new OptionData<A>(OptionState.None, default(A), null);

        OptionState state;
        A value;
        readonly OptionLazy<A> lazy;

        public OptionData(OptionState state, A value, OptionLazy<A> lazy)
        {
            this.state = state;
            this.value = value;
            this.lazy = lazy;
        }

        public A Value
        {
            get
            {
                if (state == OptionState.Some) return value;
                if (state == OptionState.None) throw new ValueIsNoneException();

                var (isSome, x) = lazy();
                if (isSome) return x;
                throw new ValueIsNoneException();
            }
        }

        public bool IsLazy => 
            state == OptionState.Lazy;

        public bool IsSome
        {
            get
            {
                if (state == OptionState.Some) return true;
                if (state == OptionState.None) return false;
                var (isSome, x) = lazy();
                return isSome;
            }
        }

        public bool IsNone => !IsSome;
    }
}
