using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Parsec
{
    public static class OperatorIO
    {
        public static OperatorIO<T> Infix<T>(Assoc assoc, Parser<char, Func<T, T, T>> p) =>
            new InfixOpIO<T>(assoc, p);

        public static OperatorIO<T> Prefix<T>(Parser<char, Func<T, T>> p) =>
            new PrefixOpIO<T>(p);

        public static OperatorIO<T> Postfix<T>(Parser<char, Func<T, T>> p) =>
            new PostfixOpIO<T>(p);
    }

    public abstract class OperatorIO<T>
    {
        public readonly OperatorTag Tag;

        public OperatorIO(OperatorTag tag)
        {
            Tag = tag;
        }

        public abstract Tuple<Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T>>>, Lst<Parser<char, Func<T, T>>>> SplitOp(
            Tuple<Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T>>>, Lst<Parser<char, Func<T, T>>>> state);
    }

    public class InfixOpIO<T> : OperatorIO<T>
    {
        public readonly Assoc Assoc;
        public readonly Parser<char, Func<T, T, T>> Op;

        internal InfixOpIO(Assoc assoc, Parser<char, Func<T, T, T>> p)
            :
            base(OperatorTag.Infix)
        {
            Assoc = assoc;
            Op = p;
        }

        public override Tuple<Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T>>>, Lst<Parser<char, Func<T, T>>>> SplitOp(
            Tuple<Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T>>>, Lst<Parser<char, Func<T, T>>>> state) =>
            state.Map(
                (rassoc, lassoc, nassoc, prefix, postfix) =>
                    Assoc == Assoc.None ? Tuple(rassoc, lassoc, Op.Cons(nassoc), prefix, postfix)
                  : Assoc == Assoc.Left ? Tuple(rassoc, Op.Cons(lassoc), nassoc, prefix, postfix)
                  : Tuple(Op.Cons(rassoc), lassoc, nassoc, prefix, postfix));
    }

    public class PrefixOpIO<T> : OperatorIO<T>
    {
        public readonly Parser<char, Func<T, T>> Op;

        internal PrefixOpIO(Parser<char, Func<T, T>> p)
            :
            base(OperatorTag.Prefix)
        {
            Op = p;
        }

        public override Tuple<Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T>>>, Lst<Parser<char, Func<T, T>>>> SplitOp(
            Tuple<Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T>>>, Lst<Parser<char, Func<T, T>>>> state) =>
            state.Map(
                (rassoc, lassoc, nassoc, prefix, postfix) =>
                    Tuple(rassoc, lassoc, nassoc, Op.Cons(prefix), postfix));

    }

    public class PostfixOpIO<T> : OperatorIO<T>
    {
        public readonly Parser<char, Func<T, T>> Op;

        internal PostfixOpIO(Parser<char, Func<T, T>> p)
            :
            base(OperatorTag.Postfix)
        {
            Op = p;
        }

        public override Tuple<Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T>>>, Lst<Parser<char, Func<T, T>>>> SplitOp(
            Tuple<Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T>>>, Lst<Parser<char, Func<T, T>>>> state) =>
            state.Map(
                (rassoc, lassoc, nassoc, prefix, postfix) =>
                    Tuple(rassoc, lassoc, nassoc, prefix, Op.Cons(postfix)));
    }
}
