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
}