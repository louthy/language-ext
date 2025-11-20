namespace Newsletter.Data;

/// <summary>
/// Collection of all the templates
/// </summary>
public record Templates(Template Email, Template RecentItem);

/// <summary>
/// Single template
/// </summary>
public record Template(string Html, string PlainText);
