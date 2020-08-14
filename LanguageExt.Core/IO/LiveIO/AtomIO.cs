using System;
using System.Threading.Tasks;

namespace LanguageExt.LiveIO
{
    public struct AtomIO : Interfaces.AtomIO
    {
        public static readonly Interfaces.AtomIO Default = new AtomIO();
        
        public Ref<A> Ref<A>(A value, Func<A, bool> validator = null) =>
            Prelude.Ref(value, validator);

        public R Sync<R>(Func<R> op, Isolation isolation = Isolation.Snapshot) =>
            Prelude.sync<R>(op, isolation);

        public Unit Sync(Action op, Isolation isolation = Isolation.Snapshot) =>
            Prelude.sync(op, isolation);

        public A Swap<A>(Ref<A> r, Func<A, A> f) =>
            Prelude.swap<A>(r, f);

        public A Swap<A, X>(Ref<A> r, X x, Func<X, A, A> f) =>
            Prelude.swap<A, X>(r, x, f);

        public A Swap<A, X, Y>(Ref<A> r, X x, Y y, Func<X, Y, A, A> f) =>
            Prelude.swap(r, x, y, f);

        public A Commute<A>(Ref<A> r, Func<A, A> f) =>
            Prelude.commute(r, f);

        public A Commute<A, X>(Ref<A> r, X x, Func<X, A, A> f) =>
            Prelude.commute(r, x, f);

        public A Commute<A, X, Y>(Ref<A> r, X x, Y y, Func<X, Y, A, A> f) =>
            Prelude.commute(r, x, y, f);

        public Atom<A> Atom<A>(A value) =>
            Prelude.Atom(value);

        public Option<Atom<A>> Atom<A>(A value, Func<A, bool> validator) =>
            Prelude.Atom(value, validator);

        public Atom<M, A> Atom<M, A>(M metadata, A value) =>
            Prelude.Atom(metadata, value);

        public Option<Atom<M, A>> Atom<M, A>(M metadata, A value, Func<A, bool> validator) =>
            Prelude.Atom(metadata, value, validator);

        public bool Swap<A>(Atom<A> atom, Func<A, A> f) =>
            Prelude.swap(atom, f);

        public Task<bool> SwapAsync<A>(Atom<A> atom, Func<A, Task<A>> f) =>
            Prelude.swapAsync(atom, f);

        public bool Swap<M, A>(Atom<M, A> atom, Func<M, A, A> f) =>
            Prelude.swap(atom, f);

        public Task<bool> SwapAsync<M, A>(Atom<M, A> atom, Func<M, A, Task<A>> f) =>
            Prelude.swapAsync(atom, f);

        public bool Swap<X, A>(Atom<A> atom, X x, Func<X, A, A> f) =>
            Prelude.swap(atom, x, f);

        public Task<bool> SwapAsync<X, A>(Atom<A> atom, X x, Func<X, A, Task<A>> f) =>
            Prelude.swapAsync(atom, x, f);

        public bool Swap<M, X, A>(Atom<M, A> atom, X x, Func<M, X, A, A> f) =>
            Prelude.swap(atom, x, f);

        public Task<bool> SwapAsync<M, X, A>(Atom<M, A> atom, X x, Func<M, X, A, Task<A>> f) =>
            Prelude.swapAsync(atom, x, f);

        public bool Swap<X, Y, A>(Atom<A> atom, X x, Y y, Func<X, Y, A, A> f) =>
            Prelude.swap(atom, x, y, f);

        public Task<bool> SwapAsync<X, Y, A>(Atom<A> atom, X x, Y y, Func<X, Y, A, Task<A>> f) =>
            Prelude.swapAsync(atom, x, y, f);

        public bool Swap<M, X, Y, A>(Atom<M, A> atom, X x, Y y, Func<M, X, Y, A, A> f) =>
            Prelude.swap(atom, x, y, f);

        public async ValueTask<bool> SwapAsync<M, X, Y, A>(Atom<M, A> atom, X x, Y y, Func<M, X, Y, A, Task<A>> f) =>
            await Prelude.swapAsync(atom, x, y, f);
    }
}
