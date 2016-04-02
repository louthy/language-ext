using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Config
{
    public class ParserState
    {
        public readonly Option<ClusterToken> Cluster;
        public readonly Map<string, ValueToken> Locals;

        public ParserState(
            Option<ClusterToken> cluster,
            Map<string, ValueToken> locals
            )
        {
            Cluster = cluster;
            Locals = locals;
        }

        public ParserState SetCluster(ClusterToken cluster) =>
            new ParserState(cluster, Locals);

        public ParserState SetCluster(Option<ClusterToken> cluster) =>
            new ParserState(cluster, Locals);

        public bool LocalExists(string name) =>
            Locals.ContainsKey(name);

        public ParserState AddLocal(string name, ValueToken value) =>
            new ParserState(Cluster, Locals.Add(name,value));

        public Option<ValueToken> Local(string name) =>
            Locals.Find(name);

        public static readonly ParserState Empty = new ParserState(None, Map.empty<string, ValueToken>());
    }
}
