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

namespace Iridium.Core
{
    public static class Geo
    {
        public static double DistanceMeters(string hash1, string hash2)
        {
            double[] latlon1 = Geohash.Decode(hash1);
            double[] latlon2 = Geohash.Decode(hash2);

            return DistanceMeters(latlon1[0], latlon1[1], latlon2[0], latlon2[1]);
        }

        public static double DistanceMeters(double lat1, double lon1, double lat2, double lon2)
        {
            const double earthRadius = 6371008.8;
            const double f = Math.PI/180;

            lat1 *= f;
            lat2 *= f;
            lon1 *= f;
            lon2 *= f;

            double angle = Haversine(lat2 - lat1) + Math.Cos(lat1) * Math.Cos(lat2) * Haversine(lon2 - lon1);

            return 2 * earthRadius * Math.Asin(Math.Min(1, Math.Sqrt(angle)));
        }

        public static double Distance(double lat1, double lon1, double lat2, double lon2, Unit unit)
        {
            double meters = DistanceMeters(lat1, lon1, lat2, lon2);

            return meters.ConvertFrom(Unit.Meters).To(unit);
        }

        private static double Haversine(double angle)
        {
            return (1 - Math.Cos(angle)) / 2;
        }

    }
}