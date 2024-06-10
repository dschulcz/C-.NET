using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.Model
{
    /// <summary>
    /// Represents a Ghost in the Bomberman game.
    /// </summary>
    public class Ghost : Boost
    {
        private static Ghost? instance = null;

        private Ghost()
        { }

        /// <summary>
        /// Destroys the Ghost.
        /// </summary>
        public void Destroy() { }

        /// <summary>
        /// Gets the instance of the Ghost.
        /// </summary>
        /// <returns>The instance of the Ghost.</returns>
        public static Ghost GetInstance()
        {
            if (instance == null)
            {
                instance = new Ghost();
            }
            return instance;
        }
    }
}
