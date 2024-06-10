using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.Model
{
    /// <summary>
    /// Every element that can be placed on the field in the Bomberman game.
    /// </summary>
    public interface Field
    {
        /// <summary>
        /// destroys the field.
        /// </summary>
        public void Destroy();
    }
}
