using Xunit;
using LanguageExt;
using LanguageExt.Common;

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
    public void ValidationT()
    {
        bool eq(K<ValidationT<StringM, Identity>, int> vx, K<ValidationT<StringM, Identity>, int> vy) =>
            vx.Run().As().Value.Equals(vy.Run().As().Value);

        AlternativeLaw<ValidationT<StringM, Identity>>.assert(eq);
    }
}
