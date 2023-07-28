using PrettierCodeshots.Core;
using PrettierCodeshots.Core.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrettierCodeshots.Sandbox
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var text =
@"namespace PrettierCodeGrabs
{
    public enum TokenType
    {
        Keyword = 0,
        String,
        Operator,
        Unknown = -1
    }

    public class Token
    {
        public TokenType Type { get; private set; }
        public string Value { get; private set; }
        public int StartIndex { get; private set; }
        public int EndIndex { get; private set; }

        public Token(TokenType type, string value, int startIndex, int endIndex)
        {
            Type = type;
            Value = value;
            StartIndex = startIndex;
            EndIndex = endIndex;
            var test = ""This is a string"";
            var multilineTest = ""This a multi line string"" +
                ""string"";
        }
    }
}";

            text = text.Replace("\r", "");
            var lexer = new Lexer(text);
            var result = lexer.Parse();

            var path = Renderer.DrawAndCopyImage(text, result);
        }
    }
}
