using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public class LogicalAndRightExpression:BaseExpression
    {
        private LogicalAndExpression value;

        public LogicalAndExpression Value
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
            if (value != null)
            {
                str.Append("and ");
                str.Append(value.ToString());
            }
            return str.ToString();
        }
    }
}
