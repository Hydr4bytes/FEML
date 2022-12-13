using System;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Text.RegularExpressions;
using FEML;

namespace FEMLTesting
{
    internal class Program
    {
        private static readonly string testingData =
        @"
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
        ";

        private static readonly string testingClassData =
        @"
            number = 69;
            message = ['I', 'love', 'FEML'];

            audio = {
                sfxLevel = 1;
                musicLevel = 2;
            };
        ";

        public class TestingClass
        {
            public class Audio
            {
                public float sfxLevel = 0f;
                public float musicLevel = 0f;
            }

            public float number = 0f;
            public List<object> message = new List<object>();
            public Audio audio = new Audio();

            public override string ToString()
            {
                return $"number: {number}, message: {string.Join(",", message)}\nfxLevel: {audio.sfxLevel}, musicLevel: {audio.musicLevel}";
            }
        }

        private static void Main(string[] args)
        {
            Dictionary<string, object> result = FEMLConvert.Deserialize(testingData);

            Console.WriteLine("JSON result:");
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions() { WriteIndented = true }));
            Console.WriteLine();

            Console.WriteLine("Serialized result:");
            Console.WriteLine(FEMLConvert.Serialize(result));
            Console.WriteLine();

            Console.WriteLine("Deserialized class:");
            Console.WriteLine(FEMLConvert.Deserialize<TestingClass>(testingClassData).ToString());
            Console.WriteLine();

            Console.WriteLine("Serialized class:");
            TestingClass testingClass = new TestingClass();
            testingClass.message = new List<object>() { "test", "hi" };
            testingClass.number = 420;
            Console.WriteLine(FEMLConvert.Serialize<TestingClass>(testingClass));
        }
    }
}