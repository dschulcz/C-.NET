using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.Model
{
    /// <summary>
    /// Represents a wall in the Bomberman game.
    /// </summary>
    public class Wall : Field
    {
        private static Wall? instance = null;

        private Wall()
        { }

        /// <summary>
        /// Destroys the wall.
        /// </summary>
        public void Destroy() { }

        /// <summary>
        /// Gets the instance of the wall.
        /// </summary>
        /// <returns>The instance of the wall.</returns>
        public static Wall GetInstance()
        {
            if (instance == null)
            {
                instance = new Wall();
            }
            return instance;
        }
    }
}
