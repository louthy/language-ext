using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LanguageExt
{
    public delegate Task<T> Async<T>();

    public static class __AsyncExt
    {
        public static Async<U> Select<T, U>(this Async<T> self, Func<T, U> map) =>
            () => 
                Observable.FromAsync(() => self())
                          .Select(map)
                          .ToTask();

        public static Async<T> Where<T>(this Async<T> self, Func<T, bool> pred) =>
            () =>
                Observable.FromAsync(() => self())
                          .Where(pred)
                          .ToTask();

        public static Async<V> SelectMany<T, U, V>(
            this Async<T> self,
            Func<T, Async<U>> bind,
            Func<T, U, V> project
            ) =>
            () =>
                Observable.FromAsync(() => self())
                          .SelectMany(
                                t => Observable.FromAsync(() => bind(t)()),
                                project)
                          .ToTask();

        public static Async<T> Async<T>(this Func<T> f) => () => new Task<T>(f);
        public static Async<T> Async<T>(this Task<T> t) => () => t;

    }
}
