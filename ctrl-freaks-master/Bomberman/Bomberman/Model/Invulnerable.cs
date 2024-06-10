using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.Model
{
    /// <summary>
    /// Represents an invulnerable boost in the game.
    /// </summary>
    public class Invulnerable : Boost
    {
        private static Invulnerable? instance = null;

        private Invulnerable()
        { }

        /// <summary>
        /// Destroys the invulnerable boost.
        /// </summary>
        public void Destroy() { }

        /// <summary>
        /// Gets the instance of the invulnerable boost.
        /// </summary>
        /// <returns>The instance of the invulnerable boost.</returns>
        public static Invulnerable GetInstance()
        {
            if (instance == null)
            {
                instance = new Invulnerable();
            }
            return instance;
        }
    }
}
