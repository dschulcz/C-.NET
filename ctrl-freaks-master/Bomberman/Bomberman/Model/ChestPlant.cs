using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.Model
{
    /// <summary>
    /// Represents a ChestPlant boost in the game.
    /// </summary>
    public class ChestPlant : Boost
    {
        private static ChestPlant? instance = null;

        private ChestPlant()
        { }

        /// <summary>
        /// Destroys the ChestPlant.
        /// </summary>
        public void Destroy() { }

        /// <summary>
        /// Gets the instance of the ChestPlant.
        /// </summary>
        /// <returns>The instance of the ChestPlant.</returns>
        public static ChestPlant GetInstance()
        {
            if (instance == null)
            {
                instance = new ChestPlant();
            }
            return instance;
        }
    }
}
