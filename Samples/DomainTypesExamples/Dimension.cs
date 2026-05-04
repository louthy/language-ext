using LanguageExt.Traits;

namespace DomainTypesExamples;

public interface DimensionSize : Const<int>
{
}

public class D3 : DimensionSize
{
    public static int Value => 3;
}

public class D128 : DimensionSize
{
    public static int Value => 128;
}
