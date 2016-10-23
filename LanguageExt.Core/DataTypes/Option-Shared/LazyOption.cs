using System;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    internal class LazyOption<A> : OptionV<A>
    {
        readonly Func<A> f;
        object sync = new object();
        bool memo;
        OptionV<A> value;

        public LazyOption(Func<A> f, bool memo)
        {
            this.memo = memo;
            this.f = f;
        }

        OptionV<A> OptionValue
        {
            get
            {
                if (memo)
                {
                    if (value != null) return value;
                    lock (sync)
                    {
                        if (value != null) return value;
                        var x = f();
                        value = isnull(x)
                            ? None<A>.Default
                            : new SomeValue<A>(x);
                    }
                    return value;
                }
                else
                {
                    var x = f();
                    return isnull(x)
                        ? None<A>.Default
                        : new SomeValue<A>(x);
                }
            }
        }

        public override bool IsSome =>
            OptionValue.IsSome;

        public override A Value =>
            OptionValue.Value;
    }

    internal class LazyOption2<A> : OptionV<A>
    {
        readonly Func<Option<A>> f;
        object sync = new object();
        bool memo;
        OptionV<A> value;

        public LazyOption2(Func<Option<A>> f, bool memo)
        {
            this.memo = memo;
            this.f = f;
        }

        OptionV<A> OptionValue
        {
            get
            {
                if (memo)
                {
                    if (value != null) return value;
                    lock (sync)
                    {
                        if (value != null) return value;
                        var x = f();
                        value = x.value;
                    }
                    return value;
                }
                else
                {
                    return f().value;
                }
            }
        }

        public override bool IsSome =>
            OptionValue.IsSome;

        public override A Value =>
            OptionValue.Value;
    }

}
