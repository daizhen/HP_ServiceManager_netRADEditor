using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public class ForStatement:BaseStatement
    {
        private AssignmentStatement forStart;
        private ArithmeticExpression forEnd;
        private BaseStatement content;

        public AssignmentStatement ForStart
        {
            get
            {
                return forStart;
            }
            set
            {
                forStart = value;
            }
        }
        public ArithmeticExpression ForEnd
        {
            get
            {
                return forEnd;
            }
            set
            {
                forEnd = value;
            }
        }
        public BaseStatement Content
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
            }
        }

        public ForStatement()
        {
            Type = StatementType.For;
        }

        public override string ToString(int nestLevel)
        {
            StringBuilder str = new StringBuilder(); 
            for (int i = 0; i < nestLevel; i++)
            {
                str.Append("    ");
            }
            str.Append("for ");
            str.Append(forStart.ToString(0));
            str.Append(" to ");
            str.Append(forEnd.ToString());
            str.Append(" do");
            str.Append("\r\n");
            str.Append(content.ToString(nestLevel + 1));
            return str.ToString();
        }
        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("for ");
            str.Append(forStart.ToString());
            str.Append(" to ");
            str.Append(forEnd.ToString());
            str.Append(" do");
            str.Append(content.ToString());
            return str.ToString();
        }
    }
}
