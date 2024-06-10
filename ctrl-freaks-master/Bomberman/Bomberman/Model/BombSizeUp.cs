using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.Model
{
    /// <summary>
    /// Represents a BombSizeUp boost in the game.
    /// </summary>
    public class BombSizeUp : Boost
    {
        private static BombSizeUp? instance = null;

        private BombSizeUp()
        { }

        /// <summary>
        /// Destroys the BombSizeUp boost.
        /// </summary>
        public void Destroy() { }

        /// <summary>
        /// Gets the instance of the BombSizeUp boost.
        /// </summary>
        /// <returns>The instance of the BombSizeUp boost.</returns>
        public static BombSizeUp GetInstance()
        {
            if (instance == null)
            {
                instance = new BombSizeUp();
            }
            return instance;
        }
    }
}
