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
using System.Reflection;

namespace Iridium.Core
{
    public class ServiceRepository
    {
        public static ServiceRepository Default = new ServiceRepository();

        private class ServiceDefinition
        {
            public ServiceDefinition(Type type, bool singleton)
            {
                Type = type;
                Singleton = singleton;

                Constructors = (from c in Type.Inspector().GetConstructors()
                    let paramCount = c.GetParameters().Length
                    orderby paramCount descending 
                    select c).ToArray();
            }

            public ServiceDefinition(Type type, object obj)
            {
                Type = type;
                Object = obj;
                Singleton = true;
            }

            public readonly Type Type;
            public object Object;
            public readonly bool Singleton;
            public readonly ConstructorInfo[] Constructors;
        }

        private readonly List<ServiceDefinition> _services = new List<ServiceDefinition>();
        

        public T Get<T>() where T:class
        {
            return (T) Get(typeof(T));
        }


        private bool CanResolve(Type type)
        {
            return _services.Any(service => type.Inspector().IsAssignableFrom(service.Type));
        }

        public object Get(Type type)
        {

            foreach (var service in _services.Where(svc => type.Inspector().IsAssignableFrom(svc.Type)))
            {
                object obj = service.Object;

                if (obj != null)
                    return obj;

                var constructor = service.Constructors.FirstOrDefault(c => c.GetParameters().All(p => CanResolve(p.ParameterType)));

                if (constructor != null)
                {
                    obj = constructor.Invoke(constructor.GetParameters().Select(p => Get(p.ParameterType)).ToArray());

                    if (service.Singleton)
                        service.Object = obj;

                    return obj;
                }
            }

            return null;
        }

        public void Register<T>(bool singleton = true) where T : class
        {
            _services.Add(new ServiceDefinition(typeof(T), singleton));
        }

        public void Register<T>(T service) where T:class
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            _services.Add(new ServiceDefinition(typeof(T), service));
        }
    }
}