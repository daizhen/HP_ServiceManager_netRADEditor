using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SM.Smorgasbord.RADParser
{
    public class CompareExpression:BaseExpression
    {
       private InExpression left;

        //Should be CompareRightExpression
       private BaseExpression right;

       public InExpression Left
       {
           get
           {
               return left;
           }
           set
           {
               left = value;
           }
       }

       //Should be CompareRightExpression
       public BaseExpression Right
       {
           get
           {
               return right;
           }
           set
           {
               right = value;
           }
       }

       public override string ToString()
       {
           StringBuilder str = new StringBuilder() ;
           str.Append(left.ToString());
           if (right != null)
           {
               str.Append(" ");
               str.Append(right.ToString());
           }
           return str.ToString();
       }
    }
}
