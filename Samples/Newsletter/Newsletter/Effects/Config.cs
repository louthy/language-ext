namespace Newsletter.Effects;

/// <summary>
/// Application configuration
/// </summary>
public static class Config<RT>
    where RT :
        Reads<Eff<RT>, RT, Config>
{
    static readonly Eff<RT, Config> trait =
        Stateful.getsM<Eff<RT>, RT, Config>(e => e.Get).As();

    /// <summary>
    /// Location of the exported member list
    /// </summary>
    public static Eff<RT, string> membersFolder =>
        trait.Map(t => t.MembersFolder);

    /// <summary>
    /// Location of the HTML and plain-text templates
    /// </summary>
    public static Eff<RT, string> templateFolder =>
        trait.Map(t => t.TemplateFolder);

    /// <summary>
    /// Location to save the newsletters to
    /// </summary>
    public static Eff<RT, string> lettersFolder =>
        trait.Map(t => t.LettersFolder);

    /// <summary>
    /// https://paullouth.com site-url
    /// </summary>
    public static Eff<RT, string> siteUrl =>
        trait.Map(t => t.SiteUrl);

    /// <summary>
    /// Maximum size of the feature image
    /// </summary>
    public static Eff<RT, int> featureImageWidth =>
        trait.Map(t => t.FeatureImageWidth);

    /// <summary>
    /// GhostCMS API key (you need to generate this in the Admin console of GhostCMS)
    /// </summary>
    public static Eff<RT, string> contentApiKey =>
        trait.Map(t => t.ContentApiKey);

    /// <summary>
    /// Api key for SendGrid!
    /// </summary>
    public static Eff<RT, string> sendGridApiKey =>
        from k in trait.Map(t => t.SendGridApiKey)
        from _ in when(k.IsNone, FailEff<RT, Unit>((Error)"SendGrid key not set.  No emails will be sent"))
        select (string)k;
}
