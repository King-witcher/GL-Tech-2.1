using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2
{
    /// <summary>
    /// Represents and empty element that can basically be used to parent groups of elements.
    /// </summary>
    public sealed class Empty : Element
    {
        /// <summary>
        /// Gets a new instance of Empty.
        /// </summary>
        /// <param name="pos">The position of the empty.</param>
        public Empty(Vector pos)
        {
            AbsolutePosition = pos;
            AbsoluteNormal = Vector.Forward;
        }

        /// <summary>
        /// Gets a new instance of Empty.
        /// </summary>
        /// <param name="x">The x cordinate</param>
        /// <param name="y">The y cordinate</param>
        public Empty(float x, float y) : this(new Vector(x, y))
        {
        }

        private protected override Vector AbsolutePosition { get; set; }
        private protected override Vector AbsoluteNormal { get; set; }
    }
}
