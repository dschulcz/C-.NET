using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Bomberman.WPF.ViewModel
{
    public class NewGameSettingsWindowVM:ViewModelBase
    {
        #region Fields
        /// <summary>
        /// path to the maps
        /// </summary>
        private readonly string MapPath = "../../../../BomberMan/Persistence/Maps";
        #endregion
        #region Properties
        /// <summary>
        /// number of players
        /// </summary>
        public int NumberOfPlayers { get; private set; }
        /// <summary>
        /// list of maps
        /// </summary>
        public ObservableCollection<string> Maps { get; private set; }
        /// <summary>
        /// selected map
        /// </summary>
        public string SelectedMap { get; set; }
        /// <summary>
        /// number of rounds
        /// </summary>
        public int NumberOfRounds { get; set; }
        /// <summary>
        /// number of players as string
        /// </summary>
        public string NumberOfPlayersString
        {
            get { return NumberOfPlayers.ToString(); }
            set
            {
                if (int.TryParse(value, out int result))
                {
                    NumberOfPlayers = result;
                }
                else
                {
                    NumberOfPlayers = 0;
                }
                OnPropertyChanged(nameof(NumberOfPlayers));
                OnPropertyChanged(nameof(NumberOfPlayersString));
            }
        }
        /// <summary>
        /// number of rounds as string
        /// </summary>
        public string NumberOfRoundsString
        {
            get { return NumberOfRounds.ToString(); }
            set
            {
                if (int.TryParse(value, out int result))
                {
                    NumberOfRounds = result;
                }
                else
                {
                    NumberOfRounds = 0;
                }
                OnPropertyChanged(nameof(NumberOfRounds));
                OnPropertyChanged(nameof(NumberOfRoundsString));
            }
        }
        /// <summary>
        /// error message for number of players
        /// </summary>
        public string NumberOfPlayersError { get; set; }
        /// <summary>
        /// error message for number of rounds
        /// </summary>
        public string NumberOfRoundsError { get; set; }
        /// <summary>
        /// delegate command for returning to main menu
        /// </summary>
        public DelegateCommand ReturnToMainMenuCommand { get; private set; }
        /// <summary>
        /// delegate command for starting the game
        /// </summary>
        public DelegateCommand StartGameCommand { get; private set; }

        #endregion
        #region Events
        /// <summary>
        /// event for returning to the main menu
        /// </summary>
        public event EventHandler? ReturnToMainMenu;
        /// <summary>
        /// event for starting the game
        /// </summary>
        public event EventHandler? StartGame;
        #endregion
        #region Constructors
        /// <summary>
        /// constructor for the new game settings window
        /// </summary>
        public NewGameSettingsWindowVM()
        {
            //default values
            NumberOfPlayers = 2;
            Maps = FindMaps();
            SelectedMap = Maps[0];
            NumberOfRounds = 3;

            NumberOfPlayersError = "";
            NumberOfRoundsError = "";

            ReturnToMainMenuCommand = new DelegateCommand(param => OnReturnToMainMenu());
            StartGameCommand = new DelegateCommand(param => OnStartGame());
        }
        #endregion
        #region Public methods
        /// <summary>
        /// sets the number of players
        /// </summary>
        /// <param name="number"></param>
        public void SetNumberOfPlayers(int number)
        {
            NumberOfPlayers = number;
            OnPropertyChanged(nameof(NumberOfPlayers));
            OnPropertyChanged(nameof(NumberOfPlayersString));
        }
        /// <summary>
        /// chooses a random map
        /// </summary>
        public void ChooseRandomMap()
        {
            Random random = new Random();
            int index = random.Next(0, Maps.Count);
            SelectedMap = Maps[index];
            OnPropertyChanged(nameof(SelectedMap));
        }
        #endregion

        #region Private methods
        private ObservableCollection<string> FindMaps()
        {
            ObservableCollection<string> maps = new ObservableCollection<string>();
            string[] files = Directory.GetFiles(MapPath, "*.txt");
            if(files.Length == 0)
            {
                throw new Exception("No maps found");
            }
            else
            {
                foreach(string file in files)
                {
                    maps.Add(Path.GetFileNameWithoutExtension(file));
                }
            }
            return maps;
        }
        #endregion
        #region Event invokers
        private void OnReturnToMainMenu()
        {
            ReturnToMainMenu?.Invoke(this, EventArgs.Empty);
        }

        private void OnStartGame()
        {
            bool valid = true;
            if(NumberOfPlayers < 2 || NumberOfPlayers > 3)
            {
                NumberOfPlayersError = "Number of players must be between 2 and 3";
                OnPropertyChanged(nameof(NumberOfPlayersError));
                valid = false;
            }
            else
            {
                NumberOfPlayersError = "";
                OnPropertyChanged(nameof(NumberOfPlayersError));
            }
            if(NumberOfRounds < 1 || NumberOfRounds > 5)
            {
                NumberOfRoundsError = "Number of rounds must be between 1 and 5";
                OnPropertyChanged(nameof(NumberOfRoundsError));
                valid = false;
            }
            else
            {
                NumberOfRoundsError = "";
                OnPropertyChanged(nameof(NumberOfRoundsError));
            }
            if(valid)
            {
                StartGame?.Invoke(this, EventArgs.Empty);
            }
            
        }
        #endregion
    }
}
