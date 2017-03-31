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

using System;
using System.Collections.Generic;
using System.Linq;
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
            var json = @"{ ""IntArray"" : [] }";

            JsonObject jsonObject = JsonParser.Parse(json)["IntArray"];
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
    ""String1"":""test1"",
    ""String2"":""test2"",
    ""SubObject1"": {
        ""String1"":""test11"",
        ""String2"":""test12"",
    },
    ""SubObject2"": {
        ""String1"":""test21"",
        ""String2"":""test22"",
        ""SubObjectIntArray"": {
            ""Value1"" : [1,2,3,4],
            ""Value2"" : [11,22,33,44]
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

            Assert.That((string)obj["String1"], Is.EqualTo("test1"));
            Assert.That((string)obj["String2"], Is.EqualTo("test2"));

            Assert.That((string)obj["SubObject1"]["String1"], Is.EqualTo("test11"));
            Assert.That((string)obj["SubObject1.String1"], Is.EqualTo("test11"));

            Assert.That((string)obj["SubObject1"]["String2"], Is.EqualTo("test12"));
            Assert.That((string)obj["SubObject1.String2"], Is.EqualTo("test12"));

            Assert.That((string)obj["SubObject2"]["String1"], Is.EqualTo("test21"));
            Assert.That((string)obj["SubObject2.String1"], Is.EqualTo("test21"));

            Assert.That((string)obj["SubObject2"]["String2"], Is.EqualTo("test22"));
            Assert.That((string)obj["SubObject2.String2"], Is.EqualTo("test22"));

            Assert.That(obj["SubObject2"]["SubObjectIntArray"]["Value1"].Select(item => (int)item), Is.EquivalentTo(new[] { 1, 2, 3, 4 }));
            Assert.That(obj["SubObject2"]["SubObjectIntArray"]["Value2"].Select(item => (int)item), Is.EquivalentTo(new[] { 11, 22, 33, 44 }));

            Assert.That(obj["SubObject2"]["SubObjectIntArray.Value1"].Select(item => (int)item), Is.EquivalentTo(new[] { 1, 2, 3, 4 }));
            Assert.That(obj["SubObject2.SubObjectIntArray.Value1"].Select(item => (int)item), Is.EquivalentTo(new[] { 1, 2, 3, 4 }));
            Assert.That(obj["SubObject2.SubObjectIntArray"]["Value1"].Select(item => (int)item), Is.EquivalentTo(new[] { 1, 2, 3, 4 }));


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
                yield return new TestCaseData("{\"value\":-1}", typeof(int), -1);
                yield return new TestCaseData("{\"value\":-10000000000}", typeof(long), -10000000000);
                yield return new TestCaseData("{\"value\":-1.0}", typeof(double), -1.0);
                yield return new TestCaseData("{\"value\":null}", null, null);
            }
        }

    }
}
