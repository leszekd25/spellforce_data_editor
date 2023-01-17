namespace SFEngine.SFLua.LuaParser
{
    public class LuaScript
    {
        public string code;
        public int position = 0;

        public LuaScript(string c)
        {
            code = c;
        }

        public int ReadIndex()  // e.g. "[139]" => 139
        {
            position += 1;
            int start = position;
            while (code[position] != ']')
            {
                position += 1;
            }

            return (int)Utility.TryParseUInt32(code.Substring(start, position - start));
        }

        public string ReadString()  // e.g. '1234', "7654tre", "BORE{DOM}"
        {
            char end = code[position];
            position += 1;
            int start = position;
            while (code[position] != end)
            {
                position += 1;
            }

            return code.Substring(start, position - start);
        }

        public string ReadLine()  // used for comments, for example "-- comment"
        {
            int start = position;
            while (code[position] != '\n')
            {
                position += 1;
                if (position == code.Length)
                {
                    position -= 1;
                    break;
                }
            }
            return code.Substring(start, position - start + 1);
        }

        public bool IsIdentifierCharacter(char c)
        {
            if (c < 48)     // '0'
            {
                return false;
            }
            else if (c > 122)    // 'z'
            {
                return false;
            }
            else if (c > 57)
            {
                if (c < 65)
                {
                    return false;
                }
                else if (c > 90)
                {
                    if (c == 95)
                    {
                        return true;
                    }
                    else if (c < 97)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        public string ReadIdentifier()
        {
            int start = position;
            while (IsIdentifierCharacter(code[position]))
            {
                position += 1;
            }

            position -= 1;
            return code.Substring(start, position - start + 1);
        }

        public bool IsNumberCharacter(char c)
        {
            if ((c >= 48) && (c <= 57))
            {
                return true;
            }

            return (c == '+') || (c == '-') || (c == '.');
        }

        public double ReadNumber()
        {
            int start = position;
            while (IsNumberCharacter(code[position]))
            {
                position += 1;
            }

            position -= 1;
            return Utility.TryParseDouble(code.Substring(start, position - start + 1));
        }

        public bool IsWhitespaceCharacter(char c)
        {
            return (c == ' ') || (c == '\n') || (c == '\t') || (c == '\r');
        }
    }
}
