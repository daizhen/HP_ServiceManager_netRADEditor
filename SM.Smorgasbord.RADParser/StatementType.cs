using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public enum StatementType
    {
        Assignment = 0,
        If = 1,
        For = 2,
        While = 3,
        Comments = 4,
        NullType = 5,
        FunctionCall = 6
    }
}
