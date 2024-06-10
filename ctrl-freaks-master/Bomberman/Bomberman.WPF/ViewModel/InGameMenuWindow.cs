using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.WPF.ViewModel
{
    delegate void Event();

    internal class InGameMenuWindow : ViewModelBase
    {

        #region Events
        /// <summary>
        /// event for pausing the game
        /// </summary>
        public event EventHandler? PauseGame;
        /// <summary>
        /// event for resuming the game
        /// </summary>
        public event EventHandler? ResumeGame;
        /// <summary>
        /// event for starting a new game
        /// </summary>
        public event EventHandler? NewGame;
        /// <summary>
        /// event for saving the game
        /// </summary>
        public event EventHandler? SaveGame;
        /// <summary>
        /// event for loading a game
        /// </summary>
        public event EventHandler? LoadGame;
        /// <summary>
        /// event for exiting the game
        /// </summary>
        public event EventHandler? ExitGame;
        #endregion

        #region Properties
        /// <summary>
        /// delegate command for pausing the game
        /// </summary>
        public DelegateCommand PauseGameCommand { get; private set; }
        /// <summary>
        /// delegate command for resuming the game
        /// </summary>
        public DelegateCommand ResumeGameCommand { get; private set; }
        /// <summary>
        /// delegate command for starting a new game
        /// </summary>
        public DelegateCommand NewGameCommand { get; private set; }
        /// <summary>
        /// delegate command for saving the game
        /// </summary>
        public DelegateCommand SaveGameCommand { get; private set; }
        /// <summary>
        /// delegate command for loading a game
        /// </summary>
        public DelegateCommand LoadGameCommand { get; private set; }
        /// <summary>
        /// delegate command for exiting the game
        /// </summary>
        public DelegateCommand ExitGameCommand { get; private set; }
        #endregion
        /// <summary>
        /// constructor for the in game menu window
        /// </summary>
        public InGameMenuWindow()
        {
            PauseGameCommand = new DelegateCommand(param => OnPauseGame());
            ResumeGameCommand = new DelegateCommand(param => OnResumeGame());
            NewGameCommand = new DelegateCommand(param => OnNewGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            ExitGameCommand = new DelegateCommand(param => OnExitGame());
        }
        /// <summary>
        /// invokes the event for pausing the game
        /// </summary>
        public void OnPauseGame()
        {
            PauseGame?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// invokes the event for resuming the game
        /// </summary>
        public void OnResumeGame()
        {
            ResumeGame?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// invokes the event for starting a new game
        /// </summary>
        public void OnNewGame()
        {
            NewGame?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// invokes the event for saving the game
        /// </summary>
        public void OnSaveGame()
        {
            SaveGame?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// invokes the event for loading a game
        /// </summary>
        public void OnLoadGame()
        {
            LoadGame?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// invokes the event for exiting the game
        /// </summary>
        public void OnExitGame()
        {
            ExitGame?.Invoke(this, EventArgs.Empty);
        }
    }
}
