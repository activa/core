using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Iridium.Core.Test
{
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