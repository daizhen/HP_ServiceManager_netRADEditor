using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public class StringHelper
    {
        public static string EscapeString(string str)
        {
            StringBuilder resultString = new  StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                char currentChar = str[i];
                if (currentChar == '"' || currentChar == '\\')
                {
                    resultString.Append("\\");
                }
                resultString.Append(currentChar);
            }
            return resultString.ToString();
        }

        public static string UnescapeString(string str)
        {
            StringBuilder resultString = new StringBuilder();

            //Used to check whether the last char is escape char. that means \

            bool isLastEscapeChar = false;
            for (int i = 1; i < str.Length - 1; i++)
            {
                char currentChar = str[i];
                if (currentChar == '"')
                {
                    if (!isLastEscapeChar)
                    {
                        break;
                    }
                    else
                    {
                        resultString.Append(currentChar);
                        isLastEscapeChar = false;
                    }
                }
                else
                {
                    if (currentChar == '\\')
                    {
                        if (!isLastEscapeChar)
                        {
                            isLastEscapeChar = true;
                        }
                        else
                        {
                            isLastEscapeChar = false;
                            resultString.Append(currentChar);
                        }
                    }
                    else
                    {
                        resultString.Append(currentChar);
                        isLastEscapeChar = false;
                    }
                }
            }

            //if(!isSuccess)
            //{
            //    throw exception("String const  not valid");
            //}
            return resultString.ToString();
        }
    }
}
