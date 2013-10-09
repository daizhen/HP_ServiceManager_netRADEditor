using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public class BracketExpression:BaseExpression
    {
        private BaseExpression value;
        public BaseExpression Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }
        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("(");
            if (value != null)
            {
                str.Append(value.ToString());
            }
            str.Append(")");
            return str.ToString();
        }
    }
}
