using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Megaparsec;

public record ParsecT<E, S, T, M, A>(UnParser<E, S, T, M, A> unParser) :
    K<ParsecT<E, S, T, M>, A>
    where S : TokenStream<S, T>
{
    
}

/*
// Testing
public record ParsecT<E, M, A>(UnParser<E, PString, char, M, A> unParser) : 
    ParsecT<E, PString, char, M, A>(unParser);

public record Parsec<A>(UnParser<Error, PString, char, Identity, A> unParser) : 
    ParsecT<Error, PString, char, Identity, A>(unParser);
    */
