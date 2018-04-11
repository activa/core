#region License
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
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Iridium.Core
{
    public class JsonObject : IFormattable, IEnumerable<JsonObject>
    {
        private object _value;
        private bool _isUndefined;

        private JsonObject(object value)
        {
            _value = value;
        }

        private JsonObject(JsonObject obj)
        {
            _value = obj._value;
            _isUndefined = obj._isUndefined;
        }

        private JsonObject()
        {
            _isUndefined = true;
        }

        [Obsolete("IsEmpty has been renamed to IsUndefined")]
        public bool IsEmpty => _isUndefined;

        public bool IsObject => _value is Dictionary<string, JsonObject>;
        public bool IsArray => _value is JsonObject[];
        public bool IsValue => !IsObject && !IsArray && !IsUndefined;
        public bool IsUndefined => _isUndefined;
        public bool IsNull => _value == null && !IsUndefined;
        public bool IsNullOrEmpty => _value == null;
        public object Value => IsValue ? _value : null;

        public static JsonObject Undefined() => new JsonObject();
        public static JsonObject EmptyObject() => new JsonObject(new Dictionary<string,JsonObject>());
        public static JsonObject EmptyArray() => new JsonObject(new JsonObject[0]);
        public static JsonObject FromValue(object value) => new JsonObject(value);

        private void Set(JsonObject o)
        {
            _value = o._value;
            _isUndefined = o._isUndefined;
        }

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

        public static implicit operator string[](JsonObject jsonObject)
        {
            return jsonObject.AsArray<string>();
        }

        public static implicit operator JsonObject(string value)
        {
            return FromValue(value);
        }

        public static implicit operator JsonObject(int value)
        {
            return FromValue(value);
        }

        public static implicit operator JsonObject(int? value)
        {
            return FromValue(value);
        }

        public static implicit operator JsonObject(long value)
        {
            return FromValue(value);
        }

        public static implicit operator JsonObject(long? value)
        {
            return FromValue(value);
        }

        public static implicit operator JsonObject(decimal value)
        {
            return FromValue(value);
        }

        public static implicit operator JsonObject(decimal? value)
        {
            return FromValue(value);
        }

        public static implicit operator JsonObject(double value)
        {
            return FromValue(value);
        }

        public static implicit operator JsonObject(double? value)
        {
            return FromValue(value);
        }

        public static implicit operator JsonObject(bool value)
        {
            return FromValue(value);
        }

        public static implicit operator JsonObject(bool? value)
        {
            return FromValue(value);
        }

        public static implicit operator JsonObject(Array arr)
        {
            return FromValue(arr.Cast<object>().Select(o => (o is JsonObject) ? (JsonObject)o : new JsonObject(o)).ToArray());
        }

        public JsonObject[] AsArray()
        {
            return _value as JsonObject[];
        }

        public T[] AsArray<T>()
        {
            if (!IsArray)
                return new T[0];

            return AsArray().Select(x => x.As<T>()).ToArray();
        }

        public Dictionary<string, JsonObject> AsDictionary()
        {
            return _value as Dictionary<string, JsonObject>;
        }

        public bool HasField(string field) => IsObject && AsDictionary().ContainsKey(field);

        public string[] Keys => IsObject ? AsDictionary().Keys.ToArray() : new string[0];

        public JsonObject this[string key]
        {
            get { return ValueForExpression(this, key, createIfNotExists:false); }
            set { ValueForExpression(this, key, createIfNotExists:true).Set(value); }
        }

        public JsonObject this[int index]
        {
            get
            {
                if (!IsArray || index >= AsArray().Length)
                    return Undefined();

                return AsArray()[index];
            }
        }
        
        private static JsonObject ValueForExpression(JsonObject obj, string key, bool createIfNotExists)
        {
            int dotIndex = key.IndexOf('.');
            int bIndex = key.IndexOf('[');
            int nextIndex = -1;

            string firstKey = key;

            if (dotIndex > 0 && (bIndex < 0 || dotIndex < bIndex))
            {
                firstKey = key.Substring(0, dotIndex);
                nextIndex = dotIndex + 1;
            }
            else if (bIndex > 0 && (dotIndex < 0 || bIndex < dotIndex))
            {
                firstKey = key.Substring(0, bIndex);
                nextIndex = bIndex;
            }

            if (nextIndex >= 0)
            {
                if (createIfNotExists)
                {
                    if (!obj.IsObject)
                        obj.Set(EmptyObject());

                    var dict = obj.AsDictionary();

                    if (!dict.ContainsKey(firstKey))
                        dict[firstKey] = Undefined();
                }

                return ValueForExpression(obj[firstKey], key.Substring(nextIndex), createIfNotExists);
            }

            if (bIndex == 0)
            {
                bIndex = key.IndexOf(']');

                if (bIndex < 2)
                    return Undefined();

                int index = key.Substring(1, bIndex - 1).To<int>();

                if (index < 0)
                    return Undefined();

                if (createIfNotExists)
                {
                    if (!obj.IsArray)
                        obj.Set(EmptyArray());

                    var arr = obj.AsArray();

                    if (arr.Length <= index)
                    {
                        var newArr = new JsonObject[index + 1];

                        for (int i = 0; i < arr.Length; i++)
                            newArr[i] = arr[i];
                        for (int i = arr.Length; i <= index; i++)
                            newArr[i] = Undefined();

                        obj._value = newArr;
                        arr = newArr;
                    }
                    else
                    {
                        arr[index] = Undefined();
                    }
                }

                if (bIndex + 1 >= key.Length)
                    return obj[index];

                if (key[bIndex + 1] == '.')
                    return ValueForExpression(obj[index], key.Substring(bIndex + 2), createIfNotExists);
                else
                    return ValueForExpression(obj[index], key.Substring(bIndex + 1), createIfNotExists);
            }

            var dic = obj.AsDictionary();

            if (dic != null && dic.TryGetValue(key, out var value))
                return value;

            var returnValue = Undefined();

            if (createIfNotExists)
            {
                if (!obj.IsObject)
                    obj.Set(EmptyObject());

                obj.AsDictionary()[key] = returnValue;
            }

            return returnValue;
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
            else if (IsUndefined)
            {
                return Enumerable.Empty<JsonObject>().GetEnumerator();
            }
            else
            {
                return Enumerable.Repeat(this, 1).GetEnumerator();
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

            if (_value is IFormattable formattable && format != null)
                return formattable.ToString(format, formatProvider);
#endif
            return _value?.ToString() ?? "null";
        }
    }
}