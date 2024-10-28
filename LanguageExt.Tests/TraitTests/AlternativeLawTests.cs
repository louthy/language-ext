using System;
using LanguageExt.Common;
using Xunit;

namespace LanguageExt.Tests.TraitTests;

public class AlternativeLawTests
{
    [Fact]
    public void Arr() =>
        AlternativeLaw<Arr>.assert();
    
    [Fact]
    public void HashSet() =>
        AlternativeLaw<HashSet>.assert();
    
    [Fact]
    public void Iterable() =>
        AlternativeLaw<Iterable>.assert();
    
    [Fact]
    public void Lst() =>
        AlternativeLaw<Lst>.assert();
    
    [Fact]
    public void Seq() =>
        AlternativeLaw<Seq>.assert();
    
    /*
    [Fact]
    public void EffRT()
    {
        bool eq(K<Eff<Unit>, int> vx, K<Eff<Unit>, int> vy) => 
            vx.Run(unit).Equals(vy.Run(unit));
        
        AlternativeLaw<Eff<Unit>>.assert(eq);
    }
    
    [Fact]
    public void Eff()
    {
        bool eq(K<Eff, int> vx, K<Eff, int> vy) => 
            vx.Run().Equals(vy.Run());
        
        AlternativeLaw<Eff>.assert(eq);
    }
    
    [Fact]
    public void IO()
    {
        bool eq(K<IO, int> vx, K<IO, int> vy) => 
            vx.RunSafe().Equals(vy.RunSafe());
        
        AlternativeLaw<IO>.assert(eq);
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
        
        AlternativeLaw<StreamT<Identity>>.assert(eq);
    }
    
    [Fact]
    public void Either() => 
        AlternativeLaw<Either<string>>.assert();

    [Fact]
    public void EitherT() => 
        AlternativeLaw<EitherT<string, Identity>>.assert();

    [Fact]
    public void Fin() => 
        AlternativeLaw<Fin>.assert();

    [Fact]
    public void FinT() => 
        AlternativeLaw<FinT<Identity>>.assert();

    [Fact]
    public void Option() => 
        AlternativeLaw<Option>.assert();

    [Fact]
    public void OptionT() => 
        AlternativeLaw<OptionT<Identity>>.assert();

    [Fact]
    public void Try()
    {
        bool eq(K<Try, int> vx, K<Try, int> vy) => 
            vx.Run().Equals(vy.Run());
        
        AlternativeLaw<Try>.assert(eq);
    }
    
    [Fact]
    public void TryT()
    {
        bool eq(K<TryT<Identity>, int> vx, K<TryT<Identity>, int> vy) => 
            vx.Run().Run().Equals(vy.Run().Run());
        
        AlternativeLaw<TryT<Identity>>.assert(eq);
    }
    
    [Fact]
    public void Validation() => 
        AlternativeLaw<Validation<StringM>>.assert();

    [Fact]
    public void ValidationT() => 
        AlternativeLaw<ValidationT<StringM, Identity>>.assert();

    [Fact]
    public void Identity() => 
        AlternativeLaw<Identity>.assert();

    [Fact]
    public void IdentityT() => 
        AlternativeLaw<IdentityT<Identity>>.assert();

    [Fact]
    public void Reader()
    {
        bool eq(K<Reader<string>, int> vx, K<Reader<string>, int> vy) => 
            vx.Run("Hello").Equals(vy.Run("Hello"));
        
        AlternativeLaw<Reader<string>>.assert(eq);
    }
    
    [Fact]
    public void ReaderT()
    {
        bool eq(K<ReaderT<string, Identity>, int> vx, K<ReaderT<string, Identity>, int> vy) => 
            vx.Run("Hello").Run().Equals(vy.Run("Hello").Run());
                 
        AlternativeLaw<ReaderT<string, Identity>>.assert(eq);
    }

    [Fact]
    public void State()
    {
        bool eq(K<State<string>, int> vx, K<State<string>, int> vy) => 
            vx.Run("Hello").Equals(vy.Run("Hello"));
        
        AlternativeLaw<State<string>>.assert(eq);
    }
    
    [Fact]
    public void StateT()
    {
        bool eq(K<StateT<string, Identity>, int> vx, K<StateT<string, Identity>, int> vy) => 
            vx.Run("Hello").Run().Equals(vy.Run("Hello").Run());
                 
        AlternativeLaw<StateT<string, Identity>>.assert(eq);
    }
    
    [Fact]
    public void Writer()
    {
        bool eq(K<Writer<StringM>, int> vx, K<Writer<StringM>, int> vy) => 
            vx.Run("Hello, ").Equals(vy.Run("Hello, "));
        
        AlternativeLaw<Writer<StringM>>.assert(eq);
    }
    
    [Fact]
    public void WriterT()
    {
        bool eq(K<WriterT<StringM, Identity>, int> vx, K<WriterT<StringM, Identity>, int> vy) => 
            vx.Run("Hello, ").Run().Equals(vy.Run("Hello, ").Run());
        
        AlternativeLaw<WriterT<StringM, Identity>>.assert(eq);
    }*/
}