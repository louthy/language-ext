using System;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using AccountingDSL.DSL;
using AccountingDSL.ScriptParsing;
using static AccountingDSL.DSL.Transform;

namespace AccountingDSL.ExpressionCompiler
{

    public static class Compiler
    {
        public static Transform<A> Compile<A>(ScriptExpr expr) =>
              expr is ParensExpr parens ? Compile<A>(parens.Expr)
            : expr is InvokeExpr invoke ? CompileInvoke<A>(invoke)
            : expr is ConstExpr constant ? CompileConst<A>(constant)
            : expr is InterpStringExpr interp ? CompileInterpolatedString(interp).Cast<string, A>()
            : expr is IdentifierExpr ident ? CompileIdentifierValue<A>(ident)
            : expr is BooleanBinaryExpr boolean ? CompileBooleanBinary(boolean).Cast<bool, A>()
            : expr is BooleanUnaryExpr uboolean ? CompileBooleanUnary(uboolean).Cast<bool, A>()
            : expr is BinaryExpr binary ? CompileBinary<A>(binary)
            : expr is UnaryExpr unary ? CompileUnary<A>(unary)
            : expr is LogExpr log ? CompileLog<A>(log)
            : expr is FailExpr fail ? CompileFail<A>(fail)
            : NotSupported<A>(expr);

        static Transform<A> CompileInvoke<A>(InvokeExpr invoke) =>
            from fun in Return(invoke.Ident)
            from args in EvalArgsSeq(Seq(invoke.Arguments.Map(Compile<object>).ToArray()))
            from res in Invoke(fun, args.ToArray())
            select (A)res;

        static Transform<A> CompileFail<A>(FailExpr fail) =>
            from m in Compile<object>(fail.Expr).Map(x => x?.ToString() ?? "`fail` called with [null] expression")
            from f in Fail<A>(m)
            select f;

        static Transform<A> NotSupported<A>(ScriptExpr expr) =>
            Fail<A>($"Expression not supported: {expr}");

        static Transform<A> CompileLog<A>(LogExpr log) =>
            from m in Compile<object>(log.Message)
            from x in Log(m?.ToString() ?? "[null]")
            select m is A
                ? (A)m
                : default;

        static Transform<string> CompileInterpolatedString(InterpStringExpr interp) =>
            from p in EvalArgsSeq(interp.Parts.Map(Compile<object>))
            select String.Join("", p);

        static Transform<Seq<object>> EvalArgsSeq(Seq<Transform<object>> args) =>
            args.IsEmpty
                ? Return(Seq<object>())
                : from x in args.Head
                  from xs in EvalArgsSeq(args.Tail)
                  select Prelude.Cons(x, xs);

        static Transform<bool> CompileConstBoolean(ConstExpr<bool> constant) =>
            Return(constant.Value);

        static Transform<A> CompileConst<A>(ConstExpr constant) =>
            from x in Return(constant.GetValue())
            from c in x is A
                ? Return((A)x)
                : Fail<A>($"Expected constant of type {constant.ConstantType.Name}.  Got {x?.GetType()?.Name ?? "[null]"} instead")
            select c;

        static Transform<A> CompileUnary<A>(UnaryExpr unary) =>
              unary.Operator == "-" ? CompileNegate<A>(unary)
            : unary.Operator == "+" ? Compile<A>(unary)
            : NotSupported<A>(unary);

        static Transform<bool> CompileBooleanUnary(UnaryExpr unary) =>
              unary.Operator == "!" ? CompileNot(unary)
            : NotSupported<bool>(unary);

        static Transform<bool> CompileBooleanBinary(BinaryExpr binary) =>
              binary.Operator == "==" ? CompileEquals(binary)
            : binary.Operator == "!=" ? CompileNotEquals(binary)
            : binary.Operator == ">" ? CompileGreaterThan(binary)
            : binary.Operator == "<" ? CompileLessThan(binary)
            : binary.Operator == ">=" ? CompileGreaterThanOrEqualTo(binary)
            : binary.Operator == "<=" ? CompileLessThanOrEqualTo(binary)
            : binary.Operator == "||" ? CompileLogicalOr(binary)
            : binary.Operator == "&&" ? CompileLogicalAnd(binary)
            : NotSupported<bool>(binary);

        static Transform<A> CompileBinary<A>(BinaryExpr binary) =>
              binary.Operator == "-" ? CompileSubtract<A>(binary)
            : binary.Operator == "+" ? CompileAdd<A>(binary)
            : binary.Operator == "/" ? CompileDivide<A>(binary)
            : binary.Operator == "*" ? CompileMultiply<A>(binary)
            : binary.Operator == ";" ? CompileStatement<A>(binary)
            : binary.Operator == "=" ? CompileAssign<A>(binary)
            : binary.Operator == "==" ? CompileEquals(binary).Cast<bool, A>()
            : binary.Operator == "!=" ? CompileNotEquals(binary).Cast<bool, A>()
            : binary.Operator == ">" ? CompileGreaterThan(binary).Cast<bool, A>()
            : binary.Operator == "<" ? CompileLessThan(binary).Cast<bool, A>()
            : binary.Operator == ">=" ? CompileGreaterThanOrEqualTo(binary).Cast<bool, A>()
            : binary.Operator == "<=" ? CompileLessThanOrEqualTo(binary).Cast<bool, A>()
            : binary.Operator == "||" ? CompileLogicalOr(binary).Cast<bool, A>()
            : binary.Operator == "&&" ? CompileLogicalAnd(binary).Cast<bool, A>()
            : NotSupported<A>(binary);

        static Transform<B> Cast<A, B>(this Transform<A> ma) =>
            ma.Map(x => (B)Convert.ChangeType(x, typeof(B)));

        static Transform<IdentifierExpr> ResolveIdentifier(ScriptExpr e) =>
            from id in e is IdentifierExpr ident
                ? Return(ident)
                : Fail<IdentifierExpr>($"Expected identifier, got {e}")
            select id;

        static Transform<A> CompileAssign<A>(BinaryExpr binary) =>
            from x in ResolveIdentifier(binary.Left)
            from y in Compile<A>(binary.Right)
            from v in SetValue(x.Value, y)
            select y;

        static Transform<A> CompileIdentifierValue<A>(ScriptExpr e) =>
            from value in e is IdentifierExpr ident
                ? from x in GetValue(ident.Value)
                  from z in x is A
                      ? Return((A)x)
                      : Fail<A>($"Identifier type mismatch for '{ident.Value}' - expected {typeof(A).Name} but '{ident.Value}' is a {x?.GetType().Name ?? "[null]"}")
                  select z
                : Fail<A>($"Expected identifier, got {e}")
            select value;

        static Transform<A> CompileStatement<A>(BinaryExpr binary) =>
            from x in Compile<object>(binary.Left)
            from y in Compile<A>(binary.Right)
            select y;

        static Transform<A> CompileSubtract<A>(BinaryExpr binary) =>
            from x in Compile<A>(binary.Left)
            from y in Compile<A>(binary.Right)
            select (A)((dynamic)x - (dynamic)y);

        static Transform<A> CompileAdd<A>(BinaryExpr binary) =>
            from x in Compile<A>(binary.Left)
            from y in Compile<A>(binary.Right)
            select (A)((dynamic)x + (dynamic)y);

        static Transform<A> CompileDivide<A>(BinaryExpr binary) =>
            from x in Compile<A>(binary.Left)
            from y in Compile<A>(binary.Right)
            select (A)((dynamic)x / (dynamic)y);

        static Transform<A> CompileMultiply<A>(BinaryExpr binary) =>
            from x in Compile<A>(binary.Left)
            from y in Compile<A>(binary.Right)
            select (A)((dynamic)x * (dynamic)y);

        static Transform<bool> CompileEquals(BinaryExpr binary) =>
            from x in Compile<object>(binary.Left)
            from y in Compile<object>(binary.Right)
            select (bool)((dynamic)x == (dynamic)y);

        static Transform<bool> CompileNotEquals(BinaryExpr binary) =>
            from x in Compile<object>(binary.Left)
            from y in Compile<object>(binary.Right)
            select (bool)((dynamic)x != (dynamic)y);

        static Transform<bool> CompileLessThan(BinaryExpr binary) =>
            from x in Compile<object>(binary.Left)
            from y in Compile<object>(binary.Right)
            select (bool)((dynamic)x < (dynamic)y);

        static Transform<bool> CompileLessThanOrEqualTo(BinaryExpr binary) =>
            from x in Compile<object>(binary.Left)
            from y in Compile<object>(binary.Right)
            select (bool)((dynamic)x <= (dynamic)y);

        static Transform<bool> CompileGreaterThan(BinaryExpr binary) =>
            from x in Compile<object>(binary.Left)
            from y in Compile<object>(binary.Right)
            select (bool)((dynamic)x > (dynamic)y);

        static Transform<bool> CompileGreaterThanOrEqualTo(BinaryExpr binary) =>
            from x in Compile<object>(binary.Left)
            from y in Compile<object>(binary.Right)
            select (bool)((dynamic)x >= (dynamic)y);

        static Transform<bool> CompileLogicalOr(BinaryExpr binary) =>
            from x in Compile<bool>(binary.Left)
            from y in Compile<bool>(binary.Right)
            select (bool)((dynamic)x || (dynamic)y);

        static Transform<bool> CompileLogicalAnd(BinaryExpr binary) =>
            from x in Compile<bool>(binary.Left)
            from y in Compile<bool>(binary.Right)
            select (bool)((dynamic)x && (dynamic)y);

        static Transform<bool> CompileNot(UnaryExpr unary) =>
            from x in Compile<bool>(unary.Expr)
            select !x;

        static Transform<A> CompileNegate<A>(UnaryExpr unary) =>
            from x in Compile<A>(unary.Expr)
            select (A)(-(dynamic)x);
    }
}
