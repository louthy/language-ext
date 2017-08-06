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
        public AccountId Debit;
        public AccountId Credit;
        public Amount Amount;
        public DateTime Date;

        public Transaction(AccountId debit, AccountId credit, Amount amount, DateTime date)
        {
            Credit = credit;
            Debit = debit;
            Amount = amount;
            Date = date;
        }
    }
}
