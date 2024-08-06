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
    Has<Eff<Runtime>, Config>,
    Has<Eff<Runtime>, HttpClient>,
    IDisposable
{
    static Config Common(string repoRootFolder, Option<string> sendGridKey) =>
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
    
    static K<Eff<Runtime>, FileIO> Has<Eff<Runtime>, FileIO>.Ask =>
        pure<Eff<Runtime>, FileIO>(LanguageExt.Sys.Live.Implementations.FileIO.Default);

    static K<Eff<Runtime>, EncodingIO> Has<Eff<Runtime>, EncodingIO>.Ask => 
        pure<Eff<Runtime>, EncodingIO>(LanguageExt.Sys.Live.Implementations.EncodingIO.Default);

    static K<Eff<Runtime>, DirectoryIO> Has<Eff<Runtime>, DirectoryIO>.Ask =>
        pure<Eff<Runtime>, DirectoryIO>(LanguageExt.Sys.Live.Implementations.DirectoryIO.Default);

    static K<Eff<Runtime>, ConsoleIO> Has<Eff<Runtime>, ConsoleIO>.Ask =>
        pure<Eff<Runtime>, ConsoleIO>(LanguageExt.Sys.Live.Implementations.ConsoleIO.Default);

    static K<Eff<Runtime>, JsonIO> Has<Eff<Runtime>, JsonIO>.Ask => 
        pure<Eff<Runtime>, JsonIO>(Impl.Json.Default); 

    static K<Eff<Runtime>, EmailIO> Has<Eff<Runtime>, EmailIO>.Ask => 
        pure<Eff<Runtime>, EmailIO>(Impl.Email.Default); 

    static K<Eff<Runtime>, WebIO> Has<Eff<Runtime>, WebIO>.Ask =>
        pure<Eff<Runtime>, WebIO>(Impl.Web.Default);

    static K<Eff<Runtime>, ImageIO> Has<Eff<Runtime>, ImageIO>.Ask =>
        pure<Eff<Runtime>, ImageIO>(Impl.Image.Default);

    static K<Eff<Runtime>, Config> Has<Eff<Runtime>, Config>.Ask { get; } = 
        liftEff<Runtime, Config>(rt => rt.Env.Config);

    static K<Eff<Runtime>, HttpClient> Has<Eff<Runtime>, HttpClient>.Ask { get; } = 
        liftEff<Runtime, HttpClient>(rt => rt.Env.HttpClient);

    public void Dispose() =>
        Env.Dispose();
}

