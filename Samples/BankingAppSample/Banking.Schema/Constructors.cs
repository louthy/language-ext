using System;

namespace Banking.Schema
{
    /// <summary>
    /// Using static Banking.Schema.Constructors to get type construction without `new`
    /// </summary>
    public static class Constructors
    {
        public static AccountId AccountId(int value) => new AccountId(value);
        public static Amount Amount(decimal value) => new Amount(value);
        public static Title Title(string value) => new Title(value);
        public static Surname Surname(string value) => new Surname(value);
        public static FirstName FirstName(string value) => new FirstName(value);
        public static Error Error(string value) => new Error(value);
        public static Account Account(AccountId id, IAccountName name, Amount balance) => new Account(id, name, balance);
        public static AccountName AccountName(string name) => new AccountName(name);
        public static PersonName PersonName(Title title, FirstName name, Surname surname) => new PersonName(title, name, surname);
        public static Transaction Transaction(AccountId debit, AccountId credit, Amount amount, DateTime date) => new Transaction(debit, credit, amount, date);
    }
}
