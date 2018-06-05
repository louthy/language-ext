using System;

namespace LanguageExt.Parsec
{
    public static partial class Operator
    {
        public static Operator<I, O> Infix<I, O>(Assoc assoc, Parser<I, Func<O, O, O>> p) =>
            new InfixOp<I, O>(assoc, p);

        public static Operator<I, O> Prefix<I, O>(Parser<I, Func<O, O>> p) =>
            new PrefixOp<I, O>(p);

        public static Operator<I, O> Postfix<I, O>(Parser<I, Func<O, O>> p) =>
            new PostfixOp<I, O>(p);
    }

    public abstract class Operator<I, O>
    {
        public readonly OperatorTag Tag;

        public Operator(OperatorTag tag)
        {
            Tag = tag;
        }

        public abstract (Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O>>>, Seq<Parser<I, Func<O, O>>>) SplitOp(
            (Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O>>>, Seq<Parser<I, Func<O, O>>>) state);
    }

    public class InfixOp<I, O> : Operator<I, O>
    {
        public readonly Assoc Assoc;
        public readonly Parser<I, Func<O, O, O>> Op;

        internal InfixOp(Assoc assoc, Parser<I, Func<O, O, O>> p)
            :
            base(OperatorTag.Infix)
        {
            Assoc = assoc;
            Op = p;
        }

        public override (Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O>>>, Seq<Parser<I, Func<O, O>>>) SplitOp(
            (Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O>>>, Seq<Parser<I, Func<O, O>>>) state) =>
            state.Map(
                (rassoc, lassoc, nassoc, prefix, postfix) =>
                    Assoc == Assoc.None ? (rassoc, lassoc, Op.Cons(nassoc), prefix, postfix)
                  : Assoc == Assoc.Left ? (rassoc, Op.Cons(lassoc), nassoc, prefix, postfix)
                  : (Op.Cons(rassoc), lassoc, nassoc, prefix, postfix));
    }

    public class PrefixOp<I, O> : Operator<I, O>
    {
        public readonly Parser<I, Func<O, O>> Op;

        internal PrefixOp(Parser<I, Func<O, O>> p)
            :
            base(OperatorTag.Prefix)
        {
            Op = p;
        }

        public override (Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O>>>, Seq<Parser<I, Func<O, O>>>) SplitOp(
            (Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O>>>, Seq<Parser<I, Func<O, O>>>) state) =>
            state.Map(
                (rassoc, lassoc, nassoc, prefix, postfix) =>
                    (rassoc, lassoc, nassoc, Op.Cons(prefix), postfix));

    }

    public class PostfixOp<I, O> : Operator<I, O>
    {
        public readonly Parser<I, Func<O, O>> Op;

        internal PostfixOp(Parser<I, Func<O, O>> p)
            :
            base(OperatorTag.Postfix)
        {
            Op = p;
        }

        public override (Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O>>>, Seq<Parser<I, Func<O, O>>>) SplitOp(
            (Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O>>>, Seq<Parser<I, Func<O, O>>>) state) =>
            state.Map(
                (rassoc, lassoc, nassoc, prefix, postfix) =>
                    (rassoc, lassoc, nassoc, prefix, Op.Cons(postfix)));
    }
}
