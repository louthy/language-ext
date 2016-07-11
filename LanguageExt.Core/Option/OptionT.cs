using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public struct OptionT<MA, A> : MonadT<MA, A> where MA : Monad<A>
    {
        readonly Option<MA> value;

        /// <summary>
        /// None
        /// </summary>
        public static readonly OptionT<MA, A> None = default(OptionT<MA, A>);

        public OptionT(MA ma)
        {
            value = ma;
            IsSome = true;
        }

        public readonly bool IsSome;
        public bool IsNone => !IsSome;

        public MonadT<MB, B> Bind<MB, B>(MonadT<MA, A> ma, Func<A, Monad<B>> f) where MB : Monad<B>
        {
            var opt = AsOpt(ma);
            if (opt.value.IsNone) return OptionT<MB, B>.None;
            return new OptionT<MB, B>((MB)opt.value.Value.Bind(opt.value.Value, f));
        }

        public MonadT<MA, A> Fail(string _ = "") =>
            None;

        public FunctorT<MB, B> Map<MB, B>(FunctorT<MA, A> fa, Func<A, B> f) where MB : Monad<B>
        {
            var opt = AsOpt(fa);
            if (opt.value.IsNone) return OptionT<MB, B>.None;
            return new OptionT<MB, B>((MB)opt.value.Value.Map(opt.value.Value, f));
        }

        public MonadT<MA, A> Return(MA x) =>
            new OptionT<MA, A>(x);

        OptionT<MA, A> AsOpt(FunctorT<MA, A> a) => (OptionT<MA, A>)a;
        OptionT<MA, A> AsOpt(MonadT<MA, A> a) => (OptionT<MA, A>)a;
        OptionT<MA, A> AsOpt(FoldableT<MA, A> a) => (OptionT<MA, A>)a;

        public S FoldT<S>(FoldableT<MA, A> fa, S state, Func<S, A, S> f)
        {
            var opt = AsOpt(fa);
            if (opt.value.IsNone) return state;
            return opt.value.Value.Fold(opt.value.Value, state, f);
        }

        public S FoldBackT<S>(FoldableT<MA, A> fa, S state, Func<S, A, S> f)
        {
            var opt = AsOpt(fa);
            if (opt.value.IsNone) return state;
            return opt.value.Value.FoldBack(opt.value.Value, state, f);
        }

        public static implicit operator OptionT<MA, A>(MA a) =>
            (OptionT<MA, A>)default(OptionT<MA, A>).Return(a);
    }
}
