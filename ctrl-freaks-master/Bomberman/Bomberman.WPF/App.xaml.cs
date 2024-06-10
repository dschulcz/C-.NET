using System.Configuration;
using System.Data;
using System.Windows;
using Bomberman.Model;
using Bomberman.Persistence;
using Bomberman.WPF.View;
using Bomberman.WPF.ViewModel;
using Microsoft.Win32;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Input;
using System.Diagnostics;
using Bomberman.WPF.ViewModel.KeyBinding;
using Bomberman.Model.KeySettings;
using Bomberman.Model.EventArguments;
using System.IO;

namespace Bomberman.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Fields
        //hard-coded fields
        private int defaultMapSize = 10;
        private List<Field>[,]? startingMap;

        private readonly string PathPrefix = "../../../../BomberMan/Persistence/Maps";


        //non-hard coded fields
        private KeySettingsModel _keySettingsModel = null!;
        private Model.Model _model = null!;
        //private Model.Model _editorModel = null!;
        private ViewModelMain _viewModel = null!;
        private GameWindow _view = null!;
        private MainMenu _mainMenu = null!;
        private PauseMenu _inGameMenu = null!;
        private KeySettingsWindow _keySettingsWindow = null!;
        private KeySettingsWindowVM _keySettingsWindowVM = null!;
        private EditorWindow _editorWindow = null!;
        private DispatcherTimer _timer = null!;
        public OpenFileDialog _restartGame = null!;
        private NewGameSettingsWindow _newGameSettingsWindow = null!;
        private NewGameSettingsWindowVM _newGameSettingsWindowVM = null!;



        //for window logic
        //private List<Dictionary<string, Direction>> playerKeyBindings;
        #endregion

        #region Constructors

        /// <summary>
        /// Alkalmazás példányosítása.
        /// </summary>
        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        #endregion
        #region Private methods
        /// <summary>
        /// generate the map for a new game
        /// </summary>
        /// <param name="playerCount">2 or 3</param>
        private void GenerateMap(int playerCount, string mapName, int numberOfRounds) {
            //find the map
            Debug.WriteLine(Directory.GetCurrentDirectory());
            string path = $"../../../../BomberMan/Persistence/Maps/{mapName}.txt";
            DataAccess dummyDataAccess = new DataAccess();
            List<Dictionary<string, Direction>> playerKeyBindings = _keySettingsModel.PlayerKeyBindings;
            //model creation
            _model = new Model.Model(path, numberOfRounds, playerCount, playerKeyBindings, dummyDataAccess);
            _model.RoundOver += new EventHandler<GameEventArgs>(Model_RoundOver);
            _model.GameOver += new EventHandler<GameEventArgs>(Model_GameOver);

            // viewmodel creation
            _viewModel = new ViewModelMain(_model);

            // view creation - it is gamewindow for now
            _view = new GameWindow();
            _view.DataContext = _viewModel;
            _view.Closing += new System.ComponentModel.CancelEventHandler(View_Closing); // eseménykezelés a bezáráshoz
            _view.KeyDown += new KeyEventHandler(KeySet);
        }
        

        #endregion
        #region Application event handlers

        private void App_Startup(object? sender, StartupEventArgs e)
        {
            _keySettingsModel = new KeySettingsModel();
            

            // Default Opened window is the main menu
            MainMenuWindow _mainMenuWindow = new MainMenuWindow();
            _mainMenu = new MainMenu();
            _mainMenu.DataContext = _mainMenuWindow;
            _mainMenu.Closing += new System.ComponentModel.CancelEventHandler(Main_Closing);
            _mainMenuWindow.NewGame += new EventHandler(Main_NewGame);
            _mainMenuWindow.Options += new EventHandler(Main_Options);
            _mainMenuWindow.Editor += new EventHandler(Main_Editor);

            _mainMenu.Show();

            // timer creation - it is not used for now

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += new EventHandler(Timer_Tick);
            //_timer.Start();

        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            // _model.PlayerMove(0,Direction.Down);
        }


        #endregion

        #region View event handlers

        // <summary>
        // Játékbéli mozgás eseménykezelője
        // </summary>
        private void KeySet(object sender, KeyEventArgs pressed)
        {
            switch (pressed.Key)
            {
                
                //keySet for maintenance
                case Key.Space:
                    View_PauseGame();
                    break;
                default:
                    _model.KeyPressed(pressed.Key.ToString());
                    Debug.WriteLine(pressed.Key.ToString());
                    break;
            }
        }


        // <summary>
        // Játék megállításának segédmetódusa  ----  jelenleg rossz helyen van, kód-rendezési szempontból
        // </summary>
        private void View_PauseGame()
        {
            _inGameMenu = new PauseMenu();
            InGameMenuWindow _inGameMenuWindow = new InGameMenuWindow();
            _inGameMenu.DataContext = _inGameMenuWindow;
            _inGameMenu.Show();
            _timer.Stop();
            _model.CreaturesStop();
            _model.BombsStop();

            _inGameMenuWindow.ResumeGame += new EventHandler(View_ResumeGame);
            _inGameMenuWindow.NewGame += new EventHandler(View_NewGame);
            _inGameMenuWindow.LoadGame += new EventHandler(View_LoadGame);
            _inGameMenuWindow.SaveGame += new EventHandler(View_SaveGame);
            _inGameMenuWindow.ExitGame += new EventHandler(View_ExitGame);
        }


        // <summary>
        // Játék folytatásának eseménykezelője
        // </summary>
        private void View_ResumeGame(object? sender, EventArgs e)
        {
            _inGameMenu.Close();
            _timer.Start();
            _model.CreaturesStart();
            _model.BombsStart();
        }

        private void View_NewGame(object? sender, EventArgs e)
        {

            //model method új játék kezdéséhez
            //_viewModel.GenerateTable(10, 10);

            _viewModel.ClearTable();
            //_viewModel.RefreshTable();

            _inGameMenu.Close();

            _timer.Start();
            _model.CreaturesStart();
            _model.BombsStart();
        }

        // <summary>
        // Játék betöltésének eseménykezelője
        // </summary>
        private void View_LoadGame(object? sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.AddExtension = true;
            ofd.Filter = "txt files (*.txt)|*.txt";
            ofd.DefaultExt = "txt";
            ofd.CheckFileExists = true;
            //ofd.DefaultDirectory = DefaultLoadPath;
            //Debug.WriteLine("File neve:" ,ofd.ShowDialog());
            //ofd.ShowDialog();
            if (ofd.ShowDialog() == true)
            {
                Debug.WriteLine(ofd.ToString());
                _model.Load(ofd.FileName, _keySettingsModel.PlayerKeyBindings);
                _restartGame = ofd;
            }
            try
            {

            }
            catch (Exception)
            {
                MessageBox.Show("Hiba keletkezett a betöltés során.", "Bomberman");
                _inGameMenu.Close();
                _timer.Start();
                return;
            }
            //_viewModel = new ViewModelMain(_model);

            _viewModel.ClearTable();
            _viewModel.GenerateTable(10, 10);
            _viewModel.RefreshTable();

            _inGameMenu.Close();
            _timer.Start();
            _model.CreaturesStart();
            _model.BombsStart();
        }

        // <summary>
        // Játék mentésének eseménykezelője
        // </summary>
        private void View_SaveGame(object? sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "bomberman.txt";
            sfd.AddExtension = true;
            sfd.Filter = "txt files (*.txt)|*.txt";
            sfd.DefaultExt = "txt";
            sfd.CheckPathExists = true;
            //sfd.DefaultDirectory = DefaultSavePath;
            //var v = sfd.ShowDialog();
            if (sfd.ShowDialog() == true)
            {
                _model.Save(sfd.FileName);
            }

            _inGameMenu.Close();
            _timer.Start();
            _model.CreaturesStart();
            _model.BombsStart();
        }

        // <summary>
        // Játék kiléptetése eseménykezelője
        // </summary>
        private void View_ExitGame(object? sender, EventArgs e)
        {

            _model.CreaturesStop();
            _model.BombsStop();
            _inGameMenu.Close();
            _view.Hide();
            _mainMenu.Show();
        }

        /// <summary>
        /// Nézet bezárásának eseménykezelője.
        /// </summary>
        private void View_Closing(object? sender, CancelEventArgs e)
        {

            //Boolean restartTimer = _timer.IsEnabled;

            _timer.Stop();

            /*
            if (MessageBox.Show("Biztos, hogy ki akar lépni?", "Bomberman", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true; // töröljük a bezárást

                if (restartTimer) // ha szükséges, elindítjuk az időzítőt
                    _timer.Start();

            }
            else
            {
                this._mainMenu.Close();
                //this._inGameMenu.Close();
            }
            */

            System.Windows.Application.Current.Shutdown();

        }

        #endregion

        #region Main Menu EventHandlers
        // <summary>
        // Játék indításának eseménykezelője  ----  jelenleg rossz helyen van, kód-rendezési szempontból
        // </summary>
        private void Main_NewGame(object? sender, EventArgs e)
        {
            _newGameSettingsWindow = new NewGameSettingsWindow();
            _newGameSettingsWindowVM = new NewGameSettingsWindowVM();
            _newGameSettingsWindow.DataContext = _newGameSettingsWindowVM;
            _newGameSettingsWindowVM.ReturnToMainMenu += new EventHandler(NewGameSettingsWindow_ReturnToMainMenu);
            _newGameSettingsWindowVM.StartGame += new EventHandler(NewGameSettingsWindow_StartGame);
            _mainMenu.Hide();
            _newGameSettingsWindow.Show();
        }


        private void Main_Options(object? sender, EventArgs e)
        {
            _keySettingsWindow = new KeySettingsWindow();
            _keySettingsWindowVM = new KeySettingsWindowVM(_keySettingsModel);
            _keySettingsWindow.DataContext = _keySettingsWindowVM;
            _keySettingsWindowVM.ReturnToMainMenu += new EventHandler(KeySettingsWindowVM_ReturnToMainMenu);
            _keySettingsWindow.Closing += new System.ComponentModel.CancelEventHandler(KeySettingsWindow_Closing);
            _keySettingsWindow.KeyDown += new KeyEventHandler(KeySettingsWindow_KeyDown);
            _mainMenu.Hide();
            _keySettingsWindow.Show();
        }

        // <summary>
        // Játék betöltésének eseménykezelője  ----  jelenleg rossz helyen van, kód-rendezési szempontból
        // </summary>
        private void Main_Editor(object? sender, EventArgs e)
        {
            _mainMenu.Hide();
            

            startingMap = new List<Field>[defaultMapSize, defaultMapSize];

            for (int i = 0; i < defaultMapSize; i++)
            {
                for (int j = 0; j < defaultMapSize; j++)
                {
                    startingMap[i, j] = new List<Field>();
                }
            }
            ConcurrentFieldMatrix fields = new ConcurrentFieldMatrix(startingMap);

            List<Player> players = new List<Player>();
            List<Creature> dummyCreatures = new List<Creature>();
            DataAccess dummyDataAccess = new DataAccess();



            _model = new Model.Model(defaultMapSize, fields, dummyCreatures, -1, -1, 3, players, dummyDataAccess);
            _model.CreaturesStop();
            //_model.BombsStop();

            _editorWindow = new EditorWindow();
            EditorWindowVM editorWindow = new EditorWindowVM(_model);
            _editorWindow.DataContext = editorWindow;
            _editorWindow.Show();

            editorWindow.PlayerSpawn += new EventHandler(Editor_Player1);
            editorWindow.Player2Spawn += new EventHandler(Editor_Player2);
            editorWindow.Player3Spawn += new EventHandler(Editor_Player3);
            editorWindow.MonsterSpawn += new EventHandler(Editor_Monster);
            editorWindow.Wall += new EventHandler(Editor_Wall);
            editorWindow.Box += new EventHandler(Editor_Box);
            editorWindow.ClearMode += new EventHandler(Editor_ClearMode);
            editorWindow.SaveMap += new EventHandler(Editor_SaveMap);
            editorWindow.LoadMap += new EventHandler(Editor_LoadMap);
            editorWindow.BackToMenu += new EventHandler(Editor_BackToMenu);
            _editorWindow.Closing += new System.ComponentModel.CancelEventHandler(Editor_Closing);

        }

        private void Main_Closing(object? sender, CancelEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }



        #endregion
        #region NewGameSettingsWindow event handlers
        private void NewGameSettingsWindow_ReturnToMainMenu(object? sender, EventArgs e)
        {
            //important difference is that this window is actually closed and not just hidden
            //needed for potential new maps to be listed
            _newGameSettingsWindow.Close();
            _mainMenu.Show();
        }

        /// <summary>
        /// should only be called if user input is valid, which is checked in the viewmodel
        /// </summary>
        private void NewGameSettingsWindow_StartGame(object? sender, EventArgs e)
        {
            _newGameSettingsWindow.Hide();
            File.WriteAllText("data.txt",_newGameSettingsWindowVM.NumberOfRoundsString);
            GenerateMap(_newGameSettingsWindowVM.NumberOfPlayers, _newGameSettingsWindowVM.SelectedMap, _newGameSettingsWindowVM.NumberOfRounds);
            _timer.Start();
            _model.CreaturesStart();
            _view.Show();
        }

        #endregion
        #region KeySettingsWindow event handlers


        private void KeySettingsWindowVM_ReturnToMainMenu(object? sender, EventArgs e)
        {
            _keySettingsWindow.Hide();
            _mainMenu.Show();
        }

        
        private void KeySettingsWindow_Closing(object? sender, CancelEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void KeySettingsWindow_KeyDown(object? sender, KeyEventArgs e)
        {
            _keySettingsWindowVM.WindowKeyDown(e);
        }
        #endregion
        #region Editor event handlers

        public void Editor_Player1(object? sender, EventArgs e)
        {
            _model.Click(1);
        }

        public void Editor_Player2(object? sender, EventArgs e)
        {
            _model.Click(2);
        }

        public void Editor_Player3(object? sender, EventArgs e)
        {
            _model.Click(3);
        }

        public void Editor_Monster(object? sender, EventArgs e)
        {
            _model.Click(5);
        }

        public void Editor_Wall(object? sender, EventArgs e)
        {
            _model.Click(4);
        }

        public void Editor_Box(object? sender, EventArgs e)
        {
            _model.Click(6);
        }

        public void Editor_PutDown(object? sender, EventArgs e)
        { 
            
        }

        public void Editor_ClearMode(object? sender, EventArgs e)
        {
            _model.Click(7);
        }

        public void Editor_LoadMap(object? sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.AddExtension = true;
            ofd.Filter = "txt files (*.txt)|*.txt";
            ofd.DefaultExt = "txt";
            ofd.CheckFileExists = true;
            //ofd.DefaultDirectory = DefaultLoadPath;
            //Debug.WriteLine("File neve:" ,ofd.ShowDialog());
            //ofd.ShowDialog();
            if (ofd.ShowDialog() == true)
            {
                //Debug.WriteLine(ofd.ToString());
                _model.Load(ofd.FileName, _keySettingsModel.PlayerKeyBindings);
                //_restartGame = ofd;
                //_restartGameString = ofd.ToString();
            }
            //editorWindow.RefreshTable();
            _model.CreaturesStop();
        }

        public void Editor_SaveMap(object? sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "bomberman.txt";
            sfd.AddExtension = true;
            sfd.Filter = "txt files (*.txt)|*.txt";
            sfd.DefaultExt = "txt";
            sfd.CheckPathExists = true;
            //sfd.DefaultDirectory = DefaultSavePath;
            //var v = sfd.ShowDialog();
            if (sfd.ShowDialog() == true)
            {
                _model.Save(sfd.FileName);
            }
            //editorWindow.RefreshTable();
            
        }

        public void Editor_BackToMenu(object? sender, EventArgs e)
        {
            _editorWindow.Hide();
            _mainMenu.Show();
        }

        private void Editor_Closing(object? sender, CancelEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        #endregion


        #region ViewModel event handlers
        /*
        /// <summary>
        /// Új játék indításának eseménykezelője.
        /// </summary>
        private void ViewModel_NewGame(object? sender, EventArgs e)
        {
            _model.NewGame();
            _timer.Start();
        }

        /// <summary>
        /// Játék betöltésének eseménykezelője.
        /// </summary>
        private async void ViewModel_LoadGame(object? sender, System.EventArgs e)
        {
            Boolean restartTimer = _timer.IsEnabled;

            _timer.Stop();

            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog(); // dialógusablak
                openFileDialog.Title = "Sudoku tábla betöltése";
                openFileDialog.Filter = "Sudoku tábla|*.stl";
                if (openFileDialog.ShowDialog() == true)
                {
                    // játék betöltése
                    await _model.LoadGameAsync(openFileDialog.FileName);

                    _timer.Start();
                }
            }
            catch (SudokuDataException)
            {
                MessageBox.Show("A fájl betöltése sikertelen!", "Sudoku", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (restartTimer) // ha szükséges, elindítjuk az időzítőt
                _timer.Start();
        }

        /// <summary>
        /// Játék mentésének eseménykezelője.
        /// </summary>
        private async void ViewModel_SaveGame(object? sender, EventArgs e)
        {
            Boolean restartTimer = _timer.IsEnabled;

            _timer.Stop();

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog(); // dialógablak
                saveFileDialog.Title = "Sudoku tábla betöltése";
                saveFileDialog.Filter = "Sudoku tábla|*.stl";
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        // játéktábla mentése
                        await _model.SaveGameAsync(saveFileDialog.FileName);
                    }
                    catch (SudokuDataException)
                    {
                        MessageBox.Show("Játék mentése sikertelen!" + Environment.NewLine + "Hibás az elérési út, vagy a könyvtár nem írható.", "Hiba!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch
            {
                MessageBox.Show("A fájl mentése sikertelen!", "Sudoku", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (restartTimer) // ha szükséges, elindítjuk az időzítőt
                _timer.Start();
        }
        */



        /// <summary>
        /// Játékból való kilépés eseménykezelője.
        /// </summary>
        private void ViewModel_ExitGame(object? sender, System.EventArgs e)
        {
            _view.Close(); // ablak bezárása
        }

        #endregion

        #region Model event handlers

        /// <summary>
        /// Kör végének eseménykezelője.
        /// </summary>
        ///
        private void Model_RoundOver(object? sender, GameEventArgs e)
        { 
            _timer.Stop();
            _model.CreaturesStop();
            _model.BombsStop();
            _model.PlayerStopTimer();

            /*if (File.Exists("data.txt"))
            {
                int rounds = Convert.ToInt32(File.ReadAllText("data.txt").Split(',')[0]);
                if(rounds == 1)
                {
                    // a játék vége
                }
                else
                {
                    rounds--;
                    string file = $"{rounds},{_model.playerCount}\n";
                    file += $"{_model.winNum1},{_model.winNum2},{_model.winNum3}";
                    File.WriteAllText("data.txt",file);
                }
            }
            else
            {
                string file = $"{_newGameSettingsWindowVM.NumberOfRounds},{_model.playerCount}\n";
                file += $"";
                File.WriteAllText("data.txt", file);
            }*/

            if (e.RoundWon)
            {
                MessageBox.Show(e.Name + " nyerte a kört, kap egy pontot!",
                                "Bomberman",
                                MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
            }

            
            //start a new round using a random map
            if(_newGameSettingsWindowVM is null)
            {
                _newGameSettingsWindowVM = new NewGameSettingsWindowVM();
                _newGameSettingsWindowVM.SetNumberOfPlayers(_model.playerCount);
                Debug.WriteLine($"New round: setting player count to {_model.playerCount}");
            }
            _newGameSettingsWindowVM.ChooseRandomMap();
            string randomMap = _newGameSettingsWindowVM.SelectedMap;
            string mapPath = Path.Combine(PathPrefix, randomMap + ".txt");
            Debug.WriteLine($"New round: loading map from {mapPath}");
            _model.LoadStartMap(mapPath, _model.playerCount,_keySettingsModel.PlayerKeyBindings);
            

            _timer.Start();
            _model.CreaturesStart();
            _model.BombsStart();

        }

        /// <summary>
        /// Játék végének eseménykezelője.
        /// </summary>
        private void Model_GameOver(object? sender, GameEventArgs e)
        {
            _timer.Stop();
            _model.CreaturesStop();
            _model.BombsStop();
            _model.PlayerStopTimer();

            if (e.GameWon)
            {
                MessageBox.Show(e.Name + " nyerte a Játékot! Gratulálunk!",
                                "Bomberman",
                                MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
            }

            //return to main menu
            //copilot solution for hiding/showing windows from "non-ui thread"
            Application.Current.Dispatcher.Invoke(() =>
            {
               _view.Hide();
               _mainMenu.Show();
            });

            
        }

        #endregion
    }

}
