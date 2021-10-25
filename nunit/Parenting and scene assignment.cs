using GLTech2;
using NUnit.Framework;
using System.Collections.Generic;

namespace UnitTests
{
    class Parenting_and_scene_assignment
    {
        [Test]
        [TestCase(1, 5)]
        [TestCase(5, 2)]
        [TestCase(5, 5)]
        public void Adding_an_element_should_add_its_childs(int childsPerElement, int depth)
        {
            Queue<Entity> queue = new Queue<Entity>();
            var scene = new Scene();
            Entity root = recursivelyCreateTree(depth);
            scene.AddElement(root);

            foreach (Entity element in queue)
                Assert.AreEqual(scene, element.Scene,
                    "The test found one element that was not added to the scene."
                    );


            Entity recursivelyCreateTree(int remainingDepth)
            {
                if (remainingDepth > 0)
                {
                    Empty element = new Empty(Vector.GetRandom(1000));
                    queue.Enqueue(element);

                    if (remainingDepth > 1)
                        for (int i = 0; i < childsPerElement; i++)
                            recursivelyCreateTree(remainingDepth - 1).Parent = element;

                    return element;
                }
                else
                    return null;
            }
        }

        [Test]
        public void Non_root_elements_cannot_be_added()
        {
            Empty parent = new Empty(Vector.GetRandom(1000));
            Empty child = new Empty(Vector.GetRandom(1000));
            Scene scene = new Scene();
            child.Parent = parent;

            scene.AddElement(child);

            Assert.IsNull(child.Scene);
            Assert.IsNull(parent.Scene);
        }
    }
}