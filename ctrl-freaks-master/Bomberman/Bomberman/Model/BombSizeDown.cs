using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.Model
{
    /// <summary>
    /// Represents a power-up that decreases the size of the bomb explosion.
    /// </summary>
    public class BombSizeDown : Boost
    {
        private static BombSizeDown? instance = null;

        private BombSizeDown()
        { }

        /// <summary>
        /// Destroys the BombSizeDown power-up.
        /// </summary>
        public void Destroy() { }

        /// <summary>
        /// Gets the instance of the BombSizeDown power-up.
        /// </summary>
        /// <returns>The instance of the BombSizeDown power-up.</returns>
        public static BombSizeDown GetInstance()
        {
            if (instance == null)
            {
                instance = new BombSizeDown();
            }
            return instance;
        }
    }
}
