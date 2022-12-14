using System;
using System.Reflection;
using FEML;

namespace FEMLUnitTesting
{
    [TestClass]
    public class UnitTest1
    {
        public class FEMLTestingClass
        {
            public float number = 69f;
            public List<string> message = new List<string>() { "I", "love", "FEML" };
            public Audio audio = new Audio();

            public class Audio
            {
                public float sfxLevel = 1f;
                public float musicLevel = 2f;
            }
        }

        [TestMethod]
        public void DataPreserveTest()
        {
            var initialClass = new FEMLTestingClass();

            string serializedData = FEMLConvert.Serialize<FEMLTestingClass>(initialClass);
            Console.WriteLine(serializedData);

            var reserializedClass = FEMLConvert.Deserialize<FEMLTestingClass>(serializedData);
            Console.WriteLine(FEMLConvert.Serialize<FEMLTestingClass>(reserializedClass));

            foreach (PropertyInfo property in initialClass.GetType().GetProperties())
            {
                Assert.AreEqual(property.GetValue(initialClass), property.GetValue(reserializedClass));
            }
        }
    }
}