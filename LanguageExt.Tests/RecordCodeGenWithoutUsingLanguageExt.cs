namespace LanguageExt.Tests;

[Record]
public partial class RecordCodeGenWithoutUsingLanguageExt
{
    public int SomeProperty { get; }

    public static RecordCodeGenWithoutUsingLanguageExt AnotherNew() => New(0b101010);
}
