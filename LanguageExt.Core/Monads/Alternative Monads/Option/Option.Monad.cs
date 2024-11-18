using System;
using LanguageExt.Traits;

namespace LanguageExt;

public class Option : 
    Monad<Option>, 
    Fallible<Unit, Option>,
    Traversable<Option>, 
    Alternative<Option>,
    Natural<Option, Arr>,
    Natural<Option, Lst>,
    Natural<Option, Seq>,
    Natural<Option, Iterable>,
    Natural<Option, Eff>,
    Natural<Option, OptionT<IO>>,
    Natural<Option, Fin>
{
    static K<Option, B> Monad<Option>.Bind<A, B>(K<Option, A> ma, Func<A, K<Option, B>> f) =>
        ma.As().Bind(f);

    static K<Option, B> Functor<Option>.Map<A, B>(Func<A, B> f, K<Option, A> ma) => 
        ma.As().Map(f);

    static K<Option, A> Applicative<Option>.Pure<A>(A value) =>
        Option<A>.Some(value);

    static K<Option, B> Applicative<Option>.Apply<A, B>(K<Option, Func<A, B>> mf, K<Option, A> ma) =>
        mf.As().Bind(ma.As().Map);

    static K<Option, B> Applicative<Option>.Action<A, B>(K<Option, A> ma, K<Option, B> mb) =>
        mb;

    static S Foldable<Option>.FoldWhile<A, S>(
        Func<A, Func<S, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState,
        K<Option, A> ta) =>
        ta.As().Match(Some: a => predicate((initialState, a)) ? f(a)(initialState) : initialState,
                      None: initialState);

    static S Foldable<Option>.FoldBackWhile<A, S>(
        Func<S, Func<A, S>> f, 
        Func<(S State, A Value), bool> predicate, 
        S initialState, 
        K<Option, A> ta) => 
        ta.As().Match(Some: a => predicate((initialState, a)) ? f(initialState)(a) : initialState,
                      None: initialState);

    static K<F, K<Option, B>> Traversable<Option>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Option, A> ta) =>
        ta.As().Match(Some: a => F.Map(Some, f(a)),
                      None: () => F.Pure(None<B>()));

    static K<Option, A> MonoidK<Option>.Empty<A>() =>
        None<A>();

    static K<Option, A> Choice<Option>.Choose<A>(K<Option, A> ma, K<Option, A> mb) =>
        ma.As() switch
        {
            { IsSome: true } => ma,
            _                => mb
        };

    static K<Option, X> Some<X>(X value) =>
        Option<X>.Some(value);

    static K<Option, X> None<X>() =>
        Option<X>.None;

    static K<Option, A> Fallible<Unit, Option>.Fail<A>(Unit _) =>
        Option<A>.None;

    static K<Option, A> Fallible<Unit, Option>.Catch<A>(
        K<Option, A> fa, 
        Func<Unit, bool> Predicate, 
        Func<Unit, K<Option, A>> Fail) => 
        fa.As().Match(Some: Option<A>.Some, 
                      None: () => Predicate(default) ? Fail(default).As() : Option<A>.None);

    static K<Arr, A> Natural<Option, Arr>.Transform<A>(K<Option, A> fa) => 
        fa.As().ToArr();

    static K<Lst, A> Natural<Option, Lst>.Transform<A>(K<Option, A> fa) => 
        fa.As().ToLst();

    static K<Seq, A> Natural<Option, Seq>.Transform<A>(K<Option, A> fa) => 
        fa.As().ToSeq();

    static K<Iterable, A> Natural<Option, Iterable>.Transform<A>(K<Option, A> fa) => 
        fa.As().ToIterable();

    static K<Eff, A> Natural<Option, Eff>.Transform<A>(K<Option, A> fa) => 
        fa.As().ToEff();

    static K<OptionT<IO>, A> Natural<Option, OptionT<IO>>.Transform<A>(K<Option, A> fa) => 
        fa.As().ToIO();

    static K<Fin, A> Natural<Option, Fin>.Transform<A>(K<Option, A> fa) => 
        fa.As().ToFin();
}
