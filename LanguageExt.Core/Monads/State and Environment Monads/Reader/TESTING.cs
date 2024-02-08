using System;
using LanguageExt.Common;
using LanguageExt.HKT;
using static LanguageExt.Prelude;

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

        var r1 = m3.Run("Hello");
        
        var m4 = from x in m1
                 from y in m2
                 from z in Maybe<int>.Nothing
                 from e in MReaderT<string, Maybe>.Ask
                 select $"{e}: {x + y + z}";

        var r3 = m3.Run("Hello");
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
    
    public static void Test3()
    {
        var m1 = ReaderT<string, MEither<string>, int>.Lift(Right<string, int>(123));
        var m2 = ReaderT<string, MEither<string>, int>.Lift(Right<string, int>(123));
        
        var m3 = from x in m1
                 from y in m2
                 from z in Right<string, int>(100)
                 from e in MReaderT<string, MEither<string>>.Ask
                 select $"{e}: {x + y + z}";

        var r1 = m3.Run("Hello");
        
        var m4 = from x in m1
                 from y in m2
                 from z in Left<string, int>("fail")
                 from e in MReaderT<string, MEither<string>>.Ask
                 select $"{e}: {x + y + z}";

        var r3 = m3.Run("Hello");
    }
}

public class Maybe : Monad<Maybe>
{
    public static KStar<Maybe, A> Pure<A>(A value) => 
        Maybe<A>.Just(value);

    public static KStar<Maybe, B> Bind<A, B>(KStar<Maybe, A> ma, Transducer<A, KStar<Maybe, B>> f) => 
        ma.As().Bind(f.Map(mb => mb.As()));
}

public record Maybe<A>(Transducer<Unit, A> M) : KStar<Maybe, A>
{
    public Transducer<Unit, A> Morphism { get; } = M;

    public static Maybe<A> Just(A value) =>
        new(Transducer.constant<Unit, A>(value));
    
    public static readonly Maybe<A> Nothing = 
        new(Transducer.fail<Unit, A>(Errors.None));

    public Maybe<B> Bind<B>(Func<A, Maybe<B>> f) =>
        new(M.Bind(a => f(a).Morphism));

    public Maybe<B> Bind<B>(Transducer<A, Maybe<B>> f) =>
        new(Transducer.compose(M, f.Map(b => b.Morphism)).Flatten());
}

public static class MaybeExt
{
    public static Maybe<A> As<A>(this KStar<Maybe, A> ma) =>
        (Maybe<A>)ma;
}
