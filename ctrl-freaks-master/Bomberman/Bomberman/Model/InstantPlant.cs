using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.Model
{
    /// <summary>
    /// Represents an InstantPlant boost in the game.
    /// </summary>
    public class InstantPlant : Boost
    {
        private static InstantPlant? instance = null;

        private InstantPlant()
        { }

        /// <summary>
        /// Destroys the InstantPlant boost.
        /// </summary>
        public void Destroy() { }

        /// <summary>
        /// Gets the instance of the InstantPlant boost.
        /// </summary>
        /// <returns>The instance of the InstantPlant boost.</returns>
        public static InstantPlant GetInstance()
        {
            if (instance == null)
            {
                instance = new InstantPlant();
            }
            return instance;
        }
    }
}
