using LanguageExt;

namespace AccountingDSL.Data
{
    public interface IOperation
    {
    }

    public class ComputeOperation : Record<ComputeOperation>, IOperation
    {
        public readonly string Id;
        public readonly string Operator;
        public readonly string SourceType;
        public readonly string Remarks;
        public ComputeOperation(string id, string @operator, string sourceType, string remarks)
        {
            Id = id;
            Operator = @operator;
            SourceType = sourceType;
            Remarks = remarks;
        }
        public static IOperation New(string id, string @operator, string sourceType, string remarks) =>
            new ComputeOperation(id, @operator, sourceType, remarks);

    }

    public class PrintOperation : Record<PrintOperation>, IOperation
    {
        public readonly string Remarks;
        public readonly Seq<string> Messages;
        public PrintOperation(string remarks, Seq<string> messages)
        {
            Remarks = remarks;
            Messages = messages;
        }
        public static IOperation New(string remarks, Seq<string> messages) =>
            new PrintOperation(remarks, messages);
    }
}
