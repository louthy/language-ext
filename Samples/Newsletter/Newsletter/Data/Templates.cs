using Newsletter.Effects;
namespace Newsletter.Data;

/// <summary>
/// Loads the templates for the emails
/// </summary>
public static class Templates<M, RT>
    where RT : 
        Has<M, EncodingIO>,
        Has<M, FileIO>,
        Has<M, DirectoryIO>,
        Reads<M, RT, Config>
    where M :
        Monad<M>,
        Fallible<M>,
        Stateful<M, RT>
{
    public static K<M, Templates> loadDefault =>
        from folder in Config<M, RT>.templateFolder
        from email in loadTemplate("email")
        from recnt in loadTemplate("recent-item")
        select new Templates(email, recnt);
    
    public static K<M, Template> loadTemplate(string name) =>
        from fldr in Config<M, RT>.templateFolder
        from html in File<M, RT>.readAllText(Path.Combine(fldr, $"{name}.html"))
        from text in File<M, RT>.readAllText(Path.Combine(fldr, $"{name}.txt"))
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
