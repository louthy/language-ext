using System;

namespace LanguageExt.ClassInstances
{
    public struct HashableException : Hashable<Exception>
    {
        public int GetHashCode(Exception x) =>
            x?.Message?.GetHashCode() ?? 0;
    }
}
