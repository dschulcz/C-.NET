using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.Model
{
    /// <summary>
    /// Represents an explosion in the game.
    /// </summary>
    public class Explosion : Field
    {
        private static Explosion? instance = null;

        private Explosion()
        { }

        /// <summary>
        /// Destroys the explosion.
        /// </summary>
        public void Destroy() { }

        /// <summary>
        /// Gets the instance of the explosion.
        /// </summary>
        /// <returns>The instance of the explosion.</returns>
        public static Explosion GetInstance()
        {
            if (instance == null)
            {
                instance = new Explosion();
            }
            return instance;
        }
    }
}
