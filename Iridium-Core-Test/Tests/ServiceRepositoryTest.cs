using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Iridium.Core.Test
{
    [TestFixture]
    public class ServiceRepositoryTest
    {
        private interface IService1
        {
        }

        private interface IService2
        {
        }

        private interface IService3
        {
            
        }

        private class Service1A : IService1
        {
        }

        private class Service1B : IService1
        {
        }

        private class Service2A : IService2
        {
            public IService1 _service1;

            public Service2A(IService1 service1)
            {
                _service1 = service1;
            }

            public Service2A(int x)
            {
                
            }
        }


        [Test]
        public void SimpleInterface()
        {
            ServiceRepository repo = new ServiceRepository();

            repo.Register<Service2A>(singleton: false);
            repo.Register<Service1A>(singleton:false);

            var s1 = repo.Get<IService1>();
            var s2 = repo.Get<IService1>();
            var s3 = repo.Get<IService3>();

            Assert.That(s1, Is.InstanceOf<Service1A>());
            Assert.That(s2, Is.InstanceOf<Service1A>());
            Assert.That(s1, Is.Not.SameAs(s2));
            Assert.That(s3, Is.Null);
        }

        [Test]
        public void SimpleInterface_Singleton()
        {
            ServiceRepository repo = new ServiceRepository();

            repo.Register<Service1A>(singleton:true);

            var s1 = repo.Get<IService1>();
            var s2 = repo.Get<IService1>();

            Assert.That(s1, Is.InstanceOf<Service1A>());
            Assert.That(s2, Is.InstanceOf<Service1A>());
            Assert.That(s1, Is.SameAs(s2));
        }

        [Test]
        public void SimpleInterface_WithDependency()
        {
            ServiceRepository repo = new ServiceRepository();

            repo.Register<Service2A>(singleton: false);

            var s1 = repo.Get<IService2>();

            Assert.That(s1, Is.Null);

            repo.Register<Service1A>(singleton: false);

            s1 = repo.Get<IService2>();

            Assert.That(s1, Is.InstanceOf<Service2A>());
            Assert.That(((Service2A)s1)._service1, Is.InstanceOf<Service1A>());
        }

        [Test]
        public void SimpleInstance()
        {
            ServiceRepository repo = new ServiceRepository();

            repo.Register(new Service1B());

            Assert.That(repo.Get<IService1>(), Is.InstanceOf<Service1B>());
        }

        [Test]
        public void RegisterNull()
        {
            Assert.Catch<ArgumentNullException>(() => new ServiceRepository().Register<IService1>(null));
        }
    }

    [TestFixture]
    public class CoreExtensionsTest
    {
        [Test]
        public void IsIn_Params()
        {
            Assert.That(5.IsIn(3, 4, 5), Is.True);
            Assert.That(6.IsIn(3, 4, 5), Is.False);
            Assert.That(((string)null).IsIn("a","b","c"), Is.False);
            Assert.That(((string)null).IsIn("a",null, "c"), Is.True);
        }

        [Test]
        public void IsIn_List()
        {
            Assert.That(5.IsIn(Enumerable.Range(3,3)), Is.True);
            Assert.That(6.IsIn(Enumerable.Range(3, 3)), Is.False);
            Assert.That(((string)null).IsIn(new [] { "a", "b", "c" }), Is.False);
            Assert.That(((string)null).IsIn(new[] { "a", null, "c" }), Is.True);

            Assert.Throws<ArgumentNullException>(() => "x".IsIn((IEnumerable<string>)null));
        }

        [Test]
        public void IndexIn_Params()
        {
            Assert.That(5.IndexIn(3, 4, 5), Is.EqualTo(2));
            Assert.That(6.IndexIn(3, 4, 5), Is.LessThan(0));
            Assert.That(((string)null).IndexIn("a", "b", "c"), Is.LessThan(0));
            Assert.That(((string)null).IndexIn("a", null, "c"), Is.EqualTo(1));
        }

        [Test]
        public void IndexIn_List()
        {
            Assert.That(5.IndexIn(Enumerable.Range(3, 3)), Is.EqualTo(2));
            Assert.That(6.IndexIn(Enumerable.Range(3, 3)), Is.LessThan(0));
            Assert.That(((string)null).IndexIn(new[] { "a", "b", "c" }), Is.LessThan(0));
            Assert.That(((string)null).IndexIn(new[] { "a", null, "c" }), Is.EqualTo(1));

            Assert.Throws<ArgumentNullException>(() => "x".IndexIn((IEnumerable<string>)null));
        }

    }
}


