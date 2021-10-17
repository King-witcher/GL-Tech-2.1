﻿using NUnit.Framework;
using GLTech2;
using System;

namespace UnitTests
{
    class Parenting_Positional_Consistency
    {
        Random rnd = new Random();

        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public void Component_Consistency()
        {
            Vector parent_pos = Vector.GetRandom(1000f);
            Vector parent_dir = Vector.GetRandom(1f);

            Vector child_pos = Vector.GetRandom(1000f);
            Vector child_dir = Vector.GetRandom(1f);

            Empty parent = new Empty(parent_pos);
            parent.WorldDirection = parent_dir;

            Empty child = new Empty(child_pos);
            child.WorldDirection = child_dir;

            float parent_rot = parent.RelativeRotation;
            float child_rot = child.RelativeRotation;

            // Relative = World
            Assert.AreEqual(child_pos, child.RelativePosition, "RelativePosition must be changed together with WorldPosition.");
            Assert.AreEqual(child_dir, child.RelativeDirection, "RelativeDirection must be changed together with WorldDirection.");

            child.Parent = parent;

            // World consistent
            Assert.AreEqual(child_pos, child.WorldPosition, "WorldPositoin cannot change when parenting.");
            Assert.AreEqual(child_dir, child.WorldDirection, "WorldDirection cannot change when parenting.");

            child.Parent = null;

            // World consistent
            Assert.AreEqual(child_pos, child.WorldPosition, "WorldPositoin cannot change when unparenting.");
            Assert.AreEqual(child_dir, child.WorldDirection, "WorldDirection cannot change when unparenting.");

            // Relative = world
            Assert.AreEqual(child_pos, child.RelativePosition, "RelativePosition must be equal to WorldPosition after unparented.");
            Assert.AreEqual(child_dir, child.RelativeDirection, "RelativeDirection must be equal to WorldDirection after unparented.");
        }

        [Test]
        public void Parenting_Same_Position()
        {
            Vector parent_pos = Vector.GetRandom(1000f);
            Vector parent_dir = Vector.GetRandom(1f);

            Empty parent = new Empty(parent_pos);
            parent.WorldDirection = parent_dir;

            Empty child = new Empty(parent_pos);
            child.WorldDirection = parent_dir;

            child.Parent = parent;

            Assert.AreEqual(new Vector(0f, 0f), child.RelativePosition, "Child position must be <0, 0> after parenting.");
            Assert.AreEqual(new Vector(0f, 1f), child.RelativeDirection, "Child position must be <0, 0> after parenting.");
        }

        [Test]
        public void Moving_Parent()
        {
            Vector child_pos = Vector.GetRandom(100f);
            Vector child_dir = Vector.GetRandom(1f);
            Vector move_dir = Vector.GetRandom(50f);

            Empty parent = new Empty(Vector.Zero);

            Empty child = new Empty(child_pos);
            child.WorldDirection = child_dir;

            child.Parent = parent;

            parent.WorldPosition += move_dir;

            bool assert = (move_dir + child_pos - child.WorldPosition).Module < 0.000001f; 

            Assert.IsTrue(assert, "Childs should move together with parents.");
        }

        [Test]
        public void Rotating_Parent()
        {
            Element parent = new Empty(Vector.Zero);
            Element child = new Empty((1, 0));

            child.Parent = parent;
            parent.RelativeRotation -= 90f;
            child.Parent = null;

            // Must be rounded due to floating point imprecision
            Vector rounded_pos = (
                (float)Math.Round(child.WorldPosition.X, 5),
                (float)Math.Round(child.WorldPosition.Y, 5));

            Assert.AreEqual(new Vector(0, 1), rounded_pos);
        }
    }
}