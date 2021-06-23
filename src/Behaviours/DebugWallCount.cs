using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2.PrefabBehaviours
{
    public class DebugWallCount : Behaviour
    {
        void Start()
        {
            Debug.Log($"Walls in the scene: {Scene.WallCount}");
        }
    }
}
