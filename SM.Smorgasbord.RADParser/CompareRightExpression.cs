using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public class CompareRightExpression : BaseExpression
    {
        private RADToken signToken;
        private CompareExpression value;

        public RADToken SignToken
        {
            get
            {
                return signToken;
            }
            set
            {
                signToken = value;
            }
        }

        public CompareExpression Value
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
            str.Append(signToken.TokenValue);
            if (value != null)
            {
                str.Append(" ");
                str.Append(value.ToString());
            }
            return str.ToString();
        }
    }
}
