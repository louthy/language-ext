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
            atomic(() =>
            {
                from.Value = from.Value.AddBalance(-amount);
                to.Value   = to.Value.AddBalance(amount);
            });
    }


    public class RefTest
    {
        [Fact]
        public void BankBalanceChangeTest()
        {
            var accountA = Account.New(200);
            var accountB = Account.New(0);

            var stateA = Option<Account>.None;
            var stateB = Option<Account>.None;
            var changedA = 0;
            var changedB = 0;
            accountA.Change += v => { stateA = v; changedA++; };
            accountB.Change += v => { stateB = v; changedB++; };

            atomic(() =>
            {
                accountA.Value = accountA.Value.AddBalance(-50);
                accountB.Value = accountB.Value.AddBalance(50);
                accountA.Value = accountA.Value.AddBalance(-5);
                accountB.Value = accountB.Value.AddBalance(5);

                Assert.Equal(None, stateA);
                Assert.Equal(None, stateB);
            });

            Assert.Equal(Some(accountA.Value), stateA);
            Assert.Equal(Some(accountB.Value), stateB);
            Assert.Equal(1, changedA);
            Assert.Equal(1, changedB);
        }

        [Fact]
        public void SimpleBankBalanceTest()
        {
            var accountA = Account.New(200);
            var accountB = Account.New(0);

            Transfer.Do(accountA, accountB, 100);

            var (balanceA, balanceB) = atomic(() => (accountA.Value.Balance, accountB.Value.Balance));

            Assert.True(balanceA == balanceB);
        }

        [Fact]
        public void CommuteTest()
        {
            const int count = 1000;

            static int inc(Ref<int> counter) => 
                atomic(() => commute(counter, static x => x + 1));

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
            atomic(() => commute(account, a => a.Deposit(amount)));
    }
}
