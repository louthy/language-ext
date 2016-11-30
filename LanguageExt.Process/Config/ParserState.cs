using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Config
{
    public class ParserState
    {
        public readonly Map<string, ClusterToken> Clusters;
        public readonly Map<string, ValueToken> Locals;

        public ParserState(
            Map<string, ClusterToken> clusters,
            Map<string, ValueToken> locals
            )
        {
            Clusters = clusters;
            Locals = locals;
        }

        public ParserState AddCluster(string alias, ClusterToken cluster) =>
            new ParserState(Map.addOrUpdate(Clusters, alias, cluster), Locals);

        public ParserState SetClusters(Map<string, ClusterToken> clusters) =>
            new ParserState(clusters, Locals);

        public bool LocalExists(string name) =>
            Locals.ContainsKey(name);

        public ParserState AddLocal(string name, ValueToken value) =>
            new ParserState(Clusters, Locals.Add(name,value));

        public Option<ValueToken> Local(string name) =>
            Locals.Find(name);

        public static readonly ParserState Empty = new ParserState(Map.empty<string,ClusterToken>(), Map.empty<string, ValueToken>());
    }
}
