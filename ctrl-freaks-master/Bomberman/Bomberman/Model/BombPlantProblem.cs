using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.Model
{
    /// <summary>
    /// Represents a Bomb Plant Problem, which is a type of Boost.
    /// </summary>
    public class BombPlantProblem : Boost
    {
        private static BombPlantProblem? instance = null;

        private BombPlantProblem()
        {
        }

        /// <summary>
        /// Destroys the Bomb Plant Problem.
        /// </summary>
        public void Destroy() { }

        /// <summary>
        /// Gets the instance of the Bomb Plant Problem.
        /// </summary>
        /// <returns>The instance of the Bomb Plant Problem.</returns>
        public static BombPlantProblem GetInstance()
        {
            if (instance == null)
            {
                instance = new BombPlantProblem();
            }
            return instance;
        }
    }
}
