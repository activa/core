using System.Linq;
using NUnit.Framework;

namespace Iridium.Core.Test
{
    [TestFixture]
    public class JsonBuilderFixture
    {
        [Test]
        public void SimpleObject()
        {
            JsonObject json = new JsonObject()
            {
                {"value1", "string1"},
                {"value2", "string2"},
                {"array1" , new[] {1,2,3}},
                {
                    "object1", 
                    new JsonObject()
                    {
                        {"value3", 123}
                    }
                }
            };

            Assert.That(json.IsObject);
            Assert.That((string)json["value1"], Is.EqualTo("string1"));
            Assert.That((string)json["value2"], Is.EqualTo("string2"));

            Assert.That(json["array1"].IsArray);
            Assert.That(json["array1"].AsArray<int>(), Is.EquivalentTo(new[] {1,2,3}));
            Assert.That(json["object1"].IsObject);
            Assert.That((int)json["object1"]["value3"], Is.EqualTo(123));
        }

    }

    [TestFixture]
    public class JsonParserPerformanceTestFixture
    {
        private string _json;

        [OneTimeSetUp]
        public void CreateTestJson()
        {
            var childObj = new
            {
                string1 = "String 1",
                string2 = "A very long String 2, A very long String 2, A very long String 2, A very long String 2, A very long String 2, A very long String 2",
                string3 = "A",
                int1 = 123,
                double1 = 567.0,
                bool1 = true,
                bool2 = false,
                array1 = Enumerable.Range(1,100).ToArray(),
                array2 = Enumerable.Range(1,100).Select(i => "string" + i).ToArray(),
            };

            var o = new
            {
                string1 = "String 1",
                string2 = "A very long String 2, A very long String 2, A very long String 2, A very long String 2, A very long String 2, A very long String 2",
                string3 = "A",
                int1 = 123,
                double1 = 567.0,
                bool1 = true,
                bool2 = false,
                array1 = Enumerable.Range(1, 100).ToArray(),
                array2 = Enumerable.Range(1, 100).Select(i => "string" + i).ToArray(),
                array3 = Enumerable.Range(1, 20).Select(i => childObj).ToArray(),
                object1 = new
                {
                    string1 = "String 1",
                    string2 = "A very long String 2, A very long String 2, A very long String 2, A very long String 2, A very long String 2, A very long String 2",
                    string3 = "A",
                    object1 = new
                    {
                        string1 = "String 1",
                        string2 = "A very long String 2, A very long String 2, A very long String 2, A very long String 2, A very long String 2, A very long String 2",
                        string3 = "A",
                        object1 = new
                        {
                            string1 = "String 1",
                            string2 = "A very long String 2, A very long String 2, A very long String 2, A very long String 2, A very long String 2, A very long String 2",
                            string3 = "A",
                            object1 = new
                            {
                                string1 = "String 1",
                                string2 = "A very long String 2, A very long String 2, A very long String 2, A very long String 2, A very long String 2, A very long String 2",
                                string3 = "A",
                            }
                        }
                    }
                }
            };

            _json = JsonSerializer.ToJson(o);
        }

        [Test]
        [Repeat(1000)]
        public void PerfTest()
        {
            var jsonObject = JsonParser.Parse(_json);
        }
    }
}