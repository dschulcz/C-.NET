using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bomberman.Model;

namespace Bomberman.WPF.ViewModel
{
    public class PlayerData:ViewModelBase
    {
        #region Fields
        private int x;
        private int y;
        private bool alive;
        private int winCounter;

        //non-timer related boosts
        private int speed;
        private int bombCapacity;
        private int detonationSize;
        //it seems skate is permanently active, so no need for a field
        //private bool skateActive;
        private bool detonatorActive;
        private int chestPlantCounter;

        //timer related boosts
        private bool ghostActive;
        private bool invulnerableActive;
        private bool bombPlantProblemActive;
        private bool bombSizeDownActive;
        private bool slowActive;
        private bool instantPlantActive;

        private string? lastPickedBoost;
        #endregion
        #region Properties
        /// <summary>
        /// x coordinate of the player
        /// </summary>
        public int X {
            get {  return x; }
            set
            {
                x = value;
                OnPropertyChanged(nameof(Value));
            }
        }
        /// <summary>
        /// y coordinate of the player
        /// </summary>
        public int Y
        {
            get { return y; }
            set
            {
                y = value;
                OnPropertyChanged(nameof(Value));
            }
        }
        /// <summary>
        /// id of the player
        /// </summary>

        public int ID { get; set; }
        /// <summary>
        /// alive status of the player
        /// </summary>
        public bool Alive
        {
            get { return alive; }
            set
            {
                alive = value;
                OnPropertyChanged(nameof(Value));
            }
        }
        /// <summary>
        /// win counter of the player
        /// </summary>
        public int WinCounter
        {
            get { return winCounter; }
            set
            {
                winCounter = value;
                OnPropertyChanged(nameof(Value));
            }
        }
        //non-timer related boosts
        /// <summary>
        /// speed of the player
        /// </summary>
        public int Speed
        {
            get { return speed; }
            set
            {
                speed = value;
                OnPropertyChanged(nameof(Value));
            }
        }
        /// <summary>
        /// bomb capacity of the player
        /// </summary>
        public int BombCapacity
        {
            get { return bombCapacity; }
            set
            {
                bombCapacity = value;
                OnPropertyChanged(nameof(Value));
            }
        }
        /// <summary>
        /// detonation size of the player
        /// </summary>
        public int DetonationSize
        {
            get { return detonationSize; }
            set
            {
                detonationSize = value;
                OnPropertyChanged(nameof(Value));
            }
        }
        /// <summary>
        /// detonator status of the player
        /// </summary>
        public bool DetonatorActive
        {
            get { return detonatorActive; }
            set
            {
                detonatorActive = value;
                OnPropertyChanged(nameof(Value));
            }
        }
        /// <summary>
        /// chest plant counter of the player
        /// </summary>
        public int ChestPlantCounter
        {
            get { return chestPlantCounter; }
            set
            {
                chestPlantCounter = value;
                OnPropertyChanged(nameof(Value));
            }
        }
        //timer related boosts
        /// <summary>
        /// ghost status of the player
        /// </summary>
        public bool GhostActive
        {
            get { return ghostActive; }
            set
            {
                ghostActive = value;
                OnPropertyChanged(nameof(Value));
            }
        }
        /// <summary>
        /// invulnerable status of the player
        /// </summary>
        public bool InvulnerableActive
        {
            get { return invulnerableActive; }
            set
            {
                invulnerableActive = value;
                OnPropertyChanged(nameof(Value));
            }
        }
        /// <summary>
        /// bomb plant problem status of the player
        /// </summary>
        public bool BombPlantProblemActive
        {
            get { return bombPlantProblemActive; }
            set
            {
                bombPlantProblemActive = value;
                OnPropertyChanged(nameof(Value));
            }
        }
        /// <summary>
        /// slow status of the player
        /// </summary>
        public bool SlowActive
        {
            get { return slowActive; }
            set
            {
                slowActive = value;
                OnPropertyChanged(nameof(Value));
            }
        }
        /// <summary>
        /// bomb size down status of the player
        /// </summary>
        public bool BombSizeDownActive
        {
            get { return bombSizeDownActive; }
            set
            {
                bombSizeDownActive = value;
                OnPropertyChanged(nameof(Value));
            }
        }
        /// <summary>
        /// instant plant status of the player
        /// </summary>
        public bool InstantPlantActive
        {
            get { return instantPlantActive; }
            set
            {
                instantPlantActive = value;
                OnPropertyChanged(nameof(Value));
            }
        }
        /// <summary>
        /// last picked boost of the player
        /// </summary>
        public string? LastPickedBoost
        {
            get { return lastPickedBoost; }
            set
            {
                lastPickedBoost = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        #endregion
        /// <summary>
        /// constructor for the player data
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="ID"></param>
        /// <param name="alive"></param>
        public PlayerData(int x, int y, int ID, bool alive) 
        { 
            this.x = x;
            this.y = y;
            this.ID = ID;
            this.alive = alive;
        }
        /// <summary>
        /// value displayed in a textblock
        /// </summary>
        public String Value
        {
            get
            {
                StringBuilder result = new StringBuilder();
                result.Append($"Status of player {ID+1}: {(Alive ? "Alive" : "Dead")}\n");
                result.Append($"Coordinates: X:{X}, Y:{Y}\n");
                result.Append($"Wins: {WinCounter}\n");
                //not documented, but speed actually is the cooldown between moves
                result.Append($"Move interval: {Speed}\n");
                result.Append($"BombCapacity: {BombCapacity}\n");
                result.Append($"Detonation size: {DetonationSize}\n");
                result.Append($"Detonator: {(DetonatorActive ? "Yes" : "No")}\n");
                result.Append($"Max chests plantable: {ChestPlantCounter}\n");

                StringBuilder timerBoosts = new StringBuilder();
                if (GhostActive) timerBoosts.Append("Ghost, ");
                if (InvulnerableActive) timerBoosts.Append("Invulnerable, ");
                if (BombPlantProblemActive) timerBoosts.Append("Bomb Plant Problem, ");
                if (SlowActive) timerBoosts.Append("Slow, ");
                if (BombSizeDownActive) timerBoosts.Append("Bomb Size Down, ");
                if (InstantPlantActive) timerBoosts.Append("Instant Plant, ");
                //remove trailing comma and space
                if (timerBoosts.Length > 0) timerBoosts.Remove(timerBoosts.Length - 2, 2);

                result.Append(timerBoosts.Length > 0 ? timerBoosts + "\n" : "");

                result.Append($"Last picked boost: {LastPickedBoost}\n");
                
                return result.ToString();
            }

        }
    }

    
}
