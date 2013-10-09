using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public enum TokenType
    {
        Number,
        StringConst,
        DateConst,
        Key,
        Variable,
        StringIndentifier,
        BoolConst,
        Comments
    }
}
