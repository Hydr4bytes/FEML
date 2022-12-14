using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FEML
{
    internal enum TokenType
    {
        Equals,
        Comma,
        OpenParenthesis,
        CloseParenthesis,
        OpenSquareBracket,
        CloseSquareBracket,
        OpenCurlyBracket,
        CloseCurlyBracket,
        Identifier,
        Comment,
        String,
        Number,
        Boolean,
        Semicolon,
        SequenceTerminator,
    }

    internal class TokenDefinition
    {
        private Regex regex;
        private readonly TokenType returnToken;

        public TokenMatch Match(string inputString)
        {
            var match = regex.Match(inputString);
            if (match.Success)
            {
                string remainingText = string.Empty;
                if (match.Length != inputString.Length)
                    remainingText = inputString.Substring(match.Length);

                return new TokenMatch()
                {
                    IsMatch = true,
                    RemainingText = remainingText,
                    TokenType = returnToken,
                    Value = match.Value
                };
            }
            else
            {
                return new TokenMatch() { IsMatch = false };
            }
        }

        public TokenDefinition(TokenType token, string regexPattern)
        {
            this.regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
            this.returnToken = token;
        }
    }

    internal class TokenMatch
    {
        public bool IsMatch { get; set; }
        public TokenType TokenType { get; set; }
        public string Value { get; set; }
        public string RemainingText { get; set; }
    }

    internal class Token
    {
        public TokenType TokenType { get; set; }
        public string Value { get; set; }

        public Token(TokenType tokenType)
        {
            TokenType = tokenType;
            Value = string.Empty;
        }

        public Token(TokenType tokenType, string value)
        {
            TokenType = tokenType;
            Value = value;
        }
    }

    internal static class Lexer
    {
        private static readonly TokenDefinition[] tokenDefinitions = new TokenDefinition[]
        {
            new TokenDefinition(TokenType.Equals, "^="),
            new TokenDefinition(TokenType.Comma, "^,"),
            new TokenDefinition(TokenType.Boolean, "^true|false"),
            new TokenDefinition(TokenType.OpenParenthesis, "^\\("),
            new TokenDefinition(TokenType.CloseParenthesis, "^\\)"),
            new TokenDefinition(TokenType.OpenSquareBracket, "^\\["),
            new TokenDefinition(TokenType.CloseSquareBracket, "^\\]"),
            new TokenDefinition(TokenType.OpenCurlyBracket, "^\\{"),
            new TokenDefinition(TokenType.CloseCurlyBracket, "^\\}"),
            new TokenDefinition(TokenType.Comment, "^\\/\\*(.|\\n)*?\\*\\/"),
            new TokenDefinition(TokenType.String, "^(\"|').*?(\"|')"),
            new TokenDefinition(TokenType.Number, "^-?\\d+(\\.\\d+)?"),
            new TokenDefinition(TokenType.Identifier, "^\\w+"),
            new TokenDefinition(TokenType.Semicolon, "^;"),
        };

        public static List<Token> Tokenize(string lqlText)
        {
            var tokens = new List<Token>();
            string remainingText = lqlText;

            foreach (TokenMatch match in FindMatches(remainingText))
            {
                tokens.Add(new Token(match.TokenType, match.Value));
            }

            return tokens;
        }

        public static IEnumerable<Token> TokenizeStream(StreamReader sr)
        {
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                foreach (TokenMatch match in FindMatches(line))
                {
                    yield return new Token(match.TokenType, match.Value);
                }
            }
        }

        private static List<TokenMatch> FindMatches(string text)
        {
            var matches = new List<TokenMatch>();

            while (!string.IsNullOrWhiteSpace(text))
            {
                var match = FindMatch(text);
                if (match.IsMatch)
                {
                    matches.Add(match);
                    text = match.RemainingText;
                }
                else
                {
                    text = text.Substring(1);
                }
            }

            return matches;
        }

        private static TokenMatch FindMatch(string text)
        {
            foreach (var tokenDefinition in tokenDefinitions)
            {
                var match = tokenDefinition.Match(text);
                if (match.IsMatch)
                    return match;
            }

            return new TokenMatch() { IsMatch = false };
        }
    }
}