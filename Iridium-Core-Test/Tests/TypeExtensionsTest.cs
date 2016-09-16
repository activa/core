using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Iridium.Core.Test
{
    [TestFixture]
    public class TypeFlagsTest
    {
        private TypeFlags[] _allTests;

        [OneTimeSetUp]
        public void Init()
        {
            //var modifiers = new[] {TypeFlags.Array, };
            var modifiers = new[] { TypeFlags.CanBeNull, TypeFlags.ElementCanBeNull, TypeFlags.Nullable, TypeFlags.ElementNullable, TypeFlags.ValueType, TypeFlags.ElementValueType, TypeFlags.Array };

            var types = Enumerable.Range(0, 19).Select(i => (TypeFlags) (1<<i)).ToArray();

            var allTests = new List<TypeFlags>();

            foreach (var type in types)
            {
                allTests.Add(type);

                foreach (var flag in modifiers.AllCombinations())
                {
                    allTests.Add(type | flag);
                    allTests.Add(flag);
                }
                
            }

            _allTests = allTests.ToArray();
        }

        [TestCaseSource(nameof(TypeTestCaseSource))]
        public void Integers(Type type, TypeFlags[] validFlags)
        {
            foreach (var flag in validFlags)
            {
                Assert.That(type.Inspector().Is(flag),Is.True,$"{type.Name} has {type.Inspector().TypeFlags} but didn't match {flag}");
            }

            foreach (var flag in _allTests)
            {
                if (validFlags.Contains(flag))
                    continue;

                Assert.That(type.Inspector().Is(flag), Is.False, $"{type.Name} has {type.Inspector().TypeFlags} and matched {flag} but it shouldn't");
            }
            
            
        }

        public static IEnumerable<TestCaseData> TypeTestCaseSource()
        {
            var primitives = new []
            {
                new { Type = typeof(int), Flags = new[] {TypeFlags.Int32, TypeFlags.Integer32, TypeFlags.Integer, TypeFlags.SignedInteger, TypeFlags.Numeric, TypeFlags.Primitive}},
                new { Type = typeof(uint), Flags = new[] {TypeFlags.UInt32, TypeFlags.Integer32, TypeFlags.Integer, TypeFlags.UnsignedInteger, TypeFlags.Numeric, TypeFlags.Primitive }},
                new { Type = typeof(byte), Flags = new[] {TypeFlags.Byte, TypeFlags.Integer8, TypeFlags.Integer, TypeFlags.UnsignedInteger, TypeFlags.Numeric, TypeFlags.Primitive }},
                new { Type = typeof(char), Flags = new[] {TypeFlags.Char, TypeFlags.Integer16, TypeFlags.Integer, TypeFlags.SignedInteger, TypeFlags.Numeric, TypeFlags.Primitive }},
                new { Type = typeof(sbyte), Flags = new[] {TypeFlags.SByte, TypeFlags.Integer8, TypeFlags.Integer, TypeFlags.SignedInteger, TypeFlags.Numeric, TypeFlags.Primitive }},
            };

            List<TypeFlags> validFlags = new List<TypeFlags>();

            foreach (var primitive in primitives)
            {
                validFlags.Clear();
                

                validFlags.AddRange(primitive.Flags);
                validFlags.Add(TypeFlags.ValueType);
                validFlags.AddRange(primitive.Flags.Select(f => f | TypeFlags.ValueType));

                yield return new TestCaseData(primitive.Type, validFlags.ToArray());

                // nullable

                foreach (var combo in new[] { TypeFlags.Nullable, TypeFlags.CanBeNull, TypeFlags.ValueType }.AllCombinations())
                {
                    validFlags.Add(combo);
                    validFlags.AddRange(primitive.Flags.Select(flag => flag | combo));
                }


                yield return new TestCaseData(typeof(Nullable<>).MakeGenericType(primitive.Type), validFlags.ToArray());

                // array

                validFlags.Clear();

                foreach (var flag in primitive.Flags)
                {
                    validFlags.Add(flag | TypeFlags.Array);

                    validFlags.Add(flag | TypeFlags.Array | TypeFlags.CanBeNull);
                    validFlags.Add(flag | TypeFlags.Array | TypeFlags.ElementValueType);
                    validFlags.Add(flag | TypeFlags.Array | TypeFlags.CanBeNull | TypeFlags.ElementValueType);

                    validFlags.AddRange(new[] { TypeFlags.Array, TypeFlags.CanBeNull, TypeFlags.ElementValueType }.AllCombinations());
                }

                yield return new TestCaseData(primitive.Type.MakeArrayType(), validFlags.ToArray());
            }

            // string

            validFlags.Clear();
            validFlags.AddRange(new[] {TypeFlags.CanBeNull, TypeFlags.String, }.AllCombinations());

            yield return new TestCaseData(typeof(string),validFlags.ToArray());


        }

    }


    [TestFixture]
    public class TypeExtensionsTest
    {
        [Test]
        public void IsNullable()
        {
            Assert.IsFalse(typeof(object).Inspector().IsNullable);
            Assert.IsFalse(typeof(int).Inspector().IsNullable);
            Assert.IsTrue(typeof(int?).Inspector().IsNullable);
            Assert.IsFalse(typeof(string).Inspector().IsNullable);
            Assert.IsFalse(typeof(DateTime).Inspector().IsNullable);
        }

        [Test]
        public void CanBeNull()
        {
            Assert.IsTrue(typeof(object).Inspector().CanBeNull);
            Assert.IsFalse(typeof(int).Inspector().CanBeNull);
            Assert.IsTrue(typeof(int?).Inspector().CanBeNull);
            Assert.IsTrue(typeof(string).Inspector().CanBeNull);
            Assert.IsFalse(typeof(DateTime).Inspector().CanBeNull);
        }

        [Test]
        public void DefaultValue()
        {
            Assert.IsNull(typeof(object).Inspector().DefaultValue());
            Assert.AreEqual(0, typeof(int).Inspector().DefaultValue());
            Assert.IsNull(typeof(int?).Inspector().DefaultValue());
            Assert.IsNull(typeof(string).Inspector().DefaultValue());
            Assert.AreEqual(new DateTime(), typeof(DateTime).Inspector().DefaultValue());
        }

        [Test]
        public void GetRealType()
        {
            Assert.AreEqual(typeof(object), typeof(object).Inspector().RealType);
            Assert.AreEqual(typeof(int), typeof(int?).Inspector().RealType);
            Assert.AreEqual(typeof(string), typeof(string).Inspector().RealType);
        }

        private class Test1Attribute  : Attribute
        {
            
        }

        [Test1]
        private class TestClass1
        {
            
        }

        private class TestClass2
        {

        }

        [Test]
        public void GetAttribute()
        {
            Assert.IsInstanceOf<Test1Attribute>(typeof(TestClass1).Inspector().GetAttribute<Test1Attribute>(false));
            Assert.IsNull(typeof(TestClass2).Inspector().GetAttribute<Test1Attribute>(false));
        }
    }

    public static class LINQExtensions
    {
        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };

            return sequences.Aggregate(
                      emptyProduct,
                        (accumulator, sequence) =>
                            from accseq in accumulator
                            from item in sequence
                            select accseq.Concat(new[] { item }));
        }

        public static IEnumerable<TSource> Prepend<TSource>(this IEnumerable<TSource> source, TSource item)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            yield return item;

            foreach (var element in source)
                yield return element;
        }

        public static IEnumerable<IEnumerable<TSource>> Combinate<TSource>(this IEnumerable<TSource> source, int k)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var list = source.ToList();
            if (k > list.Count)
                throw new ArgumentOutOfRangeException("k");

            if (k == 0)
                yield return Enumerable.Empty<TSource>();

            foreach (var l in list)
                foreach (var c in Combinate(list.Skip(list.Count - k - 2), k - 1))
                    yield return c.Prepend(l);
        }

        public static IEnumerable<IEnumerable<T>> GetPowerSet<T>(this IList<T> list)
        {
            return (from m in Enumerable.Range(0, 1 << list.Count)
                   select
                       from i in Enumerable.Range(0, list.Count)
                       where (m & (1 << i)) != 0
                       select list[i]).Skip(1);
        }

        public static IEnumerable<TypeFlags> AllCombinations(this IList<TypeFlags> list)
        {
            var combos = list.GetPowerSet();

            foreach (var combo in combos)
            {
                yield return combo.Aggregate<TypeFlags, TypeFlags>(0, (current, flag) => current | flag);
            }
        }

        

    }
}