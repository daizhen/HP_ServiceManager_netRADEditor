using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public enum ExpressionType 
    {
        In,
        Logical,
        LogicalRight,
        LogicalLevelTwo,
        LogicalLevelTwoRight,
        Arithmetic,
        ArithmeticLevelTwo,
        ArithmeticRight,
        ArithmeticLevelTwoRight,

        NullType,
        Number,
        StringConst,
        DateConst,
        Variable,
        Primitive,
        StringIdentifier
    }
}
