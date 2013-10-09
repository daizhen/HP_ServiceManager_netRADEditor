using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public enum SMDBType
    {
        Number = 1,
        Character = 2,
        Time = 3,
        Logical = 4,
        Label = 5,
        Record = 6,
        Offset = 7,
        Array=8,
        Structure = 9,
        Operator = 10,
        Expression = 11,
        PseudoField = 12,
        GlobalVariable = 13,
        LocalVariable = 14
    }
}
