#region License
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
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Iridium.Core
{
    public class JsonObject : IFormattable, IEnumerable<JsonObject>
    {
        private readonly object _value;

        internal JsonObject(object value = null, bool empty = false)
        {
            _value = value;
            IsEmpty = empty;
        }

        public bool IsObject => _value is Dictionary<string, JsonObject>;
        public bool IsArray => _value is JsonObject[];
        public bool IsValue => !IsObject && !IsArray && !IsEmpty;
        public bool IsEmpty { get; }
        public bool IsNull => _value == null && !IsEmpty;
        public bool IsNullOrEmpty => _value == null;
        public object Value => IsValue ? _value : null;

        public static JsonObject Empty { get; } = new JsonObject(empty:true);

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

        public static implicit operator int?(JsonObject jsonObject)
        {
            return jsonObject.As<int?>();
        }

        public static implicit operator long(JsonObject jsonObject)
        {
            return jsonObject.As<long>();
        }

        public static implicit operator long?(JsonObject jsonObject)
        {
            return jsonObject.As<long?>();
        }

        public static implicit operator double(JsonObject jsonObject)
        {
            return jsonObject.As<double>();
        }

        public static implicit operator double?(JsonObject jsonObject)
        {
            return jsonObject.As<double?>();
        }

        public static implicit operator decimal(JsonObject jsonObject)
        {
            return jsonObject.As<decimal>();
        }

        public static implicit operator decimal?(JsonObject jsonObject)
        {
            return jsonObject.As<decimal?>();
        }

        public static implicit operator bool(JsonObject jsonObject)
        {
            return jsonObject.As<bool>();
        }

        public static implicit operator bool?(JsonObject jsonObject)
        {
            return jsonObject.As<bool?>();
        }

        public static implicit operator JsonObject[](JsonObject jsonObject)
        {
            return jsonObject.AsArray();
        }

        public static implicit operator string[] (JsonObject jsonObject)
        {
            return jsonObject.AsArray<string>();
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
                    return Empty;

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
                return Empty;

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

            return Empty;
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