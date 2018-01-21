using System;
using LanguageExt;
using Banking.Schema;
using static LanguageExt.Prelude;
using static Banking.Schema.Constructors;

namespace Banking.Core
{
    public static class Interpreter
    {
        public static Either<Error, (A, Bank)> Interpret<A>(BankingFree<A> dsl, Bank bank) =>
            dsl is BankingFree<A>.Return r                    ? Right<Error, (A, Bank)>((r.Value, bank))
          : dsl is BankingFree<A>.CreateAccount ca            ? CreateAccount(ca, bank)
          : dsl is BankingFree<A>.AccountDetails ga           ? GetAccountDetails(ga, bank)
          : dsl is BankingFree<A>.WithdrawalAccountDetails wa ? GetWithdrawalAccountDetails(wa, bank)
          : dsl is BankingFree<A>.DepositAccountDetails dd    ? GetDepositAccountDetails(dd, bank)
          : dsl is BankingFree<A>.BankTransactions bt         ? GetBankTransactions(bt, bank)
          : dsl is BankingFree<A>.Accounts ac                 ? Accounts(ac, bank)
          : dsl is BankingFree<A>.Balance ba                  ? Balance(ba, bank)
          : dsl is BankingFree<A>.Transfer tr                 ? Transfer(tr, bank)
          : dsl is BankingFree<A>.Withdraw wd                 ? Withdraw(wd, bank)
          : dsl is BankingFree<A>.Deposit de                  ? Deposit(de, bank)
          : dsl is BankingFree<A>.Show sh                     ? Show(sh, bank)
          : throw new NotSupportedException();

        static Either<Error, (A, Bank)> CreateAccount<A>(BankingFree<A>.CreateAccount action, Bank bank)
        {
            var id = AccountId(bank.Accounts.Count + 1);
            return bank.AddAccount(Account(id, action.Name, Schema.Amount.Zero))
                       .Bind(b => Interpret(action.Next(id), b));
        }

        static Either<Error, (A, Bank)> GetAccountDetails<A>(BankingFree<A>.AccountDetails action, Bank bank) =>
            bank.Accounts.Find(action.AccountId)
                         .ToEither(Error("Account doesn't exist"))
                         .Bind(a => Interpret(action.Next(a), bank));

        static Either<Error, (A, Bank)> GetWithdrawalAccountDetails<A>(BankingFree<A>.WithdrawalAccountDetails action, Bank bank) =>
            GetAccountDetails(new BankingFree<A>.AccountDetails(bank.WithdrawalAccountId, action.Next), bank);

        static Either<Error, (A, Bank)> GetDepositAccountDetails<A>(BankingFree<A>.DepositAccountDetails action, Bank bank) =>
            GetAccountDetails(new BankingFree<A>.AccountDetails(bank.DepositedAccountId, action.Next), bank);

        static Either<Error, (A, Bank)> GetBankTransactions<A>(BankingFree<A>.BankTransactions action, Bank bank) =>
            Right<Error, Seq<Transaction>>(bank.Transactions)
                .Bind(t => Interpret(action.Next(t), bank));

        static Either<Error, (A, Bank)> Accounts<A>(BankingFree<A>.Accounts action, Bank bank) =>
            Interpret(action.Next(bank.Accounts), bank);

        static Either<Error, (A, Bank)> Balance<A>(BankingFree<A>.Balance action, Bank bank) =>
            (from a in bank.Accounts.Find(action.Account).ToEither(Error("Account doesn't exist"))
             select a.Balance)
            .Bind(a => Interpret(action.Next(a), bank));

        static Either<Error, (A, Bank)> Transfer<A>(BankingFree<A>.Transfer action, Bank bank) =>
            (from debAc in bank.Accounts.Find(action.From).ToEither(Error("Debit account doesn't exist"))
             from crdAc in bank.Accounts.Find(action.To).ToEither(Error("Credit account doesn't exist"))
             from bank2 in bank.UpdateAccount(debAc.Debit(action.Amount))
             from bank3 in bank2.UpdateAccount(crdAc.Credit(action.Amount))
             from bank4 in bank3.AddTransaction(Transaction(action.From, action.To, action.Amount, DateTime.UtcNow))
             select bank4)
            .Bind(b => Interpret(action.Next(action.Amount), b));

        static Either<Error, (A, Bank)> Withdraw<A>(BankingFree<A>.Withdraw action, Bank bank) =>
            Transfer(new BankingFree<A>.Transfer(action.Amount, action.Account, bank.WithdrawalAccountId, action.Next), bank);

        static Either<Error, (A, Bank)> Deposit<A>(BankingFree<A>.Deposit action, Bank bank) =>
            Transfer(new BankingFree<A>.Transfer(action.Amount, bank.DepositedAccountId, action.Account, action.Next), bank);

        static Either<Error, (A, Bank)> Show<A>(BankingFree<A>.Show action, Bank bank)
        {
            Console.WriteLine((action?.Value ?? "").ToString());
            return Interpret(action.Next(unit), bank);
        }
    }
}
