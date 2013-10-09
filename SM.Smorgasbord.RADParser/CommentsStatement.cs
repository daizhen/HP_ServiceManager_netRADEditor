﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public class CommentsStatement:BaseStatement
    {
        private RADToken token;

        public RADToken Token
        {
            get
            {
                return token;
            }
            set
            {
                token = value;
            }
        }
        public CommentsStatement()
        {
            Type = StatementType.Comments;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("$L.comments.AutoGenerated.byEDITOR=\"");
            str.Append(StringHelper.EscapeString(token.TokenValue));
            str.Append("\"");
            return str.ToString();
        }
        public override string ToString(int nestLevel)
        {
            StringBuilder str = new StringBuilder();

            for (int i = 0; i < nestLevel; i++)
            {
                str.Append("    ");
            }

            str.Append("//");
            str.Append(token.TokenValue);

            return str.ToString();
        }
    }
}
