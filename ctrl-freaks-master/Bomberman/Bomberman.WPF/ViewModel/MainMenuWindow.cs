using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.WPF.ViewModel
{
    internal class MainMenuWindow : ViewModelBase
    {

        #region Events
        /// <summary>
        /// event for starting a new game
        /// </summary>
        public event EventHandler? NewGame;
        /// <summary>
        /// event for opening the options
        /// </summary>
        public event EventHandler? Options;
        /// <summary>
        /// event for opening the editor
        /// </summary>
        public event EventHandler? Editor;
        #endregion

        #region Properties
        /// <summary>
        /// delegate command for starting a new game
        /// </summary>
        public DelegateCommand NewGameCommand { get; private set; }
        /// <summary>
        /// delegate command for opening the options
        /// </summary>
        public DelegateCommand OptionsCommand { get; private set; }
        /// <summary>
        /// delegate command for opening the editor
        /// </summary>
        public DelegateCommand EditorCommand { get; private set; }

        #endregion
        /// <summary>
        /// constructor for the main menu window
        /// </summary>
        public MainMenuWindow()
        {
            NewGameCommand = new DelegateCommand(param => OnNewGame());
            OptionsCommand = new DelegateCommand(param => OnOptions());
            EditorCommand = new DelegateCommand(param => OnEditor());
        }
        /// <summary>
        /// event invoker for starting a new game
        /// </summary>
        public void OnNewGame()
        {
            NewGame?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// event invoker for opening the options
        /// </summary>

        public void OnOptions()
        {
            Options?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// event invoker for opening the editor
        /// </summary>

        public void OnEditor()
        {
            Editor?.Invoke(this, EventArgs.Empty);
        }
    }
}
