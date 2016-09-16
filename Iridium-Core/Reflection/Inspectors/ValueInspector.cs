using System;
using System.Linq;

namespace Iridium.Core
{
    public class ValueInspector
    {
        private struct ValueResult
        {
            public ValueResult(object value)
            {
                Value = value;
            }

            public object Value;
        }

        private readonly object _obj;

        public ValueInspector(object obj)
        {
            _obj = obj;
        }

        public object GetValue(string path)
        {
            return GetValue(path, _obj);
        }

        public bool HasValue(string path)
        {
            return HasValue(path, _obj);
        }

        public static bool HasValue(string path, object obj)
        {
            return GetValueInternal(path, obj) != null;
        }

        public static object GetValue(string path, object obj)
        {
            return GetValueInternal(path, obj)?.Value;
        }

        private static ValueResult? GetValueInternal(string path, object obj)
        {
            int dotIndex = path.IndexOf('.');

            string field = dotIndex > 0 ? path.Substring(0, dotIndex) : path;
            string subField = dotIndex > 0 ? path.Substring(dotIndex + 1) : null;

            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static;

            var type = obj as Type;

            if (type == null)
            {
                bindingFlags |= BindingFlags.Instance;
                type = obj.GetType();
            }
            else
            {
                obj = null;
            }

            var fieldInfo = type.Inspector().GetFieldsAndProperties(bindingFlags).FirstOrDefault(f => f.Name == field);

            if (fieldInfo == null)
                return null;

            var value = fieldInfo.GetValue(obj);

            if (subField != null)
                return GetValueInternal(subField, value);

            return new ValueResult(value);
        }

    }
}