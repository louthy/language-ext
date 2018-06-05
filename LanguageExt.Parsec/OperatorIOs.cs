using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Parsec
{
    public static class OperatorIO
    {
        public static OperatorIO<I, O> Infix<I, O>(Assoc assoc, Parser<I, Func<O, O, O>> p) =>
            new InfixOpIO<I, O>(assoc, p);

        public static OperatorIO<I, O> Prefix<I, O>(Parser<I, Func<O, O>> p) =>
            new PrefixOpIO<I, O>(p);

        public static OperatorIO<I, O> Postfix<I, O>(Parser<I, Func<O, O>> p) =>
            new PostfixOpIO<I, O>(p);
    }

    public abstract class OperatorIO<I, O>
    {
        public readonly OperatorTag Tag;

        public OperatorIO(OperatorTag tag)
        {
            Tag = tag;
        }

        public abstract (Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O>>>, Seq<Parser<I, Func<O, O>>>) SplitOp(
            (Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O, O>>>, Seq<Parser<I, Func<O, O>>>, Seq<Parser<I, Func<O, O>>>) state);
    }

    public class InfixOpIO<I, O> : OperatorIO<I, O>
    {
        public readonly Assoc Assoc;
        public readonly Parser<I, Func<O, O, O>> Op;

        internal InfixOpIO(Assoc assoc, Parser<I, Func<O, O, O>> p)
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

    public class PrefixOpIO<I, O> : OperatorIO<I, O>
    {
        public readonly Parser<I, Func<O, O>> Op;

        internal PrefixOpIO(Parser<I, Func<O, O>> p)
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

    public class PostfixOpIO<I, O> : OperatorIO<I, O>
    {
        public readonly Parser<I, Func<O, O>> Op;

        internal PostfixOpIO(Parser<I, Func<O, O>> p)
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
