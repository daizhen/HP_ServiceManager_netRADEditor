using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public class SpecialFunctionStatement : BaseStatement
    {
        public SpecialFunctionStatement()
        {
            Type = StatementType.FunctionCall;
        }
        private FunctionCallExpression content;
        public FunctionCallExpression Content
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
            if (content != null)
            {
                str.Append(content.ToString());
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
            if (content != null)
            {
                str.Append(content.ToString());
            }
            return str.ToString();
        }
    }
}
