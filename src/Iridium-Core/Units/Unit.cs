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

namespace Iridium.Core
{
    public abstract class Unit
    {
        public static readonly Unit Meters = new DistanceUnit(1.0);
        public static readonly Unit Centimeters = new DistanceUnit(0.01);
        public static readonly Unit Millimeters = new DistanceUnit(0.001);
        public static readonly Unit KiloMeters = new DistanceUnit(1000.0);
        public static readonly Unit Miles = new DistanceUnit(1609.34);
        public static readonly Unit NauticalMiles = new DistanceUnit(1852);
        public static readonly Unit Yards = new DistanceUnit(0.9144);
        public static readonly Unit Inches = new DistanceUnit(0.0254);
        public static readonly Unit Feet = new DistanceUnit(0.3048);

        public static readonly Unit Grams = new WeightUnit(1.0);
        public static readonly Unit Kilograms = new WeightUnit(1000.0);
        public static readonly Unit Pounds = new WeightUnit(453.59237);
        public static readonly Unit Ounces = new WeightUnit(453.59237/16.0);
        
        public static readonly Unit Tons = new WeightUnit(453.59237*2000);
        public static readonly Unit MetricTons = new WeightUnit(1000000.0);
        public static readonly Unit MetricPounds = new WeightUnit(500);

        public abstract double Convert(double number, Unit targetUnit);
    }
}