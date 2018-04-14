using AccountingDSL.Data;
using LanguageExt;

namespace AccountingDSL.Interpreter
{
    public static class ScriptFunctions
    {
        public static Seq<AccountingRow> filter(string type, Seq<AccountingRow> rows) =>
            rows.Filter(r => r.Type == type);

        public static int sum(Seq<AccountingRow> rows) =>
            rows.Map(r => r.Amount).Sum();

        public static int count(Seq<AccountingRow> rows) =>
            rows.Map(r => r.Amount).Count;

        public static int avg(Seq<AccountingRow> rows) =>
            sum(rows) / count(rows);
    }
}
