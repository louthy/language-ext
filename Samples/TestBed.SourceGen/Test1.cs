using LanguageExt;
using LanguageExt.TypeSystem;

namespace TestBed.SourceGen;

public partial class Person : 
    deriving<Eq, Ord, Show>
{
    public readonly int Id;
    public readonly string Name; 
    public readonly string Surname;
}
