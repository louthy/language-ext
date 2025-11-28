using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Megaparsec;

public static partial class ModuleT<MP, E, S, T, M>
    where MP : MonadParsecT<MP, E, S, T, M>
    where S : TokenStream<S, T>
    where M : Monad<M>
{
    // -- | @'space' sc lineComment blockComment@ produces a parser that can parse
    // -- white space in general. It's expected that you create such a parser once
    // -- and pass it to other functions in this module as needed (when you see
    // -- @spaceConsumer@ in documentation, usually it means that something like
    // -- 'space' is expected there).
    // --
    // -- @sc@ is used to parse blocks of space characters. You can use
    // -- 'Text.Megaparsec.Char.space1' from "Text.Megaparsec.Char" for this
    // -- purpose as well as your own parser (if you don't want to automatically
    // -- consume newlines, for example). Make sure that the parser does not
    // -- succeed on the empty input though. In an earlier version of the library
    // -- 'Text.Megaparsec.Char.spaceChar' was recommended, but now parsers based
    // -- on 'takeWhile1P' are preferred because of their speed.
    // --
    // -- @lineComment@ is used to parse line comments. You can use
    // -- @skipLineComment@ if you don't need anything special.
    // --
    // -- @blockComment@ is used to parse block (multi-line) comments. You can use
    // -- @skipBlockComment@ or @skipBlockCommentNested@ if you don't need anything
    // -- special.
    // --
    // -- If you don't want to allow a kind of comment, simply pass 'empty' which
    // -- will fail instantly when parsing of that sort of comment is attempted and
    // -- 'space' will just move on or finish depending on whether there is more
    // -- white space for it to consume.
    // space ::
    //   (MonadParsec e s m) =>
    //   -- | A parser for space characters which does not accept empty
    //   -- input (e.g. 'Text.Megaparsec.Char.space1')
    //   m () ->
    //   -- | A parser for a line comment (e.g. 'skipLineComment')
    //   m () ->
    //   -- | A parser for a block comment (e.g. 'skipBlockComment')
    //   m () ->
    //   m ()
    // space sp line block =
    //   skipMany $
    //     choice
    //       [hidden sp, hidden line, hidden block]
    
}
