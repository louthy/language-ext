using System;

namespace LanguageExt.Parsec
{
    public static class IndentIO
    {
        /// <summary>
        /// Parses only when indented past the level of the reference
        /// </summary>
        /// <remarks>
        /// You must have provided a TokenPos to the initial PString&lt;TOKEN&gt; that gives accurate
        /// column and line values for this function to work.
        /// </remarks>
        public static Parser<TOKEN, A> indented<TOKEN, A>(int offset, Parser<TOKEN, A> p) =>
            inp =>
            {
                var pos   = inp.Pos;
                var col   = pos.Column + offset;
                var ln    = pos.Line;
                var toks  = inp.Value;
                var ix    = inp.Index;
                var start = inp.Index;
                var tpos  = inp.TokenPos;

                for (; ix < inp.EndIndex; ix++)
                {
                    var npos = tpos(toks[ix]);
                    if (npos.Line != ln && npos.Column < col)
                    {
                        break;
                    }
                }

                return parseBlock(p, start, ix, inp);                
            };

        /// <summary>
        /// Parses only when indented zero or more characters past the level of the reference
        /// </summary>
        /// <remarks>
        /// You must have provided a TokenPos to the initial PString&lt;TOKEN&gt; that gives accurate
        /// column and line values for this function to work.
        /// </remarks>
        public static Parser<TOKEN, A> indented<TOKEN, A>(Parser<TOKEN, A> p) =>
            indented(0, p);

        /// <summary>
        /// Parses only when indented one or more characters past the level of the reference
        /// </summary>
        /// <remarks>
        /// You must have provided a TokenPos to the initial PString&lt;TOKEN&gt; that gives accurate
        /// column and line values for this function to work.
        /// </remarks>
        public static Parser<TOKEN, A> indented1<TOKEN, A>(Parser<TOKEN, A> p) =>
            indented(1, p);

        /// <summary>
        /// Parses only when indented two or more characters past the level of the reference
        /// </summary>
        /// <remarks>
        /// You must have provided a TokenPos to the initial PString&lt;TOKEN&gt; that gives accurate
        /// column and line values for this function to work.
        /// </remarks>
        public static Parser<TOKEN, A> indented2<TOKEN, A>(Parser<TOKEN, A> p) =>
            indented(2, p);

        /// <summary>
        /// Parses only when indented four or more characters past the level of the reference
        /// </summary>
        /// <remarks>
        /// You must have provided a TokenPos to the initial PString&lt;TOKEN&gt; that gives accurate
        /// column and line values for this function to work.
        /// </remarks>
        public static Parser<TOKEN, A> indented4<TOKEN, A>(Parser<TOKEN, A> p) =>
            indented(4, p);

        /// <summary>
        /// Sets a new context for the parser p which represents a span of the input tokens
        /// </summary>
        /// <remarks>
        /// The parse fails if it doesn't consume all tokens in the block
        /// </remarks>
        static ParserResult<TOKEN, A> parseBlock<TOKEN, A>(Parser<TOKEN, A> p, int start, int end, PString<TOKEN> inp)
        {
            var pstr = new PString<TOKEN>(inp.Value, start, end, inp.UserState, inp.TokenPos);
            var pres = p.Parse(pstr);
            
            return new ParserResult<TOKEN, A>(
                      pres.Tag,
                      new Reply<TOKEN, A>(
                          pres.Reply.Tag,
                          pres.Reply.Result,
                          new PString<TOKEN>(inp.Value, pres.Reply.State.Index, inp.EndIndex, pres.Reply.State.UserState, pres.Reply.State.TokenPos),
                          pres.Reply.Error));
        }  
    }
}
