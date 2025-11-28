using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Deriving
{
    public interface Alternative<Supertype, Subtype> :
        Alternative<Supertype>,
        Choice<Supertype, Subtype>,
        Applicative<Supertype, Subtype>
        where Supertype : Alternative<Supertype, Subtype>
        where Subtype : Alternative<Subtype>;
}
