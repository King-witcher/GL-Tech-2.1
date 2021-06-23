using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2.PrefabBehaviours
{
    public class DebugPosition : Behaviour
    {
        public float Interval { get; set; } = 1f;

        float interval;

        void Update()
        {
            interval += Time.DeltaTime;

            if (interval >= Interval)
            {
                Debug.Log($"Current relative position: {Element.Position}\n" +
                    $"Current rotation: {Element.Rotation}\n" +
                    $"Current normal: {Element.Normal}");
                interval = 0f;
            }
        }
    }
}
