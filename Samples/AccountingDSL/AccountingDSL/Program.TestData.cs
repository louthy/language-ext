using AccountingDSL.Data;
using LanguageExt;
using static LanguageExt.Prelude;

namespace AccountingDSL
{
    public partial class Program
    {
        public static class Data
        {
            public static readonly Seq<AccountingRow> Rows = Seq(
                new AccountingRow("Asset", "Account Recievable", 50000),
                new AccountingRow("Asset", "Cash In Hand", 10000),
                new AccountingRow("Asset", "Bank ABC", 100000),
                new AccountingRow("Expense", "Salary", 30000),
                new AccountingRow("Expense", "Office Rent", 6000),
                new AccountingRow("Expense", "Utilities", 4000));

            public static readonly Seq<IOperation> OperationsToPerform = Seq(
               ComputeOperation.New("TotalAssets", "Sum", "Asset", "Sum all with type Assets"),
               ComputeOperation.New("TotalExpenses", "Sum", "Expense", "Sum all with type Assets"),
               ComputeOperation.New("Balance", "Expression", "TotalAssets - TotalExpenses", "Compute Balance"),
               PrintOperation.New("Print messages after substituting computed Ids in each message format string",
                    Seq(
                        "Total Assets : {TotalAssets}",
                        "Total Expenses : {TotalExpenses}",
                        "Balance (Total Assets - Total Expenses): {Balance}")));
        }
    }
}
