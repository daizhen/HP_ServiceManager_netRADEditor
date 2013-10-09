using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;

namespace SM.Smorgasbord.RADParser
{
    public class RadParser
    {
        private Collection<RADToken> radTokens;
        private HashSet<string> specialFunctions = new HashSet<string>();
        private Stack<bool> bracketFlags = new Stack<bool>();
        private int currentIndex = 0;

        public bool ForceToEnd
        {
            get;
            set;
        }

        private RADToken CurrentToken
        {
            get
            {
                return radTokens[currentIndex];
            }
        }

        private void ConstructSpecialFunctions()
        {
            FileStream fileStream = new FileStream("StandaloneFunctions.txt", FileMode.Open);
            StreamReader reader = new StreamReader(fileStream);
            while (!reader.EndOfStream)
            {
                string functionName = reader.ReadLine();
                if (!string.IsNullOrEmpty(functionName))
                {
                    specialFunctions.Add(functionName);
                }
            }
            reader.Close();
            fileStream.Dispose();
        }

        public RadParser(Collection<RADToken> radTokens)
        {
            this.radTokens = radTokens;
            ConstructSpecialFunctions();
            currentIndex = 0;
        }

        public Statements ParseStatements()
        {
            return ParseStatements(true);
        }

        public SMStructure ParseSMStructure(FileColumnInfo columnInfo)
        {
            SMStructure structure = new SMStructure();
            if (columnInfo.Type != SMDBType.Structure)
            {
                throw new Exception("Not a structure type");
            }

            if (CurrentToken.TokenValue != "{[")
            {
                throw new Exception("Not a structure data");
            }
            MoveNext();
            structure.FieldName = columnInfo.Name;
            foreach (FileColumnInfo subField in columnInfo.Children)
            {
                if (subField.Type == SMDBType.Structure)
                {
                    structure.Items.Add(subField.Name, ParseSMStructure(subField));
                }
                else if (subField.Type == SMDBType.Array)
                {
                    structure.Items.Add(subField.Name, ParseSMArray(subField));
                }
                else
                {
                    structure.Items.Add(subField.Name, GetFieldItem(subField));
                }
                if (CurrentToken.TokenValue == ",")
                {
                    MoveNext();
                }
            }
            if (CurrentToken.TokenValue != "]}")
            {
                throw new Exception("Not end with ]}");
            }
            MoveNext();
            return structure;
        }

        private SMArray ParseSMArray(FileColumnInfo columnInfo)
        {
            SMArray smArray = new SMArray();
            if (columnInfo.Type != SMDBType.Array)
            {
                throw new Exception("Not a Array type");
            }
            //Skip unknown blanks
            while (CurrentToken.TokenValue == ",")
            {
                MoveNext();
            }
            if (CurrentToken.TokenValue != "{")
            {
                throw new Exception("Not a Array data");
            }
            MoveNext();
            smArray.FieldName = columnInfo.Name;

            FileColumnInfo subField = columnInfo.Children[0];

            while (!IsEnd() && CurrentToken.TokenValue != "}")
            {
                if (subField.Type == SMDBType.Structure)
                {
                    smArray.Items.Add(ParseSMStructure(subField));
                }
                else if (subField.Type == SMDBType.Array)
                {
                    smArray.Items.Add(ParseSMArray(subField));
                }
                else
                {
                    smArray.Items.Add(GetFieldItem(subField));
                }
                if (CurrentToken.TokenValue == ",")
                {
                    MoveNext();
                }
            }

            if (IsEnd())
            {
                throw new Exception("Not end with }");
            }
            MoveNext();
            return smArray;
        }

        private SMFieldItem GetFieldItem(FileColumnInfo columnInfo)
        {

            if (columnInfo.Name == "sysmodtime")
            {
                int br = 1;
            }
            SMFieldItem fieldItem = new SMFieldItem();
            fieldItem.Type = columnInfo.Type;

            if (CurrentToken.TokenValue == "," || CurrentToken.TokenValue == "}" || CurrentToken.TokenValue == "]}")
            {
                fieldItem.IsNull = true;
            }
            else
            {
                fieldItem.IsNull = false;
                if (columnInfo.Type == SMDBType.Character || columnInfo.Type == SMDBType.Time || columnInfo.Type == SMDBType.Number )
                {
                    fieldItem.TextValue = CurrentToken.TokenValue;
                    MoveNext();
                }
                else if (columnInfo.Type == SMDBType.Logical)
                {
                    LogicalExpression logicalExpression = GetLogicalExpression();
                    fieldItem.TextValue = logicalExpression.ToString();
                }
                else if (columnInfo.Type == SMDBType.Expression)
                {
                    Statements statements = ParseStatements(false);
                    fieldItem.TextValue = statements.ToString();
                }
                else
                {
                    throw new Exception("Not supported");
                }
            }

            return fieldItem;
        }

        private Statements ParseStatements(bool forceToEnd)
        {
            Statements statements = new Statements();
            while (!IsEnd())
            {
                BaseStatement statement = null;
                if (MatchType(TokenType.Comments) || !Match(")"))
                {
                    statement = GetStatement();
                    statements.Content.Add(statement);
                }
                else
                {
                    //if(!IsEnd())
                    //{
                    //    MoveNext();
                    //}
                    //break;
                }

                if (!IsEnd())
                {
                    if (Match(")"))
                    {
                        if (bracketFlags.Count != 0)
                        {
                            break;
                        }
                        else
                        {
                            throw new RadSyntaxException("')' is not expected", CurrentToken);
                        }
                    }
                    else
                    {
                        if (!Match(";"))
                        {
                            if (forceToEnd)
                            {
                                if (IsEnd())
                                {
                                    throw new Exception("';' expected, in the end of file");
                                }
                                else
                                {
                                    throw new RadSyntaxException("';' expected", CurrentToken);
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        MoveNext();
                    }
                }
            }

            return statements;
        }

        private bool IsEnd()
        {
            if (currentIndex == radTokens.Count)
            {
                return true;
            }
            return false;
        }

        private bool MoveNext()
        {
            currentIndex++;
            return !IsEnd();
        }

        private bool Match(string tokenValue)
        {
            if (IsEnd())
            {
                return false;
            }
            if (CurrentToken.TokenValue == tokenValue)
            {
                return true;
            }
            return false;
        }

        private bool MatchType(TokenType type)
        {
            if (CurrentToken.TokenType == type)
            {
                return true;
            }
            return false;
        }

        private bool IsBracketStatements()
        {
            int rightBracketWant = 1;
            int temCurrentIndex = currentIndex;
            temCurrentIndex++;
            bool isStatement = false;
            while (temCurrentIndex != radTokens.Count)
            {
                var temCurrentToken = radTokens[temCurrentIndex];
                if (temCurrentToken.TokenValue == "(")
                {
                    rightBracketWant++;
                }
                if (temCurrentToken.TokenValue == ")")
                {
                    rightBracketWant--;
                }
                if (rightBracketWant == 0)
                {
                    break;
                }
                if (temCurrentToken.TokenValue == ";" ||
                    temCurrentToken.TokenValue == "if" ||
                    temCurrentToken.TokenValue == "while")
                {
                    isStatement = true;
                    break;
                }
                temCurrentIndex++;
            }

            if (!isStatement)
            {
                if (temCurrentIndex == radTokens.Count)
                {
                    isStatement = false;
                }
                else if (radTokens[temCurrentIndex].TokenValue == ")")
                {
                    temCurrentIndex++;
                    if (temCurrentIndex == radTokens.Count)
                    {
                        isStatement = true;
                    }
                    else if (radTokens[temCurrentIndex].TokenValue == "="
                        || radTokens[temCurrentIndex].TokenValue == "if"
                        || radTokens[temCurrentIndex].TokenValue == "else"
                        || radTokens[temCurrentIndex].TokenValue == "while"
                        || radTokens[temCurrentIndex].TokenValue == ")"
                        || radTokens[temCurrentIndex].TokenValue == ";"
                        || radTokens[temCurrentIndex].TokenValue == ","
                        || radTokens[temCurrentIndex].TokenValue == "}"
                        || radTokens[temCurrentIndex].TokenValue == "]}")
                    {
                        isStatement = true;
                    }
                    else
                    {
                        isStatement = false;
                    }
                }
            }

            return isStatement;
        }

        private BaseExpression GetTermExpression()
        {
            BaseExpression firstExpression = null;
            if (Match("("))
            {
                firstExpression = GetBracketExpression();
            }
            else if (MatchType(TokenType.StringIndentifier))
            {
                int temCurrentIndex = currentIndex;
                temCurrentIndex++;
                if (temCurrentIndex < radTokens.Count && radTokens[temCurrentIndex].TokenValue == "(")
                {
                    firstExpression = GetFunctionCallExpression();
                }
                else
                {
                    firstExpression = new PrimitiveExpression();
                    (firstExpression as PrimitiveExpression).Token = CurrentToken;
                    MoveNext();
                }
            }
            else if (Match("{"))
            {
                firstExpression = GetArrayExpression();
            }
            else if (Match("{["))
            {
                firstExpression = GetStructureExpression();
            }
            else if (MatchType(TokenType.Variable) ||
                MatchType(TokenType.StringConst) ||
                MatchType(TokenType.Number) ||
                MatchType(TokenType.DateConst) ||
                MatchType(TokenType.BoolConst))
            {
                firstExpression = new PrimitiveExpression();
                (firstExpression as PrimitiveExpression).Token = CurrentToken;
                MoveNext();
            }
            else
            {
                //Nothing to do.
            }
            return firstExpression;
        }

        private BaseExpression GetExpression()
        {
            LogicalExpression expression = GetLogicalExpression();
            if (!IsEnd() && Match(")"))
            {
                //MoveNext();
            }
            else if (IsEnd() || Match(";") || Match("=") || Match(",") || Match("}") || Match("]}")
                || Match("if") || Match("else") || Match("then") || Match("while")
                || Match("for") || Match("to"))
            {
                //MoveNext();
            }
            else
            {
                throw new RadSyntaxException("Expression end with error value", CurrentToken);
            }

            return expression;
        }

        private BaseStatement GetStatement()
        {
            if (IsEnd())
            {
                return null;
            }
            else if (MatchType(TokenType.Comments))
            {
                return GetCommentStatement();
            }
            else if (Match("if"))
            {
                return GetIfStatement();
            }
            else if (Match("while"))
            {
                return GetWhileStatement();
            }
            else if (Match("for"))
            {
                return GetForStatement();
            }
            else if (Match("(") && IsBracketStatements())
            {
                return GetBracketStatement();
            }
            else if (MatchType(TokenType.StringIndentifier) && IsCurrentSpecialFunction())
            {
                SpecialFunctionStatement statement = new SpecialFunctionStatement();
                statement.Content = GetFunctionCallExpression();
                return statement;
            }
            else
            {
                return GetAssignmentStatement();
            }
        }

        private ForStatement GetForStatement()
        {
            ForStatement forStatement = new ForStatement();
            if (MoveNext())
            {
                AssignmentStatement forStart = GetAssignmentStatement();
                ArithmeticExpression forEnd = null;
                BaseStatement statement = null;
                if (Match("to"))
                {
                    if (MoveNext())
                    {
                        forEnd = GetArithmeticExpression();
                    }
                    else
                    {
                        throw new Exception("For end expected in the end of file");
                    }
                    if (Match("do"))
                    {
                        MoveNext();
                        statement = GetStatement();
                    }
                }
                else
                {
                    if (IsEnd())
                    {
                        throw new Exception("Key word 'to'expected in the end of file");
                    }
                    else
                    {
                        throw new RadSyntaxException("Key word 'to'expected", CurrentToken);
                    }
                }
                forStatement.ForStart = forStart;
                forStatement.ForEnd = forEnd;
                forStatement.Content = statement;
            }
            return forStatement;
        }

        private CommentsStatement GetCommentStatement()
        {
            CommentsStatement statement = new CommentsStatement();
            statement.Token = CurrentToken;
            MoveNext();
            return statement;
        }

        private IfStatement GetIfStatement()
        {
            IfStatement statement = new IfStatement();

            if (MoveNext())
            {
                LogicalExpression condition = GetLogicalExpression();
                BaseStatement thenContent = null;
                BaseStatement elsecontent = null;
                if (Match("then"))
                {
                    MoveNext();
                    thenContent = GetStatement();
                    if (thenContent == null)
                    {
                        throw new Exception("There are no expressions after 'then'");
                    }
                }
                else
                {
                    if (IsEnd())
                    {
                        throw new Exception("Key word 'then' expected in the end of file");
                    }
                    else
                    {
                        throw new RadSyntaxException("Key word 'then' expected", CurrentToken);
                    }
                }
                if (!IsEnd() && Match("else"))
                {
                    MoveNext();
                    elsecontent = GetStatement();
                    if (elsecontent == null)
                    {
                        throw new Exception("There are no expressions after 'else' in the end of file");
                    }
                }
                statement.Condition = condition;
                statement.ThenContent = thenContent;
                statement.ElseContent = elsecontent;
            }

            return statement;
        }

        private WhileStatement GetWhileStatement()
        {
            WhileStatement statement = new WhileStatement();
            if (MoveNext())
            {
                LogicalExpression condition = GetLogicalExpression();
                BaseStatement content = null;
                if (Match("do"))
                {
                    MoveNext();
                    content = GetStatement();
                }
                else
                {
                    if (IsEnd())
                    {
                        throw new Exception("Key word 'do' expected in the end of file");
                    }
                    else
                    {
                        throw new RadSyntaxException("Key word 'do' expected", CurrentToken);
                    }
                }
                statement.Condition = condition;
                statement.Content = content;
            }
            return statement;
        }

        private AssignmentStatement GetAssignmentStatement()
        {
            AssignmentStatement statement = new AssignmentStatement();
            BaseExpression leftExpression = GetAssignmentLeft();

            statement.LeftExpression = leftExpression;

            if (!IsEnd())
            {
                if (Match("=") || Match("+=") || Match("-=") || Match("*=") || Match("/=") || Match("%="))
                {
                    statement.SignToken = CurrentToken;
                    if (MoveNext())
                    {
                        BaseExpression rightExpression = GetAssignmentRightExpression();
                        statement.RightExpression = rightExpression;
                    }
                    else
                    {
                        throw new Exception("Assignment right expected , in the end of file");
                    }
                }
                else
                {
                    if (IsEnd())
                    {
                        throw new Exception("Key word '=' expected , in the end of file");
                    }
                    else
                    {
                        throw new RadSyntaxException("Key word '=' expected", CurrentToken);
                    }
                }
            }
            else
            {
                throw new Exception("Key word '=' expected in the end of file");
            }
            return statement;
        }

        private BaseExpression GetAssignmentLeft()
        {
            return GetInExpression();
        }

        private BaseExpression GetAssignmentRightExpression()
        {
            return GetExpression();
        }

        private RadArrayExpression GetArrayExpression()
        {
            RadArrayExpression expression = null;
            if (Match("{"))
            {
                expression = new RadArrayExpression();
                MoveNext();
                FillCollectionItems(expression.ArrayItems);
            }
            if (!Match("}"))
            {
                if (IsEnd())
                {
                    throw new Exception("} expected, in the end of file");
                }
                else
                {
                    throw new RadSyntaxException("} expected", CurrentToken);
                }
            }
            if (!IsEnd())
            {
                MoveNext();
            }
            return expression;
        }

        private RadStructureExpression GetStructureExpression()
        {
            RadStructureExpression expression = null;
            if (Match("{["))
            {
                expression = new RadStructureExpression();
                MoveNext();
                FillCollectionItems(expression.StructItems);
            }
            if (!Match("]}"))
            {
                if (IsEnd())
                {

                    throw new Exception("]} expected, in the end of file");
                }
                else
                {
                    throw new RadSyntaxException("]} expected", CurrentToken);
                }
            }
            if (!IsEnd())
            {
                MoveNext();
            }
            return expression;
        }

        private InExpression GetInExpression()
        {
            BaseExpression inHead = GetInHeadExpression();
            BaseExpression inEnd = GetInEndExpression();

            InExpression expression = new InExpression();
            expression.Head = inHead;
            expression.End = inEnd;
            return expression;
        }

        private BaseExpression GetInHeadExpression()
        {
            return GetTermExpression();
        }

        private InEndExpression GetInEndExpression()
        {
            InEndExpression expression = null;
            if (Match("in"))
            {
                expression = new InEndExpression();
                MoveNext();
                expression.Value = GetInExpression();
            }
            return expression;
        }

        private CompareExpression GetCompareExpression()
        {
            CompareExpression expression = null;
            if (Match("not"))
            {
                MoveNext();
                expression = new LogicalNotExpression();
                (expression as LogicalNotExpression).Value = GetCompareExpression();
            }
            else
            {
                expression = new CompareExpression();

                InExpression head = GetInExpression();
                CompareRightExpression end = GetCompareRightExpression();
                expression.Left = head;
                expression.Right = end;
            }

            return expression;
        }

        private CompareRightExpression GetCompareRightExpression()
        {
            CompareRightExpression expression = null;
            if (Match("#") || Match("~#") || Match("=") || Match("~=") || Match("<") ||
                Match("<=") || Match(">") || Match(">=") || Match("isin"))
            {
                expression = new CompareRightExpression();
                expression.SignToken = CurrentToken;
                MoveNext();
                expression.Value = GetCompareExpression();
            }
            return expression;
        }

        private LogicalAndExpression GetLogicalAndExpression()
        {
            LogicalAndExpression expression = new LogicalAndExpression();
            expression.LeftSide = GetCompareExpression();
            expression.RightSide = GetLogicalAndRightExpression();
            return expression;
        }

        private LogicalAndRightExpression GetLogicalAndRightExpression()
        {
            LogicalAndRightExpression expression = null;
            if (Match("and"))
            {
                expression = new LogicalAndRightExpression();
                MoveNext();
                expression.Value = GetLogicalAndExpression();
            }
            return expression;
        }

        private ArithmeticLevelOneExpression GetArithmeticLevelOneExpression()
        {
            ArithmeticLevelOneExpression expression = new ArithmeticLevelOneExpression();
            expression.LeftSide = GetLogicalAndExpression();
            expression.RightSide = GetArithmeticLevelOneRightExpression();
            return expression;
        }

        private ArithmeticLevelOneRightExpression GetArithmeticLevelOneRightExpression()
        {
            ArithmeticLevelOneRightExpression expression = null;

            if (Match("*") || Match("/") || Match("%"))
            {
                expression = new ArithmeticLevelOneRightExpression();
                expression.SignToken = CurrentToken;
                MoveNext();
                expression.Value = GetArithmeticLevelOneExpression();
            }
            return expression;
        }

        private ArithmeticExpression GetArithmeticExpression()
        {
            ArithmeticExpression expression = new ArithmeticExpression();

            expression.LeftSide = GetArithmeticLevelOneExpression();
            expression.RightSide = GetArithmeticRightExpression();
            return expression;
        }

        private ArithmeticRightExpression GetArithmeticRightExpression()
        {
            ArithmeticRightExpression expression = null;
            if (Match("+") || Match("-"))
            {
                expression = new ArithmeticRightExpression();
                expression.SignToken = CurrentToken;
                MoveNext();
                expression.Value = GetArithmeticExpression();
            }
            return expression;
        }

        private LogicalExpression GetLogicalExpression()
        {
            LogicalExpression expression = new LogicalExpression();
            expression.Left = GetArithmeticExpression();
            expression.Right = GetLogicalRightExpression();
            return expression;
        }

        private LogicalRightExpression GetLogicalRightExpression()
        {
            LogicalRightExpression expression = null;

            if (Match("or"))
            {
                expression = new LogicalRightExpression();
                MoveNext();
                expression.Value = GetLogicalExpression();
            }
            return expression;
        }

        private FunctionCallExpression GetFunctionCallExpression()
        {
            FunctionCallExpression expression = new FunctionCallExpression();
            if (MatchType(TokenType.StringIndentifier))
            {
                expression.FunctionName=CurrentToken.TokenValue;
                MoveNext();
                if (!IsEnd())
                {
                    if (Match("("))
                    {
                        MoveNext();
                        if (!IsEnd())
                        {
                            FillCollectionItems(expression.FunctionArgs);
                        }
                        if (!Match(")"))
                        {
                            if (IsEnd())
                            {
                                throw new Exception("Should be ')', in the end of file");
                            }
                            else
                            {
                                throw new RadSyntaxException("Should be ')'", CurrentToken);
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Should be '('");
                }
            }
            MoveNext();
            return expression;
        }

        private BracketExpression GetBracketExpression()
        {
            BracketExpression bracketExpression = null;
            if (Match("("))
            {
                bracketExpression = new BracketExpression();
                MoveNext();
                BaseExpression firstExpression = GetExpression();
                if (!IsEnd())
                {
                    bracketExpression.Value=firstExpression;
                }
                else
                {
                    throw new Exception("')' expected in the end of file");
                }


                if (!Match(")"))
                {
                    if (IsEnd())
                    {
                        throw new Exception("')' expected, in the end of file");
                    }
                    else
                    {
                        throw new RadSyntaxException("')' expected", CurrentToken);
                    }
                }
                MoveNext();
            }
            return bracketExpression;
        }

        private BracketStatement GetBracketStatement()
        {
            BracketStatement statement = new BracketStatement();
            if (Match("("))
            {
                bracketFlags.Push(true);
                MoveNext();
                statement.Value = ParseStatements();
                if (!Match(")"))
                {
                    if (IsEnd())
                    {

                        throw new Exception("')' expected, in the end of file");
                    }
                    else
                    {
                        throw new RadSyntaxException("')' expected here", CurrentToken);
                    }
                }
                else
                {
                    MoveNext();
                }
                bracketFlags.Pop();
            }

            return statement;
        }
        private void FillCollectionItems(Collection<BaseExpression> items)
        {
            while (!IsEnd() && !Match("]}") && !Match("}") && !Match(")"))
            {
                BaseExpression expression = GetExpression();
                items.Add(expression);
                if (Match(","))
                {
                    MoveNext();
                    if (Match("]}") || Match("}") || Match(")"))
                    {
                        items.Add(null);
                    }
                }
            }
        }

        private bool IsCurrentSpecialFunction()
        {
            if (currentIndex >= radTokens.Count)
            {
                return false;
            }
            string nextName = radTokens[currentIndex + 1].TokenValue;
            if (nextName == "(")
            {
                string currentFunctionName = CurrentToken.TokenValue;
                if (specialFunctions.Contains(currentFunctionName))
                {
                    return true;
                }
            }
            return false;
        }
    }

}
