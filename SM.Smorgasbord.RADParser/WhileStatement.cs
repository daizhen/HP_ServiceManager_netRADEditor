using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public class WhileStatement:BaseStatement
    {
        public WhileStatement()
        {
            Type = StatementType.While;
        }
        private LogicalExpression condition;
        private BaseStatement content;

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

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append(" while ");
            str.Append(condition.ToString());
            str.Append(" do ");
            str.Append(content.ToString());
            return str.ToString();
        }

        public override string ToString(int nestLevel)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < nestLevel; i++)
            {
                str.Append("    ");
            }
            str.Append(" while ");
            str.Append(condition.ToString());
            str.Append(" do ").AppendLine();
            str.Append(content.ToString(nestLevel + 1));
            return str.ToString();
        }
    }

    
}
