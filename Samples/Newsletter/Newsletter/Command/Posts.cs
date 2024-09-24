using System.Text.Json;
using Newsletter.Data;
using Newsletter.Effects;
using Newsletter.Effects.Traits;

namespace Newsletter.Command;

public static class Posts<M, RT>
    where RT : 
        Has<M, WebIO>,
        Has<M, JsonIO>,
        Has<M, FileIO>,
        Has<M, EncodingIO>,
        Has<M, DirectoryIO>,
        Has<M, Config>,
        Has<M, HttpClient>
    where M :
        Monad<M>,
        Fallible<M>
{
    /// <summary>
    /// Read the latest post from the Ghost API
    /// </summary>
    public static K<M, Post> readFromApi =>
        readAllFromApi.Bind(
            p => p.Head()
                  .Match(Some: M.Pure,
                         None: M.Fail<Post>(Error.New("no posts found"))));

    /// <summary>
    /// Read the last n posts from the Ghost API
    /// </summary>
    public static K<M, Seq<Post>> readLastFromApi(int n) =>
        readAllFromApi.Map(ps => ps.Take(n).ToSeq());

    /// <summary>
    /// Read all posts from the Ghost API
    /// </summary>
    public static K<M, Iterable<Post>> readAllFromApi =>
        from root    in Config<M, RT>.siteUrl
        from apiKey  in Config<M, RT>.contentApiKey
        from text    in Web<M, RT>.downloadText(makePostsUrl(root, apiKey))
        from element in getPostsElementForApiResult(text)
        from posts   in readPostsFromText(element)
        select posts;

    static K<M, string> readFirstFile(string folder) =>
        from fs in Directory<M, RT>.enumerateFiles(folder, "*.json")
                                   .Bind(fs => fs.OrderDescending()
                                                 .AsIterable()
                                                 .Take(1)
                                                 .Traverse(File<M, RT>.readAllText))
        from rs in fs.ToSeq() switch
                   {
                       [var text, ..] => M.Pure(text),
                       _              => M.Fail<string>(Error.New($"no JSON posts file found in {folder}"))
                   }
        select rs;

    static K<M, JsonElement> getPostsElementForApiResult(string text) =>
        Json<M, RT>.readJson(text)
                   .Map(doc => doc.RootElement
                                  .Get("posts"));

    static K<M, JsonElement> getPostsElementForFile(string text) =>
        Json<M, RT>.readJson(text)
                   .Map(doc => doc.RootElement
                                  .Get("db")
                                  .EnumerateArray()
                                  .FirstOrDefault()
                                  .Get("data")
                                  .Get("posts"));

    static K<M, Iterable<Post>> readPostsFromText(JsonElement postsElement) =>
        postsElement.Enumerate()
                    .Traverse(readPost)
                    .Map(posts => posts.OrderDescending()
                                       .AsIterable()
                                       .Filter(p => p.IsPublic));

    static K<M, Post> readPost(JsonElement element) =>
        Config<M, RT>.siteUrl.Map(siteUrl =>
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
