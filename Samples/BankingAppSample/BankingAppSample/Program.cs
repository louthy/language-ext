using Banking.Core;
using Banking.Schema;
using static Banking.Schema.Constructors;
using static Banking.Schema.BankingFree;
using static LanguageExt.Prelude;
using LanguageExt;
using System;

namespace BankingAppSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var bank = Bank.Empty;

            var action = from accountId in CreateAccount(PersonName(Title("Mr"), FirstName("Paul"), Surname("Louth")))
                         from _1        in ShowBalance(accountId)
                         from amount1   in Deposit(Amount(100m), accountId)
                         from _2        in ShowBalance(accountId)
                         from amount2   in Withdraw(Amount(75m), accountId)
                         from _3        in ShowBalance(accountId)
                         from _4        in ShowTransactions()
                         select accountId;

            var result = Interpreter.Interpret(action, bank);
        }

        /// These functions demonstrates that once you have captured all of the
        /// actions that represent an interaction with the 'world' (i.e IO,
        /// databases, global state, etc.) then you can compose those actions
        /// without having to add new types to the BankingFree monad.


        static BankingFree<Unit> ShowBalance(AccountId id) =>
            from ac in AccountDetails(id)
            from ba in Balance(id)
            from _1 in Show($"Balance of account {ac.Name} is: ${ba}")
            from wa in WithdrawalAccountDetails
            from _2 in Show($"\tBalance of {wa.Name} is: ${wa.Balance}")
            from da in DepositAccountDetails
            from _3 in Show($"\tBalance of {da.Name} is: ${da.Balance}")
            select unit;

        static BankingFree<Unit> ShowTransactions() =>
            from t in BankTransactions
            from _ in ShowTransactions(t)
            select unit;

        static BankingFree<Unit> ShowTransactions(Seq<Transaction> transactions) =>
            transactions.Match(
                ()      => Return(unit),
                t       => Show(t),
                (t, ts) => from a in ShowTransactions(ts)
                           from b in Show(t)
                           select unit);
    }
}