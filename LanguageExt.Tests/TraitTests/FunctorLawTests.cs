#pragma warning disable LX_StreamT

using Xunit;
using LanguageExt.Common;
namespace LanguageExt.Tests.TraitTests;

public class FunctorLawTests
{
    [Fact]
    public void Arr()
    {
        Arr<int> fa = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        FunctorLaw<Arr>.assert(fa);
    }
    
    [Fact]
    public void HashMap()
    {
        HashMap<string, int> fa = [("one", 1), ("two", 2), ("three", 3), ("four", 4), ("five", 5)];
        FunctorLaw<HashMap<string>>.assert(fa);
    }
    
    [Fact]
    public void HashSet()
    {
        HashSet<int> fa = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        FunctorLaw<HashSet>.assert(fa);
    }
    
    [Fact]
    public void Iterable()
    {
        Iterable<int> fa = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        FunctorLaw<Iterable>.assert(fa);
    }
    
    [Fact]
    public void Lst()
    {
        Lst<int> fa = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        FunctorLaw<Lst>.assert(fa);
    }
    
    [Fact]
    public void Map()
    {
        Map<string, int> fa = [("one", 1), ("two", 2), ("three", 3), ("four", 4), ("five", 5)];
        FunctorLaw<Map<string>>.assert(fa);
    }
    
    [Fact]
    public void Seq()
    {
        Seq<int> fa = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
        FunctorLaw<Seq>.assert(fa);
    }    
    
    [Fact]
    public void EffRT()
    {
        bool eq(K<Eff<Unit>, int> vx, K<Eff<Unit>, int> vy) => 
            vx.Run(unit).Equals(vy.Run(unit));
        
        var ma = Eff<Unit, int>.Pure(1);
        var mx = Eff<Unit, int>.Fail(Errors.Closed);
        FunctorLaw<Eff<Unit>>.assert(ma, eq);
        FunctorLaw<Eff<Unit>>.assert(mx, eq);
    }
    
    [Fact]
    public void Eff()
    {
        bool eq(K<Eff, int> vx, K<Eff, int> vy) => 
            vx.Run().Equals(vy.Run());
        
        var ma = Eff<int>.Pure(1);
        var mx = Eff<int>.Fail(Errors.Closed);
        FunctorLaw<Eff>.assert(ma, eq);
        FunctorLaw<Eff>.assert(mx, eq);
    }
    
    [Fact]
    public void IO()
    {
        bool eq(K<IO, int> vx, K<IO, int> vy) => 
            vx.RunSafe().Equals(vy.RunSafe());
        
        var ma = IO<int>.Pure(1);
        var mx = IO<int>.Fail(Errors.Closed);
        FunctorLaw<IO>.assert(ma, eq);
        FunctorLaw<IO>.assert(mx, eq);
    }
    
    [Fact]
    public void StreamT()
    {
        static Seq<int> toSeq(StreamT<Identity, int> s) =>
            s.Head().Run()
             .Match(Some: x => x.Cons(toSeq(s.Tail())),
                    None: []);

        static bool eq(K<StreamT<Identity>, int> vx, K<StreamT<Identity>, int> vy) =>
            toSeq(vx.As()) == toSeq(vy.As());
        
        var ma = StreamT<Identity, int>.Pure(1);
        var mb = StreamT<Identity, int>.Lift([1, 2, 3, 4, 5 , 6, 7, 8, 9, 10]);
        
        FunctorLaw<StreamT<Identity>>.assert(ma, eq);
        FunctorLaw<StreamT<Identity>>.assert(mb, eq);
    }
    
    [Fact]
    public void Either()
    {
        var fa = Either<string, int>.Right(1);
        var fx = Either<string, int>.Left("failed");
        FunctorLaw<Either<string>>.assert(fa);
        FunctorLaw<Either<string>>.assert(fx);
    }
    
    [Fact]
    public void EitherT()
    {
        var fa = EitherT<string, Identity, int>.Right(1);
        var fx = EitherT<string, Identity, int>.Left("failed");
        FunctorLaw<EitherT<string, Identity>>.assert(fa);
        FunctorLaw<EitherT<string, Identity>>.assert(fx);
    }

    [Fact]
    public void Fin()
    {
        var fa = Fin<int>.Succ(1);
        var fx = Fin<int>.Fail(Errors.TimedOut);
        FunctorLaw<Fin>.assert(fa);
        FunctorLaw<Fin>.assert(fx);
    }
    
    [Fact]
    public void FinT()
    {
        var fa = FinT<Identity, int>.Succ(1);
        var fx = FinT<Identity, int>.Fail(Errors.TimedOut);
        FunctorLaw<FinT<Identity>>.assert(fa);
        FunctorLaw<FinT<Identity>>.assert(fx);
    }
    
    [Fact]
    public void Option()
    {
        var fa = Option<int>.Some(1);
        var fx = Option<int>.None;
        FunctorLaw<Option>.assert(fa);
        FunctorLaw<Option>.assert(fx);
    }
    
    [Fact]
    public void OptionT()
    {
        var fa = OptionT<Identity, int>.Some(1);
        var fx = OptionT<Identity, int>.None;
        FunctorLaw<OptionT<Identity>>.assert(fa);
        FunctorLaw<OptionT<Identity>>.assert(fx);
    }
    
    [Fact]
    public void Try()
    {
        bool eq(K<Try, int> vx, K<Try, int> vy) => 
            vx.Run().Equals(vy.Run());
        
        var fa = Try<int>.Succ(1);
        var fx = Try<int>.Fail(Errors.EndOfStream);
        FunctorLaw<Try>.assert(fa, eq);
        FunctorLaw<Try>.assert(fx, eq);
    }
    
    [Fact]
    public void TryT()
    {
        bool eq(K<TryT<Identity>, int> vx, K<TryT<Identity>, int> vy) => 
            vx.Run().Run().Equals(vy.Run().Run());
        
        var fa = TryT<Identity, int>.Succ(1);
        var fx = TryT<Identity, int>.Fail(Errors.EndOfStream);
        FunctorLaw<TryT<Identity>>.assert(fa, eq);
        FunctorLaw<TryT<Identity>>.assert(fx, eq);
    }
    
    [Fact]
    public void Validation()
    {
        var fa = Validation<StringM, int>.Success(1);
        var fx = Validation<StringM, int>.Fail("failed");
        FunctorLaw<Validation<StringM>>.assert(fa);
        FunctorLaw<Validation<StringM>>.assert(fx);
    }
    
    [Fact]
    public void ValidationT()
    {
        var fa = ValidationT<StringM, Identity, int>.Success(1);
        var fx = ValidationT<StringM, Identity, int>.Fail("failed");
        FunctorLaw<ValidationT<StringM, Identity>>.assert(fa);
        FunctorLaw<ValidationT<StringM, Identity>>.assert(fx);
    }
    
    [Fact]
    public void Identity()
    {
        var fa = LanguageExt.Identity.Pure(1);
        FunctorLaw<Identity>.assert(fa);
    }
    
    [Fact]
    public void IdentityT()
    {
        var fa = IdentityT<Identity, int>.Pure(1);
        FunctorLaw<IdentityT<Identity>>.assert(fa);
    }
    
    [Fact]
    public void Reader()
    {
        bool eq(K<Reader<string>, int> vx, K<Reader<string>, int> vy) => 
            vx.Run("Hello").Equals(vy.Run("Hello"));
        
        var fa = from e in Readable.ask<Reader<string>, string>()   
                 from v in Reader<string, int>.Pure(1)
                 select e.Length + v;
        
        FunctorLaw<Reader<string>>.assert(fa, eq);
    }
    
    [Fact]
    public void ReaderT()
    {
        bool eq(K<ReaderT<string, Identity>, int> vx, K<ReaderT<string, Identity>, int> vy) => 
            vx.Run("Hello").Run().Equals(vy.Run("Hello").Run());
        
        var fa = from e in Readable.ask<ReaderT<string, Identity>, string>()  
                 from v in ReaderT<string, Identity, int>.Pure(1)
                 select e.Length + v;
                 
        FunctorLaw<ReaderT<string, Identity>>.assert(fa, eq);
    }
    
    [Fact]
    public void State()
    {
        bool eq(K<State<string>, int> vx, K<State<string>, int> vy) => 
            vx.Run("Hello").Equals(vy.Run("Hello"));
        
        var fa = from e in Stateful.get<State<string>, string>()  
                 from v in State<string, int>.Pure(1)
                 select e.Length + v;
        
        FunctorLaw<State<string>>.assert(fa, eq);
    }
    
    [Fact]
    public void StateT()
    {
        bool eq(K<StateT<string, Identity>, int> vx, K<StateT<string, Identity>, int> vy) => 
            vx.Run("Hello").Run().Equals(vy.Run("Hello").Run());
        
        var fa = from e in Stateful.get<StateT<string, Identity>, string>()  
                 from v in StateT<string, Identity, int>.Pure(1)
                 select e.Length + v;
                 
        FunctorLaw<StateT<string, Identity>>.assert(fa, eq);
    }
    
    [Fact]
    public void Writer()
    {
        bool eq(K<Writer<StringM>, int> vx, K<Writer<StringM>, int> vy) => 
            vx.Run("Hello, ").Equals(vy.Run("Hello, "));
        
        var fa = from _ in Writable.tell<Writer<StringM>, StringM>("World")  
                 from v in Writer<StringM, int>.Pure(1)
                 select v;
        
        FunctorLaw<Writer<StringM>>.assert(fa, eq);
    }
    
    [Fact]
    public void WriterT()
    {
        bool eq(K<WriterT<StringM, Identity>, int> vx, K<WriterT<StringM, Identity>, int> vy) => 
            vx.Run("Hello, ").Run().Equals(vy.Run("Hello, ").Run());
        
        var fa = from _ in Writable.tell<WriterT<StringM, Identity>, StringM>("World")  
                 from v in WriterT<StringM, Identity, int>.Pure(1)
                 select v;
                 
        FunctorLaw<WriterT<StringM, Identity>>.assert(fa, eq);
    }      
}
