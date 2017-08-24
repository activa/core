using System.Collections.Generic;
using NUnit.Framework;

namespace Iridium.Core.Test
{
    [TestFixture]
    public class Json2Test
    {
        [Test]
        public void Test1()
        {
            Json.Value j = new Json.Object()
            {
                null,
                new Json.Field("x", null),
                { "y_int", 1 },
                { "y_short", 1 },
                { "y_bool", false },
                { "z", new { a = 1 } },
                { "q", new[] {1,2,3} },
                null,
                {
                    "array" , new Json.Array()
                    {
                        new Json.Object()
                    }
                }
            };

            Json.Array j2 = new Json.Array()
            {
                "x"
            };

            Assert.That(j["x"].IsNull);
            Assert.That(!j["x"].IsEmpty);

            Assert.That(j2.IsArray);
            Assert.That((string)j2[0].As<string>(), Is.EqualTo("x"));
        }
    }
}