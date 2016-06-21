using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Parsec
{
    public static class OperatorT
    {
        public static OperatorT<T> Infix<T>(Assoc assoc, Parser<char, Func<T, T, T>> p) =>
            new InfixOpT<T>(assoc, p);

        public static OperatorT<T> Prefix<T>(Parser<char, Func<T, T>> p) =>
            new PrefixOpT<T>(p);

        public static OperatorT<T> Postfix<T>(Parser<char, Func<T, T>> p) =>
            new PostfixOpT<T>(p);
    }

    public abstract class OperatorT<T>
    {
        public readonly OperatorTag Tag;

        public OperatorT(OperatorTag tag)
        {
            Tag = tag;
        }

        public abstract Tuple<Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T>>>, Lst<Parser<char, Func<T, T>>>> SplitOp(
            Tuple<Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T, T>>>, Lst<Parser<char, Func<T, T>>>, Lst<Parser<char, Func<T, T>>>> state);
    }

    public class InfixOpT<T> : OperatorT<T>
    {
        public readonly Assoc Assoc;
        public readonly Parser<char, Func<T, T, T>> Op;

        internal InfixOpT(Assoc assoc, Parser<char, Func<T, T, T>> p)
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

    public class PrefixOpT<T> : OperatorT<T>
    {
        public readonly Parser<char, Func<T, T>> Op;

        internal PrefixOpT(Parser<char, Func<T, T>> p)
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

    public class PostfixOpT<T> : OperatorT<T>
    {
        public readonly Parser<char, Func<T, T>> Op;

        internal PostfixOpT(Parser<char, Func<T, T>> p)
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
