using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public class SMStructure:SMFieldItem
    {
        public Dictionary<string, SMFieldItem> items = new Dictionary<string, SMFieldItem>();

        public Dictionary<string, SMFieldItem> Items
        {
            get
            {
                return items;
            }
        }

        public string FieldName
        {
            get;
            set;
        }

        public override string ToString(int nestedLevel)
        {
            StringBuilder str = new StringBuilder();
            str.Append(new string(' ', 4 * nestedLevel)).Append("{[").AppendLine() ;
            foreach (KeyValuePair<string, SMFieldItem> subItem in items)
            {
                str.AppendLine(subItem.Value.ToString(nestedLevel + 1));
            }
            str.Append(new string(' ', 4 * nestedLevel)).Append("]}").AppendLine();
            return str.ToString();
        }
    }
}
