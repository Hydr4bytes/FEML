using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FEML
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Lexer lexer = new Lexer();
            var tokens = lexer.Tokenize(@"
                /* FEML! */
                number = 69;
                message = ['I', 'love', 'FEML'];

                graphics = {
                    msaaLevel = 4;
                    shadowQuality = 2;
                    enableBloom = true;
                    presets = [1, 2, 3, 4];
                    advanced = {
                        fsr = 1.6;
                    };
                };
                audio = {
                    sfxLevel = 1;
                    musicLevel = 2;
                };
            ");

            foreach (var token in tokens)
            {
                string tokenString = String.Format("{0,-40}{1,-50}", token.Value, token.TokenType);
                if (string.IsNullOrWhiteSpace(token.Value))
                    tokenString = String.Format("{0,-40}{1,-50}", "END OF TOKENS", token.TokenType);

                for (int i = 0; i < tokenString.Length; i++)
                {
                    if (i < token.Value.Length || (string.IsNullOrEmpty(token.Value) && i < 14))
                        Console.ForegroundColor = ConsoleColor.Magenta;
                    else
                        Console.ForegroundColor = ConsoleColor.Yellow;

                    Console.Write(tokenString[i]);
                }
                Console.WriteLine();
            }
            Console.WriteLine($"Total amount of tokens: {tokens.Count}");

            Parser parser = new Parser();
            Dictionary<string, object> result = parser.Parse(tokens);

            Console.WriteLine("Result:");
            foreach (var obj in result)
            {
                Console.Write(obj.Key + ": ");

                var list = obj.Value as List<object>;
                if (list != null)
                    Console.WriteLine(string.Join(",", list));
                else
                    Console.WriteLine(obj.Value);
            }

            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions() { WriteIndented = true }));
        }
    }
}