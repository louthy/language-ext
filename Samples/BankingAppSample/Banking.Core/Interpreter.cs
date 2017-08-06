using System;
using Banking.Schema;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Banking.Core
{
    public static class Interpreter
    {
        public static Either<Error, (A, Bank)> Interpret<A>(BankingFree<A> dsl, Bank bank) =>
            dsl is BankingFree<A>.Return r         ? Right<Error, (A, Bank)>((r.Value, bank))
          : dsl is BankingFree<A>.CreateAccount ca ? CreateAccount(ca, bank)
          : dsl is BankingFree<A>.Accounts ac      ? Accounts(ac, bank)
          : dsl is BankingFree<A>.Balance ba       ? Balance(ba, bank)
          : dsl is BankingFree<A>.Transfer tr      ? Transfer(tr, bank)
          : dsl is BankingFree<A>.Withdraw wd      ? Withdraw(wd, bank)
          : dsl is BankingFree<A>.Deposit de       ? Deposit(de, bank)
          : throw new NotSupportedException();

        static Either<Error, (A, Bank)> CreateAccount<A>(BankingFree<A>.CreateAccount action, Bank bank)
        {
            var id = AccountId.New(bank.Accounts.Count + 1);
            return bank.AddAccount(new Account(id, action.Name, Amount.Zero))
                       .Bind(b => Interpret(action.Next(id), b));
        }

        static Either<Error, (A, Bank)> Accounts<A>(BankingFree<A>.Accounts action, Bank bank) =>
            Interpret(action.Next(bank.Accounts), bank);

        static Either<Error, (A, Bank)> Balance<A>(BankingFree<A>.Balance action, Bank bank) =>
            (from a in bank.Accounts.Find(action.Account).ToEither(Error.New("Account doesn't exist"))
             select a.Balance)
            .Bind(a => Interpret(action.Next(a), bank));

        static Either<Error, (A, Bank)> Transfer<A>(BankingFree<A>.Transfer action, Bank bank) =>
            (from debAc in bank.Accounts.Find(action.From).ToEither(Error.New("Debit account doesn't exist"))
             from crdAc in bank.Accounts.Find(action.To).ToEither(Error.New("Credit account doesn't exist"))
             from bank2 in bank.UpdateAccount(debAc.Debit(action.Amount))
             from bank3 in bank2.UpdateAccount(crdAc.Credit(action.Amount))
             from bank4 in bank3.AddTransaction(new Transaction(action.From, action.To, action.Amount, DateTime.UtcNow))
             select bank4)
            .Bind(b => Interpret(action.Next(action.Amount), b));

        static Either<Error, (A, Bank)> Withdraw<A>(BankingFree<A>.Withdraw action, Bank bank) =>
            Transfer(new BankingFree<A>.Transfer(action.Amount, action.Account, bank.WithdrawalAccountId, action.Next), bank);

        static Either<Error, (A, Bank)> Deposit<A>(BankingFree<A>.Deposit action, Bank bank) =>
            Transfer(new BankingFree<A>.Transfer(action.Amount, bank.DepositedAccountId, action.Account, action.Next), bank);
    }
}
