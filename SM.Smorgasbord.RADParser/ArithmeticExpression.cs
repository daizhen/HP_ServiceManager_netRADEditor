using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public class ArithmeticExpression : BaseExpression
    {
        public ArithmeticLevelOneExpression LeftSide
        {
            get;
            set;
        }
        public BaseExpression RightSide
        {
            get;
            set;
        }
        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append(LeftSide.ToString());
            if (RightSide != null)
            {
                str.Append(" ");
                str.Append(RightSide.ToString());
            }
            return str.ToString();
        }
    }
}
