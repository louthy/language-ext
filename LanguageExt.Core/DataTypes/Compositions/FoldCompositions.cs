using static LanguageExt.Prelude;
using LanguageExt.Traits;
using System;

namespace LanguageExt.ClassInstances;

public struct FoldCompositions<A>
    where A : Monoid<A>
{
    static S FoldNode<S>(Func<S, A, S> f, S state, Compositions<A>.Node node)
    {
        if (node.Children.IsNone) return f(state, node.Value);
        var (l, r) = ((Compositions<A>.Node, Compositions<A>.Node))node.Children;
        state      = FoldNode(f, state, l);
        state      = FoldNode(f, state, r);
        return state;
    }

    static S FoldNodes<S>(Func<S, A, S> f, S state, Seq<Compositions<A>.Node> nodes) =>
        nodes.Fold((s, n) => FoldNode(f, s, n), state);

    static S FoldNodeBack<S>(Func<S, A, S> f, S state, Compositions<A>.Node node)
    {
        if (node.Children.IsNone) return f(state, node.Value);
        var (l, r) = ((Compositions<A>.Node, Compositions<A>.Node))node.Children;
        state      = FoldNode(f, state, r);
        state      = FoldNode(f, state, l);
        return state;
    }

    static S FoldNodesBack<S>(Func<S, A, S> f, S state, Seq<Compositions<A>.Node> nodes) =>
        nodes.FoldBack((s, n) => FoldNode(f, s, n), state);

    internal static Seq<B> FoldMap<B>(Func<A, B> f, Seq<Compositions<A>.Node> nodes) =>
        FoldNodes((s, n) => f(n).Cons(s), Seq<B>(), nodes);

    internal static Seq<B> FoldMapBack<B>(Func<A, B> f, Seq<Compositions<A>.Node> nodes) =>
        FoldNodesBack((s, n) => f(n).Cons(s), Seq<B>(), nodes);

    public static Func<Unit, int> Count(Compositions<A> fa) => _ =>
        FoldNodes((s, _) => s + 1, 0, fa.Tree);

    public static Func<Unit, S> Fold<S>(Func<S, A, S> f, S state, Compositions<A> fa) => _ =>
        FoldNodes(f, state, fa.Tree);

    public static Func<Unit, S> FoldBack<S>(Func<S, A, S> f, S state, Compositions<A> fa) => _ =>
        FoldNodesBack(f, state, fa.Tree);
}
