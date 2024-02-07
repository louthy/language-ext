using LanguageExt.Common;
using LanguageExt.HKT;

namespace LanguageExt;

public static class Testing
{
    public static void Test1()
    {
        var m1 = ReaderT<string, Maybe, int>.Lift(Maybe<int>.Just(123));
        var m2 = ReaderT<string, Maybe, int>.Lift(Maybe<int>.Just(123));
        
        var m3 = from x in m1
                 from y in m2
                 from z in Maybe<int>.Just(100)
                 from e in MReaderT<string, Maybe>.Ask
                 select $"{e}: {x + y + z}";

        var r1 = m3.RunT("Hello");
        
        var m4 = from x in m1
                 from y in m2
                 from z in Maybe<int>.Nothing
                 from e in MReaderT<string, Maybe>.Ask
                 select $"{e}: {x + y + z}";

        var r3 = m3.RunT("Hello");
    }
    
    public static void Test2()
    {
        var m1 = Reader<string, int>.Pure(123);
        var m2 = Reader<string, int>.Pure(123);
        
        var m3 = from x in m1
                 from y in m2
                 from e in MReaderT<string, MIdentity>.Ask
                 select $"{e}: {x + y}";
        
        var m4 = from x in m1
                 from y in m2
                 from e in MReaderT<string, MIdentity>.Ask
                 select $"{e}: {x + y}";
    }
}

public class Maybe : Monad<Maybe>
{
    public static KStar<Maybe, A> Lift<A>(Transducer<Unit, A> f) => 
        new Maybe<A>(f);
}

public record Maybe<A>(Transducer<Unit, A> M) : KStar<Maybe, A>
{
    public Transducer<Unit, A> Morphism { get; } = M;

    public static Maybe<A> Just(A value) =>
        new(Transducer.constant<Unit, A>(value));
    
    public static readonly Maybe<A> Nothing = 
        new(Transducer.fail<Unit, A>(Errors.None));
}
