#pragma warning disable LX_StreamT

using Xunit;

namespace LanguageExt.Tests.TraitTests;

public class MonadLawsTest
{
    [Fact]
    public void Arr() =>
        MonadLaw<Arr>.assert();
    
    [Fact]
    public void HashSet() =>
        MonadLaw<HashSet>.assert();
    
    [Fact]
    public void Iterable() =>
        MonadLaw<Iterable>.assert();
    
    [Fact]
    public void Lst() =>
        MonadLaw<Lst>.assert();
    
    [Fact]
    public void Seq() =>
        MonadLaw<Seq>.assert();
    
    [Fact]
    public void EffRT()
    {
        bool eq(K<Eff<Unit>, int> vx, K<Eff<Unit>, int> vy) => 
            vx.Run(unit).Equals(vy.Run(unit));
        
        MonadLaw<Eff<Unit>>.assert(eq);
    }
    
    [Fact]
    public void Eff()
    {
        bool eq(K<Eff, int> vx, K<Eff, int> vy) => 
            vx.Run().Equals(vy.Run());
        
        MonadLaw<Eff>.assert(eq);
    }
    
    [Fact]
    public void IO()
    {
        bool eq(K<IO, int> vx, K<IO, int> vy) => 
            vx.RunSafe().Equals(vy.RunSafe());
        
        MonadLaw<IO>.assert(eq);
    }
    
    [Fact]
    public void StreamT()
    {
        // TODO: Restore
        
        /*
        static Seq<int> toSeq(StreamT<Identity, int> s) =>
            s.Head().Run()
             .Match(Some: x => x.Cons(toSeq(s.Tail())),
                    None: []);

        static bool eq(K<StreamT<Identity>, int> vx, K<StreamT<Identity>, int> vy) =>
            toSeq(vx.As()) == toSeq(vy.As());

        MonadLaw<StreamT<Identity>>.assert(eq);
    */
    }
    
    [Fact]
    public void Either() => 
        MonadLaw<Either<string>>.assert();

    [Fact]
    public void EitherT() => 
        MonadLaw<EitherT<string, Identity>>.assert();

    [Fact]
    public void Fin() => 
        MonadLaw<Fin>.assert();

    [Fact]
    public void FinT() => 
        MonadLaw<FinT<Identity>>.assert();

    [Fact]
    public void Option() => 
        MonadLaw<Option>.assert();

    [Fact]
    public void OptionT() => 
        MonadLaw<OptionT<Identity>>.assert();

    [Fact]
    public void Try()
    {
        bool eq(K<Try, int> vx, K<Try, int> vy) => 
            vx.Run().Equals(vy.Run());
        
        MonadLaw<Try>.assert(eq);
    }
    
    [Fact]
    public void TryT()
    {
        bool eq(K<TryT<Identity>, int> vx, K<TryT<Identity>, int> vy) => 
            vx.Run().Run().Equals(vy.Run().Run());
        
        MonadLaw<TryT<Identity>>.assert(eq);
    }
    
    [Fact]
    public void Validation() => 
        MonadLaw<Validation<StringM>>.assert();

    [Fact]
    public void ValidationT() => 
        MonadLaw<ValidationT<StringM, Identity>>.assert();

    [Fact]
    public void Identity() => 
        MonadLaw<Identity>.assert();

    [Fact]
    public void IdentityT() => 
        MonadLaw<IdentityT<Identity>>.assert();

    [Fact]
    public void Reader()
    {
        bool eq(K<Reader<string>, int> vx, K<Reader<string>, int> vy) => 
            vx.Run("Hello").Equals(vy.Run("Hello"));
        
        MonadLaw<Reader<string>>.assert(eq);
    }
    
    [Fact]
    public void ReaderT()
    {
        bool eq(K<ReaderT<string, Identity>, int> vx, K<ReaderT<string, Identity>, int> vy) => 
            vx.Run("Hello").Run().Equals(vy.Run("Hello").Run());
                 
        MonadLaw<ReaderT<string, Identity>>.assert(eq);
    }

    [Fact]
    public void State()
    {
        bool eq(K<State<string>, int> vx, K<State<string>, int> vy) => 
            vx.Run("Hello").Equals(vy.Run("Hello"));
        
        MonadLaw<State<string>>.assert(eq);
    }
    
    [Fact]
    public void StateT()
    {
        bool eq(K<StateT<string, Identity>, int> vx, K<StateT<string, Identity>, int> vy) => 
            vx.Run("Hello").Run().Equals(vy.Run("Hello").Run());
                 
        MonadLaw<StateT<string, Identity>>.assert(eq);
    }
    
    [Fact]
    public void Writer()
    {
        bool eq(K<Writer<StringM>, int> vx, K<Writer<StringM>, int> vy) => 
            vx.Run("Hello, ").Equals(vy.Run("Hello, "));
        
        MonadLaw<Writer<StringM>>.assert(eq);
    }
    
    [Fact]
    public void WriterT()
    {
        bool eq(K<WriterT<StringM, Identity>, int> vx, K<WriterT<StringM, Identity>, int> vy) => 
            vx.Run("Hello, ").Run().Equals(vy.Run("Hello, ").Run());
        
        MonadLaw<WriterT<StringM, Identity>>.assert(eq);
    }    
}
