using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
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
            return Parser.Parse(tokens);
        }

        public static Dictionary<string, object> DeserializeStream(StreamReader input)
        {
            var tokens = Lexer.TokenizeStream(input);
            return Parser.Parse(tokens.ToList());
        }

        public static T DeserializeStream<T>(StreamReader input)
        {
            var tokens = Lexer.TokenizeStream(input);
            var result = Parser.Parse(tokens.ToList());

            return RecursiveClassDeserialize<T>(result);
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
                        if (dataField.Value.GetType() == typeof(List<object>))
                        {
                            object? instance = Activator.CreateInstance(info.FieldType);
                            IList? list = (IList?)instance;

                            foreach (var item in (List<object>)dataField.Value)
                            {
                                list?.Add(item);
                            }

                            info.SetValue(obj, instance);
                        }
                        else
                        {
                            info.SetValue(obj, dataField.Value);
                        }
                    }
                }
            }

            return obj;
        }
    }
}