using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public class BaseExpression
    {
        public ExpressionType Type
        {
            get;
            set;
        } 
        //Used to indicate whether the statements is quoted with brackets.
        public bool HasBrackets
        {
            get;
            set;
        }
    }
}
