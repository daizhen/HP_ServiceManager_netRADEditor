using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public class RadSyntaxException:Exception
    {
        private int line;
        private int positionOfLine;
        private string errorMessage;
        public RadSyntaxException(string errorMessage,int exceptionLine, int column)
        {
            this.errorMessage = errorMessage;
            this.line = exceptionLine;
            this.positionOfLine = column;
            
        }

        public RadSyntaxException(string errorMessage, RADToken token)
        {
            this.errorMessage = errorMessage;
            this.line = token.Line;
            this.positionOfLine = token.PositionOfLine;

        }
        public override string Message
        {
            get 
            {
                return errorMessage + " at line: " + line + " column: " + positionOfLine;
            }
        }
    }
}
