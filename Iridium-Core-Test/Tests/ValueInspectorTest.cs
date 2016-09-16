using NUnit.Framework;

namespace Iridium.Core.Test
{
    [TestFixture]
    public class ValueInspectorTest
    {
        [Test]
        public void SimpleAnonymous()
        {
            var obj = new {Field1 = 1, Field2 = 2};

            var inspector = obj.ValueInspector();

            Assert.That(inspector.GetValue("Field1"), Is.EqualTo(1));
            Assert.That(inspector.GetValue("Field2"), Is.EqualTo(2));
        }


        [Test]
        public void NestedAnonymous()
        {
            var obj = new
            {
                Field1 = 1,
                FieldNull = (object)null,
                Obj1 = new
                {
                    Field2 = 2,
                    Obj2 = new
                    {
                        Field3 = 3
                    }
                }
            };

            var inspector = obj.ValueInspector();

            Assert.That(inspector.GetValue("Field1"), Is.EqualTo(1));
            Assert.That(inspector.GetValue("FieldNull"), Is.Null);
            Assert.That(inspector.GetValue("Obj1.Field2"), Is.EqualTo(2));
            Assert.That(inspector.GetValue("Obj1.Obj2.Field3"), Is.EqualTo(3));
            Assert.That(inspector.GetValue("Obj3.Obj2.Field2"), Is.Null);
            Assert.That(inspector.GetValue("Obj1.Obj3.Field2"), Is.Null);
            Assert.That(inspector.HasValue("Obj3.Obj2.Field2"), Is.False);
            Assert.That(inspector.HasValue("Obj1.Obj3.Field2"), Is.False);


            Assert.That(inspector.HasValue("Field1"), Is.True);
            Assert.That(inspector.HasValue("Field3"), Is.False);
        }

        [Test]
        public void NestedConcrete()
        {
            var obj = new NestedClass();

            var inspector = obj.ValueInspector();

            Assert.That(inspector.GetValue("Field1"), Is.EqualTo(1));
            Assert.That(inspector.GetValue("Field2"), Is.EqualTo(2));
            Assert.That(inspector.GetValue("Inner1.Field1"), Is.EqualTo(11));
            Assert.That(inspector.GetValue("Inner1.Field2"), Is.EqualTo(12));
            Assert.That(inspector.GetValue("Inner2.Field1"), Is.EqualTo(21));
            Assert.That(inspector.GetValue("Inner2.Field2"), Is.EqualTo(22));
            Assert.That(inspector.GetValue("Inner1.Field3"), Is.Null);
            Assert.That(inspector.GetValue("Inner2.Field3"), Is.Null);
       }

        [Test]
        public void NestedStatic()
        {
            var inspector = typeof(StaticNestedClass).ValueInspector();

            Assert.That(inspector.GetValue("Field1"), Is.EqualTo(1));
            Assert.That(inspector.GetValue("Field2"), Is.EqualTo(2));
            Assert.That(inspector.GetValue("Inner1.Field1"), Is.EqualTo(11));
            Assert.That(inspector.GetValue("Inner1.Field2"), Is.EqualTo(12));
            Assert.That(inspector.GetValue("Inner2.Field1"), Is.EqualTo(21));
            Assert.That(inspector.GetValue("Inner2.Field2"), Is.EqualTo(22));
            Assert.That(inspector.GetValue("Inner1.Field3"), Is.Null);
            Assert.That(inspector.GetValue("Inner2.Field3"), Is.Null);
        }


        public class NestedClass
        {
            public int Field1 = 1;
            public int Field2 { get; } = 2;

            public class InnerClass1
            {
                public int Field1 = 11;
                public int Field2 { get; } = 12;
            }

            public class InnerClass2
            {
                public int Field1 = 21;
                public int Field2 { get; } = 22;
            }

            public InnerClass1 Inner1 = new InnerClass1();
            public static InnerClass2 Inner2 = new InnerClass2();
        }

        public static class StaticNestedClass
        {
            public static int Field1 = 1;
            public static int Field2 { get; } = 2;

            public class InnerClass1
            {
                public int Field1 = 11;
                public int Field2 { get; } = 12;
            }

            public class InnerClass2
            {
                public int Field1 = 21;
                public int Field2 { get; } = 22;
            }

            public static InnerClass1 Inner1 = new InnerClass1();
            public static InnerClass2 Inner2 = new InnerClass2();

        }

    }
}