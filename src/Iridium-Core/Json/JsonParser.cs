using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Iridium.Core
{
    public class JsonParser
    {
        public static JsonObject Parse(string json)
        {
            return new JsonParser(json)._Parse();
        }

        public static JsonObject Parse(Stream stream)
        {
            return new JsonParser(stream)._Parse();
        }

        public static T Parse<T>(string json) where T : class, new()
        {
            return new JsonParser(json)._Parse<T>();
        }

        public static T Parse<T>(Stream stream) where T : class, new()
        {
            return new JsonParser(stream)._Parse<T>();
        }

        private readonly JsonTokenizer _tokenizer;

        public JsonParser(string s)
        {
            _tokenizer = new JsonTokenizer(s);
        }

        public JsonParser(Stream stream)
        {
            _tokenizer = new JsonTokenizer(stream);
        }

        private T _Parse<T>() where T:class
        {
            NextToken();

            return ParseObject(typeof(T)).As<T>();
        }

        private JsonObject _Parse()
        {
            NextToken();

            return ParseValue();
        }

        private JsonToken CurrentToken;

        private void NextToken()
        {
            CurrentToken = _tokenizer.NextToken();
        }

        
        private JsonObject ParseObject(Type objectType = null)
        {
            if (CurrentToken.Type != JsonTokenType.ObjectStart)
                throw new Exception("Expected {");

            object obj;
            List<MemberInspector> fieldsInType = null;
            bool isTypeMapped = true;

            if (objectType == null)
            {
                obj = new Dictionary<string, JsonObject>();

                isTypeMapped = false;
            }
            else
            {
                obj = Activator.CreateInstance(objectType);

                fieldsInType = objectType.Inspector().GetFieldsAndProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
            }

            NextToken();

            for (; ; )
            {
                if (CurrentToken.Type == JsonTokenType.ObjectEnd)
                    break;

                if (CurrentToken.Type != JsonTokenType.String)
                    throw new Exception("Expected property name");

                string propName = CurrentToken.Token;

                NextToken();

                if (CurrentToken.Type != JsonTokenType.Colon)
                    throw new Exception("Expected colon");

                NextToken();

                if (isTypeMapped)
                {
                    var field = fieldsInType.FirstOrDefault(f => string.Equals(f.Name, propName, StringComparison.OrdinalIgnoreCase));

                    if (field != null)
                    {
                        field.SetValue(obj, ParseValue(field.Type).As(field.Type));
                    }
                    else
                    {
                        ParseValue(typeof(object)); // ignore
                    }
                }
                else
                {
                    ((Dictionary<string, JsonObject>) obj)[propName] = ParseValue();
                }

                if (CurrentToken.Type != JsonTokenType.Comma)
                    break;

                NextToken();
            }

            if (CurrentToken.Type != JsonTokenType.ObjectEnd)
                throw new Exception("Expected }");

            NextToken();

            return JsonObject.FromValue(obj);
        }

        private static bool IsArray(Type type)
        {
            return type != null && (
                type.Inspector().ImplementsOrInherits<IList>() 
                ||
                type.Inspector().ImplementsOrInherits(typeof (IList<>)));
        }

        private JsonObject ParseValue(Type type = null)
        {
            bool isArray = false;

            if (type == null)
            {
                switch (CurrentToken.Type)
                {
                    case JsonTokenType.ObjectStart:
                        break;
                    case JsonTokenType.ArrayStart:
                        isArray = true;
                        break;
                    case JsonTokenType.Null:
                        NextToken();
                        return JsonObject.FromValue(null);
                    case JsonTokenType.True:
                        type = typeof(bool);
                        break;
                    case JsonTokenType.False:
                        type = typeof(bool);
                        break;
                    case JsonTokenType.Integer:
                        type = typeof(long);
                        break;
                    case JsonTokenType.Float:
                        type = typeof(double);
                        break;
                    case JsonTokenType.String:
                        type = typeof(string);
                        break;
                    default:
                        throw new Exception("Unexpected token " + CurrentToken.Type);
                }
            }

            if (isArray || IsArray(type))
                return ParseArray(type);

            if (type == null)
                return ParseObject();

            if (type == typeof(string))
                return ParseString();

            if (type == typeof(bool))
            {
                bool value = CurrentToken.Type == JsonTokenType.True;

                NextToken();

                return JsonObject.FromValue(value);
            }

            if (type == typeof(int) || type == typeof(short) || type == typeof(long) || type == typeof(double) || type == typeof(float) || type == typeof(decimal))
                return ParseNumber(type);

            return ParseObject(type);
        }

        private JsonObject ParseNumber(Type type)
        {
            if (CurrentToken.Type != JsonTokenType.Float && CurrentToken.Type != JsonTokenType.Integer)
                throw new Exception("Number expected");

            object n;

            if (CurrentToken.Type == JsonTokenType.Integer)
            {
                n = Int64.Parse(CurrentToken.Token, NumberFormatInfo.InvariantInfo);

                if ((long) n > Int32.MinValue && (long) n < Int32.MaxValue)
                {
                    n = (int) (long) n;
                    type = typeof(int);
                }
            }
            else
            {
                n = Double.Parse(CurrentToken.Token, NumberFormatInfo.InvariantInfo);
            }

            NextToken();

            return JsonObject.FromValue(Convert.ChangeType(n, type, null));
        }


        private JsonObject ParseString()
        {
            if (CurrentToken.Type != JsonTokenType.String)
                throw new Exception("Expected string");

            string s = CurrentToken.Token;

            NextToken();

            return JsonObject.FromValue(s);
        }

        private JsonObject ParseArray(Type type)
        {
            Type elementType = null;

            if (type != null && type.IsArray)
                elementType = type.GetElementType();

            if (CurrentToken.Type != JsonTokenType.ArrayStart)
                throw new Exception("Expected [");

            NextToken();
 
            var list = new List<object>();

            for (;;)
            {
                if (CurrentToken.Type == JsonTokenType.ArrayEnd)
                    break;

                list.Add(elementType == null ? ParseValue() : ParseValue().As(elementType));

                if (CurrentToken.Type != JsonTokenType.Comma)
                    break;

                NextToken();
            }

            if (CurrentToken.Type != JsonTokenType.ArrayEnd)
                throw new Exception("Expected ]");

            NextToken();

            Array array = Array.CreateInstance(elementType ?? typeof(JsonObject), list.Count);

            for (int i = 0; i < array.Length; i++)
                array.SetValue(list[i], i);

            return JsonObject.FromValue(array);
        }
    }
}