using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace SM.Smorgasbord.RADParser
{
    public class RADLexer
    {
        private Collection<RADToken> tokens = new Collection<RADToken>();
        //Index of the current cotent;
        private int currentIndex;
        //Current line
        private int currentLine;
        //The position in current line.
        private int positionOfLine;
        private string rawString;

        private HashSet<char> determinedKeyChars = new HashSet<char>();
        private HashSet<char> undeterminedKeyChars = new HashSet<char>();
        private HashSet<string> keyWords = new HashSet<string>();
        private HashSet<string> boolConsts = new HashSet<string>();

        public RADLexer(string rawString)
        {
            currentLine = 1;
            positionOfLine = 1;
            currentIndex = 0;

            this.rawString = rawString;

            ConstructDeterminedKeys();
            ConstructUndeterminedFirstChars();
            ConstructKeyWords();
            ConstructBoolConst();
        }

        public Collection<RADToken> Tokens
        {
            get
            {
                return tokens;
            }
        }
        public void Build()
        {
            int length = rawString.Length;
            SkipBlanks();
            while (currentIndex < length)
            {
                char firstChar = rawString[currentIndex];
                if (firstChar == '$')
                {
                    RADToken temToken = GetVariableToken();
                    tokens.Add(temToken);
                }
                else if (firstChar == '"')
                {
                    RADToken temToken = GetStringConstToken();
                    tokens.Add(temToken);
                }
                else if (firstChar == '\'')
                {
                    RADToken temToken = GetDateConstToken();
                    tokens.Add(temToken);
                }
                //This is comments.
                else if (firstChar == '/' && currentIndex + 1 < length && rawString[currentIndex + 1] == '/')
                {
                    //No need to check the result of the method, because of the pre condition.
                    MoveNext();
                    tokens.Add(GetCommentsToken());
                    tokens.Add(new RADToken(TokenType.Key, ";"));
                }
                else if (firstChar == '+' || firstChar == '-')
                {
                    if (tokens.Count() == 0 || LastToken.TokenValue == "("
                        || LastToken.TokenValue == "if" || LastToken.TokenValue == "then"
                        || LastToken.TokenValue == "else" || LastToken.TokenValue == ","
                        || LastToken.TokenValue == ";" || LastToken.TokenValue == "while"
                        || LastToken.TokenValue == "do" || LastToken.TokenValue == "to"
                        || LastToken.TokenValue == "for" || LastToken.TokenValue == "=")
                    {
                        //This sign is associated with a number.
                        RADToken temToken = GetSignedNumber();
                        tokens.Add(temToken);
                    }
                    else
                    {
                        if (currentIndex + 1 < length && rawString[currentIndex + 1] == '=')
                        {
                            //+= , -= is a single sign.
                            string str = "";
                            str += firstChar;
                            str += rawString[currentIndex + 1];
                            RADToken temToken = new RADToken(TokenType.Key, str);
                            //Set line info.
                            temToken.Line = currentLine;
                            temToken.PositionOfLine = positionOfLine;
                            tokens.Add(temToken);
                            //Move two steps. One for next '=' and second for next char.
                            MoveNext();
                            MoveNext();

                        }
                        else
                        {
                            // +, - is a single char sign.
                            string str = "";
                            str += firstChar;
                            RADToken temToken = new RADToken(TokenType.Key, str);

                            //Set line info.
                            temToken.Line = currentLine;
                            temToken.PositionOfLine = positionOfLine;

                            tokens.Add(temToken);
                            MoveNext();
                        }
                    }
                }
                else if (firstChar == '*' || firstChar == '/')
                {
                    if (currentIndex + 1 < length && rawString[currentIndex + 1] == '=')
                    {
                        //*= , /= is a single sign.
                        string str = "";
                        str += firstChar;
                        str += rawString[currentIndex + 1];
                        RADToken temToken = new RADToken(TokenType.Key, str);
                        //Set line info.
                        temToken.Line = currentLine;
                        temToken.PositionOfLine = positionOfLine;
                        tokens.Add(temToken);
                        //Move two steps. One for next '=' and second for next char.
                        MoveNext();
                        MoveNext();

                    }
                    else
                    {
                        // *, / is a single char sign.
                        string str = "";
                        str += firstChar;
                        RADToken temToken = new RADToken(TokenType.Key, str);
                        //Set line info.
                        temToken.Line = currentLine;
                        temToken.PositionOfLine = positionOfLine;

                        tokens.Add(temToken);
                        MoveNext();
                    }
                }
                else if (firstChar == '{')
                {
                    if (MoveNext())
                    {
                        if (CurrentChar == '[')
                        {
                            MoveNext();
                            RADToken temToken = new RADToken(TokenType.Key, "{[");
                            temToken.Line = currentLine;
                            temToken.PositionOfLine = positionOfLine;
                            tokens.Add(temToken);
                        }
                        else
                        {
                            RADToken temToken = new RADToken(TokenType.Key, "{");

                            temToken.Line = currentLine;
                            temToken.PositionOfLine = positionOfLine;
                            tokens.Add(temToken);
                        }
                    }
                    else
                    {
                        RADToken temToken = new RADToken(TokenType.Key, "{");
                        temToken.Line = currentLine;
                        temToken.PositionOfLine = positionOfLine;
                        tokens.Add(temToken);
                    }
                }
                else if (firstChar == ']')
                {
                    if (MoveNext())
                    {
                        if (CurrentChar != '}')
                        {
                            RADToken temToken = new RADToken(TokenType.Key, "]");

                            temToken.Line = currentLine;
                            temToken.PositionOfLine = positionOfLine;
                            tokens.Add(temToken);
                        }
                        else
                        {
                            MoveNext();
                            RADToken temToken = new RADToken(TokenType.Key, "]}");

                            temToken.Line = currentLine;
                            temToken.PositionOfLine = positionOfLine;
                            tokens.Add(temToken);
                        }
                    }
                    else
                    {
                        MoveNext();
                        RADToken temToken = new RADToken(TokenType.Key, "]}");

                        temToken.Line = currentLine;
                        temToken.PositionOfLine = positionOfLine;
                        tokens.Add(temToken);
                    }
                }
                else if (IsDigit(firstChar) ||
                    IsCharacter(firstChar) ||
                    firstChar == '_')
                {
                    RADToken temToken = GetUndeterminedToken();
                    tokens.Add(temToken);
                }
                else if (IsDeterminedKeyChar(firstChar))
                {
                    string str = "";
                    str += firstChar;
                    RADToken temToken = new RADToken(TokenType.Key, str);

                    temToken.Line = currentLine;
                    temToken.PositionOfLine = positionOfLine;
                    tokens.Add(temToken);
                    MoveNext();
                }
                else if (IsUndeterminedFirstChar(firstChar))
                {
                    RADToken temToken = GetUndeterminedKeyToken();
                    tokens.Add(temToken);
                }
                else
                {
                    string errorString = "not valid char:";
                    errorString += firstChar;
                    throw new Exception(errorString);
                }

                SkipBlanks();
            }

        }

        private char CurrentChar
        {
            get
            {
                return rawString[currentIndex];
            }
        }
        private void ConstructDeterminedKeys()
        {
            determinedKeyChars.Add(',');
            determinedKeyChars.Add(';');
            determinedKeyChars.Add('(');
            determinedKeyChars.Add(')');
            determinedKeyChars.Add('=');
            determinedKeyChars.Add('#');
            determinedKeyChars.Add('}');
        }

        private void ConstructUndeterminedFirstChars()
        {
            //The following to entries are handled in other place.
            undeterminedKeyChars.Add('{');
            undeterminedKeyChars.Add(']');

            undeterminedKeyChars.Add('~');
            undeterminedKeyChars.Add('<');
            undeterminedKeyChars.Add('>');
        }

        private void ConstructBoolConst()
        {
            boolConsts.Add("true");
            boolConsts.Add("t");
            boolConsts.Add("T");
            boolConsts.Add("TRUE");
            boolConsts.Add("Y");
            boolConsts.Add("y");
            boolConsts.Add("yes");
            boolConsts.Add("YES");
            boolConsts.Add("false");
            boolConsts.Add("f");
            boolConsts.Add("F");
            boolConsts.Add("FALSE");
            boolConsts.Add("N");
            boolConsts.Add("n");
            boolConsts.Add("NO");
            boolConsts.Add("unknown");
        }

        private void ConstructKeyWords()
        {
            keyWords.Add("do");
            keyWords.Add("while");
            keyWords.Add("for");
            keyWords.Add("in");
            keyWords.Add("do");
            keyWords.Add("to");
            keyWords.Add("if");
            keyWords.Add("then");
            keyWords.Add("else");
            keyWords.Add("and");
            keyWords.Add("or");
            keyWords.Add("not");
            keyWords.Add("isin");
        }
        private bool MoveNext()
        {
            currentIndex++;
            if (currentIndex >= rawString.Length)
            {
                return false;
            }
            return true;
        }

        private bool IsDigit(char ch)
        {
            if (ch >= '0' && ch <= '9')
            {
                return true;
            }
            return false;
        }

        private bool IsCharacter(char ch)
        {

            if ((ch >= 'A' && ch <= 'Z') ||
                (ch >= 'a' && ch <= 'z'))
            {
                return true;
            }
            return false;
        }

        private bool IsBlankOrEnd()
        {
            //Now end of the raw string.
            if (currentIndex >= rawString.Length)
            {
                return true;
            }
            //Check if current is blank.
            if (rawString[currentIndex] == ' ' ||
                rawString[currentIndex] == '\t' ||
                rawString[currentIndex] == '\r' ||
                rawString[currentIndex] == '\n')
            {
                return true;
            }
            return false;
        }

        private bool IsBoolConst(string str)
        {
            return boolConsts.Contains(str);
        }

        private bool IsKeyWord(string str)
        {
            return keyWords.Contains(str);
        }

        private bool IsDeterminedKeyChar(char ch)
        {
            return determinedKeyChars.Contains(ch);
        }

        private bool IsUndeterminedFirstChar(char ch)
        {
            return undeterminedKeyChars.Contains(ch);
        }

        private RADToken LastToken
        {
            get
            {
                return tokens[tokens.Count - 1];
            }
        }

        private RADToken GetSignedNumber()
        {
            RADToken token = new RADToken();
            token.Line = currentLine;
            token.PositionOfLine = positionOfLine;
            StringBuilder str = new StringBuilder();
            str.Append(CurrentChar);

            while (MoveNext())
            {
                char currentChar = CurrentChar;
                if (IsDigit(currentChar) || IsCharacter(currentChar) || currentChar == '.')
                {
                    str.Append(currentChar);
                }
                else
                {
                    break;
                }
            }
            token.TokenType = TokenType.Number;
            token.TokenValue = str.ToString();
            return token;
        }

        private RADToken GetCommentsToken()
        {
            RADToken token = new RADToken();
            token.Line = currentLine;
            token.PositionOfLine = positionOfLine;
            StringBuilder temStr = new StringBuilder();

            while (MoveNext())
            {
                if (CurrentChar != '\r')
                {
                    temStr.Append(CurrentChar);
                }
                else
                {
                    break;
                }
            }
            token.TokenType = TokenType.Comments;
            token.TokenValue = temStr.ToString();
            return token;
        }

        private RADToken GetDateConstToken()
        {
            RADToken token = new RADToken();
            token.Line = currentLine;
            token.PositionOfLine = positionOfLine;

            token.TokenType = TokenType.DateConst;
            StringBuilder dateString = new StringBuilder("'");
            bool isSuccess = false;

            while (MoveNext())
            {
                dateString.Append(CurrentChar);
                if (CurrentChar == '\'')
                {
                    MoveNext();
                    isSuccess = true;
                    break;
                }
            }
            if (!isSuccess)
            {
                throw new Exception("Date const not valid");
            }
            token.TokenValue = dateString.ToString();
            return token;
        }

        private RADToken GetStringConstToken()
        {
            RADToken token = new RADToken();
            token.Line = currentLine;
            token.PositionOfLine = positionOfLine;

            StringBuilder str = new StringBuilder("\"");
            token.TokenType = TokenType.StringConst;

            //Used to check whether the last char is escape char. that means \

            bool isLastEscapeChar = false;
            bool isSuccess = false;
            while (MoveNext())
            {
                str.Append(CurrentChar);
                if (CurrentChar == '"')
                {
                    if (!isLastEscapeChar)
                    {
                        isSuccess = true;
                        MoveNext();
                        break;
                    }
                    else
                    {
                        isLastEscapeChar = false;
                    }
                }
                else
                {
                    if (CurrentChar == '\\' && !isLastEscapeChar)
                    {
                        isLastEscapeChar = true;
                    }
                    else
                    {
                        isLastEscapeChar = false;
                    }
                }
            }

            if (!isSuccess)
            {
                throw new Exception("String const  not valid");
            }
            token.TokenValue = str.ToString();
            return token;
        }

        private RADToken GetVariableToken()
        {
            RADToken token = new RADToken();
            token.Line = currentLine;
            token.PositionOfLine = positionOfLine;

            StringBuilder str = new StringBuilder("$");
            token.TokenType = TokenType.Variable;
            while (MoveNext())
            {
                if (CurrentChar == '.' || IsDigit(CurrentChar) || IsCharacter(CurrentChar))
                {
                    str.Append(CurrentChar);
                }
                else
                {
                    break;
                }
            }
            token.TokenValue = str.ToString();
            return token;
        }

        private RADToken GetUndeterminedKeyToken()
        {
            RADToken temToken = new RADToken();

            temToken.Line = (currentLine);
            temToken.PositionOfLine = (positionOfLine);

            temToken.TokenType = TokenType.Key;
            char firstChar = CurrentChar;
            StringBuilder tokenString = new StringBuilder();
            tokenString.Append(firstChar);

            MoveNext();
            if (!IsBlankOrEnd())
            {
                char nextChar = CurrentChar;
                if (nextChar == '#' || nextChar == '=')
                {
                    tokenString.Append(CurrentChar);
                    MoveNext();
                }
            }
            temToken.TokenValue = tokenString.ToString();
            return temToken;
        }


        private RADToken GetUndeterminedToken()
        {
            RADToken token = new RADToken();
            token.Line = (currentLine);
            token.PositionOfLine = (positionOfLine);

            StringBuilder temStr = new StringBuilder();
            temStr.Append(CurrentChar);

            bool isNumber = true;

            while (MoveNext())
            {
                if (IsDigit(CurrentChar) || IsCharacter(CurrentChar) || CurrentChar == '.' || CurrentChar == '_')
                {
                    temStr.Append(CurrentChar);
                    if (!IsDigit(CurrentChar))
                    {
                        isNumber = false;
                    }
                }
                else
                {
                    break;
                }
            }

            if (isNumber)
            {
                token.TokenType = TokenType.Number;
            }
            else if (IsKeyWord(temStr.ToString()))
            {
                token.TokenType = TokenType.Key;
            }
            else if (IsBoolConst(temStr.ToString()))
            {
                token.TokenType = TokenType.BoolConst;
            }
            else
            {
                token.TokenType = TokenType.StringIndentifier;
            }
            token.TokenValue = temStr.ToString();

            return token;
        }


        private void SkipBlanks()
        {
            while (currentIndex < rawString.Length)
            {
                if (rawString[currentIndex] == ' ' ||
                    rawString[currentIndex] == '\t' ||
                    rawString[currentIndex] == '\r' ||
                    rawString[currentIndex] == '\n')
                {
                    // Store the current line and position of the line.
                    if (rawString[currentIndex] == '\n')
                    {
                        positionOfLine = 1;
                        currentLine++;
                    }
                    else
                    {
                        positionOfLine++;
                    }

                    currentIndex++;
                }
                else
                {
                    break;
                }
            }
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            foreach (RADToken token in tokens)
            {
                str.AppendLine(token.ToString());
            }
            return str.ToString();
        }
    }
}
