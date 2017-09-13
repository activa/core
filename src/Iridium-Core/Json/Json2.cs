using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iridium.Core
{
    // Experimental - do not use
    internal class Json
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

                if (o is Value value)
                    return value;

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

                return value == null ? default(T) : (T)value;
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

                    var bytes = stream.ToArray();

                    return Encoding.UTF8.GetString(bytes,0,bytes.Length);
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

                if (_value is string s)
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
                    var elementType = iList.GenericTypeArguments[0];

                    var list = (IList)Activator.CreateInstance(type);

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

                    return _fields.TryGetValue(key, out value) ? value : NoValue;
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
