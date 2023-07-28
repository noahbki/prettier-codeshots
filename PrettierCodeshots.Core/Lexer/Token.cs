namespace PrettierCodeshots.Core
{

    public enum TokenType
    {
        Keyword = 0,
        Type,
        String,
        Operator,
        NewLine,
        Space,
        Tab,
        Unknown = -1
    }

    public class Token
    {
        public TokenType Type { get; private set; }
        public string Value { get; set; }
        public int StartIndex { get; private set; }
        public int EndIndex { get; private set; }

        public Token(TokenType type, string value, int startIndex, int endIndex)
        {
            Type = type;
            Value = value;
            StartIndex = startIndex;
            EndIndex = endIndex;
        }
    }
}