namespace LanguageExt.Megaparsec;

public readonly record struct SourcePos(
    string Name, 
    Pos Line, 
    Pos Column);  
