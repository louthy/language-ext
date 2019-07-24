using LanguageExt;
using static LanguageExt.Prelude;
using static Banking.Schema.Constructors;
using System;

namespace Banking.Schema
{
    /// <summary>
    /// Data structure that represents a bank.  In real life this would obviously
    /// be a persistent store and not just an in-memory data-structure.
    /// </summary>
    public class Bank
    {
        public static readonly Bank Empty = new Bank(
            ((AccountId(1), Account(AccountId(1), AccountName("Withdrawals"), Amount.Zero)),
             (AccountId(2), Account(AccountId(2), AccountName("Deposits"), Amount.Zero))),
            Seq<Transaction>(), 
            AccountId(1), 
            AccountId(2));

        public readonly Map<AccountId, Account> Accounts;
        public readonly Seq<Transaction> Transactions;
        public readonly AccountId WithdrawalAccountId;
        public readonly AccountId DepositedAccountId;

        /// <summary>
        /// Bank constructor
        /// </summary>
        Bank(Map<AccountId, Account> accounts, Seq<Transaction> transactions, AccountId withdrawalAccountId, AccountId depositedAccountId)
        {
            Accounts = accounts;
            Transactions = transactions;
            WithdrawalAccountId = withdrawalAccountId;
            DepositedAccountId = depositedAccountId;
        }

        /// <summary>
        /// Adds a transaction to the ledger
        /// </summary>
        /// <param name="transaction">Transaction to add</param>
        /// <returns>New Bank state</returns>
        public Either<Error, Bank> AddTransaction(Transaction transaction) =>
            With(Transactions: transaction.Cons(Transactions));

        /// <summary>
        /// Add a new account
        /// </summary>
        /// <param name="account">Account to add</param>
        /// <returns>Either an Error or the new Bank state</returns>
        public Either<Error, Bank> AddAccount(Account account) =>
            Accounts.ContainsKey(account.Id)
                ? Left<Error, Bank>(Error.New("Account ID already exists"))
                : With(Accounts: Accounts.Add(account.Id, account));

        /// <summary>
        /// Update an account
        /// </summary>
        public Either<Error, Bank> UpdateAccount(Account account) =>
            (from _ in Accounts.Find(account.Id)
             select With(Accounts: Accounts.SetItem(account.Id, account)))
            .ToEither(Error.New("Account ID doesn't exist"));

        /// <summary>
        /// Record 'mutation'
        /// </summary>
        Bank With(
            Map<AccountId, Account>? Accounts = null,
            Seq<Transaction> Transactions = null) =>
            new Bank(
                Accounts ?? this.Accounts,
                Transactions ?? this.Transactions,
                WithdrawalAccountId,
                DepositedAccountId);
    }
}
