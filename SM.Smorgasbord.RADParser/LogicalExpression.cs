using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public class LogicalExpression:BaseExpression
    {
        private  ArithmeticExpression left;
        private BaseExpression right;

        public ArithmeticExpression Left
        {
            get
            {
                return left;
            }
            set
            {
                left = value;
            }
        }
        public BaseExpression Right
        {
            get
            {
                return right;
            }
            set
            {
                right = value;
            }
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            if (left != null)
            {
                str.Append(left.ToString());
            }
            if (right != null)
            {
                str.Append(" ");
                str.Append(right.ToString());
            }
            return str.ToString();
        }
    }
}
