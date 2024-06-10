using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.Model.KeySettings
{
    /// <summary>
    /// Event arguments for key settings events
    /// </summary>
    public class KeySettingsEventArgs: EventArgs
    {
        private int playerID;
        private string newKey;
        private Direction direction;
        /// <summary>
        /// id of the player whose key is being changed
        /// </summary>
        public int PlayerID { get { return playerID; } }
        /// <summary>
        /// new key for the player
        /// </summary>
        public string NewKey { get { return newKey; } }
        /// <summary>
        /// direction for which the key is being changed
        /// </summary>
        public Direction Direction { get { return direction; } }
        /// <summary>
        /// constructor for KeySettingsEventArgs
        /// </summary>
        /// <param name="playerID">id of the player whose key is being changed</param>
        /// <param name="newKey">new key for the player</param>
        /// <param name="direction">direction for which the key is being changed</param>
        public KeySettingsEventArgs(int playerID, string newKey,Direction direction)
        {
            this.playerID = playerID;
            this.newKey = newKey;
            this.direction = direction;

        }
    }
}
