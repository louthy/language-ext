using LanguageExt;
using static LanguageExt.Prelude;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct MCompositions<MonoidA, A> : Monoid<Compositions<A>> where MonoidA : struct, Monoid<A>
    {
        public Compositions<A> Append(Compositions<A> compx, Compositions<A> compy)
        {
            Seq<Compositions<A>.Node> go(Seq<Compositions<A>.Node> mx, Seq<Compositions<A>.Node> my)
            {
                if (mx.IsEmpty) return my;
                if (my.IsEmpty) return go(mx.Tail, Seq1(mx.Head));

                var x = mx.Head;
                var sx = mx.Head.Size;
                var cx = mx.Head.Children;
                var vx = mx.Head.Value;
                var xs = mx.Tail;

                var y = my.Head;
                var sy = my.Head.Size;
                var vy = my.Head.Value;
                var ys = my.Tail;

                var ord = sx.CompareTo(sy);
                if (ord < 0) return go(xs, x.Cons(my));
                else if (ord > 0)
                {
                    var (l, r) = cx.IfNone((default(Compositions<A>.Node), default(Compositions<A>.Node)));
                    return go(r.Cons(l.Cons(xs)), my);
                }
                else
                {
                    return go(new Compositions<A>.Node(sx + sy, Some((x, y)), default(MonoidA).Append(vx, vy)).Cons(xs), ys);
                }
            }
            return new Compositions<A>(go(compx.Tree, compy.Tree));
        }

        public Compositions<A> Empty() => Compositions<A>.Empty;
    }
}
