using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace FEML
{
    public class FEMLConvert
    {
        public static string Serialize(Dictionary<string, object> input)
        {
            return Serialization.Serialize(input);
        }

        public static string Serialize<T>(object obj)
        {
            return Serialization.Serialize(RecursiveClassSerialize<T>(obj));
        }

        private static Dictionary<string, object> RecursiveClassSerialize<T>(object obj, Dictionary<string, object>? input = null)
        {
            return RecursiveClassSerialize(typeof(T), obj);
        }

        public static Dictionary<string, object> RecursiveClassSerialize(Type type, object obj, Dictionary<string, object>? input = null)
        {
            if (input == null)
                input = new Dictionary<string, object>();

            foreach (FieldInfo field in type.GetFields())
            {
                object? value = field.GetValue(obj);

                if (value != null)
                {
                    if (type.GetNestedTypes().Contains(field.FieldType))
                    {
                        Dictionary<string, object> sub = RecursiveClassSerialize(value.GetType(), value);
                        input.Add(field.Name, sub);
                    }
                    else
                    {
                        input.Add(field.Name, value);
                    }
                }
            }

            return input;
        }

        public static Dictionary<string, object> Deserialize(string input)
        {
            var tokens = Lexer.Tokenize(input);

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
            Console.WriteLine();

            return Parser.Parse(tokens);
        }

        public static T Deserialize<T>(string input)
        {
            var tokens = Lexer.Tokenize(input);
            var result = Parser.Parse(tokens);

            return RecursiveClassDeserialize<T>(result);
        }

        private static T RecursiveClassDeserialize<T>(Dictionary<string, object> input)
        {
            return (T)RecursiveClassDeserialize(typeof(T), input);
        }

        private static object RecursiveClassDeserialize(Type type, Dictionary<string, object> input)
        {
            object obj = RuntimeHelpers.GetUninitializedObject(type);

            foreach (var dataField in input)
            {
                if (dataField.Value.GetType() == typeof(Dictionary<string, object>))
                {
                    FieldInfo? sub = obj.GetType().GetField(dataField.Key);

                    if (sub != null)
                    {
                        object subClassDeserialize = RecursiveClassDeserialize(sub.FieldType, (Dictionary<string, object>)dataField.Value);
                        sub.SetValue(obj, subClassDeserialize);
                    }
                }
                else
                {
                    FieldInfo? info = type.GetField(dataField.Key);
                    if (info != null)
                    {
                        info.SetValue(obj, dataField.Value);
                    }
                }
            }

            return obj;
        }
    }
}