using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PrettierCodeshots.Core
{
    public class Lexer
    {
        private Dictionary<TokenType, List<string>> _KeywordsMap; // TODO: Name?

        private int _Index;
        private string _Text;

        public Lexer(string text)
        {
            _KeywordsMap = new Dictionary<TokenType, List<string>>
            {
                { TokenType.Keyword, new List<string> { "namespace", "using", "public", "private", "internal", "sealed", "static", "get", "set" } },
                { TokenType.Type, new List<string> { "enum", "class", "var", "string", "int", "float", "double", "String", "short" } },
                { TokenType.Operator, new List<string> { "=", "-", "+",  "*", "/" } },
            };
            _Text = text.TrimEnd();
        }

        public List<Token> Parse()
        {
            _Index = 0;
            var tokens = new List<Token>();

            while (_Index < _Text.Length - 1)
            {
                while (_Index != _Text.Length - 1 && char.IsWhiteSpace(Peak()))
                {
                    if (Peak() == '\n')
                        tokens.Add(new Token(TokenType.NewLine, "\n", _Index, _Index + 1));
                    if (Peak() == ' ')
                        tokens.Add(new Token(TokenType.Space, " ", _Index, _Index + 1));
                    if (Peak() == '\t')
                        tokens.Add(new Token(TokenType.Tab, "\t", _Index, _Index + 1));
                    _Index++;
                }

                if (Peak() == '\'')
                {
                    tokens.Add(new Token(TokenType.String, _Text.Substring(_Index + 1, 2), _Index, _Index + 2));
                    _Index++;
                }

                if (Peak() == '\"' && _Text[_Index] != '\\')
                {
                    _Index++;
                    var followingText = _Text.Substring(_Index + 1, _Text.Length - _Index - 1);
                    var firstOccurenceInRemaining = FindFirstOccuranceInText(followingText, '"', true);
                    var endOfStringIndex = firstOccurenceInRemaining + _Index;
                    // If end of string operator isn't found, highlight to the end of the file.
                    if (firstOccurenceInRemaining == -1)
                    {
                        tokens.Add(new Token(TokenType.String, followingText, _Index, _Text.Length - 1));
                        return tokens;
                    }
                    tokens.Add(new Token(TokenType.String, _Text.Substring(_Index, firstOccurenceInRemaining + 2), _Index, endOfStringIndex));
                    _Index = endOfStringIndex + 1;
                }

                var startIndex = _Index == 0 ? _Index : ++_Index;
                while (_Index + 1 < _Text.Length && !char.IsWhiteSpace(Peak()))
                    _Index++;
                var substring = _Text.Substring(startIndex, (_Index - startIndex) + 1);
                var tokenType = GetTokenTypeFromString(substring);
                if (tokenType == TokenType.Unknown && substring.EndsWith(";") && substring.Length > 1)
                {
                    substring = substring.Substring(0, substring.Length - 1);
                    tokenType = GetTokenTypeFromString(substring);
                    substring.TrimEnd(';');
                    _Index--;
                }
                var token = new Token(tokenType, substring, startIndex, _Index);
                tokens.Add(token);
            }
            return tokens;
        }

        private char Peak() =>  _Text[_Index + 1];

        private string Peak(int peakCount)
        {
            if (_Index + peakCount >= _Text.Length)
                return Peak(peakCount - 1);
            return _Text.Substring(_Index, peakCount);
        }

        private TokenType GetTokenTypeFromString(string text)
        {
            foreach (KeyValuePair<TokenType, List<string>> entries in _KeywordsMap)
                if (entries.Value.Contains(text))
                    return entries.Key;

            return TokenType.Unknown;
        }

        private int FindFirstOccuranceInText(string haystack, char needle, bool ignoreIfEscaped = false)
        {
            // TODO(nki): Slow linear search :(
            for (int i = 0; i < haystack.Length; i++)
            {
                if (ignoreIfEscaped && i > 0)
                {
                    if (haystack[i] == needle && haystack[i - 1] != '\\')
                        return i;
                }
                else if (haystack[i] == needle)
                    return i;
            }
            return -1;
        }

        private List<int> FindAllOccuranceInText(string haystack, char needle, bool ignoreIfEscaped = false)
        {
            // TODO(nki): Slow linear search :(
            List<int> occurances = new List<int>();
            for (int i = 0; i < haystack.Length; i++)
            {
                if (ignoreIfEscaped && i > 0)
                {
                    if (haystack[i] == needle && haystack[i - 1] != '\\')
                        occurances.Add(i);
                }
                else if (haystack[i] == needle)
                    occurances.Add(i);
            }
            return occurances;
        }
    }
}
