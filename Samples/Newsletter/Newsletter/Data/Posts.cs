namespace Newsletter.Data;

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
