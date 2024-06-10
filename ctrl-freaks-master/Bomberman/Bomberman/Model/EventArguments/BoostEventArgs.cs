using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.Model.EventArguments
{
    /// <summary>
    /// Event arguments for boost events
    /// </summary>
    public class BoostEventArgs: EventArgs
    {
        private int playerID;
        private string boostType;
        private bool isAdded;
        /// <summary>
        /// id of the player who got/lost the boost
        /// </summary>
        public int PlayerID { get => playerID; }
        /// <summary>
        /// type of the boost
        /// </summary>
        public string BoostType { get => boostType; }
        /// <summary>
        /// true if the boost was added, false if it was removed
        /// </summary>
        public bool IsAdded { get => isAdded; }
        /// <summary>
        /// constructor for BoostEventArgs
        /// </summary>
        /// <param name="playerID">id of the player who got/lost the boost</param>
        /// <param name="boostType">type of the boost</param>
        /// <param name="isAdded">true if the boost was added, false if it was removed</param>
        public BoostEventArgs(int playerID, string boostType, bool isAdded)
        {
            this.playerID = playerID;
            this.boostType = boostType;
            this.isAdded = isAdded;
        }


    }
}
