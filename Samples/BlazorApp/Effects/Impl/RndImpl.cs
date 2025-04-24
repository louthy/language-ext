using BlazorApp.Effects.Interfaces;

namespace BlazorApp.Effects.Impl;

public class RndImpl : RndIO
{
    public static readonly RndIO Default = new RndImpl();
    
    public int Next(int min, int max) => 
        Random.Shared.Next(min, max);

    public int Next(int max) => 
        Random.Shared.Next(max);
}
