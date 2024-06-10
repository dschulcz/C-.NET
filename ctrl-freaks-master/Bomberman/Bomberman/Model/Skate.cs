using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.Model
{
    /// <summary>
    /// Represents a Skate boost in the Bomberman game.
    /// </summary>
    public class Skate : Boost
    {
        private static Skate? instance = null;

        private Skate()
        { }

        /// <summary>
        /// Destroys the Skate boost.
        /// </summary>
        public void Destroy() { }

        /// <summary>
        /// Gets the instance of the Skate boost.
        /// </summary>
        /// <returns>The instance of the Skate boost.</returns>
        public static Skate GetInstance()
        {
            if (instance == null)
            {
                instance = new Skate();
            }
            return instance;
        }
    }
}
