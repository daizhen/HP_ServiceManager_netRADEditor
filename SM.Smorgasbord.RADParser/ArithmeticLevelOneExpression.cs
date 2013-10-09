using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public class ArithmeticLevelOneExpression : BaseExpression
    {
        private LogicalAndExpression leftSide;
        private BaseExpression rightSide;

        public LogicalAndExpression LeftSide
        {
            get
            {
                return leftSide;
            }
            set
            {
                leftSide = value;
            }
        }

        public BaseExpression RightSide
        {
            get
            {
                return rightSide;
            }
            set
            {
                rightSide = value;
            }
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append(leftSide.ToString());
            if (rightSide != null)
            {
                str.Append(" ");
                str.Append(rightSide.ToString());
            }
            return str.ToString();
        }
    }
}
