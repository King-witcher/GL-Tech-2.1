using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2.PrefabBehaviours
{
    /// <summary>
    /// Debugs once how many walls are in the scene and then stops.
    /// </summary>
    public class DebugWallCount : Behaviour
    {
        void Start()
        {
            Debug.Log($"Walls in the scene: {Scene.WallCount}");
            Debug.Log();
        }
    }
}
