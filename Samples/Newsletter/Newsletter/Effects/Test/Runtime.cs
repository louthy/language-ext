using Newsletter.Effects.Traits;

namespace Newsletter.Effects.Test;

public record Runtime<M>(RuntimeEnv Env) : 
    Has<M, WebIO>,
    Has<M, FileIO>,
    Has<M, JsonIO>,
    Has<M, EmailIO>,
    Has<M, ImageIO>,
    Has<M, ConsoleIO>,
    Has<M, EncodingIO>,
    Has<M, DirectoryIO>,
    Reads<M, Runtime<M>, HttpClient>,
    Reads<M, Runtime<M>, Config>,
    IDisposable
    where M :
        Monad<M>,
        Stateful<M, Runtime<M>>
{
    public static Runtime<M> New(string repoRootFolder, Option<string> sendGridKey) =>
        new (new RuntimeEnv(
            new HttpClient(),
            new Config(
                Path.Combine(repoRootFolder, "members-test"),
                Path.Combine(repoRootFolder, "templates"),
                Path.Combine(repoRootFolder, "letters-test"),
                "https://paullouth.com",
                700,
                // Public Ghost-API key. It's ok this only accesses public data, just don't abuse it, or I'll revoke it!
                "6d15eacab40271bcdd552306d0",
                sendGridKey)));
    
    K<M, FileIO> Has<M, FileIO>.Trait =>
        M.Pure(LanguageExt.Sys.Live.Implementations.FileIO.Default);

    K<M, EncodingIO> Has<M, EncodingIO>.Trait => 
        M.Pure(LanguageExt.Sys.Live.Implementations.EncodingIO.Default);

    K<M, DirectoryIO> Has<M, DirectoryIO>.Trait =>
        M.Pure(LanguageExt.Sys.Live.Implementations.DirectoryIO.Default);

    K<M, ConsoleIO> Has<M, ConsoleIO>.Trait =>
        M.Pure(LanguageExt.Sys.Live.Implementations.ConsoleIO.Default);

    K<M, JsonIO> Has<M, JsonIO>.Trait => 
        M.Pure(Live.Json.Default); 

    K<M, EmailIO> Has<M, EmailIO>.Trait => 
        M.Pure(Live.Email.Default); 

    K<M, WebIO> Has<M, WebIO>.Trait =>
        M.Pure(Live.Web.Default);

    K<M, ImageIO> Has<M, ImageIO>.Trait =>
        M.Pure(Live.Image.Default);

    K<M, Config> Reads<M, Runtime<M>, Config>.Get { get; } = 
        M.Gets(rt => rt.Env.Config);

    K<M, HttpClient> Reads<M, Runtime<M>, HttpClient>.Get { get; } = 
        M.Gets(rt => rt.Env.HttpClient);

    public void Dispose() =>
        Env.Dispose();
}

