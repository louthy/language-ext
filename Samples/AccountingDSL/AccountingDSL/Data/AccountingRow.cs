using LanguageExt;

namespace AccountingDSL.Data
{
    public class AccountingRow : Record<AccountingRow>
    {
        public readonly string Type;
        public readonly string Id;
        public readonly int Amount;
        public AccountingRow(string type, string id, int amount)
        {
            Type = type;
            Id = id;
            Amount = amount;
        }
    }
}
