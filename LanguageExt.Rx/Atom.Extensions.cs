using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;

namespace LanguageExt
{
    public static class AtomExtensions
    {
        public static IObservable<A> OnChange<A>(this Atom<A> atom) where A : class =>
            Observable.FromEvent<A>(
                add =>
                {
                    atom.Change += new AtomChangedEvent<A>(add);
                },
                remove =>
                {
                    atom.Change -= new AtomChangedEvent<A>(remove);
                });

        public static IObservable<A> OnChange<A>(this AtomValue<A> atom) where A : struct =>
            Observable.FromEvent<A>(
                add =>
                {
                    atom.Change += new AtomChangedEvent<A>(add);
                },
                remove =>
                {
                    atom.Change -= new AtomChangedEvent<A>(remove);
                });
    }
}
