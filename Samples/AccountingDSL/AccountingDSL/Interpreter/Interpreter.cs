using System;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using AccountingDSL.DSL;
using AccountingDSL.Data;

namespace AccountingDSL.Interpreter
{
    public static class Interpreter
    {
        public static Either<string, A> Interpret<A>(Transform<A> ma, InterpreterState state)
        {
            switch (ma)
            {
                case Transform<A>.Return r: return Right(r.Value);
                case Transform<A>.Fail f: return Left(f.Value);
                case Transform<A>.Log m:
                    state.Output.OnNext(m.Value);
                    return Interpret(m.Next(unit), state);
                case Transform<A>.GetValue op:
                    return state.GetVar(op.Name)
                         .Match(Some: v => Interpret(op.Next(v), state),
                                None: () => Left($"Unknown value: {op.Name}"));
                case Transform<A>.SetValue op:
                    return Interpret(op.Next(unit), state.SetVar(op.Name, op.Value));
                case Transform<A>.Compute op:
                    return Compute(op, state);
                case Transform<A>.Print op:
                    return Print(op, state);
                case Transform<A>.FilterRows op:
                    return FilterRows(op, state);
                case Transform<A>.Invoke op:
                    return Invoke(op, state);
                default:
                    throw new NotImplementedException(ma.GetType().Name);
            }
        }

        static Either<string, A> Invoke<A>(Transform<A>.Invoke op, InterpreterState state)
        {
            try
            {
                var method = typeof(ScriptFunctions).GetMethod(op.Func);
                if (method == null) return $"Unknown function: {op.Func}";
                var parms = method.GetParameters();
                if (parms.Length != op.Args.Length) return $"Function '{op.Func}' expects {parms.Length} arguments, got {op.Args.Length}";
                return Interpret(op.Next((A)method.Invoke(null, op.Args)), state);
            }
            catch(Exception e)
            {
                return $"Error invoking function: {op.Func}, {e.Message}";
            }
        }

        static Either<string, A> FilterRows<A>(Transform<A>.FilterRows op, InterpreterState state) =>
            Interpret(op.Next(state.Rows.Filter(r => r.Type == op.Value)), state);

        static Either<string, A> Print<A>(Transform<A>.Print op, InterpreterState state)
        {
            op.Messages.Iter(state.Output.OnNext);
            return Interpret(op.Next(unit), state);
        }

        static Either<string, A> Compute<A>(Transform<A>.Compute op, InterpreterState state) =>
            op.Operation.Operator == "Sum" ? SumCompute(op, state)
          : op.Operation.Operator == "Expression" ? ExprCompute(op, state)
          : Left($"Unknown operator: {op.Operation.Operator}");

        static Either<string, A> ExprCompute<A>(Transform<A>.Compute op, InterpreterState state) =>
            op.SourceType is ValueSourceType sourceType
                ? Interpret(op.Next(unit), state.SetVar(op.Operation.Id, sourceType.Value))
                : Left("Invalid source type for Compute");

        static Either<string, A> SumCompute<A>(Transform<A>.Compute op, InterpreterState state) =>
            op.SourceType is RowsSourceType sourceType
                ? Interpret(op.Next(unit), state.SetVar(op.Operation.Id, sourceType.Rows.Map(x => x.Amount).Sum()))
                : Left("Invalid source type for Compute");
    }
}
