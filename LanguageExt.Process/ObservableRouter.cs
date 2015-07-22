using System;
using System.Reactive.Subjects;
using System.Reactive.Linq;

using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// ObservableRouter routes messages 'published' by processes using the 'pub'
    /// function.  The idea is that the routing never dies and can therefore survive
    /// a process restart without killing the subscribers. 
    /// </summary>
    internal static class ObservableRouter
    {
        static object sync = new object();

        static ObservableRouter()
        {
            Restart();
        }

        public static void Shutdown()
        {
            lock (sync)
            {
                if (Store != null)
                {
                    List.iter(Store.Values, item => item.OnCompleted());
                }
                Store = Map.create<string, Subject<object>>();
            }
        }

        public static void Restart()
        {
            Shutdown();
        }

        static Map<string, Subject<object>> Store
        {
            get;
            set;
        }

        public static IObservable<T> Observe<T>(ProcessId id) =>
            EnsureInStore(id, (store, path) =>
                from  x in store[path]
                where x is T
                select (T)x );

        public static Map<string, Subject<object>> AddOrUpdateStore(ProcessId id)
        {
            lock (sync)
            {
                var path = id.Value;
                if (Store.ContainsKey(path))
                {
                    return Store;
                }
                else
                {
                    Store = Store.Add(path, new Subject<object>());
                }
                return Store;
            }
        }

        public static Map<string, Subject<object>> RemoveFromStore(ProcessId id)
        {
            lock (sync)
            {
                var path = id.Value;
                if (Store.ContainsKey(path))
                {
                    Store[path].OnCompleted();
                    Store[path].Dispose();
                    Store = Store.Remove(path);
                }
                return Store;
            }
        }

        private static R EnsureInStore<R>(ProcessId id, Func<Map<string, Subject<object>>,string,R> f)
        {
            var store = Store;
            var path = id.ToString();

            return store.ContainsKey(path)
                ? f(store, path)
                : f(AddOrUpdateStore(id), path);
        }

        public static Unit Publish(ProcessId id, object message) =>
            EnsureInStore(id, (store, path) =>
            {
                store[path].OnNext(message);
                return unit;
            });

        public static IDisposable Subscribe<T>(ProcessId id, IObserver<T> observer) =>
            EnsureInStore(id, (store, path) =>
                (from obj in store[path]
                 where obj is T
                 select (T)obj)
                .Subscribe(observer));

        public static IDisposable Subscribe<T>(ProcessId id, Action<T> onNext, Action<Exception> onError, Action onComplete) =>
            EnsureInStore(id, (store, path) =>
                (from obj in store[path]
                 where obj is T
                 select (T)obj)
                .Subscribe(onNext, onError, onComplete));

    }
}
