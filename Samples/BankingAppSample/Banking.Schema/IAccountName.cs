using System;

namespace Banking.Schema
{
    /// <summary>
    /// Common base for AccountName and PersonName
    /// </summary>
    public interface IAccountName
    {
    }

    /// <summary>
    /// Matching for AccountNames
    /// </summary>
    public static class IAccountNameExtensions
    {
        public static R Match<R>(this IAccountName self, Func<PersonName, R> Person, Func<AccountName, R> Account) =>
            self is PersonName p  ? Person(p)
          : self is AccountName a ? Account(a)
          : throw new NotSupportedException();
    }
}
