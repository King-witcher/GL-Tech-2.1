using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace GLTech2
{
    /// <summary>
    ///     Provides an interface to handle keyboard inputs.
    /// </summary>
    public static class Keyboard
    {
        static LinkedList<Key> Keys = new LinkedList<Key>();

        /// <summary>
        ///     Checks if a specified key is pressed.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>true if the key is pressed; otherwise, false</returns>
        public static bool IsKeyDown(Key key) => Keys.Contains(key);
        public static event Action<Key> OnKeyDown;
        public static event Action<Key> OnKeyUp;

        /// <summary>
        ///     Occurs when a key is pressed.
        /// </summary>
        /// <param name="key">Key that was pressed</param>
        internal static void KeyDown(Key key)
        {
            Keys.AddFirst(key);
            OnKeyDown?.Invoke(key);
        }

        /// <summary>
        /// Occurs when a key is released.
        /// </summary>
        /// <param name="key">Key that was released</param>
        internal static void KeyUp(Key key)
        {
            while (Keys.Contains(key))  // Suboptimal
                Keys.Remove(key);
            OnKeyUp?.Invoke(key);
        }
    }
}
