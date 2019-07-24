using LanguageExt;

namespace AccountingDSL.Data
{
    public abstract class SourceType
    {
    }
    public class RowsSourceType : SourceType
    {
        public readonly Seq<AccountingRow> Rows;
        public RowsSourceType(Seq<AccountingRow> rows) =>
            Rows = rows;
    }
    public class ValueSourceType : SourceType
    {
        public readonly object Value;
        public ValueSourceType(object value) =>
            Value = value;
        public override string ToString() =>
            Value?.ToString() ?? "[null]";
    }
}
