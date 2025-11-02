using Xunit;
namespace LanguageExt.Tests.TraitTests;

public class ApplicativeFunctorLawTests
{
    [Fact]
    public void Arr() =>
        ApplicativeLaw<Arr>.assert();
    
    [Fact]
    public void HashSet() =>
        ApplicativeLaw<HashSet>.assert();
    
    [Fact]
    public void Iterable() =>
        ApplicativeLaw<Iterable>.assert();
    
    [Fact]
    public void Lst() =>
        ApplicativeLaw<Lst>.assert();
    
    [Fact]
    public void Seq() =>
        ApplicativeLaw<Seq>.assert();
    
    [Fact]
    public void EffRT()
    {
        bool eq(K<Eff<Unit>, int> vx, K<Eff<Unit>, int> vy) => 
            vx.Run(unit).Equals(vy.Run(unit));
        
        ApplicativeLaw<Eff<Unit>>.assert(eq);
    }
    
    [Fact]
    public void Eff()
    {
        bool eq(K<Eff, int> vx, K<Eff, int> vy) => 
            vx.Run().Equals(vy.Run());
        
        ApplicativeLaw<Eff>.assert(eq);
    }
    
    [Fact]
    public void IO()
    {
        bool eq(K<IO, int> vx, K<IO, int> vy) => 
            vx.RunSafe().Equals(vy.RunSafe());
        
        ApplicativeLaw<IO>.assert(eq);
    }
    
    [Fact]
    public void StreamT()
    {
        // TODO: Restore
        
        /*static Seq<int> toSeq(StreamT<Identity, int> s) =>
            s.Head().Run()
             .Match(Some: x => x.Cons(toSeq(s.Tail())),
                    None: []);

        static bool eq(K<StreamT<Identity>, int> vx, K<StreamT<Identity>, int> vy) =>
            toSeq(vx.As()) == toSeq(vy.As());
        
        ApplicativeLaw<StreamT<Identity>>.assert(eq);*/
    }
    
    [Fact]
    public void Either() => 
        ApplicativeLaw<Either<string>>.assert();

    [Fact]
    public void EitherT() => 
        ApplicativeLaw<EitherT<string, Identity>>.assert();

    [Fact]
    public void Fin() => 
        ApplicativeLaw<Fin>.assert();

    [Fact]
    public void FinT() => 
        ApplicativeLaw<FinT<Identity>>.assert();

    [Fact]
    public void Option() => 
        ApplicativeLaw<Option>.assert();

    [Fact]
    public void OptionT() => 
        ApplicativeLaw<OptionT<Identity>>.assert();

    [Fact]
    public void Try()
    {
        bool eq(K<Try, int> vx, K<Try, int> vy) => 
            vx.Run().Equals(vy.Run());
        
        ApplicativeLaw<Try>.assert(eq);
    }
    
    [Fact]
    public void TryT()
    {
        bool eq(K<TryT<Identity>, int> vx, K<TryT<Identity>, int> vy) => 
            vx.Run().Run().Equals(vy.Run().Run());
        
        ApplicativeLaw<TryT<Identity>>.assert(eq);
    }
    
    [Fact]
    public void Validation() => 
        ApplicativeLaw<Validation<StringM>>.assert();

    [Fact]
    public void ValidationT()
    {
        bool eq(K<ValidationT<StringM, Identity>, int> vx, K<ValidationT<StringM, Identity>, int> vy) =>
            vx.Run().As().Value.Equals(vy.Run().As().Value);

        ApplicativeLaw<ValidationT<StringM, Identity>>.assert(eq);
    }

    [Fact]
    public void Identity() => 
        ApplicativeLaw<Identity>.assert();

    [Fact]
    public void IdentityT() => 
        ApplicativeLaw<IdentityT<Identity>>.assert();

    [Fact]
    public void Reader()
    {
        bool eq(K<Reader<string>, int> vx, K<Reader<string>, int> vy) => 
            vx.Run("Hello").Equals(vy.Run("Hello"));
        
        ApplicativeLaw<Reader<string>>.assert(eq);
    }
    
    [Fact]
    public void ReaderT()
    {
        bool eq(K<ReaderT<string, Identity>, int> vx, K<ReaderT<string, Identity>, int> vy) => 
            vx.Run("Hello").Run().Equals(vy.Run("Hello").Run());
                 
        ApplicativeLaw<ReaderT<string, Identity>>.assert(eq);
    }

    [Fact]
    public void State()
    {
        bool eq(K<State<string>, int> vx, K<State<string>, int> vy) => 
            vx.Run("Hello").Equals(vy.Run("Hello"));
        
        ApplicativeLaw<State<string>>.assert(eq);
    }
    
    [Fact]
    public void StateT()
    {
        bool eq(K<StateT<string, Identity>, int> vx, K<StateT<string, Identity>, int> vy) => 
            vx.Run("Hello").Run().Equals(vy.Run("Hello").Run());
                 
        ApplicativeLaw<StateT<string, Identity>>.assert(eq);
    }
    
    [Fact]
    public void Writer()
    {
        bool eq(K<Writer<StringM>, int> vx, K<Writer<StringM>, int> vy) => 
            vx.Run("Hello, ").Equals(vy.Run("Hello, "));
        
        ApplicativeLaw<Writer<StringM>>.assert(eq);
    }
    
    [Fact]
    public void WriterT()
    {
        bool eq(K<WriterT<StringM, Identity>, int> vx, K<WriterT<StringM, Identity>, int> vy) => 
            vx.Run("Hello, ").Run().Equals(vy.Run("Hello, ").Run());
        
        ApplicativeLaw<WriterT<StringM, Identity>>.assert(eq);
    }      
}
