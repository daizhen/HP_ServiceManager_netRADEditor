using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public class BracketStatement: BaseStatement
    {
        private BaseStatement value;
        public BaseStatement Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        public override string ToString()
        {
            StringBuilder temStr = new StringBuilder();
            temStr.Append("( ");

            //Append the value
            temStr.Append(value.ToString());
            temStr.Append(" )");
            return temStr.ToString();
        }
        public override string ToString(int nestLevel)
        {
            StringBuilder temStr = new StringBuilder();

            //Append header
            for (int i = 0; i < nestLevel - 1; i++)
            {
                temStr.Append("    ");
            }
            temStr.Append("(\r\n");

            //Append the value
            temStr.Append(value.ToString(nestLevel));
            temStr.Append("\r\n");
            //Append the footer.
            for (int i = 0; i < nestLevel - 1; i++)
            {
                temStr.Append("    ");
            }
            temStr.Append(")");
            return temStr.ToString();
        }
    }
}
