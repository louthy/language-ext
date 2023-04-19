using System;

namespace LanguageExt.TypeSystem;

public interface Generic<A>
{
    public interface Rep
    { }
    
    Rep From(A Value); 
    A To(Rep Value);
}

public struct BoolGeneric : Generic<bool>
{
    public class BoolTrue : Generic<bool>.Rep
    {
        public static readonly Generic<bool>.Rep Default = new BoolTrue();
    }
    
    public class BoolFalse : Generic<bool>.Rep
    {
        public static readonly Generic<bool>.Rep Default = new BoolFalse();
    }
    
    public Generic<bool>.Rep From(bool Value) => 
        Value ? BoolTrue.Default : BoolFalse.Default;

    public bool To(Generic<bool>.Rep Value) => 
        Value switch
        {
            BoolTrue => true,
            BoolFalse => false,
            _ => throw new InvalidCastException()
        };
}


public struct BoolInt : Generic<int>
{
    public record V(int Value) : Generic<int>.Rep;
    
    public Generic<int>.Rep From(int Value) => 
        new V(Value);

    public int To(Generic<int>.Rep Value) =>
        Value is V v 
            ? v.Value
            : throw new InvalidCastException();
}

