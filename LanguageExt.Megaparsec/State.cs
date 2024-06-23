namespace LanguageExt.Megaparsec;

public record State<S, E>(S Input, int Offset, PosState<S> PosState, Seq<ParseError<S, E>> ParseErrors);

public record PosState<S>(S Input, int Offset, SourcePos SourcePos, int TabWidth, string LinePrefix);

public record SourcePos(string Name, int Line, int Column);  
