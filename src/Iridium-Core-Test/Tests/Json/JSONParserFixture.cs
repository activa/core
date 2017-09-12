//=============================================================================
// Iridium-Core - Portable .NET Productivity Library 
//
// Copyright (c) 2008-2017 Philippe Leybaert
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Iridium.Core.Test
{
    [TestFixture]
    public class JSONParserFixture
    {
        public class _BasicTypes
        {
            public int[] IntArray;
        }

        [Test]
        public void EmptyArray()
        {
            var json = @"{ ""intArray"" : [] }";

            JsonObject jsonObject = JsonParser.Parse(json)["intArray"];
            var typedObject = JsonParser.Parse<_BasicTypes>(json);

            Assert.IsTrue(jsonObject.IsArray);
            Assert.AreEqual(0, jsonObject.AsArray<long>().Length);

            Assert.That(typedObject.IntArray, Is.Not.Null);
            Assert.That(typedObject.IntArray.Length, Is.Zero);
        }

        [Test]
        public void Escapes()
        {
            JsonObject obj = JsonParser.Parse(@"{ ""s1"" : ""\n"", ""s2"" : ""\t"", ""s3"" : ""\\"" , ""s4"" : ""\u00aa"" }");

            Assert.That((string) obj["s1"], Is.EqualTo("\n"));
            Assert.That((string) obj["s2"], Is.EqualTo("\t"));
            Assert.That((string) obj["s3"], Is.EqualTo("\\"));
            Assert.That((string) obj["s4"], Is.EqualTo("\u00aa"));
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
        public void NullOrEmpty()
        {
            JsonObject json = JsonParser.Parse("{\"x\":null,\"y\":1}");

            Assert.That(json["x"].IsNull, Is.True);
            Assert.That(json["x"].IsNullOrEmpty, Is.True);
            Assert.That(json["x"].IsEmpty, Is.False);

            Assert.That(json["y"].IsNull, Is.False);
            Assert.That(json["y"].IsNullOrEmpty, Is.False);
            Assert.That(json["y"].IsEmpty, Is.False);

            Assert.That(json["z"].IsNull, Is.False);
            Assert.That(json["z"].IsNullOrEmpty, Is.True);
            Assert.That(json["z"].IsEmpty, Is.True);

        }


        [Test]
        public void StreamInput()
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes("{}")))
            {
                var jsonObject = JsonParser.Parse(stream);

                Assert.IsTrue(jsonObject.IsObject && jsonObject.Keys.Length == 0);
            }

            
        }

        [Test]
        public void ImplicitCasts()
        {
            Func<string, JsonObject> j = JsonParser.Parse;

            JsonObject json = JsonParser.Parse("{\"i\":123,\"s\":\"abc\",\"f\":123.0,\"b\":true}");

            int i1;
            int? i2;
            long l1;
            long? l2;
            bool b1;
            bool? b2;
            string s;

            i1 = json["i"]; Assert.That(i1, Is.EqualTo(123));
            i1 = json["s"]; Assert.That(i1, Is.EqualTo(0));
            i1 = json["f"]; Assert.That(i1, Is.EqualTo(123));
            i1 = json["b"]; Assert.That(i1, Is.EqualTo(1));

            i2 = json["i"]; Assert.That(i2, Is.EqualTo(123));
            i2 = json["s"]; Assert.That(i2, Is.Null);
            i2 = json["f"]; Assert.That(i2, Is.EqualTo(123));
            i2 = json["b"]; Assert.That(i2, Is.EqualTo(1));

            s = json["i"]; Assert.That(s, Is.EqualTo("123"));
            s = json["s"]; Assert.That(s, Is.EqualTo("abc"));
            s = json["f"]; Assert.That(s, Is.EqualTo("123"));
            s = json["b"]; Assert.That(s, Is.EqualTo("True"));

            l1 = json["i"]; Assert.That(l1, Is.EqualTo(123L));
            l1 = json["s"]; Assert.That(l1, Is.EqualTo(0L));
            l1 = json["f"]; Assert.That(l1, Is.EqualTo(123L));
            l1 = json["b"]; Assert.That(l1, Is.EqualTo(1L));

            l2 = json["i"]; Assert.That(l2, Is.EqualTo(123L));
            l2 = json["s"]; Assert.That(l2, Is.Null);
            l2 = json["f"]; Assert.That(l2, Is.EqualTo(123L));
            l2 = json["b"]; Assert.That(l2, Is.EqualTo(1L));

            b1 = json["i"]; Assert.That(b1, Is.True);
            b1 = json["s"]; Assert.That(b1, Is.False);
            b1 = json["f"]; Assert.That(b1, Is.True);
            b1 = json["b"]; Assert.That(b1, Is.True);

            b2 = json["i"]; Assert.That(b2, Is.True);
            b2 = json["s"]; Assert.That(b2, Is.False);
            b2 = json["f"]; Assert.That(b2, Is.True);
            b2 = json["b"]; Assert.That(b2, Is.True);

        }

        public class DeepClass
        {
            public class _GenericSubClass<T>
            {
                public T Value1;
                public T Value2 { get; set; }
            }

            public class _SubClass1
            {
                public string String1 { get; set; }
                public string String2;
                public int Int1 { get; set; }
                public int Int2;

                public _SubClass1 SubObject1;
                public _SubClass1 SubObject2 { get; set; }

                public _GenericSubClass<int> SubObjectInt;
                public _GenericSubClass<string> SubObjectString;
                public _GenericSubClass<bool> SubObjectBool;
                public _GenericSubClass<int[]> SubObjectIntArray;
                public _GenericSubClass<string[]> SubObjectStringArray;
            }

            public _SubClass1 SubObject1;
            public _SubClass1 SubObject2 { get; set; }

            public string String1;
            public string String2 { get; set; }
        }

        string _deepClassJson = @"
{
    ""string1"":""test1"",
    ""string2"":""test2"",
    ""subObject1"": {
        ""string1"":""test11"",
        ""string2"":""test12"",
    },
    ""subObject2"": {
        ""string1"":""test21"",
        ""string2"":""test22"",
        ""subObjectIntArray"": {
            ""value1"" : [1,2,3,4],
            ""value2"" : [11,22,33,44]
        }
    }
}
";

        [Test]
        public void TestDeepClassMapped()
        {
            DeepClass obj = JsonParser.Parse<DeepClass>(_deepClassJson);

            Assert.That(obj.String1, Is.EqualTo("test1"));
            Assert.That(obj.String2, Is.EqualTo("test2"));
            Assert.That(obj.SubObject1.String1, Is.EqualTo("test11"));
            Assert.That(obj.SubObject1.String2, Is.EqualTo("test12"));
            Assert.That(obj.SubObject2.String1, Is.EqualTo("test21"));
            Assert.That(obj.SubObject2.String2, Is.EqualTo("test22"));
            Assert.That(obj.SubObject2.SubObjectIntArray.Value1, Is.EquivalentTo(new[] {1, 2, 3, 4}));
            Assert.That(obj.SubObject2.SubObjectIntArray.Value2, Is.EquivalentTo(new[] {11, 22, 33, 44}));
        }

        [Test]
        public void TestDeepClassGeneric()
        {
            var obj = JsonParser.Parse(_deepClassJson);

            Assert.That((string)obj["string1"], Is.EqualTo("test1"));
            Assert.That((string)obj["string2"], Is.EqualTo("test2"));

            Assert.That((string)obj["subObject1"]["string1"], Is.EqualTo("test11"));
            Assert.That((string)obj["subObject1.string1"], Is.EqualTo("test11"));

            Assert.That((string)obj["subObject1"]["string2"], Is.EqualTo("test12"));
            Assert.That((string)obj["subObject1.string2"], Is.EqualTo("test12"));

            Assert.That((string)obj["subObject2"]["string1"], Is.EqualTo("test21"));
            Assert.That((string)obj["subObject2.string1"], Is.EqualTo("test21"));

            Assert.That((string)obj["subObject2"]["string2"], Is.EqualTo("test22"));
            Assert.That((string)obj["subObject2.string2"], Is.EqualTo("test22"));

            Assert.That(obj["XXXXXX"].IsEmpty, Is.True);
            Assert.That(obj["XXXXXX.string2"].IsEmpty, Is.True);

            Assert.That(obj["subObject2"]["subObjectIntArray"]["value1"].Select(item => (int)item), Is.EquivalentTo(new[] { 1, 2, 3, 4 }));
            Assert.That(obj["subObject2"]["subObjectIntArray"]["value2"].Select(item => (int)item), Is.EquivalentTo(new[] { 11, 22, 33, 44 }));

            Assert.That(obj["subObject2"]["subObjectIntArray.value1"].Select(item => (int)item), Is.EquivalentTo(new[] { 1, 2, 3, 4 }));
            Assert.That(obj["subObject2.subObjectIntArray.value1"].Select(item => (int)item), Is.EquivalentTo(new[] { 1, 2, 3, 4 }));
            Assert.That(obj["subObject2.subObjectIntArray"]["value1"].Select(item => (int)item), Is.EquivalentTo(new[] { 1, 2, 3, 4 }));


        }

        [Test]
        public void TrailingComma()
        {
            string jsonText = @"{""x"":1,}";

            var json = JsonParser.Parse(jsonText);

            Assert.That(json["x"].As<int>(), Is.EqualTo(1));
        }

        [TestCaseSource(nameof(DataTypesSource))]
        public void TestDataTypes(string json, Type type, object expectedResult)
        {
            var obj = JsonParser.Parse(json);

            if (type != null)
                Assert.That(obj["value"].Value, Is.TypeOf(type));

            Assert.That(obj["value"].Value, Is.EqualTo(expectedResult));
        }

        public static IEnumerable<TestCaseData> DataTypesSource
        {
            get
            {
                yield return new TestCaseData("{\"value\":1}", typeof(int), 1);
                yield return new TestCaseData("{\"value\":10000000000}", typeof(long), 10000000000);
                yield return new TestCaseData("{\"value\":1.0}", typeof(double), 1.0);
                yield return new TestCaseData("{\"value\":1.2E5}", typeof(double), 1.2E5);
                yield return new TestCaseData("{\"value\":1.2e5}", typeof(double), 1.2E5);
                yield return new TestCaseData("{\"value\":1.2E+5}", typeof(double), 1.2E+5);
                yield return new TestCaseData("{\"value\":1.2e+5}", typeof(double), 1.2E+5);
                yield return new TestCaseData("{\"value\":1.2E-5}", typeof(double), 1.2E-5);
                yield return new TestCaseData("{\"value\":1.2e-5}", typeof(double), 1.2E-5);
                yield return new TestCaseData("{\"value\":-1.2E5}", typeof(double), -1.2E5);
                yield return new TestCaseData("{\"value\":-1.2e5}", typeof(double), -1.2E5);
                yield return new TestCaseData("{\"value\":-1.2E+5}", typeof(double), -1.2E+5);
                yield return new TestCaseData("{\"value\":-1.2e+5}", typeof(double), -1.2E+5);
                yield return new TestCaseData("{\"value\":-1.2E-5}", typeof(double), -1.2E-5);
                yield return new TestCaseData("{\"value\":-1.2e-5}", typeof(double), -1.2E-5);
                yield return new TestCaseData("{\"value\":1.2E15}", typeof(double), 1.2E15);
                yield return new TestCaseData("{\"value\":1.2e15}", typeof(double), 1.2E15);
                yield return new TestCaseData("{\"value\":1.2E+15}", typeof(double), 1.2E+15);
                yield return new TestCaseData("{\"value\":1.2e+15}", typeof(double), 1.2E+15);
                yield return new TestCaseData("{\"value\":1.2E-15}", typeof(double), 1.2E-15);
                yield return new TestCaseData("{\"value\":1.2e-15}", typeof(double), 1.2E-15);
                yield return new TestCaseData("{\"value\":-1.2E15}", typeof(double), -1.2E15);
                yield return new TestCaseData("{\"value\":-1.2e15}", typeof(double), -1.2E15);
                yield return new TestCaseData("{\"value\":-1.2E+15}", typeof(double), -1.2E+15);
                yield return new TestCaseData("{\"value\":-1.2e+15}", typeof(double), -1.2E+15);
                yield return new TestCaseData("{\"value\":-1.2E-15}", typeof(double), -1.2E-15);
                yield return new TestCaseData("{\"value\":-1.2e-15}", typeof(double), -1.2E-15);
                yield return new TestCaseData("{\"value\":-1}", typeof(int), -1);
                yield return new TestCaseData("{\"value\":-10000000000}", typeof(long), -10000000000);
                yield return new TestCaseData("{\"value\":-1.0}", typeof(double), -1.0);
                yield return new TestCaseData("{\"value\":null}", null, null);
                yield return new TestCaseData("{\"value\":true}", typeof(bool), true);
                yield return new TestCaseData("{\"value\":false}", typeof(bool), false);
            }
        }

    }
}
