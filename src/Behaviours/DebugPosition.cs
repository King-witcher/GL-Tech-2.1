using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2.PrefabBehaviours
{
    /// <summary>
    /// Debugs all relative components of an object.
    /// </summary>
    public class DebugPosition : Behaviour
    {
        /// <summary>
        /// The interval to debug components again.
        /// </summary>
        public float Interval { get; set; } = 1f;

        float interval;

        void Update()
        {
            interval += Time.DeltaTime;

            if (interval >= Interval)
            {
                Debug.Log($"Relative position: {Element.Position}");
                Debug.Log($"Relative rotation: {Element.Rotation}");
                Debug.Log($"Relative normal: {Element.Normal}");
                Debug.Log();
                interval = 0f;
            }
        }
    }
}
