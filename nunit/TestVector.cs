using NUnit.Framework;
using GLTech2;
using System;

namespace UnitTests
{
    class TestVector
    {
        private const float TORAD = (float)Math.PI / 180f;
        private const float TODEGREE = 180f / (float)Math.PI;

        [Test]
        public void Creation_by_angle()
        {
            Random rnd = new Random();

            float angle = (float)rnd.NextDouble();
            float
                x = (float)Math.Sin(TORAD * angle),
                y = (float)Math.Cos(TORAD * angle);

            Vector vector = new Vector(angle);

            // Assert.AreEqual(1f, vector.Module, "Module");
            Assert.AreEqual(new Vector(x, y), vector, "Incorrect components.");
        }
    }
}
