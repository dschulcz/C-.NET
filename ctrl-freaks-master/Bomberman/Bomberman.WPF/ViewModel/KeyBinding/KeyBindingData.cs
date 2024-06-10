using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bomberman.Model;

namespace Bomberman.WPF.ViewModel.KeyBinding
{
    /// <summary>
    /// Represents the keybindings of the player.
    /// </summary>
    public class KeyBindingData : ViewModelBase
    {
        #region Properties
        /// <summary>
        /// The id of the player whose keybindings we use.
        /// </summary>
        private int _id;
        /// <summary>
        /// actual keybindings
        /// </summary>
        private Dictionary<string, Direction> _keybindings;
        #endregion
        #region Constructor
        /// <summary>
        /// constructor for the keybindings in the viewmodel
        /// </summary>
        /// <param name="id"></param>
        /// <param name="up"></param>
        /// <param name="down"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="plantBomb"></param>
        /// <param name="detonateBomb"></param>
        public KeyBindingData(int id, string up, string down, string left, string right, string plantBomb, string detonateBomb)
        {
            _id = id;
            _keybindings = new Dictionary<string, Direction>
            {
                { up, Direction.Up },
                { down, Direction.Down },
                { left, Direction.Left },
                { right, Direction.Right },
                { plantBomb, Direction.PlantBomb },
                { detonateBomb, Direction.PlantChest }
            };

        }
        #endregion
        #region Fields
        /// <summary>
        /// The id of the player whose keybindings we use.
        /// </summary>
        public int ID
        {
            get
            {
                return _id;
            }
            private set
            {
                _id = value;
                OnPropertyChanged(nameof(ID));
            }
        }
        /// <summary>
        /// The keybinding for moving up.
        /// </summary>
        public string Up
        {
            get
            {
                return _keybindings.FirstOrDefault(x => x.Value == Direction.Up).Key;
            }
            set
            {
                _keybindings.Remove(Up);
                _keybindings.Add(value, Direction.Up);
                OnPropertyChanged(nameof(Up));
            }
        }
        /// <summary>
        /// The keybinding for moving down.
        /// </summary>
        public string Down
        {
            get
            {
                return _keybindings.FirstOrDefault(x => x.Value == Direction.Down).Key;
            }
            set
            {
                _keybindings.Remove(Down);
                _keybindings.Add(value, Direction.Down);
                OnPropertyChanged(nameof(Down));
            }
        }
        /// <summary>
        /// The keybinding for moving left.
        /// </summary>
        public string Left
        {
            get
            {
                return _keybindings.FirstOrDefault(x => x.Value == Direction.Left).Key;
            }
            set
            {
                _keybindings.Remove(Left);
                _keybindings.Add(value, Direction.Left);
                OnPropertyChanged(nameof(Left));
            }
        }
        /// <summary>
        /// The keybinding for moving right.
        /// </summary>
        public string Right
        {
            get
            {
                return _keybindings.FirstOrDefault(x => x.Value == Direction.Right).Key;
            }
            set
            {
                _keybindings.Remove(Right);
                _keybindings.Add(value, Direction.Right);
                OnPropertyChanged(nameof(Right));
            }
        }
        /// <summary>
        /// The keybinding for planting a bomb.
        /// </summary>
        public string PlantBomb
        {
            get
            {
                return _keybindings.FirstOrDefault(x => x.Value == Direction.PlantBomb).Key;
            }
            set
            {
                _keybindings.Remove(PlantBomb);
                _keybindings.Add(value, Direction.PlantBomb);
                OnPropertyChanged(nameof(PlantBomb));
            }
        }
        /// <summary>
        /// The keybinding for planting a chest!.
        /// </summary>
        public string DetonateBomb
        {
            get
            {
                return _keybindings.FirstOrDefault(x => x.Value == Direction.PlantChest).Key;
            }
            set
            {
                _keybindings.Remove(DetonateBomb);
                _keybindings.Add(value, Direction.PlantChest);
                OnPropertyChanged(nameof(DetonateBomb));
            }
        }
        /// <summary>
        /// Delegate commands for changing the keybindings for up
        /// </summary>
        public DelegateCommand? UpClickCommand { get; set; }
        /// <summary>
        /// Delegate commands for changing the keybindings for down
        /// </summary>
        public DelegateCommand? DownClickCommand { get; set; }
        /// <summary>
        /// Delegate commands for changing the keybindings for left
        /// </summary>

        public DelegateCommand? LeftClickCommand { get; set; }
        /// <summary>
        /// Delegate commands for changing the keybindings for right
        /// </summary>
        public DelegateCommand? RightClickCommand { get; set; }
        /// <summary>
        /// Delegate commands for changing the keybindings for planting a bomb
        /// </summary>
        public DelegateCommand? PlantBombClickCommand { get; set; }
        /// <summary>
        /// Delegate commands for changing the keybindings for planting a chest!
        /// </summary>
        public DelegateCommand? DetonateBombClickCommand { get; set;}

        #endregion
    }
}
