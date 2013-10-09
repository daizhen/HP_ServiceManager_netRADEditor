using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace SM.Smorgasbord.RADParser
{
    public class RadArrayExpression : BaseExpression
    {
        private Collection<BaseExpression> arrayItems = new Collection<BaseExpression>();

        public Collection<BaseExpression> ArrayItems
        {
            get
            {
                return arrayItems;
            }
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("{");

            int argIndex = 0;
            int argCount = arrayItems.Count;

            foreach (BaseExpression item in arrayItems)
            {
                if (item != null)
                {
                    str.Append(item.ToString());
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
            str.Append("}");
            return str.ToString();
        }
    }
}
