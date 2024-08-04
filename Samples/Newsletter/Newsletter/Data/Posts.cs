using System.Text.Json;
using Newsletter.Effects;
using Newsletter.Effects.Traits;

namespace Newsletter.Data;

public static class Posts<RT>
    where RT : 
        Has<Eff<RT>, JsonIO>,
        Has<Eff<RT>, FileIO>,
        Has<Eff<RT>, WebIO>,
        Has<Eff<RT>, EncodingIO>,
        Has<Eff<RT>, DirectoryIO>,
        Reads<Eff<RT>, RT, HttpClient>,
        Reads<Eff<RT>, RT, Config>
{
    /// <summary>
    /// Read the latest post from the Ghost API
    /// </summary>
    public static Eff<RT, Post> readFromApi =>
        readAllFromApi.Bind(p => p.Head()
                                  .Match(Some: SuccessEff<RT, Post>,
                                         None: () => Fail(Error.New("no posts found"))));

    /// <summary>
    /// Read the last n posts from the Ghost API
    /// </summary>
    public static Eff<RT, Seq<Post>> readLastFromApi(int n) =>
        readAllFromApi.Map(ps => ps.Take(n).ToSeq());

    /// <summary>
    /// Read all posts from the Ghost API
    /// </summary>
    public static Eff<RT, EnumerableM<Post>> readAllFromApi =>
        from root    in Config<RT>.siteUrl
        from apiKey  in Config<RT>.contentApiKey
        from text    in Web<RT>.downloadText(makePostsUrl(root, apiKey))
        from element in getPostsElementForApiResult(text)
        from posts   in readPostsFromText(element)
        select posts;

    static K<Eff<RT>, string> readFirstFile(string folder) =>
        from fs in Directory<Eff<RT>, RT>.enumerateFiles(folder, "*.json")
                                         .Bind(fs => fs.OrderDescending()
                                         .AsEnumerableM()
                                         .Take(1)
                                         .Traverse(File<Eff<RT>, RT>.readAllText))
        from rs in fs.Head() switch
        {
            { IsSome: true, Case: string text } => SuccessEff<RT, string>(text),
            _ => Fail<Error>(Error.New($"no JSON posts file found in {folder}"))
        }
        select rs;

    static Eff<RT, JsonElement> getPostsElementForApiResult(string text) =>
        Json<RT>.readJson(text)
                .Map(doc => doc.RootElement
                               .Get("posts"));

    static Eff<RT, JsonElement> getPostsElementForFile(string text) =>
        Json<RT>.readJson(text)
                .Map(doc => doc.RootElement
                               .Get("db")
                               .EnumerateArray()
                               .FirstOrDefault()
                               .Get("data")
                               .Get("posts"));

    static Eff<RT, EnumerableM<Post>> readPostsFromText(JsonElement postsElement) =>
        postsElement.Enumerate()
                    .Traverse(readPost)
                    .Map(posts => posts.OrderDescending()
                                       .AsEnumerableM()
                                       .Filter(p => p.IsPublic))
                    .As();

    static Eff<RT, Post> readPost(JsonElement element) =>
        Config<RT>.siteUrl.Map(siteUrl =>
        {
            // Acquire
            var id = element.Get("id").GetString() ?? throw new Exception("failed to read post 'id'");
            var title = element.Get("title").GetString() ?? throw new Exception("failed to read post 'title'");
            var slug = element.Get("slug").GetString() ?? throw new Exception("failed to read post 'slug'");
            var html = element.Get("html").GetString() ?? throw new Exception("failed to read post 'html'");
            var plaintext = element.Get("plaintext").GetString() ?? throw new Exception("failed to read post 'plaintext'");
            var featureImage = element.Get("feature_image")
                                      .GetString()
                                     ?.Replace("__GHOST_URL__", siteUrl)
                                     ?? null;
            var excerpt     = element.Get("custom_excerpt").GetString() ?? "";
            var publishedAt = element.Get("published_at").GetDateTime();

            var emailRecFilter = element.TryGetProperty("email_recipient_filter", out var value1) switch
                                 { 
                                     true => value1.GetString() == "all",
                                     _    => true
                                 };  
            
            var status = element.TryGetProperty("status", out var value2) switch
                                 { 
                                     true => value2.GetString() == "published",
                                     _    => true
                                 };  
            
            var isPublic = emailRecFilter && status;
            
            // Fix up
            var url = new Uri($"{siteUrl}/{slug}");
            var featureImageUrl = featureImage == null ? null : new Uri(featureImage);
            html = fixupText(html, siteUrl);
            plaintext = fixupText(plaintext, siteUrl);
            
            return new Post(id, title, url, html, plaintext, excerpt, Optional(featureImageUrl), publishedAt, isPublic);
        });
    
    static string fixupText(string text, string siteUrl) =>
        text.Replace("\\n", "\n")
            .Replace("\\\"", "\"")
            .Replace("\\\\", "\\")
            .Replace("<code class=\"language-csharp\"", 
                     "<code class=\"language-csharp\" style=\"color: darkblue\"") 
            .Replace("<code>", 
                     "<code style=\"color: darkblue; background-color:#eee\">") 
            .Replace("__GHOST_URL__", siteUrl);

    /// <summary>
    /// Generate the Uri for the /posts API for the Ghost CMS platform 
    /// </summary>
    static Uri makePostsUrl(string root, string apiKey) =>
        new ($"{root}/ghost/api/content/posts/?key={apiKey}&formats=html,plaintext");
}

public record Post(
    string Id,
    string Title,
    Uri Url,
    string Html,
    string PlainText,
    string Excerpt,
    Option<Uri> FeatureImageUrl,
    DateTime PublishedAt,
    bool IsPublic) : IComparable<Post>
{
    public int CompareTo(Post? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return PublishedAt.CompareTo(other.PublishedAt);
    }
}
