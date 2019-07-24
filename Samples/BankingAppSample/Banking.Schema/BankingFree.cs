using LanguageExt;
using System;

namespace Banking.Schema
{
    /// <summary>
    /// IO independent operations for interacting with the bank
    /// </summary>
    public static class BankingFree
    {
        public static BankingFree<A> Return<A>(A value) => new BankingFree<A>.Return(value);
        public static BankingFree<AccountId> CreateAccount(PersonName name) => new BankingFree<AccountId>.CreateAccount(name, Return);
        public static BankingFree<Account> AccountDetails(AccountId accountId) => new BankingFree<Account>.AccountDetails(accountId, Return);
        public static BankingFree<Account> WithdrawalAccountDetails = new BankingFree<Account>.WithdrawalAccountDetails(Return);
        public static BankingFree<Account> DepositAccountDetails = new BankingFree<Account>.DepositAccountDetails(Return);
        public static BankingFree<Seq<Transaction>> BankTransactions = new BankingFree<Seq<Transaction>>.BankTransactions(Return);
        public static BankingFree<Map<AccountId, Account>> Accounts = new BankingFree<Map<AccountId, Account>>.Accounts(Return);
        public static BankingFree<Amount> Balance(AccountId accountId) => new BankingFree<Amount>.Balance(accountId, Return);
        public static BankingFree<Amount> Transfer(Amount amount, AccountId from, AccountId to) => new BankingFree<Amount>.Transfer(amount, from, to, Return);
        public static BankingFree<Amount> Withdraw(Amount amount, AccountId from) => new BankingFree<Amount>.Withdraw(from, amount, Return);
        public static BankingFree<Amount> Deposit(Amount amount, AccountId to) => new BankingFree<Amount>.Deposit(to, amount, Return);
        public static BankingFree<Unit> Show(object value) => new BankingFree<Unit>.Show(value, Return);
    }

    /// <summary>
    /// Makes the BankingFree<A> type work with LINQ
    /// </summary>
    public static class BankingFreeExtensions
    {
        public static BankingFree<B> Bind<A, B>(this BankingFree<A> ma, Func<A, BankingFree<B>> f) =>
            ma is BankingFree<A>.Return rt                   ? f(rt.Value)
          : ma is BankingFree<A>.CreateAccount ca            ? new BankingFree<B>.CreateAccount(ca.Name, n => ca.Next(n).Bind(f))
          : ma is BankingFree<A>.AccountDetails ga           ? new BankingFree<B>.AccountDetails(ga.AccountId, n => ga.Next(n).Bind(f))
          : ma is BankingFree<A>.WithdrawalAccountDetails wa ? new BankingFree<B>.WithdrawalAccountDetails(n => wa.Next(n).Bind(f))
          : ma is BankingFree<A>.DepositAccountDetails da    ? new BankingFree<B>.DepositAccountDetails(n => da.Next(n).Bind(f))
          : ma is BankingFree<A>.BankTransactions bt         ? new BankingFree<B>.BankTransactions(n => bt.Next(n).Bind(f))
          : ma is BankingFree<A>.Accounts ac                 ? new BankingFree<B>.Accounts(n => ac.Next(n).Bind(f))
          : ma is BankingFree<A>.Balance ba                  ? new BankingFree<B>.Balance(ba.Account, n => ba.Next(n).Bind(f))
          : ma is BankingFree<A>.Transfer tr                 ? new BankingFree<B>.Transfer(tr.Amount, tr.From, tr.To, n => tr.Next(n).Bind(f))
          : ma is BankingFree<A>.Withdraw wd                 ? new BankingFree<B>.Withdraw(wd.Account, wd.Amount, n => wd.Next(n).Bind(f))
          : ma is BankingFree<A>.Deposit dp                  ? new BankingFree<B>.Deposit(dp.Account, dp.Amount, n => dp.Next(n).Bind(f))
          : ma is BankingFree<A>.Show sh                     ? new BankingFree<B>.Show(sh.Value, n => sh.Next(n).Bind(f)) as BankingFree<B>
          : throw new NotSupportedException();

        public static BankingFree<B> Map<A, B>(this BankingFree<A> ma, Func<A, B> f) =>
            ma.Bind(a => BankingFree.Return(f(a)));

        public static BankingFree<B> Select<A, B>(this BankingFree<A> ma, Func<A, B> f) =>
            ma.Bind(a => BankingFree.Return(f(a)));

        public static BankingFree<C> SelectMany<A, B, C>(this BankingFree<A> ma, Func<A, BankingFree<B>> bind, Func<A, B, C> project) =>
            ma.Bind(a => bind(a).Select(b => project(a, b)));
    }

    /// <summary>
    /// Base class for operations that interact with the bank
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public abstract class BankingFree<A>
    {
        /// <summary>
        /// Identity type - simply returns the value provided
        /// </summary>
        public class Return : BankingFree<A>
        {
            public readonly A Value;
            public Return(A value) =>
                Value = value;
        }

        /// <summary>
        /// Represents an operation that creates a new bank account
        /// </summary>
        public class CreateAccount : BankingFree<A>
        {
            public readonly PersonName Name;
            public readonly Func<AccountId, BankingFree<A>> Next;
            public CreateAccount(PersonName name, Func<AccountId, BankingFree<A>> next)
            {
                Name = name;
                Next = next;
            }
        }

        /// <summary>
        /// Represents an operation that retrieves an account from its ID
        /// </summary>
        public class AccountDetails : BankingFree<A>
        {
            public readonly AccountId AccountId;
            public readonly Func<Account, BankingFree<A>> Next;
            public AccountDetails(AccountId accountId, Func<Account, BankingFree<A>> next)
            {
                AccountId = accountId;
                Next = next;
            }
        }

        /// <summary>
        /// Represents an operation that retrieves the withdrawals account
        /// </summary>
        public class WithdrawalAccountDetails : BankingFree<A>
        {
            public readonly Func<Account, BankingFree<A>> Next;
            public WithdrawalAccountDetails(Func<Account, BankingFree<A>> next)
            {
                Next = next;
            }
        }

        /// <summary>
        /// Represents an operation that retrieves the deposit account
        /// </summary>
        public class DepositAccountDetails : BankingFree<A>
        {
            public readonly Func<Account, BankingFree<A>> Next;
            public DepositAccountDetails(Func<Account, BankingFree<A>> next)
            {
                Next = next;
            }
        }

        /// <summary>
        /// Represents an operation that retrieves all of the transactions in the bank
        /// </summary>
        public class BankTransactions : BankingFree<A>
        {
            public readonly Func<Seq<Transaction>, BankingFree<A>> Next;
            public BankTransactions(Func<Seq<Transaction>, BankingFree<A>> next)
            {
                Next = next;
            }
        }

        /// <summary>
        /// Represents an operation that gets a list of all accounts
        /// </summary>
        public class Accounts : BankingFree<A>
        {
            public readonly Func<Map<AccountId, Account>, BankingFree<A>> Next;
            public Accounts(Func<Map<AccountId, Account>, BankingFree<A>> next) =>
                Next = next;
        }

        /// <summary>
        /// Represents an operation that provides the balance for the account requested
        /// </summary>
        public class Balance : BankingFree<A>
        {
            public readonly AccountId Account;
            public readonly Func<Amount, BankingFree<A>> Next;
            public Balance(AccountId account, Func<Amount, BankingFree<A>> next)
            {
                Account = account;
                Next = next;
            }
        }

        /// <summary>
        /// Represents an operation that transfers money from one account to another
        /// </summary>
        public class Transfer : BankingFree<A>
        {
            public readonly Amount Amount;
            public readonly AccountId From;
            public readonly AccountId To;
            public readonly Func<Amount, BankingFree<A>> Next;

            public Transfer(Amount amount, AccountId from, AccountId to, Func<Amount, BankingFree<A>> next)
            {
                Amount = amount;
                From = from;
                To = to;
                Next = next;
            }
        }

        /// <summary>
        /// Represents an operation that withdraws money from an account
        /// </summary>
        public class Withdraw : BankingFree<A>
        {
            public readonly AccountId Account;
            public readonly Amount Amount;
            public readonly Func<Amount, BankingFree<A>> Next;

            public Withdraw(AccountId account, Amount amount, Func<Amount, BankingFree<A>> next)
            {
                Account = account;
                Amount = amount;
                Next = next;
            }
        }

        /// <summary>
        /// Represents an operation that deposits money into an account
        /// </summary>
        public class Deposit : BankingFree<A>
        {
            public readonly AccountId Account;
            public readonly Amount Amount;
            public readonly Func<Amount, BankingFree<A>> Next;

            public Deposit(AccountId account, Amount amount, Func<Amount, BankingFree<A>> next)
            {
                Account = account;
                Amount = amount;
                Next = next;
            }
        }

        /// <summary>
        /// Represents an operation that credits an account
        /// </summary>
        public class CreditAccount : BankingFree<A>
        {
            public readonly AccountId Account;
            public readonly Amount Amount;
            public readonly Func<Amount, BankingFree<A>> Next;

            public CreditAccount(AccountId account, Amount amount, Func<Amount, BankingFree<A>> next)
            {
                Account = account;
                Amount = amount;
                Next = next;
            }
        }

        /// <summary>
        /// Represents an operation that logs a value
        /// </summary>
        public class Show : BankingFree<A>
        {
            public readonly object Value;
            public readonly Func<Unit, BankingFree<A>> Next;
            public Show(object value, Func<Unit, BankingFree<A>> next)
            {
                Value = value;
                Next = next;
            }
        }
    }
}
