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
using System.Collections.Generic;
using System.Linq;

namespace Iridium.Core
{
    public class ServiceRepository
    {
        private static readonly List<object> _services = new List<object>();
        private static readonly Dictionary<Type,object> _cachedServices = new Dictionary<Type, object>();

        public static T Get<T>() where T:class
        {
            lock (_services)
            {
                object obj;

                if (_cachedServices.TryGetValue(typeof(T), out obj))
                    return (T) obj;

                obj = _services.OfType<T>().FirstOrDefault();

                if (obj == null)
                    return null;

                _cachedServices[typeof(T)] = obj;

                return (T) obj;
            }
        }

        public static void Register<T>(T service) where T:class
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            lock (_services)
            {
                if (_services.Contains(service))
                    return;

                _services.Add(service);
                _cachedServices.Clear();
            }
        }
    }
}