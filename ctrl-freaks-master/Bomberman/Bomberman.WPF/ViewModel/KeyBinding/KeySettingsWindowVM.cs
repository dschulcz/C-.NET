using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Bomberman.Model;
using Bomberman.Model.KeySettings;

namespace Bomberman.WPF.ViewModel.KeyBinding
{
    public class KeySettingsWindowVM:ViewModelBase
    {
        /// <summary>
        /// state of the key settings window
        /// </summary>
        public enum KeySettingsWindowState
        {
            Idle, SettingKey, KeyAlreadyInUseMessage
        }
        #region Fields
        private KeySettingsModel _keySettingsModel = null!;
        private KeySettingsWindowState _state;
        private int _playerIDToChange;
        private Direction _directionToChange;
        #endregion
        #region Properties
        /// <summary>
        /// Delegate command for setting the keybindings to default
        /// </summary>
        public DelegateCommand SetToDefaultCommand { get; private set; }
        /// <summary>
        /// Delegate command for returning to the main menu
        /// </summary>
        public DelegateCommand ReturnToMainMenuCommand { get; private set; }
        

        /// <summary>
        /// Displays a small text that prompts the user to press a key.
        /// Can also display a message if the key is already in use
        /// </summary>
        
        public string State
        {
            get
            {
                switch (_state)
                {
                    case KeySettingsWindowState.Idle:
                        return "Press a button to change the key binding";
                    case KeySettingsWindowState.SettingKey:
                        return "Press a key";
                    case KeySettingsWindowState.KeyAlreadyInUseMessage:
                        return "Key is already in use";
                    default:
                        throw new Exception("Invalid state");
                }
            }
        }
        /// <summary>
        /// The keybindings of the players
        /// </summary>
        public ObservableCollection<KeyBindingData> KeyBindings { get; set; }
        #endregion
        #region Events
        /// <summary>
        /// Event for setting the keybindings to default
        /// </summary>
        public event EventHandler? ReturnToMainMenu;
        #endregion
        #region Constructors
        /// <summary>
        /// constructor for the key settings window viewmodel
        /// </summary>
        /// <param name="keySettingsModel"></param>
        public KeySettingsWindowVM(KeySettingsModel keySettingsModel)
        {
            _keySettingsModel = keySettingsModel;
            _state = KeySettingsWindowState.Idle;
            KeyBindings = null!;

            _keySettingsModel.KeySettingsChanged += new EventHandler<KeySettingsEventArgs>(KeySettingsModel_KeySettingsChanged);

            GenerateTable();
            SetToDefaultCommand = new DelegateCommand(param => OnSetToDefault());
            ReturnToMainMenuCommand = new DelegateCommand(param => OnReturnToMainMenu());
            

        }
        #endregion
        #region Public methods
        /*public void SetState(KeySettingsWindowState state)
        {
            _state = state;
        }*/
        /// <summary>
        /// reads the key that the user pressed and changes the keybinding accordingly
        /// </summary>
        /// <param name="e"></param>

        public void WindowKeyDown(KeyEventArgs e)
        {
            if (_state == KeySettingsWindowState.SettingKey)
            {
                _state = KeySettingsWindowState.Idle;
                OnPropertyChanged(nameof(State));
                if(!_keySettingsModel.ChangeKeyBinding(_playerIDToChange, e.Key.ToString(), _directionToChange))
                {
                    _state = KeySettingsWindowState.KeyAlreadyInUseMessage;
                    OnPropertyChanged(nameof(State));
                }

            }
            else
            {
                Debug.WriteLine($"WindowKeyDown called when state is not SettingKey");
            }

        }
        #endregion
        #region Private methods
        /// <summary>
        /// assumes that the keybindings are for 3 players
        /// </summary>
        /// <param name="keyBindings"></param>
        private void GenerateTable()
        {
            List<Dictionary<String, Direction>> keyBindings = _keySettingsModel.PlayerKeyBindings;
            KeyBindings = new ObservableCollection<KeyBindingData>();
            for(int i=0; i < 3; i++)
            {
                string up = keyBindings[i].FirstOrDefault(x => x.Value == Direction.Up).Key;
                string down = keyBindings[i].FirstOrDefault(x => x.Value == Direction.Down).Key;
                string left = keyBindings[i].FirstOrDefault(x => x.Value == Direction.Left).Key;
                string right = keyBindings[i].FirstOrDefault(x => x.Value == Direction.Right).Key;
                string plantBomb = keyBindings[i].FirstOrDefault(x => x.Value == Direction.PlantBomb).Key;
                string detonateBomb = keyBindings[i].FirstOrDefault(x => x.Value == Direction.PlantChest).Key;
                KeyBindings.Add(new KeyBindingData(i, up, down, left, right, plantBomb, detonateBomb));
                //god knows why I have to do this
                //TODO: why? if I use i directly in the lambda expression, it will always be 3 for some reason
                int idx = i;
                KeyBindings[i].UpClickCommand = new DelegateCommand(param => KeyMightChange(idx, Direction.Up));
                KeyBindings[i].DownClickCommand = new DelegateCommand(param => KeyMightChange(idx, Direction.Down));
                KeyBindings[i].LeftClickCommand = new DelegateCommand(param => KeyMightChange(idx, Direction.Left));
                KeyBindings[i].RightClickCommand = new DelegateCommand(param => KeyMightChange(idx, Direction.Right));
                KeyBindings[i].PlantBombClickCommand = new DelegateCommand(param => KeyMightChange(idx, Direction.PlantBomb));
                KeyBindings[i].DetonateBombClickCommand = new DelegateCommand(param => KeyMightChange(idx, Direction.PlantChest));
                //Debug.WriteLine($"{i}");

            }
            OnPropertyChanged(nameof(KeyBindings));
        }
        private void RefreshTableAt(int playerID, string newKey, Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    KeyBindings[playerID].Up = newKey;
                    break;
                case Direction.Down:
                    KeyBindings[playerID].Down = newKey;
                    break;
                case Direction.Left:
                    KeyBindings[playerID].Left = newKey;
                    break;
                case Direction.Right:
                    KeyBindings[playerID].Right = newKey;
                    break;
                case Direction.PlantBomb:
                    KeyBindings[playerID].PlantBomb = newKey;
                    break;
                case Direction.PlantChest:
                    KeyBindings[playerID].DetonateBomb = newKey;
                    break;
                default:
                    throw new Exception("Invalid direction");
            }
            OnPropertyChanged(nameof(KeyBindings));
        }
        private void KeyMightChange(int playerID, Direction direction)
        {
            Debug.WriteLine($"KeyMightChange called with playerID: {playerID}, direction: {direction}");
            _state = KeySettingsWindowState.SettingKey;
            OnPropertyChanged(nameof(State));
            _playerIDToChange = playerID;
            _directionToChange = direction;
        }
        #endregion
        #region Event handlers
        /// <summary>
        /// event handler for when the key settings change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void KeySettingsModel_KeySettingsChanged(object? sender, KeySettingsEventArgs e)
        {
            RefreshTableAt(e.PlayerID, e.NewKey, e.Direction);
        }
        /// <summary>
        /// event handler for when the user wants to set the keybindings to default
        /// </summary>
        public void OnSetToDefault()
        {
            _state = KeySettingsWindowState.Idle;
            OnPropertyChanged(nameof(State));
            _keySettingsModel.SetToDefault();
            GenerateTable();
            //SetToDefault?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// event handler for when the user wants to return to the main menu
        /// </summary>
        public void OnReturnToMainMenu()
        {
            ReturnToMainMenu?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }

}
