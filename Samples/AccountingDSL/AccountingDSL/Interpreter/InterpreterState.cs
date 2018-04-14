using LanguageExt;
using static LanguageExt.Prelude;
using System.Reactive.Subjects;
using AccountingDSL.Data;

namespace AccountingDSL.Interpreter
{
    public class InterpreterState
    {
        public readonly Map<string, object> Vars;
        public readonly Seq<AccountingRow> Rows;
        public readonly Subject<string> Output;

        public static InterpreterState Empty => new InterpreterState(default, Seq<AccountingRow>(), new Subject<string>());

        public InterpreterState(Map<string, object> vars, Seq<AccountingRow> rows, Subject<string> output)
        {
            Vars = vars;
            Rows = rows;
            Output = output;
        }
        public InterpreterState SetVar(string name, object value) =>
            With(Vars: Vars.AddOrUpdate(name, value));

        public Option<object> GetVar(string name) =>
            name == "rows"
                ? Rows
                : Vars.Find(name);

        public InterpreterState With(
            Map<string, object>? Vars = null,
            Seq<AccountingRow> Rows = null,
            Subject<string> Output = null
            ) =>
            new InterpreterState(
                Vars ?? this.Vars,
                Rows ?? this.Rows,
                Output ?? this.Output
                );
    }
}
