using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.Model
{
    /// <summary>
    /// Represents a Bomb Counter Up boost.
    /// </summary>
    public class BombCounterUp : Boost
    {
        private static BombCounterUp? instance = null;

        private BombCounterUp()
        { }

        /// <summary>
        /// Destroys the Bomb Counter Up boost.
        /// </summary>
        public void Destroy() { }

        /// <summary>
        /// Gets the instance of the Bomb Counter Up boost.
        /// </summary>
        /// <returns>The instance of the Bomb Counter Up boost.</returns>
        public static BombCounterUp GetInstance()
        {
            if (instance == null)
            {
                instance = new BombCounterUp();
            }
            return instance;
        }
    }
}
