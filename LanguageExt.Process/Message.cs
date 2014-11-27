using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    internal abstract class Message
    {
        public enum Type
        {
            User,
            System,
            UserControl
        }

        public abstract Type MessageType
        {
            get;
        }
    }
}
