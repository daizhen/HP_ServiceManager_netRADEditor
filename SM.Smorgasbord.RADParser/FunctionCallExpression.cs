using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace SM.Smorgasbord.RADParser
{
    public class FunctionCallExpression : BaseExpression
    {
        private string functionName;
        private Collection<BaseExpression> functionArgs = new Collection<BaseExpression>();

        public string FunctionName
        {
            get
            {
                return functionName;
            }
            set
            {
                functionName = value;
            }
        }
        public Collection<BaseExpression> FunctionArgs
        {
            get
            {
                return functionArgs;
            }
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append(functionName);
            str.Append("(");

            int argIndex = 0;
            int argCount = functionArgs.Count();

            foreach (BaseExpression expression in functionArgs)
            {
                if (expression != null)
                {
                    str.Append(expression.ToString());
                }
                else
                {
                    str.Append(" ");
                }
                if (argIndex < argCount - 1)
                {
                    str.Append(",");
                }
                argIndex++;
            }
            str.Append(")");
            return str.ToString();
        }
    }
}
