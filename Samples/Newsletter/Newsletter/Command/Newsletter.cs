using System.Text;
using System.Web;
using Newsletter.Data;
using Newsletter.Effects;

namespace Newsletter.Command;

public static class Newsletter<M, RT>
    where RT : 
        Has<M, FileIO>,
        Has<M, EncodingIO>,
        Reads<M, RT, Config>,
        Reads<M, RT, HttpClient>
    where M :
        Monad<M>,
        Fallible<M>,
        Stateful<M, RT>
{
    /// <summary>
    /// Builds the newsletter
    /// </summary>
    public static K<M, Letter> make(Seq<Post> posts, Templates templates) =>
        M.Pure(Newsletter.make(posts, templates));
    
    /// <summary>
    /// Saves the newsletter as HTML and plain-text to the letters folder
    /// </summary>
    public static K<M, Unit> save(Letter letter) =>
        from f in Config<M, RT>.lettersFolder
        from h in File<M, RT>.writeAllText(Path.Combine(f, $"letter-{letter.PublishedAt:yyyy-MM-dd}.html"), letter.Html)
        from t in File<M, RT>.writeAllText(Path.Combine(f, $"letter-{letter.PublishedAt:yyyy-MM-dd}.txt"), letter.PlainText)
        select unit;
}

public static class Newsletter
{
    /// <summary>
    /// Builds the newsletter
    /// </summary>
    public static Letter make(Seq<Post> posts, Templates templates)
    {
        var post = posts[0];
        var recentHtml = new StringBuilder();
        var recentText = new StringBuilder();
        
        foreach(var recent in posts.Tail)
        {
            var rhtml = templates.RecentItem
                                         .Html
                                         .Replace("[TITLE]", HttpUtility.HtmlEncode(recent.Title))
                                         .Replace("[EXCERPT]", HttpUtility.HtmlEncode(recent.Excerpt))
                                         .Replace("[URL]", recent.Url.ToString());
            
            if (recent.FeatureImageUrl)
            {
                rhtml = rhtml.Replace("[IMAGE_URL]", ((Uri)recent.FeatureImageUrl).ToString());
            }
            recentHtml.Append(rhtml);
            
            recentText.Append(templates.RecentItem
                                       .PlainText
                                       .Replace("[TITLE]", recent.Title)
                                       .Replace("[EXCERPT]", recent.Excerpt)
                                       .Replace("[URL]", recent.Url.ToString()));
        }

        var yearRange = DateTime.Now.Year switch
                        {
                            2024  => "2024",
                            var y => $"2024 - {y}"
                        };
        
        var html = templates.Email
                            .Html
                            .Replace("[POST_CONTENT]", post.Html)
                            .Replace("[POST_DATE]", $"{post.PublishedAt.Day} {monthToString(post.PublishedAt.Month)} {post.PublishedAt.Year}")
                            .Replace("[POST_URL]", post.Url.ToString())
                            .Replace("[POST_TITLE]", HttpUtility.HtmlEncode(post.Title))
                            .Replace("[YEAR_RANGE]", yearRange)
                            .Replace("[RECENT_ARTICLES]", recentHtml.ToString());

        if (post.FeatureImageUrl)
        {
            html = html.Replace("[FEATURE_IMAGE_URL]", ((Uri)post.FeatureImageUrl).ToString());
        }

        var text = templates.Email
                            .PlainText
                            .Replace("[POST_CONTENT]", post.PlainText)
                            .Replace("[POST_DATE]", $"{post.PublishedAt.Day} {monthToString(post.PublishedAt.Month)} {post.PublishedAt.Year}")
                            .Replace("[POST_URL]", post.Url.ToString())
                            .Replace("[POST_TITLE]", post.Title)
                            .Replace("[YEAR_RANGE]", yearRange)
                            .Replace("[RECENT_ARTICLES]", recentText.ToString());

        return new Letter(post.Title, html, text, post.PublishedAt);
    }

    /// <summary>
    /// Convert month integer to a string
    /// </summary>
    static string monthToString(int month) =>
        month switch
        {
            1  => "Jan",
            2  => "Feb",
            3  => "Mar",
            4  => "Apr",
            5  => "May",
            6  => "Jun",
            7  => "Jul",
            8  => "Aug",
            9  => "Sep",
            10 => "Oct",
            11 => "Nov",
            12 => "Dec",
            _  => "?"
        };
}
