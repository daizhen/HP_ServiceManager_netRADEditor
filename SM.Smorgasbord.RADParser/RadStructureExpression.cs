using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace SM.Smorgasbord.RADParser
{
    public class RadStructureExpression:BaseExpression
    {
        private Collection<BaseExpression> structItems = new Collection<BaseExpression>();

        public Collection<BaseExpression> StructItems
        {
            get
            {
                return structItems;
            }
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("{[");

            int argIndex = 0;
            int argCount = structItems.Count;

            foreach (BaseExpression item in structItems)
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
            str.Append("]}");
            return str.ToString();
        }
    }
}
