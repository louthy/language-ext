using System;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    public class Account
    {
        public int Balance;
        Account(int balance)
        {
            Balance = balance;
        }

        public Account SetBalance(int value) =>
            new Account(value);

        public static Ref<Account> New(int balance) =>
            Ref(new Account(balance), Account.Validate);

        public static bool Validate(Account a) =>
            a.Balance >= 0;

        public override string ToString() =>
            Balance.ToString();
    }

    public static class Transfer
    {
        public static Unit Do(Ref<Account> from, Ref<Account> to, int amount) =>
            dosync(() =>
            {
                from.Swap(a => a.SetBalance(a.Balance - amount));
                to.Swap(a => a.SetBalance(a.Balance + amount));
            });
    }


    public class RefTest
    {
        [Fact]
        public void SimpleBankBalanceTest()
        {
            var accountA = Account.New(100);
            var accountB = Account.New(100);

            Transfer.Do(accountA, accountB, 100);
        }
    }
}
