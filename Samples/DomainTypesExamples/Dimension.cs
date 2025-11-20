namespace DomainTypesExamples;

public interface Dimension
{
    public static abstract int Size { get; } 
}

public class D3 : Dimension
{
    public static int Size => 3;
}

public class D128 : Dimension
{
    public static int Size => 128;
}
