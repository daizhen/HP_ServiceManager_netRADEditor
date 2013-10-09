using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public class IfStatement : BaseStatement
    {
        public IfStatement()
        {
            Type = StatementType.If;
        }
        private LogicalExpression condition;
        private BaseStatement thenContent;
        private BaseStatement elseContent;

        public LogicalExpression Condition
        {
            get
            {
                return condition;
            }
            set
            {
                condition = value;
            }
        }

        public BaseStatement ThenContent
        {
            get
            {
                return thenContent;
            }
            set
            {
                thenContent = value;
            }
        }

        public BaseStatement ElseContent
        {
            get
            {
                return elseContent;
            }
            set
            {
                elseContent = value;
            }
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("if ");
            str.Append(condition.ToString());
            str.Append(" then ");
            str.Append(thenContent.ToString());
            if (elseContent != null)
            {
                str.Append(" else ");
                str.Append(elseContent.ToString());
            }
            return str.ToString();
        }

        public override string ToString(int nestLevel)
        {
            StringBuilder str = new StringBuilder();

            for (int i = 0; i < nestLevel; i++)
            {
                str.Append("    ");
            }
            str.Append("if ");
            str.Append(condition.ToString());
            str.Append(" then");
            str.Append("\r\n");
            str.Append(thenContent.ToString(nestLevel + 1));
            if (elseContent != null)
            {
                str.Append("\r\n");
                for (int i = 0; i < nestLevel; i++)
                {
                    str.Append("    ");
                }
                str.Append("else");
                str.Append("\r\n");
                str.Append(elseContent.ToString(nestLevel + 1));
            }
            return str.ToString();
        }
    }
}
