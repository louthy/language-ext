// TODO: Decide whether you want to develop this idea or not
// https://mmapped.blog/posts/25-domain-types.html

namespace LanguageExt.Traits.Domain;

public interface DomainType<out SELF, REPR> where SELF : DomainType<SELF, REPR> 
{
    /// <summary>
    /// Creates a domain value from its representation value 
    /// </summary>
    public static abstract SELF From(REPR repr);
    
    /// <summary>
    /// Extracts the representation value from its domain value  
    /// </summary>
    REPR To();
}
