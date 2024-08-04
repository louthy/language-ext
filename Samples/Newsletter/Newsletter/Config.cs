namespace Newsletter;

public record Config(
        string MembersFolder,         // Location of the exported member list  
        string TemplateFolder,        // Location of the HTML and plain-text templates  
        string LettersFolder,         // Location to save the newsletters to
        string SiteUrl,               // https://paullouth.com site-url
        int FeatureImageWidth,        // Maximum size of the feature image
        string ContentApiKey,         // GhostCMS API key (you need to generate this in the Admin console of GhostCMS)
        Option<string> SendGridApiKey // Api key for SendGrid!  
    );            


