using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFEngine.SFLua.LuaParser
{
    public enum ParseState
    {
        READ_START,
        READ_END,
        READ_EQUAL_SIGN,
        READ_IDENTIFIER,
        READ_VALUE,
        READ_COMMA
    }

    public class LuaTable
    {
        public Dictionary<object, object> entries = new Dictionary<object, object>();
        ParseState state = ParseState.READ_START;

        public object this[object key]
        {
            get
            {
                if (!entries.ContainsKey(key))
                    return null;
                return entries[key];
            }
            set
            {
                if (!entries.ContainsKey(key))
                    entries.Add(key, value);
                else
                    entries[key] = value;
            }
        }

        public bool Parse(LuaScript scr)
        {
            entries.Clear();
            int max_index = 0;
            object next_key = null;

            while (true)
            {
                //char c = scr.code[scr.position];
                //System.Diagnostics.Debug.WriteLine("POS " + scr.position.ToString() + " | CHAR " + c + " | STATE "+state.ToString());
                switch (scr.code[scr.position])
                {
                    case '{':
                        if (state == ParseState.READ_IDENTIFIER)
                        {
                            max_index += 1;
                            next_key = (double)max_index;
                            state = ParseState.READ_VALUE;
                        }
                        if (state == ParseState.READ_VALUE)
                        {
                            LuaTable t = new LuaTable();
                            if (!t.Parse(scr))
                                return false;
                            if (entries.ContainsKey(next_key))
                                entries[next_key] = t;
                            else
                                entries.Add(next_key, t);
                            next_key = null;
                            state = ParseState.READ_COMMA;
                        }
                        if (state == ParseState.READ_START)
                            state = ParseState.READ_IDENTIFIER;
                        break;
                    case '}':
                        if ((state == ParseState.READ_COMMA) || (state == ParseState.READ_IDENTIFIER))
                            state = ParseState.READ_END;
                        if (state == ParseState.READ_END)
                            return true;
                        break;
                    case '[':
                        if(state == ParseState.READ_IDENTIFIER)
                        {
                            int index = scr.ReadIndex();
                            next_key = (double)index;
                            state = ParseState.READ_EQUAL_SIGN;
                        }
                        break;
                    case '=':
                        if (state == ParseState.READ_EQUAL_SIGN)
                            state = ParseState.READ_VALUE;
                        break;
                    case ',':
                        if(state == ParseState.READ_EQUAL_SIGN)
                        {
                            max_index += 1;
                            if (entries.ContainsKey((double)max_index))
                                entries[(double)max_index] = next_key;
                            else
                                entries.Add((double)max_index, next_key);   // named constants, named functions etc. (handle differently, todo) 
                            next_key = null;
                            state = ParseState.READ_COMMA;
                        }
                        if (state == ParseState.READ_COMMA)
                            state = ParseState.READ_IDENTIFIER;
                        break;
                    case '"':
                    case '\'':
                        if (state == ParseState.READ_IDENTIFIER)
                        {
                            max_index += 1;
                            next_key = (double)max_index;
                            state = ParseState.READ_VALUE;
                        }
                        if(state == ParseState.READ_VALUE)
                        {
                            string str = scr.ReadString();
                            if (entries.ContainsKey(next_key))
                                entries[next_key] = str;
                            else
                                entries.Add(next_key, str);
                            next_key = null;
                            state = ParseState.READ_COMMA;
                        }
                        break;
                    default:
                        if(scr.IsWhitespaceCharacter(scr.code[scr.position]))
                        {
                            while (scr.IsWhitespaceCharacter(scr.code[scr.position]))
                            {
                                scr.position += 1;
                                if (scr.position == scr.code.Length)
                                    break;
                            }
                            scr.position -= 1;
                            break;
                        }
                        if((scr.IsIdentifierCharacter(scr.code[scr.position]))&&(!scr.IsNumberCharacter(scr.code[scr.position])))
                        {
                            if (state == ParseState.READ_IDENTIFIER)
                            {
                                next_key = scr.ReadIdentifier();
                                state = ParseState.READ_EQUAL_SIGN;
                            }
                            if(state == ParseState.READ_VALUE)
                            {
                                string val = scr.ReadIdentifier();
                                if (entries.ContainsKey(next_key))
                                    entries[next_key] = val;
                                else
                                    entries.Add(next_key, val);
                                next_key = null;
                                state = ParseState.READ_COMMA;
                            }
                        }
                        else if(scr.IsNumberCharacter(scr.code[scr.position]))
                        {
                            if ((scr.code[scr.position] == '-') && (scr.code[scr.position + 1] == '-')) // comment
                            {
                                scr.ReadLine();
                            }
                            else
                            {
                                if (state == ParseState.READ_IDENTIFIER)
                                {
                                    // check if comment
                                    max_index += 1;
                                    next_key = (double)max_index;
                                    state = ParseState.READ_VALUE;
                                }
                                if (state == ParseState.READ_VALUE)
                                {
                                    double val = scr.ReadNumber();
                                    if (entries.ContainsKey(next_key))
                                        entries[next_key] = val;
                                    else
                                        entries.Add(next_key, val);
                                    next_key = null;
                                    state = ParseState.READ_COMMA;
                                }
                            }
                        }
                        break;
                }
                scr.position += 1;

                if (scr.position == scr.code.Length)
                    return false;
            }
        }
    }
}
