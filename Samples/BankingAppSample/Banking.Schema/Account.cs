using LanguageExt;
using System;

namespace Banking.Schema
{
    /// <summary>
    /// Individual account in a bank
    /// </summary>
    public class Account : Record<Account>
    {
        public readonly AccountId Id;
        public readonly IAccountName Name;
        public readonly Amount Balance;

        public Account(AccountId id, IAccountName name, Amount balance)
        {
            Id = id;
            Name = name;
            Balance = balance;
        }

        public Account Credit(Amount amount) =>
            new Account(Id, Name, Balance + amount);

        public Account Debit(Amount amount) =>
            new Account(Id, Name, Balance - amount);
    }
}
