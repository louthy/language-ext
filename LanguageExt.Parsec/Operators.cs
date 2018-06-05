using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Parsec
{
    public enum Assoc
    {
        None,
        Left,
        Right
    }

    public enum OperatorTag
    {
        Infix,
        Prefix,
        Postfix
    }

    public static class Operator
    {
        public static Operator<T> Infix<T>(Assoc assoc, Parser<Func<T, T, T>> p) =>
            new InfixOp<T>(assoc, p);

        public static Operator<T> Prefix<T>(Parser<Func<T, T>> p) =>
            new PrefixOp<T>(p);

        public static Operator<T> Postfix<T>(Parser<Func<T, T>> p) =>
            new PostfixOp<T>(p);
    }

    public abstract class Operator<T>
    {
        public readonly OperatorTag Tag;

        public Operator(OperatorTag tag)
        {
            Tag = tag;
        }

        public abstract (Seq<Parser<Func<T,T,T>>>, Seq<Parser<Func<T, T, T>>>, Seq<Parser<Func<T, T, T>>>, Seq<Parser<Func<T, T>>>, Seq<Parser<Func<T, T>>>) SplitOp(
            (Seq<Parser<Func<T, T, T>>>, Seq<Parser<Func<T, T, T>>>, Seq<Parser<Func<T, T, T>>>, Seq<Parser<Func<T, T>>>, Seq<Parser<Func<T, T>>>) state);
    }

    public class InfixOp<T> : Operator<T>
    {
        public readonly Assoc Assoc;
        public readonly Parser<Func<T, T, T>> Op;

        internal InfixOp(Assoc assoc, Parser<Func<T, T, T>> p)
            :
            base(OperatorTag.Infix)
        {
            Assoc = assoc;
            Op = p;
        }

        public override (Seq<Parser<Func<T, T, T>>>, Seq<Parser<Func<T, T, T>>>, Seq<Parser<Func<T, T, T>>>, Seq<Parser<Func<T, T>>>, Seq<Parser<Func<T, T>>>) SplitOp(
            (Seq<Parser<Func<T, T, T>>>, Seq<Parser<Func<T, T, T>>>, Seq<Parser<Func<T, T, T>>>, Seq<Parser<Func<T, T>>>, Seq<Parser<Func<T, T>>>) state) =>
            state.Map(
                (rassoc, lassoc, nassoc, prefix, postfix) =>
                    Assoc == Assoc.None ? (rassoc, lassoc, Op.Cons(nassoc), prefix, postfix)
                  : Assoc == Assoc.Left ? (rassoc, Op.Cons(lassoc), nassoc, prefix, postfix)
                  : (Op.Cons(rassoc), lassoc, nassoc, prefix, postfix));

    }

    public class PrefixOp<T> : Operator<T>
    {
        public readonly Parser<Func<T, T>> Op;

        internal PrefixOp(Parser<Func<T, T>> p)
            :
            base(OperatorTag.Prefix)
        {
            Op = p;
        }

        public override (Seq<Parser<Func<T, T, T>>>, Seq<Parser<Func<T, T, T>>>, Seq<Parser<Func<T, T, T>>>, Seq<Parser<Func<T, T>>>, Seq<Parser<Func<T, T>>>) SplitOp(
            (Seq<Parser<Func<T, T, T>>>, Seq<Parser<Func<T, T, T>>>, Seq<Parser<Func<T, T, T>>>, Seq<Parser<Func<T, T>>>, Seq<Parser<Func<T, T>>>) state) =>
            state.Map(
                (rassoc, lassoc, nassoc, prefix, postfix) =>
                    (rassoc, lassoc, nassoc, Op.Cons(prefix), postfix));

    }

    public class PostfixOp<T> : Operator<T>
    {
        public readonly Parser<Func<T, T>> Op;

        internal PostfixOp(Parser<Func<T, T>> p)
            :
            base(OperatorTag.Postfix)
        {
            Op = p;
        }

        public override (Seq<Parser<Func<T, T, T>>>, Seq<Parser<Func<T, T, T>>>, Seq<Parser<Func<T, T, T>>>, Seq<Parser<Func<T, T>>>, Seq<Parser<Func<T, T>>>) SplitOp(
            (Seq<Parser<Func<T, T, T>>>, Seq<Parser<Func<T, T, T>>>, Seq<Parser<Func<T, T, T>>>, Seq<Parser<Func<T, T>>>, Seq<Parser<Func<T, T>>>) state) =>
            state.Map(
                (rassoc, lassoc, nassoc, prefix, postfix) =>
                    (rassoc, lassoc, nassoc, prefix, Op.Cons(postfix)));
    }
}
