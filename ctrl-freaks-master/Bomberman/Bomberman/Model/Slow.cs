using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.Model
{
    /// <summary>
    /// Represents a Slow boost in the game.
    /// </summary>
    public class Slow : Boost
    {
        private static Slow? instance = null;

        private Slow()
        { }

        /// <summary>
        /// Destroys the Slow boost.
        /// </summary>
        public void Destroy() { }

        /// <summary>
        /// Gets the instance of the Slow boost.
        /// </summary>
        /// <returns>The instance of the Slow boost.</returns>
        public static Slow GetInstance()
        {
            if (instance == null)
            {
                instance = new Slow();
            }
            return instance;
        }
    }
}
