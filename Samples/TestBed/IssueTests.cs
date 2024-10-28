using System;
using System.Net.Http;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace TestBed;

public class IssueTests
{
    
    private HttpClient client;

    record Download
    {
        public static Task<Download<System.IO.Stream>> CreateDownloadStream(string url, HttpResponseMessage response) =>
            throw new NotImplementedException();
    }

    public record Download<A>(string Url, A Value);

    /*
    EitherT<YourError, IO, A> yourLift<A>(Func<Task<A>> f) =>
        liftIO(f)
           .Match(Succ: Right<YourError, A>,
                  Fail: e => Left<YourError, A>(YourError.Except(e)));
                  */

public static App<HttpResponseMessage> GetAsync(string url) =>
    from client   in App.httpClient
    from response in App.liftIO(async () => await client.GetAsync(url))
    from _        in guardnot(response.IsSuccessStatusCode, YourError.ResponseFailure(url, response))
    select response;

public static App<Download<System.IO.Stream>> CreateDownloadStream(HttpResponseMessage response, string url) =>
    App.liftIO(async () => await Download.CreateDownloadStream(url, response))
       .MapFail(e => YourError.DownloadFailure(url, e));

public static App<Download<System.IO.Stream>> DownloadByteStream(string url) =>
    from response in GetAsync(url)
    from download in CreateDownloadStream(response, url)
    select download;

}

public partial record YourError;
public record HttpClientAlreadySet : YourError;
public record HttpClientNotSet : YourError;
public record ExceptError(Error Error) : YourError;
public record ResponseFailureError(string Url, HttpResponseMessage Response) : YourError;
public record DownloadFailureError(string Url, YourError Error) : YourError;

public partial record YourError
{
    public static YourError HttpClientAlreadySet =>
        new HttpClientAlreadySet();
        
    public static YourError HttpClientNotSet =>
        new HttpClientNotSet();
        
    public static YourError ResponseFailure(string url, HttpResponseMessage response) =>
        new ResponseFailureError(url, response);
        
    public static YourError DownloadFailure(string url, YourError except) => 
        new DownloadFailureError(url, except);
        
    public static YourError Except(Error error) => 
        new ExceptError(error);
}

public record AppEnv(Option<HttpClient> Client);

public record App<A>(ReaderT<AppEnv, EitherT<YourError, IO>, A> runApp) : K<App, A>
{
    public App<B> Map<B>(Func<A, B> f) => this.Kind().Map(f).As();
    public App<B> Select<B>(Func<A, B> f) => this.Kind().Map(f).As();
    public App<A> MapFail(Func<YourError, YourError> f) => this.Kind().Catch(f).As();
    public App<B> Bind<B>(Func<A, K<App, B>> f) => this.Kind().Bind(f).As();
    public App<B> Bind<B>(Func<A, K<IO, B>> f) => this.Kind().Bind(f).As();
    public App<C> SelectMany<B, C>(Func<A, K<App, B>> b, Func<A, B, C> p) => this.Kind().SelectMany(b, p).As();
    public App<C> SelectMany<B, C>(Func<A, K<IO, B>> b, Func<A, B, C> p) => this.Kind().SelectMany(b, p).As();
    public App<C> SelectMany<C>(Func<A, Guard<YourError, Unit>> b, Func<A, Unit, C> p) => SelectMany(a => App.lift(b(a)), p);
}

public static class AppExtensions
{
    public static App<A> As<A>(this K<App, A> ma) =>
        (App<A>)ma;
    
    public static IO<Either<YourError, A>> Run<A>(this K<App, A> ma, AppEnv config) =>
        ma.As().runApp.Run(config).Run().As();

    public static App<C> SelectMany<A, B, C>(this K<App, A> ma, Func<A, App<B>> b, Func<A, B, C> p) => ma.Kind().SelectMany(b, p).As();
    public static App<C> SelectMany<A, B, C>(this K<IO, A> ma, Func<A, K<App, B>> b, Func<A, B, C> p) => ma.Kind().SelectMany(b, p).As();
    public static App<C> SelectMany<B, C>(this Guard<YourError, Unit> ma, Func<Unit, K<App, B>> b, Func<Unit, B, C> p) => App.lift(ma).SelectMany(b, p);
}

public partial class App : 
    Monad<App>,
    Fallible<YourError, App>,
    Readable<App, AppEnv>
{
    static K<App, B> Monad<App>.Bind<A, B>(K<App, A> ma, Func<A, K<App, B>> f) => 
        new App<B>(ma.As().runApp.Bind(x => f(x).As().runApp));

    static K<App, B> Functor<App>.Map<A, B>(Func<A, B> f, K<App, A> ma) => 
        new App<B>(ma.As().runApp.Map(f));

    static K<App, A> Applicative<App>.Pure<A>(A value) => 
        new App<A>(ReaderT.lift<AppEnv, EitherT<YourError, IO>, A>(EitherT.Right<YourError, IO, A>(value)));

    static K<App, B> Applicative<App>.Apply<A, B>(K<App, Func<A, B>> mf, K<App, A> ma) => 
        new App<B>(mf.As().runApp.Apply(ma.As().runApp).As());

    static K<App, A> MonadIO<App>.LiftIO<A>(IO<A> ma) =>
        new App<A>(
            ReaderT.lift<AppEnv, EitherT<YourError, IO>, A>(
                EitherT.liftIO<YourError, IO, A>(
                    ma.Match(Succ: Right<YourError, A>,
                             Fail: e => Left<YourError, A>(YourError.Except(e))))));

    static K<App, A> Fallible<YourError, App>.Fail<A>(YourError error) =>
        new App<A>(ReaderT.lift<AppEnv, EitherT<YourError, IO>, A>(EitherT.Left<YourError, IO, A>(error)));

    static K<App, A> Fallible<YourError, App>.Catch<A>(
        K<App, A> fa, 
        Func<YourError, bool> Predicate, 
        Func<YourError, K<App, A>> Fail) => 
        new App<A>(
            new ReaderT<AppEnv, EitherT<YourError, IO>, A>(
                env => fa.As().runApp.runReader(env).Catch(Predicate, e => Fail(e).As().runApp.runReader(env)).As()));

    static K<App, A> Readable<App, AppEnv>.Asks<A>(Func<AppEnv, A> f) => 
        new App<A>(ReaderT.asks<EitherT<YourError, IO>, A, AppEnv>(f));

    static K<App, A> Readable<App, AppEnv>.Local<A>(Func<AppEnv, AppEnv> f, K<App, A> ma) => 
        new App<A>(ReaderT.local(f, ma.As().runApp));
}

public partial class App
{
    public static App<A> pure<A>(A value) =>
        pure<App, A>(value).As();
    
    public static App<Unit> fail(YourError error) =>
        fail<YourError, App>(error).As();
    
    public static App<A> fail<A>(YourError error) =>
        fail<YourError, App, A>(error).As();

    public static App<A> liftIO<A>(IO<A> computation) =>
        MonadIO.liftIO<App, A>(computation).As();

    public static App<A> liftIO<A>(Func<Task<A>> computation) =>
        MonadIO.liftIO<App, A>(IO.liftAsync(computation)).As();

    public static App<A> lift<A>(Either<YourError, A> either) =>
        lift(EitherT.lift<YourError, IO, A>(either)).As();

    public static App<A> lift<A>(EitherT<YourError, IO, A> eitherT) =>
        new App<A>(ReaderT.lift<AppEnv, EitherT<YourError, IO>, A>(eitherT)).As();

    public static App<Unit> lift(Guard<YourError, Unit> guard) =>
        lift(guard.ToEither());

    public static App<AppEnv> ask =>
        Readable.ask<App, AppEnv>().As();

    public static App<A> asks<A>(Func<AppEnv, A> f) =>
        Readable.asks<App, AppEnv, A>(f).As();

    public static App<A> local<A>(Func<AppEnv, AppEnv> f, App<A> ma) =>
        Readable.local(f, ma).As();

    public static App<A> withHttpClient<A>(App<A> ma) =>
        from c in asks(e => e.Client)
        from _ in when(c.IsSome, fail(YourError.HttpClientAlreadySet))
        from r in local(e => e with { Client = new HttpClient() }, ma)
        select r;

    public static App<HttpClient> httpClient =>
        from c in asks(e => e.Client)
        from _ in when(c.IsNone, fail(YourError.HttpClientNotSet))
        select (HttpClient)c;
}
