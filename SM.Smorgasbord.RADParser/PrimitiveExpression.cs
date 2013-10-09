using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public class PrimitiveExpression:BaseExpression
    {
        private RADToken token;

        public RADToken Token
        {
            get
            {
                return token;
            }
            set
            {
                token = value;
            }
        }
        public override string ToString()
        {
            return token.TokenValue;
        }
    }
}
