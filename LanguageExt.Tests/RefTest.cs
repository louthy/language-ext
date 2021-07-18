using System;
using System.Linq;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    public class Account
    {
        public readonly int Balance;

        Account(int balance) =>
            Balance = balance;

        public Account SetBalance(int value) =>
            new Account(value);

        public Account AddBalance(int value) =>
            new Account(Balance + value);

        public Account Deposit(int value) =>
            new Account(Balance + value);

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
            sync(() =>
            {
                from.Value = from.Value.AddBalance(-amount);
                to.Value   = to.Value.AddBalance(amount);
            });
    }


    public class RefTest
    {
        [Fact]
        public void SimpleBankBalanceTest()
        {
            var accountA = Account.New(200);
            var accountB = Account.New(0);

            Transfer.Do(accountA, accountB, 100);

            var (balanceA, balanceB) = sync(() => (accountA.Value.Balance, accountB.Value.Balance));

            Assert.True(balanceA == balanceB);
        }

        [Fact]
        public void CommuteTest()
        {
            const int count = 1000;

            static int inc(Ref<int> counter) => 
                sync(() => commute(counter, static x => x + 1));

            var num = Ref(0);

            var res = Range(0, count).AsParallel()
                                     .Select(_ => inc(num))
                                     .ToSeq()
                                     .Strict();

            Assert.True(num == count);
        }

        [Fact]
        static void DepositCommuteTest()
        {
            var bank = Account.New(0);

            LogDeposit(bank, 100);
            LogDeposit(bank, 50);

            Assert.True(bank.Value.Balance == 150);

            var bank2 = Account.New(0);

            LogDeposit(bank2, 50);
            LogDeposit(bank2, 100);

            Assert.True(bank2.Value.Balance == 150);

        }

        static void LogDeposit(Ref<Account> account, int amount) =>
            sync(() => commute(account, a => a.Deposit(amount)));
    }
}
