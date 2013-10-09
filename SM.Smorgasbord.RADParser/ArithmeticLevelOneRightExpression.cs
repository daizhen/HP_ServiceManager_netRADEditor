using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public class ArithmeticLevelOneRightExpression : BaseExpression
    {
        private ArithmeticLevelOneExpression value;
        private RADToken signToken;

        public ArithmeticLevelOneExpression Value
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

        public bool IsNull
        {
            get;
            set;
        }
        public override string ToString()
        {
            StringBuilder str = new StringBuilder(); ;
            if (value != null)
            {
                str.Append(signToken.TokenValue);
                str.Append(" ");
                str.Append(value.ToString());
            }
            return str.ToString();
        }
    }
}
