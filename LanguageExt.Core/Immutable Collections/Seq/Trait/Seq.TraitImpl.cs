using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;
using System.Collections.Generic;

namespace LanguageExt;

public partial class Seq : 
    Monad<Seq>, 
    MonoidK<Seq>,
    Alternative<Seq>, 
    Traversable<Seq>,
    Foldable<Seq, Seq.FoldState>
{
    static K<Seq, B> Monad<Seq>.Recur<A, B>(A value, Func<A, K<Seq, Next<A, B>>> f)
    {
        return toSeq(go());
        IEnumerable<B> go()
        {
            List<A> values = [value];
            List<A> next   = [];

            while (true)
            {
                foreach (var x in values)
                {
                    foreach (var mb in +f(x))
                    {
                        if (mb.IsDone)
                        {
                            yield return mb.Done;
                        }
                        else
                        {
                            next.Add(mb.Loop);
                        }
                    }
                }

                if (next.Count == 0)
                {
                    break;
                }
                else
                {
                    (values, next) = (next, values);
                    next.Clear();
                }
            }
        }
    }
    
    static K<Seq, B> Monad<Seq>.Bind<A, B>(K<Seq, A> ma, Func<A, K<Seq, B>> f)
    {
        return new Seq<B>(go());
        IEnumerable<B> go()
        {
            foreach (var x in ma.As())
            {
                foreach (var y in f(x).As())
                {
                    yield return y;
                }
            }
        }
    }

    static K<Seq, B> Functor<Seq>.Map<A, B>(Func<A, B> f, K<Seq, A> ma) 
    {
        return new Seq<B>(go());
        IEnumerable<B> go()
        {
            foreach (var x in ma.As())
            {
                yield return f(x);
            }
        }
    }

    static K<Seq, A> Applicative<Seq>.Pure<A>(A value) =>
        singleton(value);

    static K<Seq, B> Applicative<Seq>.Apply<A, B>(K<Seq, Func<A, B>> mf, K<Seq, A> ma) 
    {
        return new Seq<B>(go());
        IEnumerable<B> go()
        {
            foreach (var f in mf.As())
            {
                foreach (var a in ma.As())
                {
                    yield return f(a);
                }
            }
        }
    }

    static K<Seq, B> Applicative<Seq>.Apply<A, B>(K<Seq, Func<A, B>> mf, Memo<Seq, A> ma) 
    {
        return new Seq<B>(go());
        IEnumerable<B> go()
        {
            foreach (var f in mf.As())
            {
                foreach (var a in ma.Value.As())
                {
                    yield return f(a);
                }
            }
        }
    }

    static K<Seq, A> MonoidK<Seq>.Empty<A>() =>
        Seq<A>.Empty;

    static K<Seq, A> Alternative<Seq>.Empty<A>() =>
        Seq<A>.Empty;

    static K<Seq, A> Choice<Seq>.Choose<A>(K<Seq, A> ma, K<Seq, A> mb) => 
        ma.As().IsEmpty ? mb : ma;

    static K<Seq, A> Choice<Seq>.Choose<A>(K<Seq, A> ma, Memo<Seq, A> mb) => 
        ma.As().IsEmpty ? ~mb : ma;

    static K<Seq, A> SemigroupK<Seq>.Combine<A>(K<Seq, A> ma, K<Seq, A> mb) =>
        ma.As() + mb.As();

    static long Foldable<Seq>.Count<A>(K<Seq, A> ta) =>
        ta.As().Count;

    static FoldState IterableK<Seq, FoldState>.StepSetup<A>(K<Seq, A> ta) => 
        ta.As().InitFoldState();

    static bool IterableK<Seq, FoldState>.Step<A>(K<Seq, A> ta, ref FoldState refState, out A value)
    {
        if (FoldState.MoveNext<A>(ref refState, out var v))
        {
            value = v;
            return true;
        }
        else
        {
            value = default!;
            return false;
        }
    }

    static bool Foldable<Seq>.IsEmpty<A>(K<Seq, A> ta) =>
        ta.As().IsEmpty;

    static Option<A> Foldable<Seq>.At<A>(long index, K<Seq, A> ta)
    {
        var list = ta.As();
        return index >= 0 && index < list.Count
                   ? Some(list[index])
                   : Option<A>.None;
    }

    static Option<A> Foldable<Seq>.Head<A>(K<Seq, A> ta) =>
        ta.As().Head;
    
    static K<F, K<Seq, B>> Traversable<Seq>.Traverse<F, A, B>(Func<A, K<F, B>> f, K<Seq, A> ta)
    {
        return Foldable.fold(add, F.Pure(Seq<B>.Empty), ta)
                       .Map(bs => bs.Kind());

        K<F, Seq<B>> add(K<F, Seq<B>> state, A value) =>
            Applicative.lift((bs, b) => bs.Add(b), state, f(value));
    }

    static K<F, K<Seq, B>> Traversable<Seq>.TraverseM<F, A, B>(Func<A, K<F, B>> f, K<Seq, A> ta) =>
        ta.FoldM((bs, a) => f(a).Map(bs.Add), Seq<B>.Empty)
          .Map(bs => bs.Kind());

    static Iterator<A> IterableK<Seq>.ForwardIterator<A>(K<Seq, A> fa) =>
        new Iterator<A>.IterSeq(+fa);
}
