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

namespace Iridium.Core
{
    public static class UnitConverterExtensions
    {
        public static NumberWithUnit ConvertFrom(this double number, Unit fromUnit)
        {
            return new NumberWithUnit(number, fromUnit);
        }

        public static double ConvertUnits(this double number, Unit fromUnit, Unit toUnit)
        {
            return fromUnit.Convert(number, toUnit);
        }

        public static NumberWithUnit In(this double number, Unit unit) => new NumberWithUnit(number,unit);
        public static NumberWithUnit In(this int number, Unit unit) => new NumberWithUnit(number, unit);
        public static NumberWithUnit In(this short number, Unit unit) => new NumberWithUnit(number, unit);
        public static NumberWithUnit In(this ushort number, Unit unit) => new NumberWithUnit(number, unit);
        public static NumberWithUnit In(this long number, Unit unit) => new NumberWithUnit(number, unit);
        public static NumberWithUnit In(this uint number, Unit unit) => new NumberWithUnit(number, unit);
        public static NumberWithUnit In(this ulong number, Unit unit) => new NumberWithUnit(number, unit);
        public static NumberWithUnit In(this decimal number, Unit unit) => new NumberWithUnit((double) number, unit);
    }
}