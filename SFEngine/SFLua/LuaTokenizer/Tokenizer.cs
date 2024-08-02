using SFEngine.SFLua.LuaDecompiler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.PointOfService;
using Windows.Data.Text;
using System.Configuration;

namespace SFEngine.SFLua.LuaTokenizer
{
    public enum TokenType
    {
        /* terminal symbols denoted by reserved words */
        TK_AND, TK_BREAK,
        TK_DO, TK_ELSE, TK_ELSEIF, TK_END, TK_FOR, TK_FUNCTION, TK_IF, TK_LOCAL,
        TK_NIL, TK_NOT, TK_OR, TK_REPEAT, TK_RETURN, TK_THEN, TK_UNTIL, TK_WHILE,
        /* other terminal symbols */
        TK_NAME, TK_CONCAT, TK_DOTS, TK_EQ, TK_GE, TK_LE, TK_NE, TK_NUMBER,
        TK_STRING, TK_EOS,
        /* these are made up by me :) */
        TK_WHITESPACE, TK_NEWLINE, TK_COMMA, TK_SQBRACKET_OPEN, TK_SQBRACKET_CLOSE,
        TK_DOT_SINGLE, TK_ASSIGN, TK_GT, TK_LT, TK_NEG, TK_COMMENT,
        TK_ADD, TK_SUB, TK_MUL, TK_DIV, TK_EXP,
        TK_RNDBRACKET_OPEN, TK_RNDBRACKET_CLOSE,
        TK_CRLBRACKET_OPEN, TK_CRLBRACKET_CLOSE,
        TK_UPVALUE, TK_MEMBER_ACCESSOR,
    }


    public class Token
    {
        public TokenType type;
        public object value;

        public override string ToString()
        {
            return $"{type} {(value == null? "": value.ToString())}";
        }
    }

    // step 1 of parsing scripts: turn sequence of characters into sequence of tokens
    public class Tokenizer
    {
        List<Token> tokens = new();

        public Tokenizer(string c)
        {
            string result_s;
            double result_d;

            int s = 0;
            while(true)
            {
                if(s == c.Length)
                {
                    tokens.Add(new() { type = TokenType.TK_EOS });
                    break;
                }

                switch(c[s])
                {
                    case ' ':
                    case '\t':
                    case '\r':
                        break;
                    case '\n':
                        break;
                    case '$':
                    case '!':
                    case '&':
                    case '#':
                    case '`':
                    case '@':
                    case '?':
                    case '\\':
                        break;
                    case '+':
                        tokens.Add(new() { type = TokenType.TK_ADD });
                        break;
                    case '-':
                        if ((s + 1 < c.Length) && (c[s + 1] == '-'))
                        {
                            s = ReadCommentString(ref c, s, out result_s);
                            tokens.Add(new() { type = TokenType.TK_COMMENT, value = new string(result_s) });
                        }
                        else
                        {
                            tokens.Add(new() { type = TokenType.TK_SUB });
                        }
                        break;
                    case '*':
                        tokens.Add(new() { type = TokenType.TK_MUL });
                        break;
                    case '/':
                        tokens.Add(new() { type = TokenType.TK_DIV });
                        break;
                    case '^':
                        tokens.Add(new() { type = TokenType.TK_EXP });
                        break;
                    case '(':
                        tokens.Add(new() { type = TokenType.TK_RNDBRACKET_OPEN });
                        break;
                    case ')':
                        tokens.Add(new() { type = TokenType.TK_RNDBRACKET_CLOSE });
                        break;
                    case '{':
                        tokens.Add(new() { type = TokenType.TK_CRLBRACKET_OPEN });
                        break;
                    case '}':
                        tokens.Add(new() { type = TokenType.TK_CRLBRACKET_CLOSE });
                        break;
                    case '[':
                        if ((s + 1 < c.Length) && (c[s + 1] == '['))
                        {
                            s = ReadLongString(ref c, s, out result_s);
                            tokens.Add(new() { type = TokenType.TK_STRING, value = new string(result_s) });
                        }
                        else
                        {
                            tokens.Add(new() { type = TokenType.TK_SQBRACKET_OPEN });
                        }
                        break;
                    case ']':
                        tokens.Add(new() { type = TokenType.TK_SQBRACKET_CLOSE });
                        break;
                    case '%':
                        tokens.Add(new() { type = TokenType.TK_UPVALUE });
                        break;
                    case '=':
                        if ((s + 1 < c.Length) && (c[s + 1] == '='))
                        {
                            tokens.Add(new() { type = TokenType.TK_EQ });
                            s++;
                        }
                        else
                        {
                            tokens.Add(new() { type = TokenType.TK_ASSIGN });
                        }
                        break;
                    case '<':
                        if ((s + 1 < c.Length) && (c[s + 1] == '='))
                        {
                            tokens.Add(new() { type = TokenType.TK_LE });
                            s++;
                        }
                        else
                        {
                            tokens.Add(new() { type = TokenType.TK_LT });
                        }
                        break;
                    case '>':
                        if ((s + 1 < c.Length) && (c[s + 1] == '='))
                        {
                            tokens.Add(new() { type = TokenType.TK_GE });
                            s++;
                        }
                        else
                        {
                            tokens.Add(new() { type = TokenType.TK_GT });
                        }
                        break;
                    case '~':
                        if ((s + 1 < c.Length) && (c[s + 1] == '='))
                        {
                            tokens.Add(new() { type = TokenType.TK_NE });
                            s++;
                        }
                        else
                        {
                            tokens.Add(new() { type = TokenType.TK_NEG });
                        }
                        break;
                    case '"':
                    case '\'':
                        s = ReadString(ref c, s, out result_s);
                        tokens.Add(new() { type = TokenType.TK_STRING, value = new string(result_s) });
                        break;
                    case '.':
                        if (s + 1 < c.Length)
                        {
                            if (c[s + 1] == '.')
                            {
                                if (s + 2 < c.Length)
                                {
                                    if (c[s + 2] == '.')
                                    {
                                        tokens.Add(new() { type = TokenType.TK_DOTS });
                                        s += 2;
                                    }
                                    else
                                    {
                                        tokens.Add(new() { type = TokenType.TK_CONCAT });
                                        s += 1;
                                    }
                                }
                                else
                                {
                                    tokens.Add(new() { type = TokenType.TK_CONCAT });
                                    s += 1;
                                }
                            }
                            else
                            {
                                if ((c[s + 1] >= '0') && (c[s + 1] <= '9'))
                                {
                                    s = ReadNumber(ref c, s, out result_d);
                                    tokens.Add(new() { type = TokenType.TK_NUMBER, value = result_d });
                                }
                            }
                        }
                        else
                        {
                            tokens.Add(new() { type = TokenType.TK_DOT_SINGLE });
                        }
                        break;
                    case ':':
                        tokens.Add(new() { type = TokenType.TK_MEMBER_ACCESSOR });
                        break;
                    case ',':
                        tokens.Add(new() { type = TokenType.TK_COMMA });
                        break;
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        s = ReadNumber(ref c, s, out result_d);
                        tokens.Add(new() { type = TokenType.TK_NUMBER, value = result_d });
                        break;
                    case '_':
                        goto tname;
                    default:
                        if (!char.IsAsciiLetter(c[s]))
                        {
                            if(char.IsControl(c[s]))
                            {
                                throw new Exception("Tokenizer(): invalid character");
                            }
                            break;
                        }
                    tname:
                        s = ReadNameString(ref c, s, out result_s);
                        // check reserved words
                        switch (result_s)
                        {
                            case "and":
                                tokens.Add(new() { type = TokenType.TK_AND });
                                break;
                            case "break":
                                tokens.Add(new() { type = TokenType.TK_BREAK });
                                break;
                            case "do":
                                tokens.Add(new() { type = TokenType.TK_DO });
                                break;
                            case "else":
                                tokens.Add(new() { type = TokenType.TK_ELSE });
                                break;
                            case "elseif":
                                tokens.Add(new() { type = TokenType.TK_ELSEIF });
                                break;
                            case "end":
                                tokens.Add(new() { type = TokenType.TK_END });
                                break;
                            case "for":
                                tokens.Add(new() { type = TokenType.TK_FOR });
                                break;
                            case "function":
                                tokens.Add(new() { type = TokenType.TK_FUNCTION });
                                break;
                            case "if":
                                tokens.Add(new() { type = TokenType.TK_IF });
                                break;
                            case "local":
                                tokens.Add(new() { type = TokenType.TK_LOCAL });
                                break;
                            case "nil":
                                tokens.Add(new() { type = TokenType.TK_NIL });
                                break;
                            case "not":
                                tokens.Add(new() { type = TokenType.TK_NOT });
                                break;
                            case "or":
                                tokens.Add(new() { type = TokenType.TK_OR });
                                break;
                            case "repeat":
                                tokens.Add(new() { type = TokenType.TK_REPEAT });
                                break;
                            case "return":
                                tokens.Add(new() { type = TokenType.TK_RETURN });
                                break;
                            case "then":
                                tokens.Add(new() { type = TokenType.TK_THEN });
                                break;
                            case "until":
                                tokens.Add(new() { type = TokenType.TK_UNTIL });
                                break;
                            case "while":
                                tokens.Add(new() { type = TokenType.TK_WHILE });
                                break;
                            default:
                                tokens.Add(new() { type = TokenType.TK_NAME, value = new string(result_s) });
                                break;
                        }

                        break;
                }

                s++;
            }
        }

        private int ReadNameString(ref string c, int s, out string result)
        {
            int start = s;
            while (true)
            {
                if (s == c.Length)
                {
                    break;
                }

                if ((char.IsAsciiLetterOrDigit(c[s])) || (c[s] == '_'))
                {
                    s++;
                    continue;
                }
                break;
            }
            result = c.Substring(start, s - start);
            s -= 1;
            return s;
        }

        private int ReadNumber(ref string c, int s, out double result)
        {
            int start = s;
            int dotpos = -1;
            int exppos = -1;
            bool stop = false;
            while(s < c.Length)
            {
                switch(c[s])
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        s++;
                        break;
                    case '.':
                        if(dotpos == s-1)
                        {
                            throw new Exception("Tokenizer.ReadNumber(): invalid number");
                        }
                        else if(dotpos == -1)
                        {
                            if(exppos != -1)
                            {
                                stop = true;
                                break;
                            }
                            dotpos = s;
                            s++;
                        }
                        else
                        {
                            stop = true;
                            break;
                        }
                        break;
                    case 'e':
                    case 'E':
                        if(exppos != -1)
                        {
                            stop = true;
                            break;
                        }
                        exppos = s;
                        s++;
                        break;
                    case '+':
                    case '-':
                        if(exppos != s-1)
                        {
                            stop = true;
                            break;
                        }
                        s++;
                        break;
                    default:
                        stop = true;
                        break;
                }
                if(stop)
                {
                    break;
                }
            }
            double.TryParse(c.Substring(start, s - start), out result);
            return s - 1;
        }

        private int ReadString(ref string c, int s, out string result)
        {
            char lim = c[s];
            s += 1;
            StringWriter sw = new();
            while (true)
            {
                if (s == c.Length)
                {
                    throw new Exception("Tokenizer.ReadString(): Unfinished string");
                }
                if (c[s] == lim)
                {
                    break;
                }

                switch (c[s])
                {
                    case '\\':
                        if (s + 1 < c.Length)
                        {
                            switch (c[s + 1])
                            {
                                case 'a':
                                    sw.Write('\a');
                                    break;
                                case 'b':
                                    sw.Write('\b');
                                    break;
                                case 'f':
                                    sw.Write('\f');
                                    break;
                                case 'n':
                                    sw.Write('\n');
                                    break;
                                case 'r':
                                    sw.Write('\r');
                                    break;
                                case 't':
                                    sw.Write('\t');
                                    break;
                                case 'v':
                                    sw.Write('\v');
                                    break;
                                case '\n':
                                    sw.Write('\n');
                                    break;
                                case '0':
                                case '1':
                                case '2':
                                case '3':
                                case '4':
                                case '5':
                                case '6':
                                case '7':
                                case '8':
                                case '9':
                                    int cc = 0;
                                    int ii = 0;
                                    do
                                    {
                                        cc = 10 * cc + (c[s + 1] - '0');
                                        s++;
                                        ii++;
                                        if(ii >= 3)
                                        {
                                            break;
                                        }
                                        if(s + 1 >= c.Length)
                                        {
                                            break;
                                        }
                                        if ((c[s + 1] < '0') || (c[s + 1] > '9'))
                                        {
                                            break;
                                        }
                                    }
                                    while (true);
                                    if(cc >= 256)
                                    {
                                        throw new Exception("Tokenizer.ReadString(): invalid character escape sequence");
                                    }
                                    sw.Write((char)cc);
                                    s++;
                                    break;
                                default:
                                    sw.Write(c[s]);
                                    s++;
                                    sw.Write(c[s]);
                                    break;
                            }
                        }
                        break;
                    default:
                        sw.Write(c[s]);
                        break;
                }
                s++;
            }
            result = sw.ToString();
            return s;
        }

        private int ReadLongString(ref string c, int s, out string result)
        {
            int cont = 0;
            s += 2;
            cont += 1;
            int start = s;
            while (true)
            {
                if (s == c.Length)
                {
                    throw new Exception("Tokenizer.ReadLongString(): Unfinished string");
                }

                switch (c[s])
                {
                    case '[':
                        if ((s + 1 < c.Length) && (c[s + 1] == '['))
                        {
                            cont += 1;
                        }
                        break;
                    case ']':
                        if ((s + 1 < c.Length) && (c[s + 1] == ']'))
                        {
                            cont -= 1;
                            if (cont == 0)
                            {
                                goto endloop;
                            }
                        }
                        break;
                    default:
                        break;
                }

                s++;
            }
        endloop:
            result = c.Substring(start, s - start);
            s += 1;
            return s;
        }

        private int ReadCommentString(ref string c, int s, out string result)
        {
            s += 2;
            int start = s;
            while (true)
            {
                if (s == c.Length)
                {
                    goto endloop;
                }

                switch (c[s])
                {
                    case '\n':
                        goto endloop;
                    default:
                        break;
                }

                s++;
            }
        endloop:
            result = c.Substring(start, s - start);
            s -= 1;
            return s;
        }
    }
}
