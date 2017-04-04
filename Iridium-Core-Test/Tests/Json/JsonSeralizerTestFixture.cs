using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Iridium.Core.Test
{
    [TestFixture]
    public class JsonSeralizerTestFixture
    {
        [Test]
        public void TestArray()
        {
            string json = JsonSerializer.ToJson(new[] { 1, 2, 3 });

            Assert.AreEqual("[1,2,3]", json);
        }

        [Test]
        public void TestList()
        {
            string json = JsonSerializer.ToJson(new List<int>(new[] { 1, 2, 3 }));

            Assert.AreEqual("[1,2,3]", json);
        }

        [Test]
        public void TestInt()
        {
            Assert.AreEqual("123", JsonSerializer.ToJson(123));
        }

        [Test]
        public void TestSring()
        {
            Assert.AreEqual("\"abc\"", JsonSerializer.ToJson("abc"));
        }

        [Test]
        public void TestDouble()
        {
            Assert.AreEqual("12.34", JsonSerializer.ToJson(12.34));
        }

        [Test]
        public void TestBool()
        {
            Assert.AreEqual("true", JsonSerializer.ToJson(true));
            Assert.AreEqual("false", JsonSerializer.ToJson(false));
        }

        [Test]
        public void TestDictionary()
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();

            dic["a"] = 1;
            dic["b"] = 2;

            Assert.AreEqual("{\"a\":1,\"b\":2}", JsonSerializer.ToJson(dic));
        }

        private class TestClass
        {
            public string field1 = "A";
            public int field2 = 123;
            public string field3 { get { return "B"; } }
            private string field4 = "C";
        }

        [Test]
        public void TestObject()
        {
            Assert.AreEqual("{\"field1\":\"A\",\"field2\":123,\"field3\":\"B\"}", JsonSerializer.ToJson(new TestClass()));
        }

        [Test]
        public void TestDate()
        {
            DateTime date = DateTime.Now;

            DateTime utcBase = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            long utcMs = (long)(date.ToUniversalTime() - utcBase).TotalMilliseconds;

            Assert.AreEqual("new Date(" + utcMs + ")", JsonSerializer.ToJson(date, JsonDateFormat.NewDate));
            Assert.AreEqual("\"Date(" + utcMs + ")\"", JsonSerializer.ToJson(date, JsonDateFormat.Date));
            Assert.AreEqual("\"/Date(" + utcMs + ")/\"", JsonSerializer.ToJson(date, JsonDateFormat.SlashDate));
            Assert.AreEqual("\"\\/Date(" + utcMs + ")\\/\"", JsonSerializer.ToJson(date, JsonDateFormat.EscapedSlashDate));
            Assert.AreEqual("\"" + date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") + "\"", JsonSerializer.ToJson(date, JsonDateFormat.UtcISO));


            date = new DateTime(1999, 12, 1, 10, 0, 0, DateTimeKind.Utc);

            utcMs = (long)(date.ToUniversalTime() - utcBase).TotalMilliseconds;

            Assert.AreEqual("new Date(" + utcMs + ")", JsonSerializer.ToJson(date,JsonDateFormat.NewDate));
            Assert.AreEqual("\"1999-12-01T10:00:00Z\"", JsonSerializer.ToJson(date, JsonDateFormat.UtcISO));

            date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            Assert.AreEqual("new Date(0)", JsonSerializer.ToJson(date,JsonDateFormat.NewDate));
            Assert.AreEqual("\"1970-01-01T00:00:00Z\"", JsonSerializer.ToJson(date, JsonDateFormat.UtcISO));
        }

        [Test]
        public void TestNull()
        {
            Assert.AreEqual("null", JsonSerializer.ToJson(null));
        }

        [Test]
        public void TestChar()
        {
            Assert.AreEqual("\"x\"", JsonSerializer.ToJson('x'));
        }

        private enum TestEnumeration
        {
            FirstValue,
            SecondValue
        }

        [Test]
        public void TestEnum()
        {
            Assert.AreEqual("\"FirstValue\"", JsonSerializer.ToJson(TestEnumeration.FirstValue));
            Assert.AreEqual("\"SecondValue\"", JsonSerializer.ToJson(TestEnumeration.SecondValue));
        }

        private class TestClass2
        {
            public string Field1 = "abc";
            public int[] Field2 = new[] { 1, 2, 3 };
        }

        [Test]
        public void TestArrayInObject()
        {
            Assert.AreEqual("{\"Field1\":\"abc\",\"Field2\":[1,2,3]}", JsonSerializer.ToJson(new TestClass2()));
        }

        [Test]
        public void TestSerializeJsonObject()
        {
            var jsonText = JsonSerializer.ToJson(new TestClass2());

            var json = JsonParser.Parse(jsonText);

            Assert.That(JsonSerializer.ToJson(json), Is.EqualTo(jsonText));
        }

        [Test]
        public void TestCircularReferences()
        {
            json_ParentClass obj1 = new json_ParentClass();
            json_ParentClass obj2 = new json_ParentClass();

            obj1.Id = "A";
            obj2.Id = "B";

            obj1.Children = new List<json_ChildClass>()
                                  {
                                      new json_ChildClass
                                          {
                                              Id = "A1",
                                              Parent = obj1,
                                              Others = new List<json_ParentClass>() { obj1,obj2 }
                                          }
                                  };

            string json = JsonSerializer.ToJson(obj1);

            Assert.AreEqual("{\"Id\":\"A\",\"Children\":[{\"Id\":\"A1\",\"Parent\":null,\"Others\":[null,{\"Id\":\"B\",\"Children\":null}]}]}",json);

        }

        private class json_ParentClass
        {
            public string Id;
            public List<json_ChildClass> Children;
        }

        private class json_ChildClass
        {
            public string Id;
            public json_ParentClass Parent;
            public List<json_ParentClass> Others;
        }
    }

    [TestFixture]
    public class Dummy
    {
        [Test]
        public void DummyMethod()
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

            var t = typeof(SomeStringList).Inspector();

            bool x;

            x = t.ImplementsOrInherits<IList>();
            x = t.ImplementsOrInherits(typeof(IList<>));

            x = typeof(string[]).Inspector().ImplementsOrInherits(typeof(IList<>));
            x = typeof(string[]).Inspector().ImplementsOrInherits(typeof(IList<string>));
        }

        public class SomeStringList : List<string>
        {
            
        }
    }


    public class Json
    {
        public abstract class Value : IEnumerable<Value>
        {
            public static NoValue NoValue { get; } = new NoValue();

            public abstract bool IsObject { get; }
            public abstract bool IsArray { get; }
            public abstract bool IsEmpty { get; }
            public abstract bool IsNull { get; }

            public bool IsValue => !IsObject && !IsArray && !IsEmpty;
            public bool IsNullOrEmpty => IsNull || IsEmpty;

            public static Value CreateValue(object o)
            {
                if (o == null)
                    return new NullValue();

                if (o is Value)
                    return (Value) o;

                var t = o.GetType().Inspector();

                if (t.Is(TypeFlags.Numeric) || t.Is(TypeFlags.String) || t.Is(TypeFlags.Boolean))
                    return new SimpleValue(o);


                return new NullValue();
            }

            public virtual Value this[string key] => NoValue;
            public virtual Value this[int i] => NoValue;

            public virtual IEnumerator<Value> GetEnumerator()
            {
                yield break;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public virtual object As(Type type) => null;

            public virtual T As<T>()
            {
                var value = As(typeof(T));

                return value == null ? default(T) : (T) value;
            }

            public virtual IEnumerable<T> AsArray<T>()
            {
                yield break;
            }

            public string Serialize(bool pretty = false)
            {
                using (var stream = new MemoryStream())
                {
                    using (var streamWriter = new StreamWriter(stream, Encoding.UTF8))
                        Serialize(streamWriter, pretty);

                    return Encoding.UTF8.GetString(stream.ToArray());
                }
            }

            public void Serialize(Stream stream, bool pretty = false)
            {
                using (var streamWriter = new StreamWriter(stream, Encoding.UTF8))
                    Serialize(streamWriter, pretty);
            }

            protected internal abstract void Serialize(StreamWriter stream, bool pretty = false);
        }

        public class NoValue : Value
        {
            public override bool IsObject => false;
            public override bool IsArray => false;
            public override bool IsEmpty => true;
            public override bool IsNull => false;

            protected internal override void Serialize(StreamWriter stream, bool pretty = false)
            {
                throw new NotSupportedException();
            }
        }

        public class NullValue : SimpleValue
        {
            public NullValue() : base(null)
            {
            }

            protected internal override void Serialize(StreamWriter stream, bool pretty = false)
            {
                stream.Write("null");
            }
        }

        public class SimpleValue : Value
        {
            readonly object _value;

            public SimpleValue(object value)
            {
                _value = value;
            }

            public override bool IsObject => false;
            public override bool IsArray => false;
            public override bool IsEmpty => false;
            public override bool IsNull => _value == null;

            public override object As(Type type)
            {
                return _value.Convert(type);
            }

            protected internal override void Serialize(StreamWriter stream, bool pretty = false)
            {
                if (_value == null)
                {
                    stream.Write("null");
                    return;
                }

                string s = _value as string;

                if (s != null)
                {
                    stream.Write('\"');

                    foreach (char c in s)
                    {
                        switch (c)
                        {
                            case '\t': stream.Write("\\t"); break;
                            case '\r': stream.Write("\\r"); break;
                            case '\n': stream.Write("\\n"); break;
                            case '"':
                            case '\\': stream.Write("\\" + c); break;
                            default:
                                {
                                    if (c >= ' ' && c < 128)
                                        stream.Write(c);
                                    else
                                        stream.Write("\\u" + ((int)c).ToString("X4"));
                                }
                                break;
                        }
                    }

                    stream.Write('\"');

                    return;
                }
            }

        }

        public class Field 
        {
            public string Name { get; }
            public Value Value { get; set; }

            public Field(string name, object value)
            {
                Name = name;
                Value = Value.CreateValue(value);
            }
        }

        public class Array : Value
        {
            private readonly List<Value> _items = new List<Value>();

            public void Add(object value)
            {
                _items.Add(CreateValue(value));
            }

            public override bool IsObject => false;
            public override bool IsArray => true;
            public override bool IsEmpty => false;
            public override bool IsNull => false;

            public override Value this[int i] => (i >= 0 && i < _items.Count) ? _items[i] : NoValue;

            public override IEnumerator<Value> GetEnumerator()
            {
                return _items.GetEnumerator();
            }

            public override IEnumerable<T> AsArray<T>()
            {
                return _items.Select(item => item.As<T>());
            }

            
            public override object As(Type type)
            {
                var t = type.Inspector();

                var iList = t.GetGenericInterfaces(typeof(IList<>)).FirstOrDefault();

                if (iList != null)
                {
                    var elementType = iList.GetGenericArguments()[0];

                    var list = (IList) Activator.CreateInstance(type);

                    foreach (var item in _items)
                        list.Add(item.As(elementType));

                    return list;
                }

                return null;
            }

            protected internal override void Serialize(StreamWriter streamWriter, bool pretty = false)
            {
                streamWriter.Write('[');

                bool first = true;

                foreach (var item in _items)
                {
                    if (first)
                        first = false;
                    else
                        streamWriter.Write(',');

                    item.Serialize(streamWriter, pretty);
                }
                streamWriter.Write(']');
            }

        }

        public class Object : Value
        {
            private readonly Dictionary<string, Value> _fields = new Dictionary<string, Value>();

            public void Add(Field field)
            {
                if (field?.Name != null)
                    _fields.Add(field.Name, field.Value);
            }

            public void Add(string name, object value)
            {
                if (name != null)
                    Add(new Field(name, value));
            }

            public override bool IsObject => true;
            public override bool IsArray => false;
            public override bool IsEmpty => false;
            public override bool IsNull => false;

            public override Value this[string key]
            {
                get
                {
                    Value value;

                    return _fields.TryGetValue(key, out value)  ? value : NoValue;
                }
            }

            public override IEnumerator<Value> GetEnumerator()
            {
                return _fields.Keys.Select(fieldName => new SimpleValue(fieldName)).GetEnumerator();
            }

            protected internal override void Serialize(StreamWriter streamWriter, bool pretty = false)
            {
                streamWriter.Write('{');
                bool first = true;
                foreach (var field in _fields)
                {
                    if (first)
                        first = false;
                    else
                        streamWriter.Write(',');

                    streamWriter.Write($"\"{field.Key}\":");

                    field.Value.Serialize(streamWriter, pretty);
                }

                streamWriter.Write('}');
            }
        }
    }

}