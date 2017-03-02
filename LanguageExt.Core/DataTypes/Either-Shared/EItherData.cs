using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageExt.DataTypes.Serialisation
{
    public class EitherData<L, R>
    {
        public readonly EitherStatus State;
        public readonly R Right;
        public readonly L Left;

        public EitherData(EitherStatus state, R right, L left)
        {
            State = state;
            Right = right;
            Left = left;
        }
    }
}
