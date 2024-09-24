namespace Newsletter.Data;

/// <summary>
/// Member record
/// </summary>
public record Member(string Id, string Email, string Name, bool SubscribedToEmails, bool Supporter);
