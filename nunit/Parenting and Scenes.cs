using NUnit.Framework;
using GLTech2;
using System;

namespace UnitTests
{
    class Parenting_And_Scenes_Behaviour
    {
        [Test]
        public void Both_null_scenes_works()
        {
            Empty parent = new Empty(Vector.Zero);
            Empty child = new Empty(Vector.Zero);

            child.Parent = parent;

            Assert.AreEqual(parent, child.Parent, "Parenting should have worked.");
            Assert.AreEqual(null, child.Scene, "Scenes must have kept untouched.");
            Assert.AreEqual(null, parent.Scene, "Scenes must have kept untouched.");
        }

        public void Works_if_scenes_are_equal(bool parentScene, bool childScene)
        {
            Empty parent = new Empty(Vector.Zero);
            Empty child = new Empty(Vector.Zero);

            new Scene().AddElements(parent, child);

            child.Parent = parent;

            Assert.AreEqual(parent, child.Parent, "Parenting should have worked.");
            Assert.AreNotEqual(parent.Scene, child.Scene, "Scenes must have kept untouched.");
        }

        [Test]
        [TestCase(true, true, Description = "Both have a scene.")]
        [TestCase(true, false, Description = "Only parent has a scene.")]
        [TestCase(false, true, Description = "Only child has a scene.")]
        public void Does_not_work_if_scenes_are_different(bool parentScene, bool childScene)
        {
            Empty parent = new Empty(Vector.Zero);
            Empty child = new Empty(Vector.Zero);

            if (parentScene)
                new Scene().AddElement(parent);
            if (childScene)
                new Scene().AddElement(child);

            child.Parent = parent;

            Assert.IsNull(child.Parent, "Child's parent must keep untouched.");
            Assert.AreNotEqual(parent.Scene, child.Scene, "Child's scene must keep itself untouched.");
        }
    }
}