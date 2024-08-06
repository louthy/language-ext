using Newsletter.Effects.Traits;

namespace Newsletter.Effects;

/// <summary>
/// Web downloader
/// </summary>
public static class Web<M, RT>
    where RT : 
        Has<M, WebIO>,
        Has<M, EncodingIO>,
        Has<M, HttpClient>
    where M :
        Monad<M>
{
    static K<M, WebIO> trait =>
        Has<M, RT, WebIO>.ask;

    static K<M, HttpClient> client =>
        Has<M, RT, HttpClient>.ask;

    public static K<M, string> downloadText(Uri uri) =>
        from en in Enc<M, RT>.encoding
        from bs in download(uri)
        select en.GetString(bs);
    
    public static K<M, string> downloadBase64(Uri uri) =>
        download(uri).Map(Convert.ToBase64String);
    
    public static K<M, byte[]> download(Uri uri) =>
        from c in client
        from t in trait
        from r in liftIO(() => t.Download(uri, c))
        select r;
}
