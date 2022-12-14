using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FEML
{
    internal static class Parser
    {
        private static int tokenIdx = 0;
        private static List<Token> tokens;
        private static Dictionary<string, object> result;

        private static TokenType NextTokenType => tokens[tokenIdx + 1].TokenType;

        public static Dictionary<string, object> Parse(List<Token> inputTokens)
        {
            tokenIdx = 0;
            tokens = inputTokens;
            result = new Dictionary<string, object>();

            while (tokenIdx < tokens.Count)
            {
                Token currentToken = tokens[tokenIdx];
                if (currentToken.TokenType == TokenType.Comment)
                {
                    tokenIdx++;
                    if (tokenIdx < tokens.Count)
                        currentToken = tokens[tokenIdx];
                    else
                        break;
                }
                else if (currentToken.TokenType != TokenType.Identifier)
                {
                    throw new Exception($"Expected identifier, got {currentToken.TokenType}");
                }

                var variable = ConsumeVariable(currentToken);
                result.Add(variable.Item1, variable.Item2);
                tokenIdx++;
            }

            return result;
        }

        private static object ParseTokenValue(Token token)
        {
            object value = null;
            switch (token.TokenType)
            {
                case TokenType.String:
                    value = token.Value.Replace("'", string.Empty).Replace("\"", string.Empty);
                    break;

                case TokenType.Number:
                    value = float.Parse(token.Value, CultureInfo.InvariantCulture.NumberFormat);
                    break;

                case TokenType.Boolean:
                    value = bool.Parse(token.Value);
                    break;
            }

            return value;
        }

        private static Tuple<string, object> ConsumeVariable(Token startToken)
        {
            string name = string.Empty;
            object value = null;

            name = startToken.Value;
            ConsumeToken(TokenType.Equals);

            switch (NextTokenType)
            {
                case TokenType.OpenSquareBracket:
                    value = ConsumeArray();
                    break;

                case TokenType.OpenCurlyBracket:
                    value = ConsumeStruct();
                    break;

                default:
                    value = ParseTokenValue(ConsumeToken(new TokenType[] { TokenType.String, TokenType.Number, TokenType.Boolean }));
                    break;
            }

            ConsumeToken(TokenType.Semicolon);

            return Tuple.Create(name, value);
        }

        private static List<object> ConsumeArray()
        {
            ConsumeToken(TokenType.OpenSquareBracket);

            List<object> array = new List<object>();
            while (NextTokenType != TokenType.CloseSquareBracket)
            {
                var token = ConsumeToken(new TokenType[] { TokenType.String, TokenType.Number, TokenType.Boolean });
                array.Add(ParseTokenValue(token));

                if (NextTokenType == TokenType.Comma)
                    ConsumeToken(TokenType.Comma);
            }

            ConsumeToken(TokenType.CloseSquareBracket);

            return array;
        }

        private static Dictionary<string, object> ConsumeStruct()
        {
            ConsumeToken(TokenType.OpenCurlyBracket);
            Dictionary<string, object> dict = new Dictionary<string, object>();
            while (NextTokenType != TokenType.CloseCurlyBracket)
            {
                var token = ConsumeToken(TokenType.Identifier);
                var variable = ConsumeVariable(token);
                dict.Add(variable.Item1, variable.Item2);
            }

            ConsumeToken(TokenType.CloseCurlyBracket);

            return dict;
        }

        private static Token ConsumeToken(params TokenType[] types)
        {
            tokenIdx++;
            if (tokenIdx >= tokens.Count)
            {
                throw new Exception("Unexpected end of token stream");
            }

            ExpectTokenType(tokens[tokenIdx], types);

            return tokens[tokenIdx];
        }

        private static void ExpectTokenType(Token token, params TokenType[] types)
        {
            bool matched = false;
            foreach (TokenType type in types)
            {
                if (tokens[tokenIdx].TokenType == type)
                {
                    matched = true;
                    break;
                }
            }

            if (!matched)
                throw new Exception($"Expected {string.Join(",", types)}, got {tokens[tokenIdx].TokenType}");
        }
    }
}