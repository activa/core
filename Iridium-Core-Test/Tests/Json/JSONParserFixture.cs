//=============================================================================
// Iridium-Core - Portable .NET Productivity Library 
//
// Copyright (c) 2008-2016 Philippe Leybaert
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//=============================================================================

using System.Linq;
using NUnit.Framework;

namespace Iridium.Core.Test
{
    [TestFixture]
    public class JSONParserFixture
    {
        private const string _json1 = @"
{ 
    ""name"" :""John Doe"" ,
    ""salary"" : 4500.50
}
";

        private const string _json = @"
{ 
    ""name"" :""John Doe"" ,
    ""salary"" : 4500.20 ,
    ""children"" : [ ""Sarah"", ""Jessica"" ]
}
";

        private const string _json2 = @"

{
    ""glossary"": {
        ""title"": ""example glossary"",
		""GlossDiv"": {
            ""title"": ""S"",
			""GlossList"": {
                ""GlossEntry"": {
                    ""ID"": ""SGML"",
					""SortAs"": ""SGML"",
					""GlossTerm"": ""Standard Generalized Markup Language"",
					""Acronym"": ""SGML"",
					""Abbrev"": ""ISO 8879:1986"",
					""GlossDef"": {
                        ""para"": ""A meta-markup language, used to create markup languages such as DocBook."",
						""GlossSeeAlso"": [""GML"", ""XML""]
                    },
					""GlossSee"": ""markup""
                }
            }
        }
    }
}
";

        private const string _json3 = @"
{""menu"": {
    ""header"": ""SVG Viewer"",
    ""items"": [
        {""id"": ""Open""},
        {""id"": ""OpenNew"", ""label"": ""Open New""},
        null,
        {""id"": ""ZoomIn"", ""label"": ""Zoom In""},
        {""id"": ""ZoomOut"", ""label"": ""Zoom Out""},
        {""id"": ""OriginalView"", ""label"": ""Original View""},
        null,
        {""id"": ""Quality""},
        {""id"": ""Pause""},
        {""id"": ""Mute""},
        null,
        {""id"": ""Find"", ""label"": ""Find...""},
        {""id"": ""FindAgain"", ""label"": ""Find Again""},
        {""id"": ""Copy""},
        {""id"": ""CopyAgain"", ""label"": ""Copy Again""},
        {""id"": ""CopySVG"", ""label"": ""Copy SVG""},
        {""id"": ""ViewSVG"", ""label"": ""View SVG""},
        {""id"": ""ViewSource"", ""label"": ""View Source""},
        {""id"": ""SaveAs"", ""label"": ""Save As""},
        null,
        {""id"": ""Help""},
        {""id"": ""About"", ""label"": ""About Adobe CVG Viewer...""}
    ]
}}
";

        private class Person
        {
            public string name;
            public decimal  salary;
            public string[] children;
        }

        [Test]
        public void SimpleTypedObject()
        {
            Person person = JsonParser.Parse<Person>(_json);

            Assert.AreEqual("John Doe",person.name);
            Assert.AreEqual(4500.20m,person.salary);
            Assert.AreEqual(2,person.children.Length);
            Assert.AreEqual("Sarah", person.children[0]);
            Assert.AreEqual("Jessica", person.children[1]);
        }

        [Test]
        public void ComplexDictionary()
        {
            JsonObject jsonObject = JsonParser.Parse(_json3);

            Assert.IsTrue(jsonObject.IsObject);
            Assert.IsTrue(jsonObject["menu"].IsObject);

            Assert.AreEqual(2, jsonObject["menu"].Keys.Length);

            Assert.AreEqual("SVG Viewer", jsonObject["menu.header"].As<string>());
            Assert.AreEqual("SVG Viewer", jsonObject["menu"]["header"].As<string>());

            Assert.AreEqual(22, jsonObject["menu"]["items"].AsArray().Length);
            Assert.AreEqual(22, jsonObject["menu.items"].AsArray().Length);

            Assert.AreEqual(22,jsonObject["menu.items"].Count());

            Assert.AreEqual("OpenNew", jsonObject["menu"]["items"][1]["id"].As<string>());
            Assert.IsTrue(jsonObject["menu"]["items"][2].IsNull);
        }

        [Test]
        public void EmptyArray()
        {
            JsonObject jsonObject = JsonParser.Parse(@"{ ""array"" : [] }")["array"];

            Assert.IsTrue(jsonObject.IsArray);
            Assert.AreEqual(0,jsonObject.AsArray<long>().Length);
        }

        [Test]
        public void Escapes()
        {
            JsonObject obj = JsonParser.Parse(@"{ ""s1"" : ""\n"", ""s2"" : ""\t"", ""s3"" : ""\\"" , ""s4"" : ""\u00aa"" }");

            Assert.That((string)obj["s1"], Is.EqualTo("\n"));
            Assert.That((string)obj["s2"], Is.EqualTo("\t"));
            Assert.That((string)obj["s3"], Is.EqualTo("\\"));
            Assert.That((string)obj["s4"], Is.EqualTo("\u00aa"));
        }

        [Test]
        public void SimpleValues()
        {
            Assert.That(JsonParser.Parse("\"x\"").As<string>(), Is.EqualTo("x"));
            Assert.That(JsonParser.Parse("1").As<int>(), Is.EqualTo(1));
        }

        [Test]
        public void EmptyObject()
        {
            JsonObject jsonObject = JsonParser.Parse("{}");

            Assert.IsTrue(jsonObject.IsObject && jsonObject.Keys.Length == 0);
        }

        [Test]
        public void IntValues()
        {
            string json = "{\"value1\":1, \"value2\":null, \"value3\":-1, \"value4\":123, \"value5\":-123 }";

            JsonObject jsonObject = JsonParser.Parse(json);

            Assert.That(jsonObject["value1"], Is.InstanceOf<JsonObject>());
            Assert.That(jsonObject["value1"].Value, Is.EqualTo(1));
            Assert.That((int)jsonObject["value1"], Is.EqualTo(1));

            Assert.That(jsonObject["value2"].Value, Is.Null);
            Assert.That((int)jsonObject["value2"], Is.EqualTo(0));
            Assert.That((int?)jsonObject["value2"], Is.Null);

            Assert.That(jsonObject["value3"], Is.InstanceOf<JsonObject>());
            Assert.That(jsonObject["value3"].Value, Is.EqualTo(-1));
            Assert.That((int)jsonObject["value3"], Is.EqualTo(-1));

            Assert.That((int)jsonObject["value4"], Is.EqualTo(123));
            Assert.That((int)jsonObject["value5"], Is.EqualTo(-123));
        }

        [Test]
        public void FloatValues()
        {
            string json = "{\"value1\":1.1, \"value2\":null, \"value3\":-1.1, \"value4\":123.0, \"value5\":-123.0 }";

            JsonObject jsonObject = JsonParser.Parse(json);

            Assert.That(jsonObject["value1"], Is.InstanceOf<JsonObject>());
            Assert.That(jsonObject["value1"].Value, Is.EqualTo(1.1).Within(0.000001));
            Assert.That((double)jsonObject["value1"], Is.EqualTo(1.1).Within(0.000001));

            Assert.That(jsonObject["value2"].Value, Is.Null);
            Assert.That((double)jsonObject["value2"], Is.EqualTo(0.0));
            Assert.That((double?)jsonObject["value2"], Is.Null);

            Assert.That(jsonObject["value3"], Is.InstanceOf<JsonObject>());
            Assert.That(jsonObject["value3"].Value, Is.EqualTo(-1.1).Within(0.000001));
            Assert.That((double)jsonObject["value3"], Is.EqualTo(-1.1).Within(0.000001));

            Assert.That((double)jsonObject["value4"], Is.EqualTo(123.0).Within(0.000001));
            Assert.That((double)jsonObject["value5"], Is.EqualTo(-123.0).Within(0.000001));
        }

    }
}
