using Xunit;

namespace LanguageExt.Tests.TraitTests;

public class ChoiceLawTests
{
    [Fact]
    public void Arr() =>
        ChoiceLaw<Arr>.assert();
    
    [Fact]
    public void HashSet() =>
        ChoiceLaw<HashSet>.assert();
    
    [Fact]
    public void Iterable() =>
        ChoiceLaw<Iterable>.assert();
    
    [Fact]
    public void Lst() =>
        ChoiceLaw<Lst>.assert();
    
    [Fact]
    public void Seq() =>
        ChoiceLaw<Seq>.assert();
    
    [Fact]
    public void EffRT()
    {
        bool eq(K<Eff<Unit>, int> vx, K<Eff<Unit>, int> vy) => 
            vx.Run(unit).Equals(vy.Run(unit));
        
        ChoiceLaw<Eff<Unit>>.assert(eq);
    }
    
    [Fact]
    public void Eff()
    {
        bool eq(K<Eff, int> vx, K<Eff, int> vy) => 
            vx.Run().Equals(vy.Run());
        
        ChoiceLaw<Eff>.assert(eq);
    }
    
    [Fact]
    public void IO()
    {
        bool eq(K<IO, int> vx, K<IO, int> vy) => 
            vx.RunSafe().Equals(vy.RunSafe());
        
        ChoiceLaw<IO>.assert(eq);
    }
    
    [Fact]
    public void Either() => 
        ChoiceLaw<Either<string>>.assert();

    [Fact]
    public void EitherT() => 
        ChoiceLaw<EitherT<string, Identity>>.assert();

    [Fact]
    public void Fin() => 
        ChoiceLaw<Fin>.assert();

    [Fact]
    public void FinT() => 
        ChoiceLaw<FinT<Identity>>.assert();

    [Fact]
    public void Option() => 
        ChoiceLaw<Option>.assert();

    [Fact]
    public void OptionT() => 
        ChoiceLaw<OptionT<Identity>>.assert();

    [Fact]
    public void Try()
    {
        bool eq(K<Try, int> vx, K<Try, int> vy) => 
            vx.Run().Equals(vy.Run());
        
        ChoiceLaw<Try>.assert(eq);
    }
    
    [Fact]
    public void TryT()
    {
        bool eq(K<TryT<Identity>, int> vx, K<TryT<Identity>, int> vy) => 
            vx.Run().Run().Equals(vy.Run().Run());
        
        ChoiceLaw<TryT<Identity>>.assert(eq);
    }
    
    [Fact]
    public void Validation() => 
        ChoiceLaw<Validation<StringM>>.assert();

    [Fact]
    public void ValidationT() => 
        ChoiceLaw<ValidationT<StringM, Identity>>.assert();

    [Fact]
    public void Identity() => 
        ChoiceLaw<Identity>.assert();

    [Fact]
    public void IdentityT() => 
        ChoiceLaw<IdentityT<Identity>>.assert();

    [Fact]
    public void Reader()
    {
        bool eq(K<Reader<string>, int> vx, K<Reader<string>, int> vy) => 
            vx.Run("Hello").Equals(vy.Run("Hello"));
        
        ChoiceLaw<Reader<string>>.assert(eq);
    }
    
    [Fact]
    public void ReaderT()
    {
        bool eq(K<ReaderT<string, Identity>, int> vx, K<ReaderT<string, Identity>, int> vy) => 
            vx.Run("Hello").Run().Equals(vy.Run("Hello").Run());
                 
        ChoiceLaw<ReaderT<string, Identity>>.assert(eq);
    }

    [Fact]
    public void State()
    {
        bool eq(K<State<string>, int> vx, K<State<string>, int> vy) => 
            vx.Run("Hello").Equals(vy.Run("Hello"));
        
        ChoiceLaw<State<string>>.assert(eq);
    }
    
    [Fact]
    public void StateT()
    {
        bool eq(K<StateT<string, Identity>, int> vx, K<StateT<string, Identity>, int> vy) => 
            vx.Run("Hello").Run().Equals(vy.Run("Hello").Run());
                 
        ChoiceLaw<StateT<string, Identity>>.assert(eq);
    }
    
    [Fact]
    public void Writer()
    {
        bool eq(K<Writer<StringM>, int> vx, K<Writer<StringM>, int> vy) => 
            vx.Run("Hello, ").Equals(vy.Run("Hello, "));
        
        ChoiceLaw<Writer<StringM>>.assert(eq);
    }
    
    [Fact]
    public void WriterT()
    {
        bool eq(K<WriterT<StringM, Identity>, int> vx, K<WriterT<StringM, Identity>, int> vy) => 
            vx.Run("Hello, ").Run().Equals(vy.Run("Hello, ").Run());
        
        ChoiceLaw<WriterT<StringM, Identity>>.assert(eq);
    }
}
