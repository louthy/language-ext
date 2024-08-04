using Newsletter.Effects.Traits;

namespace Newsletter.Effects.Test;

public record Runtime(RuntimeEnv Env) : 
    Has<Eff<Runtime>, WebIO>,
    Has<Eff<Runtime>, FileIO>,
    Has<Eff<Runtime>, JsonIO>,
    Has<Eff<Runtime>, EmailIO>,
    Has<Eff<Runtime>, ImageIO>,
    Has<Eff<Runtime>, ConsoleIO>,
    Has<Eff<Runtime>, EncodingIO>,
    Has<Eff<Runtime>, DirectoryIO>,
    Reads<Eff<Runtime>, Runtime, HttpClient>,
    Reads<Eff<Runtime>, Runtime, Config>,
    IDisposable
{
    public static Runtime New(string repoRootFolder, Option<string> sendGridKey) =>
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
    
    K<Eff<Runtime>, FileIO> Has<Eff<Runtime>, FileIO>.Trait =>
        SuccessEff<Runtime, FileIO>(LanguageExt.Sys.Live.Implementations.FileIO.Default);

    K<Eff<Runtime>, EncodingIO> Has<Eff<Runtime>, EncodingIO>.Trait => 
        SuccessEff<Runtime, EncodingIO>(LanguageExt.Sys.Live.Implementations.EncodingIO.Default);

    K<Eff<Runtime>, DirectoryIO> Has<Eff<Runtime>, DirectoryIO>.Trait =>
        SuccessEff<Runtime, DirectoryIO>(LanguageExt.Sys.Live.Implementations.DirectoryIO.Default);

    K<Eff<Runtime>, ConsoleIO> Has<Eff<Runtime>, ConsoleIO>.Trait =>
        SuccessEff<Runtime, ConsoleIO>(LanguageExt.Sys.Live.Implementations.ConsoleIO.Default);

    K<Eff<Runtime>, JsonIO> Has<Eff<Runtime>, JsonIO>.Trait => 
        SuccessEff<Runtime, JsonIO>(Live.Json.Default); 

    K<Eff<Runtime>, EmailIO> Has<Eff<Runtime>, EmailIO>.Trait => 
        SuccessEff<Runtime, EmailIO>(Live.Email.Default); 

    K<Eff<Runtime>, WebIO> Has<Eff<Runtime>, WebIO>.Trait =>
        SuccessEff<Runtime, WebIO>(Live.Web.Default);

    K<Eff<Runtime>, Config> Reads<Eff<Runtime>, Runtime, Config>.Get { get; } = 
        liftEff<Runtime, Config>(rt => rt.Env.Config);

    K<Eff<Runtime>, HttpClient> Reads<Eff<Runtime>, Runtime, HttpClient>.Get { get; } = 
        liftEff<Runtime, HttpClient>(rt => rt.Env.HttpClient);

    K<Eff<Runtime>, ImageIO> Has<Eff<Runtime>, ImageIO>.Trait =>
        SuccessEff<Runtime, ImageIO>(Live.Image.Default);

    public void Dispose() =>
        Env.Dispose();
}

