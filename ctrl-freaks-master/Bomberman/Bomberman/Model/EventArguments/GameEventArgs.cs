using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.Model.EventArguments
{
    /// <summary>
    /// event args type common game events, such as fieldchanged, roundover, gameover
    /// </summary>
    public class GameEventArgs: EventArgs
    {
        private String _name;
        private Boolean _roundWon;
        private Boolean _gameWon;

        /// <summary>
        /// Name/ID of the player who won the round/game
        /// </summary>
        public String Name { get { return _name; } }
        /// <summary>
        /// True if invoked when a round is over
        /// </summary>
        public Boolean RoundWon { get { return _roundWon; } }
        /// <summary>
        /// True if invoked when a game is over
        /// </summary>
        public Boolean GameWon { get { return _gameWon; } }
        /// <summary>
        /// constructor for GameEventArgs
        /// </summary>
        /// <param name="roundWon">Name/ID of the player who won the round/game</param>
        /// <param name="gameWon">True if invoked when a round is over</param>
        /// <param name="playerName">True if invoked when a game is over</param>
        public GameEventArgs(bool roundWon, bool gameWon, string playerName)
        {
            _roundWon = roundWon;
            _gameWon = gameWon;
            _name = playerName;
        }
    }
}
