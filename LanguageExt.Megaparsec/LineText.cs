namespace LanguageExt.Megaparsec;

/// <summary>
/// Lazy text structure
/// </summary>
/// <param name="lazyText">Lazy text function</param>
public class LineText(Func<string> lazyText)
{
    int isAvailable;
    string text = "";
    
    /// <summary>
    /// Construct a new LineText
    /// </summary>
    /// <param name="lazyText">Lazy text function</param>
    /// <returns>LineText</returns>
    public static LineText Lift(Func<string> lazyText) =>
        new (lazyText);
    
    /// <summary>
    /// Read text
    /// </summary>
    public string Text => 
        GetText();
    
    string GetText()
    {
        if(isAvailable == 2) return text;
        SpinWait sw = default;
        while (true)
        {
            if(isAvailable == 2) return text;
            if (Interlocked.CompareExchange(ref isAvailable, 1, 0) == 0)
            {
                try
                {
                    text = lazyText();
                    isAvailable = 2;
                    return text;
                }
                catch
                {
                    isAvailable = 0;
                    throw;
                }
            }
            else
            {
                sw.SpinOnce();
            }
        }
    }
}
