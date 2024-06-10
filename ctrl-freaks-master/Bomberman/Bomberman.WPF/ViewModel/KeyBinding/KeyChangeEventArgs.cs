using Bomberman.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.WPF.ViewModel.KeyBinding
{
    

    ///<summary>
    /// Represents the event arguments for a key change event.
    /// </summary>
    public class KeyChangeEventArgs : EventArgs
    {
        private int _id;
        private Direction _direction;

        /// <summary>
        /// Gets the ID associated with the key change event.
        /// </summary>
        public int ID { get { return _id; } }

        /// <summary>
        /// Gets the direction associated with the key change event.
        /// </summary>
        /// <returns>The direction associated with the key change event.</returns>
        public Direction GetDirection() { return _direction; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyChangeEventArgs"/> class.
        /// </summary>
        /// <param name="id">The ID associated with the key change event.</param>
        /// <param name="direction">The direction associated with the key change event.</param>
        public KeyChangeEventArgs(int id, Direction direction)
        {
            _id = id;
            _direction = direction;
        }
    }
}
