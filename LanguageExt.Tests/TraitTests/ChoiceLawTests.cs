using LanguageExt.Common;
using Xunit;

namespace LanguageExt.Tests.TraitTests;

public class ChoiceLawTests
{
    [Fact]
    public void Arr() =>
        ChoiceLaw<Arr>.assert(Arr<int>.Empty);
    
    [Fact]
    public void HashSet() =>
        ChoiceLaw<HashSet>.assert(HashSet<int>.Empty);
    
    [Fact]
    public void Iterable() =>
        ChoiceLaw<Iterable>.assert(Iterable<int>.Empty);
    
    [Fact]
    public void Lst() =>
        ChoiceLaw<Lst>.assert(Lst<int>.Empty);
    
    [Fact]
    public void Seq() =>
        ChoiceLaw<Seq>.assert(Seq<int>.Empty);
    
    [Fact]
    public void EffRT()
    {
        bool eq(K<Eff<Unit>, int> vx, K<Eff<Unit>, int> vy) => 
            vx.Run(unit).Equals(vy.Run(unit));
        
        ChoiceLaw<Eff<Unit>>.assert(error<Eff<Unit>, int>(Errors.None), eq);
    }
    
    [Fact]
    public void Eff()
    {
        bool eq(K<Eff, int> vx, K<Eff, int> vy) => 
            vx.Run().Equals(vy.Run());
        
        ChoiceLaw<Eff>.assert(error<Eff, int>(Errors.None), eq);
    }
    
    [Fact]
    public void IO()
    {
        bool eq(K<IO, int> vx, K<IO, int> vy) => 
            vx.RunSafe().Equals(vy.RunSafe());
        
        ChoiceLaw<IO>.assert(error<IO, int>(Errors.None), eq);
    }
    
    [Fact]
    public void Either() => 
        ChoiceLaw<Either<string>>.assert(fail<string, Either<string>, int>("error"));

    [Fact]
    public void EitherT() => 
        ChoiceLaw<EitherT<string, Identity>>.assert(fail<string, EitherT<string, Identity>, int>("error"));

    [Fact]
    public void Fin() => 
        ChoiceLaw<Fin>.assert(error<Fin, int>(Errors.None));

    [Fact]
    public void FinT() => 
        ChoiceLaw<FinT<Identity>>.assert(error<FinT<Identity>, int>(Errors.None));

    [Fact]
    public void Option() => 
        ChoiceLaw<Option>.assert(fail<Unit, Option, int>(unit));

    [Fact]
    public void OptionT() => 
        ChoiceLaw<OptionT<Identity>>.assert(fail<Unit, OptionT<Identity>, int>(unit));

    [Fact]
    public void Try()
    {
        bool eq(K<Try, int> vx, K<Try, int> vy) => 
            vx.Run().Equals(vy.Run());
        
        ChoiceLaw<Try>.assert(error<Try, int>(Errors.None), eq);
    }
    
    [Fact]
    public void TryT()
    {
        bool eq(K<TryT<Identity>, int> vx, K<TryT<Identity>, int> vy) => 
            vx.Run().Run().Equals(vy.Run().Run());

        ChoiceLaw<TryT<Identity>>.assert(error<TryT<Identity>, int>(Errors.None), eq);
    }
    
    [Fact]
    public void Validation() => 
        ChoiceLaw<Validation<StringM>>.assert(fail<StringM, Validation<StringM>, int>("error"));

    [Fact]
    public void ValidationT()
    {
        bool eq(K<ValidationT<StringM, Identity>, int> vx, K<ValidationT<StringM, Identity>, int> vy) =>
            vx.Run().As().Value.Equals(vy.Run().As().Value);
        
        ChoiceLaw<ValidationT<StringM, Identity>>.assert(fail<StringM, ValidationT<StringM, Identity>, int>("error"), eq);
    }
}
