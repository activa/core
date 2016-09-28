using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Iridium.Core
{
    public class JsonTokenizer
    {
        public JsonTokenizer(string s)
        {
            _charFeeder = new StringCharFeeder(s);
        }

        public JsonTokenizer(Stream stream)
        {
            _charFeeder = new StreamCharFeeder(stream);
        }

        public abstract class CharFeeder
        {
            private bool _backTracked = false;
            private char _current;

            public char Current => _current;

            protected abstract int ReadNext();

            public char Next()
            {
                if (_backTracked)
                {
                    _backTracked = false;
                }
                else
                {
                    int next = ReadNext();

                    if (next < 0)
                        _current = (char) 0;
                    else
                        _current = (char) next;
                }

                return _current;
            }

            public void Backtrack()
            {
                _backTracked = true;
            }
        }

        public class StringCharFeeder : CharFeeder
        {
            private string _s;
            private int _index;

            public StringCharFeeder(string s)
            {
                _s = s;
                _index = 0;
            }

            protected override int ReadNext()
            {
                return _index >= _s.Length ? -1 : _s[_index++];
            }
        }

        public class StreamCharFeeder : CharFeeder
        {
            private readonly StreamReader _reader;
            private char _c;

            public StreamCharFeeder(Stream stream)
            {
                _reader = new StreamReader(stream, Encoding.UTF8);
            }

            protected override int ReadNext()
            {
                return _reader.Read();
            }
        }


        private readonly CharFeeder _charFeeder;

        //private JsonToken _token;

        private static Dictionary<char, JsonTokenType> _tokenTypes = new Dictionary<char, JsonTokenType>()
        {
            {'{', JsonTokenType.ObjectStart},
            {'}', JsonTokenType.ObjectEnd },
            {'[',JsonTokenType.ArrayStart },
            {']',JsonTokenType.ArrayEnd },
            {',',JsonTokenType.Comma },
            {':',JsonTokenType.Colon }
        };

        private static Dictionary<string, JsonTokenType> _keywords = new Dictionary<string, JsonTokenType>()
        {
            {"null", JsonTokenType.Null},
            {"true", JsonTokenType.True},
            {"false", JsonTokenType.False}
        };

        public JsonToken NextToken()
        {
            for (;;)
            {
                char c = _charFeeder.Next();

                if (char.IsWhiteSpace(c))
                    continue;

                JsonTokenType tokenType;

                if (_tokenTypes.TryGetValue(c, out tokenType))
                {
                    return new JsonToken(tokenType);
                }
                if (c == '"')
                {
                    return ReadStringToken();
                }
                if (char.IsDigit(c))
                {
                    return ReadNumber();
                }
                if (char.IsLetter(c))
                {
                    return ReadKeyword();
                }

                return new JsonToken(JsonTokenType.EOF);
            }
        }

        private JsonToken ReadKeyword()
        {
            char c = _charFeeder.Current;

            string keyword = c.ToString();

            for (;;)
            {
                JsonTokenType match;

                if (_keywords.TryGetValue(keyword, out match))
                {
                    return new JsonToken(match);
                }

                keyword += _charFeeder.Next();

                if (!char.IsLetter(c))
                    throw new Exception();
            }
        }

        private JsonToken ReadNumber()
        {
            bool hasDot = false;
            string s = new string(_charFeeder.Current,1);

            for (;;)
            {
                char c = _charFeeder.Next();

                if (c == '.' && !hasDot)
                {
                    hasDot = true;
                    s += c;
                }
                else if (char.IsDigit(c))
                {
                    s += c;
                }
                else
                {
                    _charFeeder.Backtrack();

                    return new JsonToken(hasDot ? JsonTokenType.Float : JsonTokenType.Integer, s);
                }
            }
        }

        private JsonToken ReadStringToken()
        {
            bool inEscape = false;
            bool foundEscape = false;
            StringBuilder s = new StringBuilder();

            for (;;)
            {
                char c = _charFeeder.Next();

                if (c == 0)
                    throw new Exception();

                if (inEscape)
                {
                    inEscape = false;
                }
                else if (c == '\\')
                {
                    inEscape = true;
                    foundEscape = true;
                }
                else if (c == '"')
                {
                    if (foundEscape)
                    {
                        s.Replace("\\n", "\n");
                        s.Replace("\\r", "\r");
                        s.Replace("\\t", "\t");
                        s.Replace("\\\"", "\"");

                        if (s.ToString().IndexOf("\\u", StringComparison.Ordinal) >= 0)
                        {
                            s = new StringBuilder(Regex.Replace(s.ToString(), @"\\[uU][a-fA-F0-9]{4}", m => ((char) uint.Parse(m.Value.Substring(2), NumberStyles.HexNumber)).ToString()));
                        }

                        s.Replace("\\\\", "\\");
                        s.Replace("\\/", "/");
                    }

                    return new JsonToken(JsonTokenType.String, s.ToString());
                }

                s.Append(c);
            }
        }

    }

    public enum JsonTokenType
    {
        ObjectStart,
        ObjectEnd,
        ArrayStart,
        ArrayEnd,
        Null,
        True,
        False,
        Whitespace,
        Comma,
        Colon,
        Integer,
        Float,
        String,
        EOF
    }
}