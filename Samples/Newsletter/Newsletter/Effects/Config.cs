namespace Newsletter.Effects;

/// <summary>
/// Config record
/// </summary>
public record Config(
    string MembersFolder,         // Location of the exported member list  
    string TemplateFolder,        // Location of the HTML and plain-text templates  
    string LettersFolder,         // Location to save the newsletters to
    string SiteUrl,               // https://paullouth.com site-url
    int FeatureImageWidth,        // Maximum size of the feature image
    string ContentApiKey,         // GhostCMS API key (you need to generate this in the Admin console of GhostCMS)
    Option<string> SendGridApiKey // Api key for SendGrid!  
);            

/// <summary>
/// Application configuration
/// </summary>
public static class Config<M, RT>
    where RT :
        Has<M, Config>
    where M :
        Monad<M>,
        Fallible<M>
{
    static readonly K<M, Config> configIO =
        RT.Ask;

    /// <summary>
    /// Location of the exported member list
    /// </summary>
    public static K<M, string> membersFolder =>
        configIO.Map(t => t.MembersFolder);

    /// <summary>
    /// Location of the HTML and plain-text templates
    /// </summary>
    public static K<M, string> templateFolder =>
        configIO.Map(t => t.TemplateFolder);

    /// <summary>
    /// Location to save the newsletters to
    /// </summary>
    public static K<M, string> lettersFolder =>
        configIO.Map(t => t.LettersFolder);

    /// <summary>
    /// https://paullouth.com site-url
    /// </summary>
    public static K<M, string> siteUrl =>
        configIO.Map(t => t.SiteUrl);

    /// <summary>
    /// Maximum size of the feature image
    /// </summary>
    public static K<M, int> featureImageWidth =>
        configIO.Map(t => t.FeatureImageWidth);

    /// <summary>
    /// GhostCMS API key (you need to generate this in the Admin console of GhostCMS)
    /// </summary>
    public static K<M, string> contentApiKey =>
        configIO.Map(t => t.ContentApiKey);

    /// <summary>
    /// Api key for SendGrid!
    /// </summary>
    public static K<M, string> sendGridApiKey =>
        from k in configIO.Map(t => t.SendGridApiKey)
        from _ in when(k.IsNone, error<M>((Error)"SendGrid key not set.  No emails will be sent"))
        select (string)k;
}
