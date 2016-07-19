#region License
//=============================================================================
// VeloxDB Core - Portable .NET Productivity Library 
//
// Copyright (c) 2008-2015 Philippe Leybaert
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
using System.Globalization;
using System.Linq;

#if VELOX_DB
namespace Velox.DB.Core
#else
namespace Velox.Core
#endif
{
    public static class ObjectConverter
    {
        public static T Convert<T>(this object value)
        {
            return (T) Convert(value, typeof (T));
        }

        public static object Convert(this object value, Type targetType, DateConversionMethod dateConversionMethod = DateConversionMethod.DoubleIsJulian|DateConversionMethod.LongIsTicks)
        {
            if (targetType == typeof(object))
                return value;

            if (value is string)
                return ((string)value).To(targetType);

            var targetTypeInspector = targetType.Inspector();

            if (value == null)
				return targetTypeInspector.DefaultValue();

//            var originalTargetType = targetType;


			targetType = targetTypeInspector.RealType;

			Type sourceType = value.GetType();

			if (sourceType == targetType)
                return value;

            var sourceTypeInspector = sourceType.Inspector();

			var implicitOperator = targetTypeInspector.GetMethod("op_Implicit", new [] {sourceType});

            if (implicitOperator != null)
                return implicitOperator.Invoke(null, new [] {value});


			if (targetType == typeof(string))
            {
                if (sourceTypeInspector.Is(TypeFlags.Decimal))
                    return ((decimal)value).ToString(CultureInfo.InvariantCulture);
                if (sourceTypeInspector.Is(TypeFlags.FloatingPoint))
                    return (System.Convert.ToDouble(value)).ToString(CultureInfo.InvariantCulture);

                return value.ToString();
            }

			if (targetType == typeof (Guid) && value is byte[])
                return new Guid((byte[]) value);

			if (targetType == typeof (byte[]) && value is Guid)
                return ((Guid) value).ToByteArray();

			if (targetTypeInspector.IsEnum)
            {
                try
                {
                    value = System.Convert.ToInt64(value);

					value = Enum.ToObject(targetType, value);
                }
                catch
                {
                    return targetTypeInspector.DefaultValue();
                }

                if (Enum.IsDefined(targetType, value))
                    return value;

                if (!char.IsDigit(value.ToString()[0]))
                    return value;

                return targetTypeInspector.DefaultValue();
            }

			if (targetTypeInspector.IsAssignableFrom(value.GetType()))
                return value;

			if (targetType.IsArray && sourceType.IsArray)
			{
				Type targetArrayType = targetType.GetElementType();
				Array sourceArray = (Array) value;

				Array array = Array.CreateInstance(targetArrayType, new [] { sourceArray.Length }, new [] { 0 });

				for (int i = 0; i < sourceArray.Length; i++)
				{
					array.SetValue(sourceArray.GetValue(i).Convert(targetArrayType), i);
				}

				return array;
			}

            if (targetTypeInspector.Is(TypeFlags.DateTime))
                return ToDateTime(value, sourceTypeInspector,dateConversionMethod) ?? targetTypeInspector.DefaultValue();

            if (targetType == typeof(TimeSpan))
                return ToTimeSpan(value, sourceTypeInspector) ?? targetTypeInspector.DefaultValue();

            try
            {
				return System.Convert.ChangeType(value, targetType, null);
            }
            catch
            {
                return targetTypeInspector.DefaultValue();
            }
        }

        private static DateTime? ToDateTime(object value, TypeInspector type, DateConversionMethod dateConversion)
        {
            if (type.Is(TypeFlags.Integer64))
            {
                if ((dateConversion & DateConversionMethod.LongIsTicks) != 0)
                    return new DateTime(System.Convert.ToInt64(value));
                if ((dateConversion & DateConversionMethod.LongIsUnix) != 0)
                    return new DateTime(1970, 1, 1).AddSeconds(System.Convert.ToInt64(value));
            }

            if (type.Is(TypeFlags.Integer32))
            {
                return new DateTime(1970, 1, 1).AddSeconds(System.Convert.ToInt64(value));
            }

            if (type.Is(TypeFlags.FloatingPoint))
            {
                if ((dateConversion & DateConversionMethod.DoubleIsTicks) != 0)
                    return new DateTime(System.Convert.ToInt64(value));
                if ((dateConversion & DateConversionMethod.DoubleIsUnix) != 0)
                    return new DateTime(1970, 1, 1).AddSeconds(System.Convert.ToDouble(value));
                if ((dateConversion & DateConversionMethod.DoubleIsJulian) != 0)
                    return FromJulian(System.Convert.ToDouble(value));
            }

            return null;
        }

        private static TimeSpan? ToTimeSpan(object value, TypeInspector type)
        {
            if (type.Is(TypeFlags.Integer64))
            {
                
                return new TimeSpan(System.Convert.ToInt64(value));
            }

            if (type.Is(TypeFlags.Numeric))
            {
                return TimeSpan.FromSeconds(System.Convert.ToDouble(value));
            }

            return null;
        }

        public static int JGREG = 15 + 31 * (10 + 12 * 1582);
        public static double HALFSECOND = 0.5;

        private static DateTime FromJulian(double injulian)
        {
            int ja, jb, jc, jd, je, year, month, day;
            double julian = injulian + HALFSECOND / 86400.0;
            ja = (int) julian;

            if (ja >= JGREG)
            {
                var jalpha = (int)(((ja - 1867216) - 0.25) / 36524.25);
                ja = ja + 1 + jalpha - jalpha / 4;
            }

            jb = ja + 1524;
            jc = (int)(6680.0 + ((jb - 2439870) - 122.1) / 365.25);
            jd = 365 * jc + jc / 4;
            je = (int)((jb - jd) / 30.6001);
            day = jb - jd - (int)(30.6001 * je);
            month = je - 1;

            if (month > 12)
                month = month - 12;

            year = jc - 4715;

            if (month > 2)
                year--;

            if (year <= 0)
                year--;

            return new DateTime(year, month, day).AddDays(julian - ja);
        }

    }
}