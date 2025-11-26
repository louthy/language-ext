using static LanguageExt.Prelude;

namespace LanguageExt.Megaparsec;

public static class ParseError
{
    public static ParseError<T, E> Trivial<T, E>(
        int offset, 
        Option<ErrorItem<T>> unexpected, 
        Set<ErrorItem<T>> expected) =>
        new ParseError<T, E>.Trivial(offset, unexpected, expected);
    
    public static ParseError<T, E> Trivial<T, E>(
        int offset, 
        Option<ErrorItem<T>> unexpected, 
        ErrorItem<T> expected) =>
        new ParseError<T, E>.Trivial(offset, unexpected, Set.singleton(expected));

    public static ParseError<T, E> Trivial<T, E>(
        int offset,
        Option<ErrorItem<T>> unexpected,
        Option<ErrorItem<T>> expected) =>
        new ParseError<T, E>.Trivial(offset, unexpected,
                                     expected.IsSome ? Set.singleton((ErrorItem<T>)expected) : default);

    public static ParseError<T, E> Trivial<T, E>(
        int offset, 
        Option<ErrorItem<T>> unexpected) =>
        new ParseError<T, E>.Trivial(offset, unexpected, default);
    
    public static ParseError<T, E> Fancy<T, E>(int offset, Set<ErrorFancy<E>> errors) => 
        new ParseError<T, E>.Fancy(offset, errors);
    
    public static ParseError<T, E> Fancy<T, E>(int offset, ErrorFancy<E> errors) => 
        new ParseError<T, E>.Fancy(offset, Set.singleton(errors));

    public static ParseError<T, E> mergeError<T, E>(ParseError<T, E> e1, ParseError<T, E> e2)
    {
        return (e1.Offset.CompareTo(e2.Offset)) switch
               {
                   < 0 => e2,
                   0 => (e1, e2) switch
                        {
                            (ParseError<T, E>.Trivial (var s1, var u1, var p1), ParseError<T, E>.Trivial (_, var u2, var p2)) =>
                                Trivial<T, E>(s1, n(u1, u2), p1 + p2),
                            
                            (ParseError<T, E>.Fancy, ParseError<T, E>.Trivial) =>
                                e1,
                            
                            (ParseError<T, E>.Trivial, ParseError<T, E>.Fancy) =>
                                e2,
                            
                            (ParseError<T, E>.Fancy (var s1, var x1), ParseError<T, E>.Fancy(_, var x2) ) =>
                                Fancy<T, E>(s1, x1 + x2),
                            
                            _ => e1
                        },
                   > 0 => e1
               };
        
        // NOTE The logic behind this merging is that since we only combine
        // parse errors that happen at exactly the same position, all the
        // unexpected items will be prefixes of input stream at that position or
        // labels referring to the same thing. Our aim here is to choose the
        // longest prefix (merging with labels and end of input is somewhat
        // arbitrary, but is necessary because otherwise we can't make
        // ParseError lawful Monoid and have nice parse errors at the same
        // time).
        static Option<ErrorItem<T>> n(Option<ErrorItem<T>> mx, Option<ErrorItem<T>> my) =>
            (mx, my) switch
            {
                ({ IsNone: true }, { IsNone: true }) => None,
                ({ IsSome: true }, { IsNone: true }) => mx,
                ({ IsNone: true }, { IsSome: true }) => my,
                (_, _)                               => Some((ErrorItem<T>)mx > (ErrorItem<T>)my 
                                                                 ? (ErrorItem<T>)mx 
                                                                 : (ErrorItem<T>)my)
            };
    }

}
