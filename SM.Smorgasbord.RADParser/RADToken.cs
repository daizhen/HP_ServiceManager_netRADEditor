using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public class RADToken
    {
        public TokenType TokenType
        {
            get;
            set;
        }
        public string TokenValue
        {
            get;
            set;
        }

        public int Line
        {
            get;
            set;
        }

        public int PositionOfLine
        {
            get;
            set;
        }

        public RADToken(TokenType type, string value)
        {
            this.TokenValue = value;
            this.TokenType = type;
        }
        public RADToken()
        {

        }

        public override string ToString()
        {
            StringBuilder valueString = new StringBuilder() ;
            valueString.Append(Enum.GetName(typeof(TokenType), TokenType));


            valueString.Append(":\t\t\t");
            valueString.Append(TokenValue);
            return valueString.ToString();
        }

    }
}
