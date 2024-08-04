namespace Newsletter.Data;

public record Letter(
    string Title,
    string Html, 
    string PlainText, 
    DateTime PublishedAt);
