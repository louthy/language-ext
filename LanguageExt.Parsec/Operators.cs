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

    public static partial class Operator
    {
        public static Operator<A> Infix<A>(Assoc assoc, Parser<Func<A, A, A>> p) =>
            new InfixOp<A>(assoc, p);

        public static Operator<A> Prefix<A>(Parser<Func<A, A>> p) =>
            new PrefixOp<A>(p);

        public static Operator<A> Postfix<A>(Parser<Func<A, A>> p) =>
            new PostfixOp<A>(p);
    }

    public abstract class Operator<A>
    {
        public readonly OperatorTag Tag;

        public Operator(OperatorTag tag)
        {
            Tag = tag;
        }

        public abstract (Seq<Parser<Func<A,A,A>>>, Seq<Parser<Func<A, A, A>>>, Seq<Parser<Func<A, A, A>>>, Seq<Parser<Func<A, A>>>, Seq<Parser<Func<A, A>>>) SplitOp(
            (Seq<Parser<Func<A, A, A>>>, Seq<Parser<Func<A, A, A>>>, Seq<Parser<Func<A, A, A>>>, Seq<Parser<Func<A, A>>>, Seq<Parser<Func<A, A>>>) state);
    }

    public class InfixOp<A> : Operator<A>
    {
        public readonly Assoc Assoc;
        public readonly Parser<Func<A, A, A>> Op;

        internal InfixOp(Assoc assoc, Parser<Func<A, A, A>> p)
            :
            base(OperatorTag.Infix)
        {
            Assoc = assoc;
            Op = p;
        }

        public override (Seq<Parser<Func<A, A, A>>>, Seq<Parser<Func<A, A, A>>>, Seq<Parser<Func<A, A, A>>>, Seq<Parser<Func<A, A>>>, Seq<Parser<Func<A, A>>>) SplitOp(
            (Seq<Parser<Func<A, A, A>>>, Seq<Parser<Func<A, A, A>>>, Seq<Parser<Func<A, A, A>>>, Seq<Parser<Func<A, A>>>, Seq<Parser<Func<A, A>>>) state) =>
            state.Map(
                (rassoc, lassoc, nassoc, prefix, postfix) =>
                    Assoc == Assoc.None ? (rassoc, lassoc, Op.Cons(nassoc), prefix, postfix)
                  : Assoc == Assoc.Left ? (rassoc, Op.Cons(lassoc), nassoc, prefix, postfix)
                  : (Op.Cons(rassoc), lassoc, nassoc, prefix, postfix));

    }

    public class PrefixOp<A> : Operator<A>
    {
        public readonly Parser<Func<A, A>> Op;

        internal PrefixOp(Parser<Func<A, A>> p)
            :
            base(OperatorTag.Prefix)
        {
            Op = p;
        }

        public override (Seq<Parser<Func<A, A, A>>>, Seq<Parser<Func<A, A, A>>>, Seq<Parser<Func<A, A, A>>>, Seq<Parser<Func<A, A>>>, Seq<Parser<Func<A, A>>>) SplitOp(
            (Seq<Parser<Func<A, A, A>>>, Seq<Parser<Func<A, A, A>>>, Seq<Parser<Func<A, A, A>>>, Seq<Parser<Func<A, A>>>, Seq<Parser<Func<A, A>>>) state) =>
            state.Map(
                (rassoc, lassoc, nassoc, prefix, postfix) =>
                    (rassoc, lassoc, nassoc, Op.Cons(prefix), postfix));

    }

    public class PostfixOp<A> : Operator<A>
    {
        public readonly Parser<Func<A, A>> Op;

        internal PostfixOp(Parser<Func<A, A>> p)
            :
            base(OperatorTag.Postfix)
        {
            Op = p;
        }

        public override (Seq<Parser<Func<A, A, A>>>, Seq<Parser<Func<A, A, A>>>, Seq<Parser<Func<A, A, A>>>, Seq<Parser<Func<A, A>>>, Seq<Parser<Func<A, A>>>) SplitOp(
            (Seq<Parser<Func<A, A, A>>>, Seq<Parser<Func<A, A, A>>>, Seq<Parser<Func<A, A, A>>>, Seq<Parser<Func<A, A>>>, Seq<Parser<Func<A, A>>>) state) =>
            state.Map(
                (rassoc, lassoc, nassoc, prefix, postfix) =>
                    (rassoc, lassoc, nassoc, prefix, Op.Cons(postfix)));
    }
}
