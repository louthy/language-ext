using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Deriving
{
    public interface Alternative<Supertype, Subtype> :
        Alternative<Supertype>,
        Choice<Supertype, Subtype>,
        MonoidK<Supertype, Subtype>
        where Supertype : Alternative<Supertype, Subtype>
        where Subtype : Alternative<Subtype>;
}
