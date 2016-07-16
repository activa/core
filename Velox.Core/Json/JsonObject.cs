using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Velox.Core.StringExtensions;

namespace Velox.Core.Json
{
    public class JsonObject : IFormattable, IEnumerable<JsonObject>
    {
        private static readonly JsonObject _emptyInstance = new JsonObject(empty:true);

        private readonly object _value;
        private readonly bool _empty;

        internal JsonObject(object value = null, bool empty = false)
        {
            _value = value;
            _empty = empty;
        }

        public bool IsObject => _value is Dictionary<string, JsonObject>;
        public bool IsArray => _value is JsonObject[];
        public bool IsValue => !IsObject && !IsArray;
        public bool IsEmpty => _empty;
        public bool IsNull => _value == null && !_empty;
        public bool IsNullOrEmpty => _value == null;

        internal object Value => _value;

        public static JsonObject Empty => _emptyInstance;

        public object As(Type type)
        {
            return _value.Convert(type);
        }

        public T As<T>()
        {
            return _value.Convert<T>();
        }

        public static implicit operator string(JsonObject jsonObject)
        {
            return jsonObject.As<string>();
        }

        public static implicit operator int(JsonObject jsonObject)
        {
            return jsonObject.As<int>();
        }

        public static implicit operator long(JsonObject jsonObject)
        {
            return jsonObject.As<long>();
        }

        public static implicit operator double(JsonObject jsonObject)
        {
            return jsonObject.As<double>();
        }

        public static implicit operator decimal(JsonObject jsonObject)
        {
            return jsonObject.As<decimal>();
        }

        public static implicit operator bool(JsonObject jsonObject)
        {
            return jsonObject.As<bool>();
        }

        public JsonObject[] AsArray()
        {
            return _value as JsonObject[];
        }

        public T[] AsArray<T>()
        {
            if (!IsArray)
                return null;

            return AsArray().Select(x => x.As<T>()).ToArray();
        }

        public Dictionary<string, JsonObject> AsDictionary()
        {
            return _value as Dictionary<string,JsonObject>;
        } 

        public string[] Keys => IsObject ? AsDictionary().Keys.ToArray() : new string[0];

        public JsonObject this[string key] => ValueForExpression(this, key);

        public JsonObject this[int index]
        {
            get
            {
                if (!IsArray || index >= AsArray().Length)
                    return _emptyInstance;

                return AsArray()[index];
            }
        }

        private static IEnumerable<string> AllKeyParts(string key)
        {
            int index = 0;

            for (; ; )
            {
                int dotIndex = key.IndexOf('.', index);

                if (dotIndex < 0)
                {
                    yield return key;
                    break;
                }

                yield return key.Substring(0, dotIndex);

                index = dotIndex + 1;
            }
        }

        private static JsonObject ValueForExpression(JsonObject obj, string key)
        {
            if (!obj.IsObject)
                return _emptyInstance;

            foreach (var keyPart in AllKeyParts(key).Reverse().ToArray())
            {
                var dic = obj.AsDictionary();

                if (dic.ContainsKey(keyPart))
                {
                    var value = dic[keyPart];

                    if (keyPart.Length == key.Length)
                        return value;

                    string key2 = key.Substring(keyPart.Length + 1);

                    return ValueForExpression(value,key2);
                }

            }

            return _emptyInstance;
        }

        public IEnumerator<JsonObject> GetEnumerator()
        {
            if (IsObject)
            {
                return AsDictionary().Values.GetEnumerator();
            }
            else if (IsArray)
            {
                return (from obj in AsArray() select obj).GetEnumerator();
            }
            else
            {
                return (IEnumerator<JsonObject>) (new[] {this}).GetEnumerator();
            }
            
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public override string ToString()
        {
            return ToString(null, null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
#if DEBUG
            if (IsArray)
                return "[" + AsArray().Length + " items]";

            if (IsObject)
                return "{...}";

            if (_value == null)
                return "(null)";

            if (_value is IFormattable && format != null)
                return ((IFormattable) _value).ToString(format, formatProvider);
#endif
            return _value.ToString();
        }
    }
}