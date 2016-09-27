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
    public class NumberWithUnit
    {
        private readonly double _number;
        private readonly Unit _unit;

        public NumberWithUnit(double number, Unit unit)
        {
            _number = number;
            _unit = unit;
        }

        public NumberWithUnit(NumberWithUnit other, Unit targetUnit)
        {
            _unit = targetUnit;
            _number = other.To(targetUnit);
        }

        public double To(Unit toUnit)
        {
            return _unit.Convert(_number, toUnit);
        }

        public double In(Unit toUnit)
        {
            return _unit.Convert(_number, toUnit);
        }

        public double From(Unit fromUnit)
        {
            return fromUnit.Convert(_number, _unit);
        }
    }
}