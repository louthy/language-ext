using LanguageExt;
using LanguageExt.ClassInstances;
using LanguageExt.ClassInstances.Const;
using LanguageExt.ClassInstances.Pred;
using System;

namespace Banking.Schema
{
    public class AccountId : NewType<AccountId, int, GreaterThan<TInt, int, I0>> { public AccountId(int value) : base(value) { } }
    public class Title : NewType<Title, string, StrLen<I1, I40>> { public Title(string value) : base(value) { } }
    public class Surname : NewType<Surname, string, StrLen<I1, I400>> { public Surname(string value) : base(value) { } }
    public class FirstName : NewType<FirstName, string, StrLen<I1, I400>> { public FirstName(string value) : base(value) { } }
    public class Error : NewType<Error, string> { public Error(string value) : base(value) { } }

    public class Amount : FloatType<Amount, TDecimal, decimal>
    {
        public static Amount Zero = new Amount(0);
        public Amount(decimal value) : base(value) { }
    }
}
