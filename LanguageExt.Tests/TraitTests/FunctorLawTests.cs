using LanguageExt.Common;
using Xunit;
namespace LanguageExt.Tests.TraitTests;

public class FunctorLawTests
{
    [Fact]
    public void EffRT()
    {
        bool eq(K<Eff<Unit>, int> vx, K<Eff<Unit>, int> vy) => 
            vx.Run(unit).Equals(vy.Run(unit));
        
        var ma = Eff<Unit, int>.Pure(1);
        var mx = Eff<Unit, int>.Fail(Errors.Closed);
        var r1 = FunctorLaw<Eff<Unit>>.lawsHold(ma, eq);
        var r2 = FunctorLaw<Eff<Unit>>.lawsHold(mx, eq);
        Assert.True(r1);
        Assert.True(r2);
    }
    
    [Fact]
    public void Eff()
    {
        bool eq(K<Eff, int> vx, K<Eff, int> vy) => 
            vx.Run().Equals(vy.Run());
        
        var ma = Eff<int>.Pure(1);
        var mx = Eff<int>.Fail(Errors.Closed);
        var r1 = FunctorLaw<Eff>.lawsHold(ma, eq);
        var r2 = FunctorLaw<Eff>.lawsHold(mx, eq);
        Assert.True(r1);
        Assert.True(r2);
    }
    
    [Fact]
    public void IO()
    {
        bool eq(K<IO, int> vx, K<IO, int> vy) => 
            vx.RunSafe().Equals(vy.RunSafe());
        
        var ma = IO<int>.Pure(1);
        var mx = IO<int>.Fail(Errors.Closed);
        var r1 = FunctorLaw<IO>.lawsHold(ma, eq);
        var r2 = FunctorLaw<IO>.lawsHold(mx, eq);
        Assert.True(r1);
        Assert.True(r2);
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
        
        var r1 = FunctorLaw<StreamT<Identity>>.lawsHold(ma, eq);
        var r2 = FunctorLaw<StreamT<Identity>>.lawsHold(mb, eq);
        
        Assert.True(r1);
        Assert.True(r2);
    }
    
    [Fact]
    public void Either()
    {
        var fa = Either<string, int>.Right(1);
        var fx = Either<string, int>.Left("failed");
        var r1 = FunctorLaw<Either<string>>.lawsHold(fa);
        var r2 = FunctorLaw<Either<string>>.lawsHold(fx);
        Assert.True(r1);
        Assert.True(r2);
    }
    
    [Fact]
    public void EitherT()
    {
        var fa = EitherT<string, Identity, int>.Right(1);
        var fx = EitherT<string, Identity, int>.Left("failed");
        var r1 = FunctorLaw<EitherT<string, Identity>>.lawsHold(fa);
        var r2 = FunctorLaw<EitherT<string, Identity>>.lawsHold(fx);
        Assert.True(r1);
        Assert.True(r2);
    }

    [Fact]
    public void Fin()
    {
        var fa = Fin<int>.Succ(1);
        var fx = Fin<int>.Fail(Errors.TimedOut);
        var r1 = FunctorLaw<Fin>.lawsHold(fa);
        var r2 = FunctorLaw<Fin>.lawsHold(fx);
        Assert.True(r1);
        Assert.True(r2);
    }
    
    [Fact]
    public void FinT()
    {
        var fa = FinT<Identity, int>.Succ(1);
        var fx = FinT<Identity, int>.Fail(Errors.TimedOut);
        var r1 = FunctorLaw<FinT<Identity>>.lawsHold(fa);
        var r2 = FunctorLaw<FinT<Identity>>.lawsHold(fx);
        Assert.True(r1);
        Assert.True(r2);
    }
    
    [Fact]
    public void Option()
    {
        var fa = Option<int>.Some(1);
        var fx = Option<int>.None;
        var r1 = FunctorLaw<Option>.lawsHold(fa);
        var r2 = FunctorLaw<Option>.lawsHold(fx);
        Assert.True(r1);
        Assert.True(r2);
    }
    
    [Fact]
    public void OptionT()
    {
        var fa = OptionT<Identity, int>.Some(1);
        var fx = OptionT<Identity, int>.None;
        var r1 = FunctorLaw<OptionT<Identity>>.lawsHold(fa);
        var r2 = FunctorLaw<OptionT<Identity>>.lawsHold(fx);
        Assert.True(r1);
        Assert.True(r2);
    }
    
    [Fact]
    public void Try()
    {
        bool eq(K<Try, int> vx, K<Try, int> vy) => 
            vx.Run().Equals(vy.Run());
        
        var fa = Try<int>.Succ(1);
        var fx = Try<int>.Fail(Errors.EndOfStream);
        var r1 = FunctorLaw<Try>.lawsHold(fa, eq);
        var r2 = FunctorLaw<Try>.lawsHold(fx, eq);
        Assert.True(r1);
        Assert.True(r2);
    }
    
    [Fact]
    public void TryT()
    {
        bool eq(K<TryT<Identity>, int> vx, K<TryT<Identity>, int> vy) => 
            vx.Run().Run().Equals(vy.Run().Run());
        
        var fa = TryT<Identity, int>.Succ(1);
        var fx = TryT<Identity, int>.Fail(Errors.EndOfStream);
        var r1 = FunctorLaw<TryT<Identity>>.lawsHold(fa, eq);
        var r2 = FunctorLaw<TryT<Identity>>.lawsHold(fx, eq);
        Assert.True(r1);
        Assert.True(r2);
    }
    
    [Fact]
    public void Validation()
    {
        var fa = Validation<StringM, int>.Success(1);
        var fx = Validation<StringM, int>.Fail("failed");
        var r1 = FunctorLaw<Validation<StringM>>.lawsHold(fa);
        var r2 = FunctorLaw<Validation<StringM>>.lawsHold(fx);
        Assert.True(r1);
        Assert.True(r2);
    }
    
    [Fact]
    public void ValidationT()
    {
        var fa = ValidationT<StringM, Identity, int>.Success(1);
        var fx = ValidationT<StringM, Identity, int>.Fail("failed");
        var r1 = FunctorLaw<ValidationT<StringM, Identity>>.lawsHold(fa);
        var r2 = FunctorLaw<ValidationT<StringM, Identity>>.lawsHold(fx);
        Assert.True(r1);
        Assert.True(r2);
    }
    
    [Fact]
    public void Identity()
    {
        var fa = LanguageExt.Identity.Pure(1);
        var r1 = FunctorLaw<Identity>.lawsHold(fa);
        Assert.True(r1);
    }
    
    [Fact]
    public void IdentityT()
    {
        var fa = IdentityT<Identity, int>.Pure(1);
        var r1 = FunctorLaw<IdentityT<Identity>>.lawsHold(fa);
        Assert.True(r1);
    }
}

