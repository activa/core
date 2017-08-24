using NUnit.Framework;

namespace Iridium.Core.Test
{
    /*
    [TestFixture]
    public class GeohashTest
    {
        [TestCase("9v6kdnsgp", "9v6kdnsg", "9v6kdns", "9v6kdns" )]
        public void LatLonToGeohash(params string[] hashes)
        {
            double lat = 30.3585;
            double lon = -97.9912;

            foreach (var s in hashes)
            {
                Assert.That(Geohash.Encode(lat,lon,s.Length), Is.EqualTo(s));
            }

            Assert.That(Geohash.Decode(hashes[0])[0], Is.EqualTo(lat).Within(0.0001));
            Assert.That(Geohash.Decode(hashes[0])[1], Is.EqualTo(lon).Within(0.0001));
        }

        [TestCase("gbsuv","gbsvh","gbsvj","gbsvn","gbsuu","gbsuy","gbsus","gbsut","gbsuw")]
        public void Neighbors(string loc, params string[] neighbors)
        {
            Assert.That(Geohash.Neighbors(loc),Is.EquivalentTo(neighbors));
        }
    }
    */
    [TestFixture]
    public class UnitsTest
    {
        [Test]
        public void Units()
        {
            Assert.That(1000.0.ConvertFrom(Unit.Meters).To(Unit.KiloMeters), Is.EqualTo(1.0).Within(0.00001));
            Assert.That(1609.34.ConvertFrom(Unit.Meters).To(Unit.Miles), Is.EqualTo(1.0).Within(0.00001));

            Assert.That(1.0.In(Unit.Miles).In(Unit.Meters), Is.EqualTo(1609.34).Within(0.0001));
        }
    }
    
}