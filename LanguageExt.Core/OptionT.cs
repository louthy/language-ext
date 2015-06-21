using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Immutable;
using System.Linq;
using System.ComponentModel;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Trans
{
    public static partial class OptionT
    {
        // 
        // Option<T> extensions 
        // 
        public static Unit IterT<T>(this Option<T> self, Action<T> action) =>
            self.IfSome(action);

        public static int CountT<T>(this Option<T> self) =>
            self.IsSome
                ? 1
                : 0;

        public static bool ForAllT<T>(this Option<T> self, Func<T, bool> pred) =>
            self.IsSome
                ? pred(self.Value)
                : true;

        public static bool ExistsT<T>(this Option<T> self, Func<T, bool> pred) =>
            self.IsSome
                ? pred(self.Value)
                : false;

        public static S FoldT<S, T>(this Option<T> self, S state, Func<S, T, S> folder) =>
            self.IsSome
                ? folder(state, self.Value)
                : state;

        public static Option<R> MapT<T, R>(this Option<T> self, Func<T, R> mapper) =>
            self.IsSome
                ? Option.Cast(mapper(self.Value))
                : Option<R>.None;

        public static Option<T> FilterT<T>(this Option<T> self, Func<T, bool> pred) =>
            self.IsSome
                ? pred(self.Value)
                    ? self
                    : None
                : self;

        public static Option<R> BindT<T, R>(this Option<T> self, Func<T, Option<R>> binder) =>
            self.IsSome
                ? binder(self.Value)
                : Option<R>.None;

        // 
        // Option<IEnumerable<T>> extensions 
        // 

        public static Unit IterT<T>(this Option<IEnumerable<T>> self, Action<T> action) =>
            self.IsSome
                ? self.Value.IterT(action)
                : Unit.Default;

        public static int CountT<T>(this Option<IEnumerable<T>> self) =>
            self.IsSome
                ? self.Value.CountT()
                : 0;

        public static bool ForAllT<T>(this Option<IEnumerable<T>> self, Func<T, bool> pred) =>
            self.IsSome
                ? self.Value.ForAllT(pred)
                : true;

        public static S FoldT<T, S>(this Option<IEnumerable<T>> self, S state, Func<S, T, S> folder) =>
            self.IsSome
                ? self.Value.FoldT(state, folder)
                : state;

        public static bool ExistsT<T>(this Option<IEnumerable<T>> self, Func<T, bool> pred) =>
            self.IsSome
                ? self.Value.ExistsT(pred)
                : false;

        public static Option<IEnumerable<R>> MapT<T, R>(this Option<IEnumerable<T>> self, Func<T, R> mapper) =>
            self.IsSome
                ? Option.Cast(self.Value.MapT(mapper))
                : None;

        public static Option<IEnumerable<T>> Filter<T>(this Option<IEnumerable<T>> self, Func<T, bool> pred) =>
            self.IsSome
                ? Some(self.Value.FilterT(pred))
                : self;

        public static Option<IEnumerable<R>> BindT<T, R>(this Option<IEnumerable<T>> self, Func<T, Option<R>> binder) =>
            self.IsSome
                ? Some(self.Value.MapT(x => binder(x))
                                 .FilterT(x => x.IsSome)
                                 .MapT(x => x.Value))
                : None;

        public static Option<IEnumerable<U>> Select<T, U>(this Option<IEnumerable<T>> self, Func<T, U> map) =>
            self.IsSome
                ? Some(self.Value.MapT(map))
                : None;

        public static Option<IEnumerable<V>> SelectMany<T, U, V>(this Option<IEnumerable<T>> self,
            Func<T, IEnumerable<U>> bind,
            Func<T, U, V> project
            ) =>
            self.IsSome
                ? Some(self.Value.SelectMany(bind, project))
                : None;

        public static Option<IEnumerable<T>> Where<T>(this Option<IEnumerable<T>> self, Func<T, bool> pred) =>
            self.MapT(x => x.FilterT(pred));

        // 
        // Option<Lst<T>> extensions 
        // 

        public static Unit IterT<T>(this Option<Lst<T>> self, Action<T> action) =>
            self.IsSome
                ? self.Value.IterT(action)
                : Unit.Default;

        public static int CountT<T>(this Option<Lst<T>> self) =>
            self.IsSome
                ? self.Value.Count()
                : 0;

        public static bool ForAllT<T>(this Option<Lst<T>> self, Func<T, bool> pred) =>
            self.IsSome
                ? self.Value.ForAllT(pred)
                : true;

        public static S FoldT<T, S>(this Option<Lst<T>> self, S state, Func<S, T, S> folder) =>
            self.IsSome
                ? self.Value.FoldT(state, folder)
                : state;

        public static bool ExistsT<T>(this Option<Lst<T>> self, Func<T, bool> pred) =>
            self.IsSome
                ? self.Value.ExistsT(pred)
                : false;

        public static Option<Lst<R>> MapT<T, R>(this Option<Lst<T>> self, Func<T, R> mapper) =>
            self.IsSome
                ? Option.Cast(self.Value.MapT(mapper).Freeze())
                : None;

        public static Option<Lst<T>> FilterT<T>(this Option<Lst<T>> self, Func<T, bool> pred) =>
            self.IsSome
                ? Some(self.Value.FilterT(pred).Freeze())
                : self;

        public static Option<Lst<R>> BindT<T, R>(this Option<Lst<T>> self, Func<T, Lst<R>> binder) =>
            self.IsSome
                ? Some(self.Value.BindT(binder))
                : None;

        public static Option<Lst<U>> Select<T, U>(this Option<Lst<T>> self, Func<T, U> map) =>
            self.IsSome
                ? Some(self.Value.MapT(map).Freeze())
                : None;

        public static Option<Lst<V>> SelectMany<T, U, V>(this Option<Lst<T>> self,
            Func<T, IEnumerable<U>> bind,
            Func<T, U, V> project
            ) =>
            self.IsSome
                ? Some(self.Value.SelectMany(bind, project).Freeze())
                : None;

        public static Option<Lst<T>> Where<T>(this Option<Lst<T>> self, Func<T, bool> pred) =>
            self.FilterT(pred);

        // 
        // Option<Map<T>> extensions 
        // 

        public static Unit IterT<K, V>(this Option<Map<K, V>> self, Action<V> action) =>
            self.IsSome
                ? self.Value.IterT(action)
                : Unit.Default;

        public static Unit IterT<K, V>(this Option<Map<K, V>> self, Action<K, V> action) =>
            self.IsSome
                ? self.Value.IterT(action)
                : Unit.Default;

        public static int CountT<K, V>(this Option<Map<K, V>> self) =>
            self.IsSome
                ? self.Value.CountT()
                : 0;

        public static bool ForAllT<K, V>(this Option<Map<K, V>> self, Func<K, bool> pred) =>
            self.IsSome
                ? self.Value.ForAll(pred)
                : true;

        public static bool ForAllT<K, V>(this Option<Map<K, V>> self, Func<V, bool> pred) =>
            self.IsSome
                ? self.Value.ForAllT(pred)
                : true;

        public static bool ForAllT<K, V>(this Option<Map<K, V>> self, Func<K, V, bool> pred) =>
            self.IsSome
                ? self.Value.ForAllT(pred)
                : true;

        public static S FoldT<K, V, S>(this Option<Map<K, V>> self, S state, Func<S, K, V, S> folder) =>
            self.IsSome
                ? self.Value.FoldT(state, folder)
                : state;

        public static S FoldT<K, V, S>(this Option<Map<K, V>> self, S state, Func<S, V, S> folder) =>
            self.IsSome
                ? self.Value.FoldT(state, folder)
                : state;

        public static S FoldT<K, V, S>(this Option<Map<K, V>> self, S state, Func<S, K, S> folder) =>
            self.IsSome
                ? self.Value.FoldT(state, folder)
                : state;

        public static bool ExistsT<K, V>(this Option<Map<K, V>> self, Func<K, V, bool> pred) =>
            self.IsSome
                ? self.Value.ExistsT(pred)
                : false;

        public static bool ExistsT<K, V>(this Option<Map<K, V>> self, Func<K, bool> pred) =>
            self.IsSome
                ? self.Value.ExistsT(pred)
                : false;

        public static bool ExistsT<K, V>(this Option<Map<K, V>> self, Func<V, bool> pred) =>
            self.IsSome
                ? self.Value.ExistsT(pred)
                : false;

        public static Option<Map<K, R>> MapT<K, V, R>(this Option<Map<K, V>> self, Func<K, V, R> mapper) =>
            self.IsSome
                ? Option.Cast(self.Value.MapT(mapper))
                : None;

        public static Option<Map<K, R>> MapT<K, V, R>(this Option<Map<K, V>> self, Func<V, R> mapper) =>
            self.IsSome
                ? Option.Cast(self.Value.MapT(mapper))
                : None;

        public static Option<Map<K, V>> FilterT<K, V>(this Option<Map<K, V>> self, Func<K, V, bool> pred) =>
            self.IsSome
                ? Some(self.Value.FilterT(pred))
                : self;

        public static Option<Map<K, V>> FilterT<K, V>(this Option<Map<K, V>> self, Func<V, bool> pred) =>
            self.IsSome
                ? Some(self.Value.FilterT(pred))
                : self;

        public static Option<Map<K, V>> FilterT<K, V>(this Option<Map<K, V>> self, Func<K, bool> pred) =>
            self.IsSome
                ? Some(self.Value.FilterT(pred))
                : self;

        public static Option<Map<K, V>> WhereT<K, V>(this Option<Map<K, V>> self, Func<K, V, bool> pred) =>
            self.FilterT(pred);

        public static Option<Map<K, V>> WhereT<K, V>(this Option<Map<K, V>> self, Func<K, bool> pred) =>
            self.FilterT(pred);

        public static Option<Map<K, V>> WhereT<K, V>(this Option<Map<K, V>> self, Func<V, bool> pred) =>
            self.FilterT(pred);


        // 
        // Option<Option<T>> extensions 
        // 

        public static Unit IterT<T>(this Option<Option<T>> self, Action<T> action) =>
            self.IsSome
                ? self.Value.IterT(action)
                : Unit.Default;

        public static int CountT<T>(this Option<Option<T>> self) =>
            self.IsSome
                ? self.Value.CountT()
                : 0;

        public static bool ForAllT<T>(this Option<Option<T>> self, Func<T, bool> pred) =>
            self.IsSome
                ? self.Value.ForAllT(pred)
                : true;

        public static S FoldT<T, S>(this Option<Option<T>> self, S state, Func<S, T, S> folder) =>
            self.IsSome
                ? self.Value.FoldT(state, folder)
                : state;

        public static bool ExistsT<T>(this Option<Option<T>> self, Func<T, bool> pred) =>
            self.IsSome
                ? self.Value.ExistsT(pred)
                : false;

        public static Option<Option<R>> MapT<T, R>(this Option<Option<T>> self, Func<T, R> mapper) =>
            self.IsSome
                ? Option.Cast(self.Value.MapT(mapper))
                : None;

        public static Option<Option<T>> FilterT<T>(this Option<Option<T>> self, Func<T, bool> pred) =>
            self.IsSome
                ? Some(self.Value.FilterT(pred))
                : self;

        public static Option<Option<R>> BindT<T, R>(this Option<Option<T>> self, Func<T, Option<R>> binder) =>
            self.IsSome
                ? Some(self.Value.BindT(binder))
                : None;

        public static Option<Option<U>> Select<T, U>(this Option<Option<T>> self, Func<T, U> map) =>
            self.IsSome
                ? Some(self.Value.MapT(map))
                : None;

        public static Option<Option<V>> SelectMany<T, U, V>(this Option<Option<T>> self,
            Func<T, Option<U>> bind,
            Func<T, U, V> project
            ) =>
            self.IsSome
                ? Some(self.Value.SelectMany(bind, project))
                : None;

        public static Option<Option<T>> Where<T>(this Option<Option<T>> self, Func<T, bool> pred) =>
            self.IsSome
                ? Some(self.Value.FilterT(pred))
                : None;

        // 
        // Option<TryOption<T>> extensions 
        // 

        public static Unit IterT<T>(this Option<TryOption<T>> self, Action<T> action) =>
            self.IsSome
                ? self.Value.IterT(action)
                : Unit.Default;

        public static int CountT<T>(this Option<TryOption<T>> self) =>
            self.IsSome
                ? self.Value.CountT()
                : 0;

        public static bool ForAllT<T>(this Option<TryOption<T>> self, Func<T, bool> pred) =>
            self.IsSome
                ? self.Value.ForAllT(pred)
                : true;

        public static S FoldT<T, S>(this Option<TryOption<T>> self, S state, Func<S, T, S> folder) =>
            self.IsSome
                ? self.Value.FoldT(state, folder)
                : state;

        public static bool ExistsT<T>(this Option<TryOption<T>> self, Func<T, bool> pred) =>
            self.IsSome
                ? self.Value.ExistsT(pred)
                : false;

        public static Option<TryOption<R>> MapT<T, R>(this Option<TryOption<T>> self, Func<T, R> mapper) =>
            self.IsSome
                ? Option.Cast(self.Value.MapT(mapper))
                : None;

        public static Option<TryOption<T>> FilterT<T>(this Option<TryOption<T>> self, Func<T, bool> pred) =>
            self.IsSome
                ? Some(self.Value.FilterT(pred))
                : self;

        public static Option<TryOption<R>> BindT<T, R>(this Option<TryOption<T>> self, Func<T, TryOption<R>> binder) =>
            self.IsSome
                ? Some(self.Value.BindT(binder))
                : None;

        public static Option<TryOption<U>> Select<T, U>(this Option<TryOption<T>> self, Func<T, U> map) =>
            self.IsSome
                ? Some(self.Value.MapT(map))
                : None;

        public static Option<TryOption<V>> SelectMany<T, U, V>(this Option<TryOption<T>> self,
            Func<T, TryOption<U>> bind,
            Func<T, U, V> project
            ) =>
            self.IsSome
                ? Some(self.Value.SelectMany(bind, project))
                : None;

        public static Option<TryOption<T>> Where<T>(this Option<TryOption<T>> self, Func<T, bool> pred) =>
            self.IsSome
                ? Some(self.Value.FilterT(pred))
                : None;

        // 
        // Option<Try<T>> extensions 
        // 

        public static Unit IterT<T>(this Option<Try<T>> self, Action<T> action) =>
            self.IsSome
                ? self.Value.IterT(action)
                : Unit.Default;

        public static int CountT<T>(this Option<Try<T>> self) =>
            self.IsSome
                ? self.Value.CountT()
                : 0;

        public static bool ForAllT<T>(this Option<Try<T>> self, Func<T, bool> pred) =>
            self.IsSome
                ? self.Value.ForAllT(pred)
                : true;

        public static S FoldT<T, S>(this Option<Try<T>> self, S state, Func<S, T, S> folder) =>
            self.IsSome
                ? self.Value.FoldT(state, folder)
                : state;

        public static bool ExistsT<T>(this Option<Try<T>> self, Func<T, bool> pred) =>
            self.IsSome
                ? self.Value.ExistsT(pred)
                : false;

        public static Option<Try<R>> MapT<T, R>(this Option<Try<T>> self, Func<T, R> mapper) =>
            self.IsSome
                ? Option.Cast(self.Value.MapT(mapper))
                : None;

        public static Option<Try<T>> FilterT<T>(this Option<Try<T>> self, Func<T, bool> pred) =>
            self.IsSome
                ? Some(self.Value.FilterT(pred))
                : self;

        public static Option<Try<R>> BindT<T, R>(this Option<Try<T>> self, Func<T, Try<R>> binder) =>
            self.IsSome
                ? Some(self.Value.Bind(binder))
                : None;

        public static Option<Try<U>> Select<T, U>(this Option<Try<T>> self, Func<T, U> map) =>
            self.IsSome
                ? Some(self.Value.MapT(map))
                : None;

        public static Option<Try<V>> SelectMany<T, U, V>(this Option<Try<T>> self,
            Func<T, Try<U>> bind,
            Func<T, U, V> project
            ) =>
                self.IsSome
                    ? Some(self.Value.SelectMany(bind, project))
                    : None;

        public static Option<Try<T>> Where<T>(this Option<Try<T>> self, Func<T, bool> pred) =>
            self.IsSome
                ? Some(self.Value.FilterT(pred))
                : None;

        // 
        // Option<Either<L,R>> extensions 
        // 

        public static Unit IterT<L, R>(this Option<Either<L, R>> self, Action<R> action) =>
            self.IsSome
                ? self.Value.IterT(action)
                : Unit.Default;

        public static int CountT<L, R>(this Option<Either<L, R>> self) =>
            self.IsSome
                ? self.Value.CountT()
                : 0;

        public static bool ForAllT<L, R>(this Option<Either<L, R>> self, Func<R, bool> pred) =>
            self.IsSome
                ? self.Value.ForAllT(pred)
                : true;

        public static S FoldT<L, R, S>(this Option<Either<L, R>> self, S state, Func<S, R, S> folder) =>
            self.IsSome
                ? self.Value.FoldT(state, folder)
                : state;

        public static bool ExistsT<L, R>(this Option<Either<L, R>> self, Func<R, bool> pred) =>
            self.IsSome
                ? self.Value.ExistsT(pred)
                : false;

        public static Option<Either<L, R2>> MapT<L, R, R2>(this Option<Either<L, R>> self, Func<R, R2> mapper) =>
            self.IsSome
                ? Option.Cast(self.Value.MapT(mapper))
                : None;

        public static Option<Either<Unit, R>> FilterT<L, R>(this Option<Either<L, R>> self, Func<R, bool> pred) =>
            self.IsSome
                ? Some(self.Value.FilterT(pred))
                : None;

        public static Option<Either<L, R2>> BindT<L, R, R2>(this Option<Either<L, R>> self, Func<R, Either<L, R2>> binder) =>
            self.IsSome
                ? Some(self.Value.BindT(binder))
                : None;

        public static Option<Either<L, U>> Select<L, R, U>(this Option<Either<L, R>> self, Func<R, U> map) =>
            self.IsSome
                ? Some(self.Value.MapT(map))
                : None;

        public static Option<Either<L, V>> SelectMany<L, R, U, V>(this Option<Either<L, R>> self,
            Func<R, Either<L, U>> bind,
            Func<R, U, V> project
            ) =>
            self.IsSome
                ? Some(self.Value.SelectMany(bind, project))
                : None;

        public static Option<Either<Unit, R>> Where<L, R>(this Option<Either<L, R>> self, Func<R, bool> pred) =>
            self.IsSome
                ? Some(self.Value.FilterT(pred))
                : None;

        // 
        // Option<Reader<Env,T>> extensions 
        // 

        public static Reader<Env, Unit> IterT<Env, T>(this Option<Reader<Env, T>> self, Action<T> action) =>
            self.IsSome
                ? self.Value.IterT(action)
                : env => Unit.Default;

        public static int CountT<Env, T>(this Option<Reader<Env, T>> self) =>
            self.IsSome
                ? self.Value.CountT()
                : 0;

        public static Reader<Env, bool> ForAllT<Env, T>(this Option<Reader<Env, T>> self, Func<T, bool> pred) =>
            self.IsSome
                ? self.Value.ForAllT(pred)
                : env => true;

        public static Reader<Env, S> FoldT<Env, S, T>(this Option<Reader<Env, T>> self, S state, Func<S, T, S> folder) =>
            self.IsSome
                ? self.Value.FoldT(state, folder)
                : env => state;

        public static Reader<Env, bool> ExistsT<Env, T>(this Option<Reader<Env, T>> self, Func<T, bool> pred) =>
            self.IsSome
                ? self.Value.ExistsT(pred)
                : env => false;

        public static Option<Reader<Env, R>> MapT<Env, T, R>(this Option<Reader<Env, T>> self, Func<T, R> mapper) =>
            self.IsSome
                ? Option.Cast(self.Value.MapT(mapper))
                : None;

        public static Option<Reader<Env, R>> BindT<Env, T, R>(this Option<Reader<Env, T>> self, Func<T, Reader<Env, R>> binder) =>
            self.IsSome
                ? Some(self.Value.BindT(binder))
                : None;

        public static Option<Reader<Env, U>> Select<Env, T, U>(this Option<Reader<Env, T>> self, Func<T, U> map) =>
            self.IsSome
                ? Some(self.Value.MapT(map))
                : None;

        public static Option<Reader<Env, V>> SelectMany<Env, T, U, V>(this Option<Reader<Env, T>> self,
            Func<T, Reader<Env, U>> bind,
            Func<T, U, V> project
            ) =>
            self.IsSome
                ? Some(self.Value.SelectMany(bind, project))
                : None;


        // 
        // Option<Writer<Out,T>> extensions 
        // 

        public static Unit IterT<Out, T>(this Option<Writer<Out, T>> self, Action<T> action) =>
            self.IsSome
                ? self.Value.IterT(action)
                : Unit.Default;

        public static int CountT<Out, T>(this Option<Writer<Out, T>> self) =>
            self.IsSome
                ? self.Value.CountT()
                : 0;

        public static bool ForAllT<Out, T>(this Option<Writer<Out, T>> self, Func<T, bool> pred) =>
            self.IsSome
                ? self.Value.ForAllT(pred)
                : true;

        public static S FoldT<Out, S, T>(this Option<Writer<Out, T>> self, S state, Func<S, T, S> folder) =>
            self.IsSome
                ? self.Value.FoldT(state, folder)
                : state;

        public static bool ExistsT<Out, T>(this Option<Writer<Out, T>> self, Func<T, bool> pred) =>
            self.IsSome
                ? self.Value.ExistsT(pred)
                : false;

        public static Option<Writer<Out, R>> MapT<Out, T, R>(this Option<Writer<Out, T>> self, Func<T, R> mapper) =>
            self.IsSome
                ? Option.Cast(self.Value.MapT(mapper))
                : None;

        public static Option<Writer<Out, R>> BindT<Out, T, R>(this Option<Writer<Out, T>> self, Func<T, Writer<Out, R>> binder) =>
            self.IsSome
                ? Some(self.Value.BindT(binder))
                : None;

        public static Option<Writer<Out, U>> Select<Out, T, U>(this Option<Writer<Out, T>> self, Func<T, U> map) =>
            self.IsSome
                ? Some(self.Value.MapT(map))
                : None;

        public static Option<Writer<Out, V>> SelectMany<Out, T, U, V>(this Option<Writer<Out, T>> self,
            Func<T, Writer<Out, U>> bind,
            Func<T, U, V> project
            ) =>
            self.IsSome
                ? Some(self.Value.SelectMany(bind, project))
                : None;

        // 
        // Option<State<S,T>> extensions 
        // 

        public static State<S, Unit> IterT<S, T>(this Option<State<S, T>> self, Action<T> action) =>
            self.IsSome
                ? self.Value.IterT(action)
                : s => new StateResult<S, Unit>(s, Unit.Default);

        public static int CountT<S, T>(this Option<State<S, T>> self) =>
            self.IsSome
                ? self.Value.CountT()
                : 0;

        public static State<S, bool> ForAllT<S, T>(this Option<State<S, T>> self, Func<T, bool> pred) =>
            self.IsSome
                ? self.Value.ForAllT(pred)
                : s => new StateResult<S, bool>(s, true);

        public static State<S, FState> FoldT<S, T, FState>(this Option<State<S, T>> self, FState state, Func<FState, T, FState> folder) =>
            self.IsSome
                ? self.Value.FoldT(state, folder)
                : s => new StateResult<S, FState>(s, state);

        public static State<S, Unit> FoldT<S, T>(this Option<State<S, T>> self, Func<S, T, S> folder) =>
            self.IsSome
                ? self.Value.FoldT(folder)
                : s => new StateResult<S, Unit>(s, unit);

        public static State<S, bool> ExistsT<S, T>(this Option<State<S, T>> self, Func<T, bool> pred) =>
            self.IsSome
                ? self.Value.ExistsT(pred)
                : s => new StateResult<S, bool>(s, false);

        public static Option<State<S, R>> MapT<S, T, R>(this Option<State<S, T>> self, Func<T, R> mapper) =>
            self.IsSome
                ? Option.Cast(self.Value.MapT(mapper))
                : None;

        public static Option<State<S, R>> BindT<S, T, R>(this Option<State<S, T>> self, Func<T, State<S, R>> binder) =>
            self.IsSome
                ? Some(self.Value.BindT(binder))
                : None;

        public static Option<State<S, U>> Select<S, T, U>(this Option<State<S, T>> self, Func<T, U> map) =>
            self.IsSome
                ? Some(self.Value.MapT(map))
                : None;

        public static Option<State<S, V>> SelectMany<S, T, U, V>(this Option<State<S, T>> self,
            Func<T, State<S, U>> bind,
            Func<T, U, V> project
            ) =>
            self.IsSome
                ? Some(self.Value.SelectMany(bind, project))
                : None;
    }
}
