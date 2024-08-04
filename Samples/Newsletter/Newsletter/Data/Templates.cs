using Newsletter.Effects;
namespace Newsletter.Data;

/// <summary>
/// Loads the templates for the emails
/// </summary>
public static class Templates<RT>
    where RT : 
    Has<Eff<RT>, EncodingIO>,
    Has<Eff<RT>, FileIO>,
    Has<Eff<RT>, DirectoryIO>,
    Reads<Eff<RT>, RT, Config>
{
    public static Eff<RT, Templates> loadDefault =>
        from folder in Config<RT>.templateFolder
        from email in loadTemplate("email")
        from recnt in loadTemplate("recent-item")
        select new Templates(email, recnt);
    
    public static Eff<RT, Template> loadTemplate(string name) =>
        from fldr in Config<RT>.templateFolder
        from html in File<Eff<RT>, RT>.readAllText(Path.Combine(fldr, $"{name}.html"))
        from text in File<Eff<RT>, RT>.readAllText(Path.Combine(fldr, $"{name}.txt"))
        select new Template(html, text);
    
}

/// <summary>
/// Collection of all the templates
/// </summary>
public record Templates(Template Email, Template RecentItem);

/// <summary>
/// Single template
/// </summary>
public record Template(string Html, string PlainText);
