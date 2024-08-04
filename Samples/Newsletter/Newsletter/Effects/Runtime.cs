using Newsletter.Effects.Traits;

namespace Newsletter.Effects;

public record Runtime(RuntimeEnv Env) :
    Has<Eff<Runtime>, WebIO>,
    Has<Eff<Runtime>, FileIO>,
    Has<Eff<Runtime>, JsonIO>,
    Has<Eff<Runtime>, EmailIO>,
    Has<Eff<Runtime>, ImageIO>,
    Has<Eff<Runtime>, ConsoleIO>,
    Has<Eff<Runtime>, EncodingIO>,
    Has<Eff<Runtime>, DirectoryIO>,
    Reads<Eff<Runtime>, Runtime, Config>,
    Reads<Eff<Runtime>, Runtime, HttpClient>,
    IDisposable
{
    private static Config Common(string repoRootFolder, Option<string> sendGridKey) =>
        new ("",
             Path.Combine(repoRootFolder, "templates"),
             "",
             "https://paullouth.com",
             700,
             // Public Ghost-API key. It's ok this only accesses public data,
             // just don't abuse it, or I'll revoke it!
             "6d15eacab40271bcdd552306d0",
             sendGridKey);

    public static Runtime Test(string repoRootFolder, Option<string> sendGridKey) =>
        New(new RuntimeEnv(
                new HttpClient(),
                Common(repoRootFolder, sendGridKey) with
                {
                    MembersFolder = Path.Combine(repoRootFolder, "members-test"),
                    LettersFolder = Path.Combine(repoRootFolder, "letters-test")
                }));

    public static Runtime Live(string repoRootFolder, Option<string> sendGridKey) =>
        New(new RuntimeEnv(
                new HttpClient(),
                Common(repoRootFolder, sendGridKey) with
                {
                    MembersFolder = Path.Combine(repoRootFolder, "members-live"),
                    LettersFolder = Path.Combine(repoRootFolder, "letters-live")
                }));
    
    public static Runtime New(RuntimeEnv env) =>
        new (env);
    
    K<Eff<Runtime>, FileIO> Has<Eff<Runtime>, FileIO>.Trait =>
        pure<Eff<Runtime>, FileIO>(LanguageExt.Sys.Live.Implementations.FileIO.Default);

    K<Eff<Runtime>, EncodingIO> Has<Eff<Runtime>, EncodingIO>.Trait => 
        pure<Eff<Runtime>, EncodingIO>(LanguageExt.Sys.Live.Implementations.EncodingIO.Default);

    K<Eff<Runtime>, DirectoryIO> Has<Eff<Runtime>, DirectoryIO>.Trait =>
        pure<Eff<Runtime>, DirectoryIO>(LanguageExt.Sys.Live.Implementations.DirectoryIO.Default);

    K<Eff<Runtime>, ConsoleIO> Has<Eff<Runtime>, ConsoleIO>.Trait =>
        pure<Eff<Runtime>, ConsoleIO>(LanguageExt.Sys.Live.Implementations.ConsoleIO.Default);

    K<Eff<Runtime>, JsonIO> Has<Eff<Runtime>, JsonIO>.Trait => 
        pure<Eff<Runtime>, JsonIO>(Impl.Json.Default); 

    K<Eff<Runtime>, EmailIO> Has<Eff<Runtime>, EmailIO>.Trait => 
        pure<Eff<Runtime>, EmailIO>(Impl.Email.Default); 

    K<Eff<Runtime>, WebIO> Has<Eff<Runtime>, WebIO>.Trait =>
        pure<Eff<Runtime>, WebIO>(Impl.Web.Default);

    K<Eff<Runtime>, ImageIO> Has<Eff<Runtime>, ImageIO>.Trait =>
        pure<Eff<Runtime>, ImageIO>(Impl.Image.Default);

    K<Eff<Runtime>, Config> Reads<Eff<Runtime>, Runtime, Config>.Get { get; } = 
        liftEff<Runtime, Config>(rt => rt.Env.Config);

    K<Eff<Runtime>, HttpClient> Reads<Eff<Runtime>, Runtime, HttpClient>.Get { get; } = 
        liftEff<Runtime, HttpClient>(rt => rt.Env.HttpClient);

    public void Dispose() =>
        Env.Dispose();
}

