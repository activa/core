using NUnit.Framework;

namespace Iridium.Core.Test
{
    [TestFixture]
    public class ServiceRepositoryGenericTest
    {
        private interface IGenericService1<T>
        {
        }

        private interface IGenericService2<T>
        {
        }


        private class GenericService1<T> : IGenericService1<T>
        {
        }

        private class GenericService2<T> : IGenericService2<T>
        {
        }

        private class GenericServiceWithParam<T> : IGenericService2<T>
        {
            public T Svc1;

            public GenericServiceWithParam(T service1)
            {
                Svc1 = service1;
            }

            public GenericServiceWithParam(int x)
            {

            }
        }


        private class GenericService12A<T> : IGenericService1<T>, IGenericService2<T>
        {
        }

        private class GenericService12B<T> : IGenericService1<T>, IGenericService2<T>
        {
        }

        private class GenericService1Int : IGenericService1<int>
        {
        }

        private class GenericService1Bool : IGenericService1<bool>
        {
        }

        

        [Test]
        public void SimpleInterface()
        {
            ServiceRepository repo = new ServiceRepository();

            repo.Register(typeof(GenericService1<>));

            var s1 = repo.Get<IGenericService1<int>>();
            var s2 = repo.Get<IGenericService1<int>>();

            Assert.That(s1, Is.InstanceOf<GenericService1<int>>());
            Assert.That(s2, Is.InstanceOf<GenericService1<int>>());
            Assert.That(s1, Is.Not.SameAs(s2));
        }

        [Test]
        public void SimpleInterface_Singleton()
        {
            ServiceRepository repo = new ServiceRepository();

            repo.Register(typeof(GenericService1<>)).Singleton();

            var s1 = repo.Get<IGenericService1<int>>();
            var s2 = repo.Get<IGenericService1<int>>();

            Assert.That(s1, Is.InstanceOf<GenericService1<int>>());
            Assert.That(s2, Is.InstanceOf<GenericService1<int>>());
            Assert.That(s1, Is.SameAs(s2));
        }

        [Test]
        public void SimpleInterface_WithDependency()
        {
            ServiceRepository repo = new ServiceRepository();

            repo.Register(typeof(GenericServiceWithParam<>));

            Assert.IsNull(repo.Get<IGenericService2<int>>());
            Assert.IsNull(repo.Get<IGenericService2<IGenericService1<int>>>());

            repo.Register(typeof(GenericService1<>));

            var s1 = repo.Get<IGenericService2<IGenericService1<int>>>();

            Assert.That(s1, Is.InstanceOf<GenericServiceWithParam<IGenericService1<int>>>());
            Assert.That(((GenericServiceWithParam<IGenericService1<int>>)s1).Svc1, Is.InstanceOf<GenericService1<int>>());
        }
        
        [Test]
        public void SimpleInstance()
        {
            ServiceRepository repo = new ServiceRepository();

            var svc = new GenericService1<int>();

            repo.Register(svc);

            Assert.That(repo.Get<GenericService1<int>>(), Is.SameAs(svc));
            Assert.That(repo.Get<GenericService1<bool>>(), Is.Null);
        }
        
        [Test]
        public void MultiInstance1()
        {
            GenericService12A<int> svc12a = new GenericService12A<int>();
            GenericService12B<int> svc12b = new GenericService12B<int>();

            ServiceRepository repo = new ServiceRepository();

            repo.Register<IGenericService1<int>>(svc12a);
            repo.Register<IGenericService2<int>>(svc12b);

            Assert.That(repo.Get<IGenericService1<int>>(), Is.SameAs(svc12a));
            Assert.That(repo.Get<IGenericService2<int>>(), Is.SameAs(svc12b));
        }
        

        [Test]
        public void MultiInterface()
        {
            ServiceRepository repo = new ServiceRepository();

            repo.Register(typeof(GenericService12A<>)).As(typeof(IGenericService1<>));
            repo.Register(typeof(GenericService12B<>)).As(typeof(IGenericService2<>));

            Assert.That(repo.Get<IGenericService1<int>>(), Is.InstanceOf<GenericService12A<int>>());
            Assert.That(repo.Get<IGenericService2<bool>>(), Is.InstanceOf<GenericService12A<bool>>());
        }


        [Test]
        public void Create1()
        {
            ServiceRepository repo = new ServiceRepository();

            var svc1 = new GenericService1<int>();

            repo.Register(svc1);

            var svc3 = repo.Create<GenericServiceWithParam<IGenericService1<int>>>();

            Assert.That(svc3, Is.Not.Null);
            Assert.That(svc3.Svc1, Is.SameAs(svc1));
        }

        [Test]
        public void Mixed()
        {
            ServiceRepository repo = new ServiceRepository();

            repo.Register<GenericService1Int>();
            repo.Register<GenericService1Bool>();

            Assert.That(repo.Get<IGenericService1<int>>(), Is.InstanceOf<GenericService1Int>());
            Assert.That(repo.Get<IGenericService1<bool>>(), Is.InstanceOf<GenericService1Bool>());
        }
    }
}