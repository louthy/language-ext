using System;
using System.Linq;
using LanguageExt;
using AccountingDSL.Data;
using static LanguageExt.Prelude;
using static AccountingDSL.ExpressionCompiler.Compiler;
using AccountingDSL.ScriptParsing;
using Microsoft.CodeAnalysis.CSharp;

namespace AccountingDSL.DSL
{
    /// <summary>
    /// Factory type for generating Transform&lt;A&gt; values
    /// </summary>
    public static class Transform
    {
        public static Transform<A> Return<A>(A value) =>
            new Transform<A>.Return(value);

        public static Transform<A> Fail<A>(string value) =>
            new Transform<A>.Fail(value);

        public static Transform<Unit> Log(string value) =>
            new Transform<Unit>.Log(value, Return);

        public static Transform<object> Invoke(string func, object[] args) =>
            new Transform<object>.Invoke(func, args, Return);

        public static Transform<Seq<AccountingRow>> AllRows =
            new Transform<Seq<AccountingRow>>.AllRows(Return);

        public static Transform<Seq<AccountingRow>> FilterRows(string value) =>
            new Transform<Seq<AccountingRow>>.FilterRows(value, Return);

        public static Transform<Unit> SetValue(string name, object value) =>
            new Transform<Unit>.SetValue(name, value, Return);

        public static Transform<object> GetValue(string name) =>
            new Transform<object>.GetValue(name, Return);

        public static Transform<Unit> Compute(string id, string @operator, string sourceType, string remarks) =>
            Compute(new ComputeOperation(id, @operator, sourceType, remarks));

        public static Transform<Unit> Compute(ComputeOperation op)
        {
            var sourceType = from expr in Parse(op.SourceType)
                             from type in op.Operator == "Expression"
                                  ? from value in Compile<object>(expr)
                                    select new ValueSourceType(value) as SourceType
                                  : from rows in FilterRowsBy(expr)
                                    select new RowsSourceType(rows) as SourceType
                             select type;

            return from type in sourceType
                   from compute in new Transform<Unit>.Compute(op, type, Return)
                   select compute;
        }

        public static Transform<Unit> Print(string remarks, Seq<string> messages) =>
            Print(new PrintOperation(remarks, messages));

        public static Transform<Unit> Print(PrintOperation op)
        {
            var messages = op.Messages
                             .Map(DecorateWithQuotes)
                             .Map(Parse)
                             .Map(msg => from e in msg
                                         from r in Compile<string>(e)
                                         select r);

            return from msgs in Sequence(messages)
                   from compute in new Transform<Unit>.Print(op, msgs, Return)
                   select compute;
        }

        static string DecorateWithQuotes(string x) =>
            SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(x)).ToString();

        static Transform<Seq<AccountingRow>> FilterRowsBy(ScriptExpr expr) =>
            expr is IdentifierExpr ident
                ? FilterRows(ident.Value)
                : Fail<Seq<AccountingRow>>($"Invalid SourceType: {expr} for non-Expression operation");

        static Transform<ScriptExpr> Parse(string script) =>
            ScriptParser.Parse(script).Match(
                Right: e => Return(e),
                Left: er => Fail<ScriptExpr>(er + " at " + script));

        static Transform<Seq<A>> Sequence<A>(Seq<Transform<A>> seq) =>
            seq.IsEmpty
                ? Return(Seq<A>())
                : from x in seq.Head
                  from xs in Sequence(seq.Tail)
                  select Prelude.Cons(x, xs);
    }
}
