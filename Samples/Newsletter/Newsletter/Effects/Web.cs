using Newsletter.Effects.Traits;

namespace Newsletter.Effects;

/// <summary>
/// Web downloader
/// </summary>
public static class Web<RT>
    where RT : Has<Eff<RT>, WebIO>,
               Has<Eff<RT>, EncodingIO>,
               Reads<Eff<RT>, RT, HttpClient> 
{
    static readonly Eff<RT, WebIO> trait =
        Stateful.getsM<Eff<RT>, RT, WebIO>(rt => ((Has<Eff<RT>, WebIO>)rt).Trait).As();

    static readonly Eff<RT, HttpClient> client =
        Stateful.getsM<Eff<RT>, RT, HttpClient>(rt => rt.Get).As();

    public static Eff<RT, string> downloadText(Uri uri) =>
        (from en in Enc<Eff<RT>, RT>.encoding
         from bs in download(uri)
         select en.GetString(bs)).As();
    
    public static Eff<RT, string> downloadBase64(Uri uri) =>
        download(uri).Map(Convert.ToBase64String);
    
    public static Eff<RT, byte[]> download(Uri uri) =>
        from c in client
        from t in trait
        from r in liftIO(() => t.Download(uri, c))
        select r;
}
