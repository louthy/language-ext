using LanguageExt;
using System;

namespace Banking.Schema
{
    /// <summary>
    /// Represents a transaction.  Which is the debitting of monies 
    /// from one account and the crediting of another.
    /// </summary>
    public class Transaction : Record<Transaction>
    {
        public readonly AccountId Debit;
        public readonly AccountId Credit;
        public readonly Amount Amount;
        public readonly DateTime Date;

        public Transaction(AccountId debit, AccountId credit, Amount amount, DateTime date)
        {
            Credit = credit;
            Debit = debit;
            Amount = amount;
            Date = date;
        }
    }
}
