using LanguageExt;
using static LanguageExt.Prelude;
using LanguageExt.TypeClasses;
using System;

namespace LanguageExt.ClassInstances
{
    public struct FoldCompositions<A> : Foldable<Compositions<A>, A>
    {
        S FoldNode<S>(S state, Func<S, A, S> f, Compositions<A>.Node node)
        {
            if (node.Children.IsNone) return f(state, node.Value);
            var (l, r) = node.Children.IfNone((default(Compositions<A>.Node), default(Compositions<A>.Node)));
            state = FoldNode(state, f, l);
            state = FoldNode(state, f, r);
            return state;
        }

        S FoldNodes<S>(S state, Func<S, A, S> f, Seq<Compositions<A>.Node> nodes) =>
            nodes.Fold(state, (s, n) => default(FoldCompositions<A>).FoldNode(s, f, n));

        S FoldNodeBack<S>(S state, Func<S, A, S> f, Compositions<A>.Node node)
        {
            if (node.Children.IsNone) return f(state, node.Value);
            var (l, r) = node.Children.IfNone((default(Compositions<A>.Node), default(Compositions<A>.Node)));
            state = FoldNode(state, f, r);
            state = FoldNode(state, f, l);
            return state;
        }

        S FoldNodesBack<S>(S state, Func<S, A, S> f, Seq<Compositions<A>.Node> nodes) =>
            nodes.FoldBack(state, (s, n) => default(FoldCompositions<A>).FoldNode(s, f, n));

        internal Seq<B> FoldMap<S, B>(Func<A, B> f, Seq<Compositions<A>.Node> nodes) =>
            FoldNodes(Seq<B>(), (s, n) => f(n).Cons(s), nodes);

        internal Seq<B> FoldMapBack<S, B>(Func<A, B> f, Seq<Compositions<A>.Node> nodes) =>
            FoldNodesBack(Seq<B>(), (s, n) => f(n).Cons(s), nodes);

        public Func<Unit, int> Count(Compositions<A> fa) => _ =>
           default(FoldCompositions<A>).FoldNodes(0, (s, n) => s + 1, fa.Tree);

        public Func<Unit, S> Fold<S>(Compositions<A> fa, S state, Func<S, A, S> f) => _ =>
            default(FoldCompositions<A>).FoldNodes(state, f, fa.Tree);

        public Func<Unit, S> FoldBack<S>(Compositions<A> fa, S state, Func<S, A, S> f) => _ =>
            default(FoldCompositions<A>).FoldNodesBack(state, f, fa.Tree);
    }
}
