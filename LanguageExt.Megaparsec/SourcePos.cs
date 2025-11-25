namespace LanguageExt.Megaparsec;

/// <summary>
/// Represents a source position in a named location (i.e. a file)
/// </summary>
/// <param name="Name">Source location name (file name)</param>
/// <param name="Line">Line number, starting at 1</param>
/// <param name="Column">Column number, starting at 1</param>
public readonly record struct SourcePos(
    string Name,
    Pos Line,
    Pos Column)
{
    /// <summary>
    /// Create a source position from a name
    /// </summary>
    /// <param name="name">Name</param>
    /// <returns>SourcePos</returns>
    public static SourcePos FromName(string name) =>
        new (name, Pos.One, Pos.One);
    
    /// <summary>
    /// Convert to string
    /// </summary>
    /// <returns>String representation of the structure</returns>
    public override string ToString() => 
        $"{Name}({Line},{Column})";

    /// <summary>
    /// Move to the beginning of the next line
    /// </summary>
    /// <returns></returns>
    public SourcePos NextLine =>
        this with { Line = Line + 1, Column = Pos.One };

    /// <summary>
    /// Move to the next token
    /// </summary>
    /// <returns></returns>
    public SourcePos NextToken =>
        this with { Column = Column + 1 };    

    /// <summary>
    /// Move to the next token
    /// </summary>
    /// <returns></returns>
    public SourcePos Next(int amount) =>
        this with { Column = Column + amount };    
}
