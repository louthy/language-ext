// TODO: Delete this or turn it into some real tests

using System;
using System.IO;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static class Testing
{
    public static void Test1()
    {
        var m1 = ReaderT<string>.lift(Some(123));
        var m2 = ReaderT<string>.lift(Some(123));
        var mt = ReaderT<string>.lift(Some(unit));
                
        var m0 = from w in Pure(123)
                 from x in m1
                 from _ in unless(true, mt)
                 from y in m2
                 from z in asks((string env) => env.Length)
                 from e in ask<string>()
                 from n in ReaderT<string, Option, string>.Lift(Some("Paul"))
                 select $"{e} {n}: {w + x + y + z}";

        var m3 = from w in Pure(123)
                 from x in m1
                 from y in m2
                 from z in Some(100)
                 from e in ask<string>()
                 select $"{e}: {w + x + y + z}";

        var r1 = m3.As().Run("Hello");
        
        var m4 = from x in m1
                 from y in m2
                 from e in ask<string>()
                 select $"{e}: {x + y}";

        var r2 = m4.As().Run("Hello").As();
    }
    
    public static void Test2()
    {
        var m1 = Reader<string, int>.Pure(123);
        var m2 = Reader<string, int>.Pure(123);
        
        var m3 = from x in m1
                 from y in m2
                 from e in ask<string>()
                 select $"{e}: {x + y}";
        
        var m4 = from x in m1
                 from y in m2
                 from e in ask<string>()
                 from z in Pure(234)
                 select $"{e}: {x + y}";
    }
    
    public static void Test3()
    {
        var m1 = ReaderT<string>.lift(Right<string, int>(123));
        var m2 = ReaderT<string>.lift(Right<string, int>(123));
        
        var m3 = from w in Pure(123)
                 from x in m1
                 from y in m2
                 from z in Right(100)
                 from e in ask<string>()
                 select $"{e}: {w + x + y + z}";

        var r1 = m3.Run("Hello");
        
        var m4 = from x in m1
                 from y in m2
                 from z in Left<string, int>("fail")
                 from e in ask<string>()
                 select $"{e}: {x + y + z}";

        var r2 = m4.Run("Hello");        
    }
    
    
    public static void Test4()
    {
        var m1 = App.Pure(123);
        var m2 = App.Pure(123);
        var m3 = App.Fail<int>(Error.New("fail"));
        
        var m4 = from w in Pure(234)
                 from x in m1
                 from y in m2
                 from z in m3
                 from r in App.rootFolder
                 from t in liftIO(async () => await File.ReadAllTextAsync($"{r}\\test.txt"))
                 select $"{t}: {w + x + y + z}";

        var r1 = m4.Run(new AppConfig("", "")).As();
    }
   
    public static void Test6()
    {
        var m1 = ReaderT<string>.lift(IdentityT.lift(IO.pure(123)));
        var m2 = ReaderT<string>.lift(IdentityT.lift(IO.pure(123)));
                
        var m0 = from w in Pure(123)
                 from p in ReaderT.ask<IdentityT<IO>, string>()
                 from x in IO.pure("Hello")
                 from i in ReaderT<string, IdentityT<IO>>.liftIO(IO.pure("Hello"))
                 from j in IO.pure("Hello").Fork()
                 from r in envIO 
                 from y in m2
                 select $"{p} {y} {j}";

        var value = m0.Run("Hello").As().Value.As().Run(EnvIO.New());
    }
   
    public static void Test7()
    {
        var m1 = ReaderT<string>.lift(IO.pure(123));
        var m2 = ReaderT<string>.lift(IO.pure(123));
                
        var m0 = from w in Pure(123)
                 from q in m1
                 from f in use(() => File.Open("c:\\test.txt", FileMode.Open))
                 from p in ReaderT.ask<IO, string>()
                 from x in IO.pure("Hello")
                 from i in ReaderT<string, IO>.liftIO(IO.pure("Hello"))
                 from j in IO.pure("Hello").Fork()
                 from r in envIO 
                 from y in m2
                 select $"{p} {y} {j}";

        var value = m0.Run("Hello").As();
    }
   
    public static void Test8()
    {
        var m1 = OptionT.lift(ReaderT<string>.lift(IO.pure(123)));
        var m2 = OptionT.lift(ReaderT<string>.lift(IO.pure(123)));

        var m0 = from w in Pure(123)
                 from q in m1
                 from f in use(() => File.Open("c:\\test.txt", FileMode.Open))
                 from p in ask<string>()
                 from i in liftIO(IO.pure("Hello"))
                 from j in IO.pure("Hello").Fork()
                 from r in envIO
                 from _ in release(f)
                 from y in m2
                 select $"{w} {f} {i}";

        var value = m0.Match(Some: v => $"foo {v}",
                             None: () => "bar").As()
                      .Run("Paul").As()
                      .Run();

        OptionT<ReaderT<Env, IO>, Env> ask<Env>() =>
            OptionT.lift(ReaderT.ask<IO, Env>());
        
        OptionT<ReaderT<string, IO>, A> liftIO<A>(IO<A> ma) =>
            OptionT.liftIO<ReaderT<string, IO>, A>(ma);
    }
       
    /*public static void Test9()
    {
        var m1 = OptionT.lift(StateT<string>.lift(IO.pure(100)));
        var m2 = OptionT.lift(StateT<string>.lift(IO.pure(200)));

        var m0 = from w in Pure(123)
                 from q in m1
                 from x in StateT.get<IO, string>()
                 from i in OptionT.liftIO<StateT<string, IO>, string>(IO.pure("Hello"))
                 from j in IO.pure("Hello").Fork()
               //from k in m1.ForkIO()                          -- Can't work, because OptionT is not MonadUnliftIO
                 from k in m1.Run().Run("state").As().Fork()    // But we can manually unpack 
                 from _ in StateT.put<IO, string>(x)
                 from r in envIO
                 from y in m2
                 select $"{w} {j} {i}";

        var value = m0.Match(Some: v => $"foo {v}", 
                             None: () => "bar").As()
                      .Run("Paul").As()
                      .Run(); 
    }*/
       
    public static void Test10()
    {
        var m1 = StateT<string>.lift(OptionT.lift(IO.pure(100)));
        var m2 = StateT<string>.lift(OptionT.lift(IO.pure(200)));

        var m0 = from w in Pure(123)
                 from q in m1
                 from x in StateT.get<OptionT<IO>, string>()
                 from i in StateT<string, OptionT<IO>>.liftIO(IO.pure("Hello"))
                 from j in IO.pure("Hello").Fork()
                 from _ in StateT.put<OptionT<IO>, string>(x)
                 from r in envIO
                 from y in m2
                 select $"{w} {j} {i}";

        var value = m0.Run("Paul").As()
                      .Match(Some: v => $"value: {v.Value}, state: {v.State}", 
                             None: () => "bar").As()
                      .Run(); 
    }

    static void TravTest1()
    {
        var xs = Seq(1, 2, 3, 4, 5);
        var r1 = Traversable.traverse(x => Some(x), xs);
        
        var ys = Seq(Some(1).Kind(), Some(2).Kind(), Option<int>.None.Kind(), Some(4).Kind(), Option<int>.None.Kind());
        var r2 = Traversable.sequenceM(ys);
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 
//  Generalised IO
//

public static class GeneralIO<M>
    where M : Monad<M>
{
    public static K<M, string> readAllText(string path) =>
        M.LiftIOMaybe(liftIO(async _ => await File.ReadAllTextAsync(path)));
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 
//  Maybe test
//

public class Maybe : Monad<Maybe>
{
    public static Maybe<A> Just<A>(A value) =>
        new Just<A>(value);

    public static K<Maybe, B> Bind<A, B>(K<Maybe, A> ma, Func<A, K<Maybe, B>> f) => 
        ma.As().Bind(f);

    public static K<Maybe, B> Map<A, B>(Func<A, B> f, K<Maybe, A> ma) => 
        ma.As().Map(f);
    
    public static K<Maybe, A> Pure<A>(A value) => 
        Just(value);

    public static K<Maybe, B> Apply<A, B>(K<Maybe, Func<A, B>> mf, K<Maybe, A> ma) =>
        mf.As().Bind(f => ma.As().Map(f));

    public static K<Maybe, B> Action<A, B>(K<Maybe, A> ma, K<Maybe, B> mb) =>
        ma.As().Bind(_ => mb);

    public static K<Maybe, A> LiftIOMaybe<A>(IO<A> ma) => 
        throw new NotImplementedException();
}

public abstract record Maybe<A> : K<Maybe, A>
{
    public static readonly Maybe<A> Nothing = 
        new Nothing<A>();
    
    public abstract Maybe<B> Map<B>(Func<A, B> f);

    public abstract Maybe<B> Bind<B>(Func<A, Maybe<B>> f);

    public virtual Maybe<B> Bind<B>(Func<A, K<Maybe, B>> f) =>
        Bind(x => f(x).As());

    public Maybe<C> SelectMany<B, C>(Func<A, Maybe<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    public Maybe<C> SelectMany<B, C>(Func<A, K<Maybe, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => bind(x).As(), project);
}

public record Just<A>(A Value) : Maybe<A>
{
    public override Maybe<B> Map<B>(Func<A, B> f) => 
        new Just<B>(f(Value));

    public override Maybe<B> Bind<B>(Func<A, Maybe<B>> f) =>
        f(Value);
}

public record Nothing<A> : Maybe<A>
{
    public override Maybe<B> Map<B>(Func<A, B> f) => 
        Maybe<B>.Nothing;

    public override Maybe<B> Bind<B>(Func<A, Maybe<B>> f) =>
        Maybe<B>.Nothing;
}

public static class MaybeExt
{
    public static Maybe<A> As<A>(this K<Maybe, A> ma) =>
        (Maybe<A>)ma;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 
//  App test
//

// Domain monad
public record App<A>(ReaderT<AppConfig, Either<Error>, A> runReader) : K<App, A>;

// Application environment
public record AppConfig(string ConnectionString, string RootFolder);

public static class AppExtensions
{
    public static App<A> As<A>(this K<App, A> ma) =>
        (App<A>)ma;
    
    public static Either<Error, A> Run<A>(this K<App, A> ma, AppConfig config) =>
        ma.As().runReader.Run(config).As();
}

public class App :
    Fallible<App>,
    Deriving.MonadT<App, ReaderT<AppConfig, Either<Error>>, Either<Error>>,
    Deriving.MonadIO<App, ReaderT<AppConfig, Either<Error>>>,
    Deriving.Readable<App, AppConfig, ReaderT<AppConfig, Either<Error>>>
{
    public static App<A> Pure<A>(A value) =>
        Applicative.pure<App, A>(value).As();

    public static App<A> Fail<A>(Error error) =>
        Fallible.error<App, A>(error).As();

    public static K<App, A> Catch<A>(K<App, A> fa, Func<Error, bool> Predicate, Func<Error, K<App, A>> Fail) => 
        throw new NotImplementedException();

    public static App<string> connectionString =>
        Readable.asks<App, AppConfig, string>(env => env.ConnectionString).As();

    public static App<string> rootFolder =>
        Readable.asks<App, AppConfig, string>(env => env.RootFolder).As();

    public static K<ReaderT<AppConfig, Either<Error>>, A> Transform<A>(K<App, A> fa) => 
        fa.As().runReader;

    public static K<App, A> CoTransform<A>(K<ReaderT<AppConfig, Either<Error>>, A> fa) => 
        new App<A>(fa.As());

    static K<App, A> Fallible<Error, App>.Fail<A>(Error error) =>
        new App<A>(ReaderT.lift<AppConfig, Either<Error>, A>(Left<Error, A>(error).As()));
}

