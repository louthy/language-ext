using Newsletter.Effects.Traits;

namespace Newsletter.Effects;

/// <summary>
/// Web downloader
/// </summary>
public static class Web<M, RT>
    where RT : 
        Has<M, WebIO>,
        Has<M, EncodingIO>,
        Reads<M, RT, HttpClient>
    where M :
        Monad<M>,
        Stateful<M, RT>
{
    static readonly K<M, WebIO> trait =
        Stateful.getsM<M, RT, WebIO>(rt => ((Has<M, WebIO>)rt).Trait);

    static readonly K<M, HttpClient> client =
        Stateful.getsM<M, RT, HttpClient>(rt => rt.Get);

    public static K<M, string> downloadText(Uri uri) =>
        (from en in Enc<M, RT>.encoding
         from bs in download(uri)
         select en.GetString(bs));
    
    public static K<M, string> downloadBase64(Uri uri) =>
        download(uri).Map(Convert.ToBase64String);
    
    public static K<M, byte[]> download(Uri uri) =>
        from c in client
        from t in trait
        from r in liftIO(() => t.Download(uri, c))
        select r;
}
