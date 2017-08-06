using LanguageExt;

namespace Banking.Schema
{
    /// <summary>
    /// Name of a non-human account
    /// </summary>
    public class AccountName : Record<AccountName>, IAccountName
    {
        public readonly string Name;

        public AccountName(string name) =>
            Name = name;
    }
}
