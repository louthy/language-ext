using System;
using LanguageExt;

namespace AccountingDSL.ScriptParsing
{
    /// <summary>
    /// Type that represents the root expression type for all expressions parsed by ScriptParser
    /// The static functions represent the full set of expressions that can be instantiated.
    /// </summary>
    public class ScriptExpr
    {
        public static ScriptExpr Args(ScriptExpr left, ScriptExpr right) => new TupleExpr(left, right);
        public static ScriptExpr Binary(ScriptExpr left, ScriptExpr right, string @operator) => new BinaryExpr(left, right, @operator);
        public static ScriptExpr BooleanBinary(ScriptExpr left, ScriptExpr right, string @operator) => new BooleanBinaryExpr(left, right, @operator);
        public static ScriptExpr BooleanPrefix(ScriptExpr expr, string @operator) => new BooleanUnaryExpr(true, expr, @operator);
        public static ScriptExpr Prefix(ScriptExpr expr, string @operator) => new UnaryExpr(true, expr, @operator);
        public static ScriptExpr Postfix(ScriptExpr expr, string @operator) => new UnaryExpr(false, expr, @operator);
        public static ScriptExpr Const<A>(A value) => new ConstExpr<A>(value);
        public static ScriptExpr ConstChar(char value) => new CharConstExpr(value);
        public static ScriptExpr ConstString(string value) => new StringConstExpr(value);
        public static ScriptExpr Invoke(string ident, Seq<ScriptExpr> args) => new InvokeExpr(ident, args);
        public static ScriptExpr InterpString(Seq<ScriptExpr> value) => new InterpStringExpr(value);
        public static ScriptExpr True = new ConstExpr<bool>(true);
        public static ScriptExpr False = new ConstExpr<bool>(false);
        public static ScriptExpr Ident(string value) => new IdentifierExpr(value);
        public static ScriptExpr Parens(ScriptExpr expr) => new ParensExpr(expr);
        public static ScriptExpr Log(ScriptExpr expr) => new LogExpr(expr);
        public static ScriptExpr Fail(ScriptExpr expr) => new FailExpr(expr);
    }

    public class InterpStringExpr : ScriptExpr
    {
        public Seq<ScriptExpr> Parts;
        public InterpStringExpr(Seq<ScriptExpr> parts) => Parts = parts;
    }
    public class FailExpr : ScriptExpr
    {
        public readonly ScriptExpr Expr;
        public FailExpr(ScriptExpr expr) => Expr = expr;
        public override string ToString() => $"fail({Expr})";
    }
    public class LogExpr : ScriptExpr
    {
        public readonly ScriptExpr Message;
        public LogExpr(ScriptExpr message) => Message = message;
        public override string ToString() => $"log({Message})";
    }
    public class TupleExpr : BinaryExpr
    {
        public TupleExpr(ScriptExpr left, ScriptExpr right) : base(left, right, ",") { }
    }
    public abstract class ConstExpr : ScriptExpr
    {
        public abstract object GetValue();
        public abstract Type ConstantType { get; }
    }
    public class ConstExpr<A> : ConstExpr
    {
        public readonly A Value;
        public ConstExpr(A value) =>
            Value = value;
        public override string ToString() =>
            Value.ToString();
        public override object GetValue() => Value;
        public override Type ConstantType => typeof(A);
    }
    public class CharConstExpr : ConstExpr<char>
    {
        public CharConstExpr(char value) : base(value)
        {
        }
        public override string ToString() =>
            Value.ToString();
    }
    public class StringConstExpr : ConstExpr<string>
    {
        public StringConstExpr(string value) : base(value)
        {
        }
        public override string ToString() =>
            Value.ToString();
    }
    public class ParensExpr : ScriptExpr
    {
        public readonly ScriptExpr Expr;
        public ParensExpr(ScriptExpr expr) =>
            Expr = expr;
        public override string ToString() =>
            $"({Expr})";
    }
    public class IdentifierExpr : ScriptExpr
    {
        public readonly string Value;
        public IdentifierExpr(string value) => Value = value;
        public override string ToString() => Value;
    }
    public class BinaryExpr : ScriptExpr
    {
        public readonly ScriptExpr Left;
        public readonly ScriptExpr Right;
        public readonly string Operator;
        public BinaryExpr(ScriptExpr left, ScriptExpr right, string @operator)
        {
            Left = left;
            Right = right;
            Operator = @operator;
        }
        public override string ToString() =>
            $"{Left} {Operator} {Right}";
    }
    public class BooleanBinaryExpr : BinaryExpr
    {
        public BooleanBinaryExpr(ScriptExpr left, ScriptExpr right, string @operator)
            : base(left, right, @operator) { }
    }
    public class UnaryExpr : ScriptExpr
    {
        public readonly new bool Prefix;
        public readonly ScriptExpr Expr;
        public readonly string Operator;

        public UnaryExpr(bool prefix, ScriptExpr expr, string @operator)
        {
            Prefix = prefix;
            Expr = expr;
            Operator = @operator;
        }
        public override string ToString() =>
            Prefix
                ? $"{Operator}{Expr}"
                : $"{Expr}{Operator}";
    }
    public class BooleanUnaryExpr : UnaryExpr
    {
        public BooleanUnaryExpr(bool prefix, ScriptExpr expr, string @operator) : base(prefix, expr, @operator)
        { }
    }
    public class InvokeExpr : ScriptExpr
    {
        public readonly new string Ident;
        public readonly Seq<ScriptExpr> Arguments;
        public InvokeExpr(string ident, Seq<ScriptExpr> arguments)
        {
            Ident = ident;
            Arguments = arguments;
        }
        public override string ToString() =>
            $"{Ident}({String.Join(", ", Arguments)})";
    }
}
