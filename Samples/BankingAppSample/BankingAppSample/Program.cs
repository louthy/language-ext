using Banking.Schema;
using static Banking.Schema.NewTypes;
using static Banking.Schema.BankingFree;
using LanguageExt;
using System;
using Banking.Core;

namespace BankingAppSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var bank = Bank.Empty;

            var action = from accountId in CreateAccount(new PersonName(Title("Mr"), FirstName("Paul"), Surname("Louth")))
                         from amount1   in Deposit(Amount(100m), accountId)
                         from balance1  in Balance(accountId)
                         from amount2   in Withdraw(Amount(50m), accountId)
                         from balance2  in Balance(accountId)
                         select accountId;

            var result = Interpreter.Interpret(action, bank);
        }
    }
}