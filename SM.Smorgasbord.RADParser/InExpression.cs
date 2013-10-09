using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public class InExpression:BaseExpression
    {
        private BaseExpression head;
        //This should be InEnd object.
        private BaseExpression end;

        public BaseExpression Head
        {
            get
            {
                return head;
            }
            set
            {
                head = value;
            }
        }
        public BaseExpression End
        {
            get
            {
                return end;
            }
            set
            {
                end = value;
            }
        }
        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            if (head != null)
            {
                str.Append(head.ToString());
            }

            if (end != null)
            {
                str.Append(" ");
                str.Append(end.ToString());
            }
            return str.ToString();
        }
    }
}
