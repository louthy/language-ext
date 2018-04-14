using System;
using static AccountingDSL.ScriptParsing.ScriptParser;
using static AccountingDSL.ExpressionCompiler.Compiler;
using static AccountingDSL.Interpreter.Interpreter;
using static AccountingDSL.DSL.Transform;
using static LanguageExt.Prelude;
using AccountingDSL.Interpreter;
using AccountingDSL.DSL;

namespace AccountingDSL
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Test_Script();
            Test_LoadFromData();
            Test_LINQ();
        }

        static void Test_Script()
        {
            var script = @" TotalAssets = sum(filter(""Asset"", rows));
                            TotalExpenses = sum(filter(""Expense"", rows));
                            Balance = TotalAssets - TotalExpenses;
                            log(""Total Assets : {TotalAssets}"");
                            log(""Total Expenses : {TotalExpenses}"");
                            log(""Balance (Total Assets - Total Expenses): {Balance}""); ";

            var result = from transform in Parse(script).Map(Compile<object>)
                         let state = InterpreterState.Empty.With(Rows: Data.Rows)
                         let disp = state.Output.Subscribe(Console.WriteLine)
                         from _ in Interpret(transform, state)
                         select unit; 
        }

        static void Test_LoadFromData()
        {
            var transform = Data.OperationsToPerform.ToTransform();

            var state = InterpreterState.Empty.With(Rows: Data.Rows);
            state.Output.Subscribe(Console.WriteLine);
            var result = Interpret(transform, state);
        }

        static void Test_LINQ()
        {
            var transform = from c1 in Compute("TotalAssets", "Sum", "Asset", "Sum all with type Assets")
                            from c2 in Compute("TotalExpenses", "Sum", "Expense", "Sum all with type Assets")
                            from c3 in Compute("Balance", "Expression", "TotalAssets - TotalExpenses", "Compute Balance")
                            from pr in Print("Print messages after substituting computed Ids in each message format string",
                                            Seq(
                                                "Total Assets : {TotalAssets}",
                                                "Total Expenses : {TotalExpenses}",
                                                "Balance (Total Assets - Total Expenses): {Balance}"))
                            select unit;

            var state = InterpreterState.Empty.With(Rows: Data.Rows);
            state.Output.Subscribe(Console.WriteLine);
            var result = Interpret(transform, state);
        }
    }
}
