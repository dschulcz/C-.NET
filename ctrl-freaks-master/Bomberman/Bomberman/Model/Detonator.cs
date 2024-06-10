using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.Model
{
    /// <summary>
    /// Represents a detonator boost in the Bomberman game.
    /// </summary>
    public class Detonator : Boost
    {
        private static Detonator? instance = null;

        private Detonator()
        { }

        /// <summary>
        /// Destroys the detonator.
        /// </summary>
        public void Destroy() { }

        /// <summary>
        /// Gets the instance of the detonator.
        /// </summary>
        /// <returns>The instance of the detonator.</returns>
        public static Detonator GetInstance()
        {
            if (instance == null)
            {
                instance = new Detonator();
            }
            return instance;
        }
    }
}
