using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace SM.Smorgasbord.RADParser
{
    public class SMArray:SMFieldItem
    {
        private Collection<SMFieldItem> items = new Collection<SMFieldItem>();
        public Collection<SMFieldItem> Items
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
            str.Append(new string(' ', 4 * nestedLevel)).Append("{").AppendLine();
            foreach (SMFieldItem subItem in items)
            {
                str.AppendLine(subItem.ToString(nestedLevel + 1));
            }
            str.Append(new string(' ', 4 * nestedLevel)).Append("}").AppendLine();
            return str.ToString();
        }
    }
}
