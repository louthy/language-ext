/*
 WIP
 
using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
//https://hackage.haskell.org/package/containers-0.2.0.1/docs/src/Data-Graph.html#topSort
    
    /// <summary>
    /// Strongly connected component
    /// </summary>
    public abstract record SCC<VERTEX>
    {
        /// <summary>
        /// Return the vertices of a strongly connected component
        /// </summary>
        public abstract Seq<VERTEX> Flatten();
    }

    /// <summary>
    /// A single vertex that is not in any cycle
    /// </summary>
    public record AcyclicSCC<VERTEX>(VERTEX vertex) : SCC<VERTEX>
    {
        public override Seq<VERTEX> Flatten() =>
            Seq(vertex);
    }

    /// <summary>
    /// A maximal set of mutually reachable vertices.
    /// </summary>
    public record CyclicSCC<VERTEX>(Seq<VERTEX> vertices) : SCC<VERTEX>
    {
        public override Seq<VERTEX> Flatten() =>
            vertices;
    }

    /// <summary>
    /// The graph: a list of nodes uniquely identified by keys,
    /// with a list of keys of nodes this node has edges to.
    /// </summary>
    public record EdgeNodes<Node, Key>(Seq<(Node Node, Key Key, Seq<Key> Edges)> Value)
    {
        public bool IsEmpty =>
            Value.IsEmpty;
    }

    /// <summary>
    /// The graph: a list of nodes uniquely identified by keys,
    /// with a list of keys of nodes this node has edges to.
    /// </summary>
    public record EdgeNodeSCC<Node, Key>(SCC<(Node Node, Key Key, Seq<Key> Edges)> Value);

    public static class SCC
    {
        /// <summary>
        /// Returns the vertices of a list of strongly connected components.
        /// </summary>
        public static Seq<A> flatten<A>(this Seq<SCC<A>> sccs) =>
            sccs.Bind(static scc => scc.Flatten());

        /// <summary>
        /// The strongly connected components of a directed graph, topologically sorted.
        /// </summary>
        public static Seq<SCC<Node>> stronglyConnComp<OrdKey, Node, Key>(EdgeNodes<Node, Key> edges)
            where OrdKey : struct, Ord<Key>
        {
            return stronglyConnCompR<OrdKey, Node, Key>(edges).Map(getNode);

            SCC<Node> getNode(EdgeNodeSCC<Node, Key> scc) =>
                scc.Value switch
                {
                    AcyclicSCC<(Node Node, Key Key, Seq<Key> Edges)> v      => new AcyclicSCC<Node>(v.vertex.Node),
                    CyclicSCC<(Node Node, Key Key, Seq<Key> Edges)> triples => new CyclicSCC<Node>(triples.vertices.Map(static t => t.Node)),
                    _                                                       => throw new NotSupportedException()
                };
        }

        /// <summary>
        /// The strongly connected components of a directed graph, topologically
        /// sorted.  The function is the same as 'stronglyConnComp', except that
        /// all the information about each node retained.
        /// 
        /// This interface is used when you expect to apply 'SCC' to
        /// (some of) the result of 'SCC', so you don't want to lose the
        /// dependency information.
        /// </summary>
        public static Seq<EdgeNodeSCC<Node, Key>> stronglyConnCompR<OrdKey, Node, Key>(EdgeNodes<Node, Key> edges)
            where OrdKey : struct, Ord<Key>
        {
            if (edges.IsEmpty) return Empty;

            var (graph, vertex_fn, _) = graphFromEdges(edges);
            var forest = scc(graph);
            
            // TODO

        }

    }
}
*/
