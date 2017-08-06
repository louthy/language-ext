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
                         select accountId;

            var result = Interpreter.Interpret(action, bank);
        }

        /// <summary>
        /// This function demonstrates that once you have captured all of the
        /// actions that represent an interaction with the 'world' (i.e IO,
        /// databases, global state, etc.) then you can compose those actions
        /// without having to add new types to the BankingFree monad.
        /// </summary>
        static BankingFree<Unit> ShowBalance(AccountId id) =>
            from a in AccountDetails(id)
            from v in Balance(id)
            from _ in Show($"Balance of account {a.Name} is: ${v}")
            select unit;
    }
}