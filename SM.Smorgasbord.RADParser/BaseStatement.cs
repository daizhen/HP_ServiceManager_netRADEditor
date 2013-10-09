using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public class BaseStatement
    {
        public StatementType Type
        {
            get;
            set;
        }
        public virtual string ToString(int nestLevel)
        {
            return string.Empty;
        }
    }
}
