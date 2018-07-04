using System.Collections.Generic;
using NUnit.Framework;

namespace Iridium.Core.Test
{
    [TestFixture]
    public class UnitsTest
    {
        [TestCaseSource(nameof(TestCases))]
        public void ConvertUnits(Unit from, Unit to, double result)
        {
            Assert.That(1.0.ConvertUnits(from, to), Is.EqualTo(result).Within(0.00001));
        }

        [TestCaseSource(nameof(TestCases))]
        public void NumberWithUnit(Unit from, Unit to, double result)
        {
            Assert.That(new NumberWithUnit(1.0,from).To(to), Is.EqualTo(result).Within(0.00001));
        }

        public static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(Unit.KiloMeters,Unit.Meters,1000.0);
                yield return new TestCaseData(Unit.Miles,Unit.Meters,1609.34);
                yield return new TestCaseData(Unit.Inches,Unit.Millimeters,25.4);
                yield return new TestCaseData(Unit.Yards,Unit.Centimeters,91.44);
                yield return new TestCaseData(Unit.Feet,Unit.Centimeters,30.48);
                yield return new TestCaseData(Unit.NauticalMiles,Unit.Meters,1852.0);
            }
        }
    }
    
}