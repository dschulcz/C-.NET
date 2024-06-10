using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.Model.KeySettings
{
    public class KeySettingsModel
    {
        #region Fields
        /// <summary>
        /// every players' key bindings, each player has a dictionary with keys and directions
        /// </summary>
        private List<Dictionary<string, Direction>> playerKeyBindings;
        #endregion
        #region Properties
        /// <summary>
        /// every players' key bindings, each player has a dictionary with keys and directions
        /// </summary>
        public List<Dictionary<string, Direction>> PlayerKeyBindings
        {
            get { return playerKeyBindings; }
            private set { playerKeyBindings = value; }
        }
        #endregion
        #region Events
        /// <summary>
        /// event for when key settings are changed
        /// </summary>
        public event EventHandler<KeySettingsEventArgs>? KeySettingsChanged;
        #endregion
        #region Constructors
        /// <summary>
        /// constructor for KeySettingsModel with default key bindings
        /// </summary>
        public KeySettingsModel()
        {
            playerKeyBindings = new List<Dictionary<string, Direction>>();
            Dictionary<String, Direction> dic = new Dictionary<string, Direction>();
            dic.Add("W", Direction.Up);
            dic.Add("A", Direction.Left);
            dic.Add("S", Direction.Down);
            dic.Add("D", Direction.Right);
            dic.Add("Q", Direction.PlantBomb);
            dic.Add("E", Direction.PlantChest);
            playerKeyBindings.Add(dic);

            dic = new Dictionary<string, Direction>();
            dic.Add("Up", Direction.Up);
            dic.Add("Left", Direction.Left);
            dic.Add("Down", Direction.Down);
            dic.Add("Right", Direction.Right);
            dic.Add("M", Direction.PlantBomb);
            dic.Add("N", Direction.PlantChest);
            playerKeyBindings.Add(dic);

            dic = new Dictionary<string, Direction>();
            dic.Add("NumPad8", Direction.Up);
            dic.Add("NumPad4", Direction.Left);
            dic.Add("NumPad5", Direction.Down);
            dic.Add("NumPad6", Direction.Right);
            dic.Add("NumPad7", Direction.PlantBomb);
            dic.Add("NumPad9", Direction.PlantChest);
            playerKeyBindings.Add(dic);
        }
        #endregion
        #region Public methods
        /// <summary>
        /// Changes the key binding for a player if there are no conflicts
        /// </summary>
        /// <param name="playerIndex">which player</param>
        /// <param name="key">new key to check</param>
        /// <param name="direction">direction to check</param>
        /// <returns>true if no conflics, false if yes</returns>
        public bool ChangeKeyBinding(int playerIndex, string key, Direction direction)
        {
            if (IsKeyAlreadyBound(key))
            {
                Debug.WriteLine($"Key {key} is already bound");
                return false;
            }
            //remove old key binding
            string oldkey = playerKeyBindings[playerIndex].FirstOrDefault(x => x.Value == direction).Key;
            playerKeyBindings[playerIndex].Remove(oldkey);
            //add new key binding
            playerKeyBindings[playerIndex][key] = direction;
            OnKeySettingsChanged(playerIndex, key, direction);
            return true;
        }
        /// <summary>
        /// Sets the key bindings to default
        /// </summary>
        public void SetToDefault()
        {
            playerKeyBindings[0].Clear();
            playerKeyBindings[0]["W"] = Direction.Up;
            playerKeyBindings[0]["A"] = Direction.Left;
            playerKeyBindings[0]["S"] = Direction.Down;
            playerKeyBindings[0]["D"] = Direction.Right;
            playerKeyBindings[0]["Q"] = Direction.PlantBomb;
            playerKeyBindings[0]["E"] = Direction.PlantChest;
            OnKeySettingsChanged(0, "W", Direction.Up);
            OnKeySettingsChanged(0, "A", Direction.Left);
            OnKeySettingsChanged(0, "S", Direction.Down);
            OnKeySettingsChanged(0, "D", Direction.Right);
            OnKeySettingsChanged(0, "Q", Direction.PlantBomb);
            OnKeySettingsChanged(0, "E", Direction.PlantChest);

            playerKeyBindings[1].Clear();
            playerKeyBindings[1]["Up"] = Direction.Up;
            playerKeyBindings[1]["Left"] = Direction.Left;
            playerKeyBindings[1]["Down"] = Direction.Down;
            playerKeyBindings[1]["Right"] = Direction.Right;
            playerKeyBindings[1]["M"] = Direction.PlantBomb;
            playerKeyBindings[1]["N"] = Direction.PlantChest;
            OnKeySettingsChanged(1, "Up", Direction.Up);
            OnKeySettingsChanged(1, "Left", Direction.Left);
            OnKeySettingsChanged(1, "Down", Direction.Down);
            OnKeySettingsChanged(1, "Right", Direction.Right);
            OnKeySettingsChanged(1, "M", Direction.PlantBomb);
            OnKeySettingsChanged(1, "N", Direction.PlantChest);


            playerKeyBindings[2].Clear();
            playerKeyBindings[2]["NumPad8"] = Direction.Up;
            playerKeyBindings[2]["NumPad4"] = Direction.Left;
            playerKeyBindings[2]["NumPad5"] = Direction.Down;
            playerKeyBindings[2]["NumPad6"] = Direction.Right;
            playerKeyBindings[2]["NumPad7"] = Direction.PlantBomb;
            playerKeyBindings[2]["NumPad9"] = Direction.PlantChest;
            OnKeySettingsChanged(2, "NumPad8", Direction.Up);
            OnKeySettingsChanged(2, "NumPad4", Direction.Left);
            OnKeySettingsChanged(2, "NumPad5", Direction.Down);
            OnKeySettingsChanged(2, "NumPad6", Direction.Right);
            OnKeySettingsChanged(2, "NumPad7", Direction.PlantBomb);
            OnKeySettingsChanged(2, "NumPad9", Direction.PlantChest);


        }
        #endregion
        #region Private methods
        /// <summary>
        /// Checks if a key is already bound
        /// </summary>
        /// <param name="key">new key to check for</param>
        /// <returns>True if already bound, false otherwise</returns>
        private bool IsKeyAlreadyBound(string key)
        {
            //special case for space
            if (key == "Space")
            {
                return true;
            }
            foreach (var player in playerKeyBindings)
            {
                foreach (var playerKey in player)
                {
                    if (playerKey.Key == key)
                    {
                        return true;
                    }
                }
                
            }
            return false;
        }
        #endregion
        #region Private event methods
        /// <summary>
        /// event invoker for when key settings are changed
        /// </summary>
        /// <param name="playerIndex">id of player</param>
        /// <param name="key">new key</param>
        /// <param name="direction">which direction</param>
        private void OnKeySettingsChanged(int playerIndex, string key, Direction direction)
        {
            KeySettingsChanged?.Invoke(this, new KeySettingsEventArgs(playerIndex, key, direction));
        }
        #endregion
    }
}
