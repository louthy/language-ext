using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    public class ParserError
    {
        public string Message;
        public PString Location;

        public ParserError(string message, PString location)
        {
            Message = message;
            Location = location;
        }
    }
}
