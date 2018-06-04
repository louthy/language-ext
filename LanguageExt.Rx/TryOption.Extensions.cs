using System;
using System.Reactive.Linq;

namespace LanguageExt
{
    public static class TryOptionRxExtensions
    {
        public static IObservable<A> ToObservable<A>(this TryOption<A> ma) =>
            ma.Match(
                Some: Observable.Return,
                None: () => Observable.Empty<A>(),
                Fail: e => Observable.Empty<A>());

        public static IObservable<R> MatchObservable<A, R>(this TryOption<A> ma, Func<A, IObservable<R>> Some, Func<R> Fail) =>
            ma.Match(
                Some: Some,
                Fail: () => Observable.Return(Fail()));

        public static IObservable<R> MatchObservable<A, R>(this TryOption<A> ma, Func<A, IObservable<R>> Some, Func<R> None, Func<Exception, R> Fail) =>
            ma.Match(
                Some: Some,
                None: () => Observable.Return(None()),
                Fail: e => Observable.Return(Fail(e)));

        public static IObservable<R> MatchObservable<A, R>(this TryOption<A> ma, Func<A, IObservable<R>> Some, Func<IObservable<R>> Fail) =>
            ma.Match(
                Some: Some,
                Fail: Fail);

        public static IObservable<R> MatchObservable<A, R>(this TryOption<A> ma, Func<A, IObservable<R>> Some, Func<IObservable<R>> None, Func<Exception, IObservable<R>> Fail) =>
            ma.Match(
                Some: Some,
                None: None,
                Fail: Fail);

        public static IObservable<R> MatchObservable<A, R>(this TryOption<A> ma, Func<A, R> Some, Func<IObservable<R>> Fail) =>
            ma.Match(
                Some: x => Observable.Return(Some(x)),
                Fail: Fail);

        public static IObservable<R> MatchObservable<A, R>(this TryOption<A> ma, Func<A, R> Some, Func<IObservable<R>> None, Func<Exception, IObservable<R>> Fail) =>
            ma.Match(
                Some: x => Observable.Return(Some(x)),
                None: None,
                Fail: Fail);


        public static IObservable<R> MatchObservable<T, R>(this IObservable<TryOption<T>> self, Func<T, R> Some, Func<R> Fail) =>
            self.Select(trySelf =>
            {
                var res = trySelf.Try();
                return res.Match(Some, Fail, _ => Fail());
            });

        public static IObservable<R> MatchObservable<T, R>(this IObservable<TryOption<T>> self, Func<T, IObservable<R>> Some, Func<R> Fail) =>
            from tt in self.SelectMany(trySelf =>
            {
                var res = trySelf.Try();
                return res.Match(Some, () => Observable.Return(Fail()), _ => Observable.Return(Fail()));
            })
            select tt;

        public static IObservable<R> MatchObservable<T, R>(this IObservable<TryOption<T>> self, Func<T, IObservable<R>> Some, Func<IObservable<R>> Fail) =>
            from tt in self.SelectMany(trySelf =>
            {
                var res = trySelf.Try();
                return res.Match(Some, Fail, _ => Fail());
            })
            select tt;

        public static IObservable<R> MatchObservable<T, R>(this IObservable<TryOption<T>> self, Func<T, R> Some, Func<IObservable<R>> Fail) =>
            from tt in self.SelectMany(trySelf =>
            {
                var res = trySelf.Try();
                return res.Match(x => Observable.Return(Some(x)), Fail, _ => Fail());
            })
            select tt;

        public static IObservable<R> MatchObservable<T, R>(this IObservable<TryOption<T>> self, Func<T, R> Some, Func<R> None, Func<Exception, R> Fail) =>
            self.Select(trySelf =>
            {
                var res = trySelf.Try();
                return res.Match(Some, None, Fail);
            });

        public static IObservable<R> MatchObservable<T, R>(this IObservable<TryOption<T>> self, Func<T, IObservable<R>> Some, Func<R> None, Func<Exception, R> Fail) =>
            from tt in self.SelectMany(trySelf =>
            {
                var res = trySelf.Try();
                return res.Match(Some, () => Observable.Return(None()), e => Observable.Return(Fail(e)));
            })
            select tt;

        public static IObservable<R> MatchObservable<T, R>(this IObservable<TryOption<T>> self, Func<T, IObservable<R>> Some, Func<IObservable<R>> None, Func<Exception, IObservable<R>> Fail) =>
            from tt in self.SelectMany(trySelf =>
            {
                var res = trySelf.Try();
                return res.Match(Some, None, Fail);
            })
            select tt;

        public static IObservable<R> MatchObservable<T, R>(this IObservable<TryOption<T>> self, Func<T, R> Some, Func<IObservable<R>> None, Func<Exception, IObservable<R>> Fail) =>
            from tt in self.SelectMany(trySelf =>
            {
                var res = trySelf.Try();
                return res.Match(x => Observable.Return(Some(x)), None, Fail);
            })
            select tt;

    }
}
