using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLTech2.PrefabBehaviours
{
    /// <summary>
    /// Debugs how much fps the scene is running.
    /// </summary>
    public class DebugFPS : Behaviour
    {
        /// <summary>
        /// The debug step interval in seconds
        /// </summary>
        public float Interval { get; set; } = 1f;

        int frames;
        float frametime;
        double rendertime;

        void Update()
        {
            frames++;
            rendertime += Time.RenderTime;
            frametime += Time.DeltaTime;

            if (frametime >= Interval)
            {
                Debug.Log("Raycast only fps: \t" + frames / rendertime);
                Debug.Log("Full time fps: \t" + frames / frametime);
                Debug.Log();
                frames = 0;
                rendertime = 0;
                frametime = 0;
            }
        }
    }
}
