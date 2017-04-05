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
            public ServiceDefinition(Type type)
            {
                RegistrationType = type;
                Type = type;
                Singleton = false;

                Constructors = (from c in Type.Inspector().GetConstructors()
                    let paramCount = c.GetParameters().Length
                    orderby paramCount descending 
                    select c).ToArray();
            }

            public ServiceDefinition(Type registrationType, object obj)
            {
                RegistrationType = registrationType;
                Type = obj.GetType();
                Object = obj;
                Singleton = true;
            }

            public readonly Type Type;
            public Type RegistrationType;
            public object Object;
            public bool Singleton;
            public readonly ConstructorInfo[] Constructors;
        }

        private readonly List<ServiceDefinition> _services = new List<ServiceDefinition>();
        

        public T Get<T>() where T:class
        {
            return (T) Get(typeof(T));
        }


        private bool CanResolve(Type type)
        {
            return _services.Any(service => type.Inspector().IsAssignableFrom(service.RegistrationType));
        }

        public object Get(Type type)
        {
            foreach (var service in _services.Where(svc => type.Inspector().IsAssignableFrom(svc.RegistrationType)))
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

        public T Create<T>() where T:class
        {
            var service = new ServiceDefinition(typeof(T));

            var constructor = service.Constructors.FirstOrDefault(c => c.GetParameters().All(p => CanResolve(p.ParameterType)));

            return (T) constructor?.Invoke(constructor.GetParameters().Select(p => Get(p.ParameterType)).ToArray());
        }

        public IRegistrationResult Register<T>() where T : class
        {
            var serviceDefinition = new ServiceDefinition(typeof(T));

            _services.Add(serviceDefinition);

            return new RegistrationResult(serviceDefinition);
        }

        public IRegistrationResult Register<T>(T service) where T:class
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            var serviceDefinition = new ServiceDefinition(typeof(T), service);

            _services.Add(serviceDefinition);

            return new RegistrationResult(serviceDefinition);
        }

        private class RegistrationResult : IRegistrationResult
        {
            private ServiceDefinition Svc { get; }

            public RegistrationResult(ServiceDefinition svc)
            {
                Svc = svc;
            }

            public IRegistrationResult As<T>()
            {
                if (!typeof(T).GetTypeInfo().IsAssignableFrom(Svc.Type.GetTypeInfo()))
                    throw new ArgumentException("Type is not compatible");

                Svc.RegistrationType = typeof(T);

                return this;
            }

            public IRegistrationResult Singleton()
            {
                Svc.Singleton = true;

                return this;
            }

            public Type RegisteredAsType => Svc.RegistrationType;
        }

        public interface IRegistrationResult
        {
            IRegistrationResult As<T>();
            IRegistrationResult Singleton();

            Type RegisteredAsType { get; }
        }
    }
}