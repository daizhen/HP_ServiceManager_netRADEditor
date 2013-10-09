using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public class InEndExpression:BaseExpression
    {
        public InExpression value;
        public InExpression Value
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

            return "in " + value.ToString(); 
        }
    }
}
