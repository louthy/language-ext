using System;

namespace LanguageExt
{
    public struct Unit : IEquatable<Unit>
    {
        public static readonly Unit Default = new Unit();

        public override int GetHashCode() => 
            0;

        public override bool Equals(object obj) =>
            obj is Unit;

        public override string ToString() => 
            "()";

        public bool Equals(Unit other) =>
            true;

        public static bool operator ==(Unit lhs, Unit rhs) =>
            true;

        public static bool operator !=(Unit lhs, Unit rhs) =>
            false;

    }
}
