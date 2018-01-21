using LanguageExt;

namespace Banking.Schema
{
    /// <summary>
    /// Represents a person's name.  The idea is to capture the complexity
    /// inherent in human identities (obviously this class doesn't actually
    /// do that, just an example of how it could be done).
    /// </summary>
    public class PersonName : Record<PersonName>, IAccountName
    {
        public readonly Title Title;
        public readonly FirstName Name;
        public readonly Surname Surname;
        public PersonName(Title title, FirstName name, Surname surname)
        {
            Title = title;
            Name = name;
            Surname = surname;
        }
    }
}
