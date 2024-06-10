using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.Model.EventArguments
{
    /// <summary>
    /// EventArgs used for movement, which typically means two fields are changed
    /// </summary>
    public class MovementEventArgs: EventArgs
    {
        private int _oldX;
        private int _oldY;
        private int _newX;
        private int _newY;
        /// <summary>
        /// Original x position
        /// </summary>
        public int OldX { get { return _oldX; } }
        /// <summary>
        /// Original y position
        /// </summary>
        public int OldY { get { return _oldY; } }
        /// <summary>
        /// New x position
        /// </summary>
        public int NewX { get { return _newX; } }
        /// <summary>
        /// New y position
        /// </summary>
        public int NewY { get { return _newY;} }
        /// <summary>
        /// constructor for MovementEventArgs
        /// </summary>
        /// <param name="oldX">Original x position</param>
        /// <param name="oldY">Original y position</param>
        /// <param name="newX">New x position</param>
        /// <param name="newY">New y position</param>
        public MovementEventArgs(int oldX, int oldY, int newX, int newY)
        {
            _oldX = oldX;
            _oldY = oldY;
            _newX = newX;
            _newY = newY;
        }

    }
}
