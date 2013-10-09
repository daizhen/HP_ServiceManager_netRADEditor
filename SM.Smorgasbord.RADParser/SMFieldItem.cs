using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public class SMFieldItem
    {
        public string TextValue
        {
            get;
            set;
        }

        public bool IsNull
        {
            get;
            set;
        }

        public SMDBType Type
        {
            get;
            set;
        }

        public virtual string ToString(int nestedLevel)
        {
            return new string(' ', 4 * nestedLevel) + TextValue;
        }
    }
}
