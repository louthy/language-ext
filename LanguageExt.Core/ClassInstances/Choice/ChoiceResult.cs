using System;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct ChoiceResult<A> : Choice<Result<A>, Exception, A>
    {
        public bool IsBottom(Result<A> choice)
        {
            throw new NotImplementedException();
        }

        public bool IsChoice1(Result<A> choice)
        {
            throw new NotImplementedException();
        }

        public bool IsChoice2(Result<A> choice)
        {
            throw new NotImplementedException();
        }

        public bool IsUnsafe(Result<A> choice)
        {
            throw new NotImplementedException();
        }

        public C Match<C>(Result<A> choice, Func<Exception, C> Choice1, Func<A, C> Choice2, Func<C> Bottom = null)
        {
            throw new NotImplementedException();
        }

        public Unit Match(Result<A> choice, Action<Exception> Choice1, Action<A> Choice2, Action Bottom = null)
        {
            throw new NotImplementedException();
        }

        public C MatchUnsafe<C>(Result<A> choice, Func<Exception, C> Choice1, Func<A, C> Choice2, Func<C> Bottom = null)
        {
            throw new NotImplementedException();
        }
    }
}
