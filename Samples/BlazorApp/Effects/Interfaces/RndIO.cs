namespace BlazorApp.Effects.Interfaces;

public interface RndIO
{
    /// <summary>
    /// Get a random integer between min and max, inclusive.
    /// </summary>
    int Next(int min, int max);
    
    /// <summary>
    /// Get a random integer up to max, inclusive.
    /// </summary>
    int Next(int max);    
}

