using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bomberman.Model;

namespace Bomberman.WPF.ViewModel
{
    internal class EditorWindowVM : ViewModelBase
    {

        #region Fields

        private Model.Model _editorModel;
        /// <summary>
        /// width of the game field
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// height of the game field
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// fields of the game in the editor
        /// </summary>
        public ObservableCollection<GameParts> EditorFields { get; set; }

        #endregion


        #region Events
        /// <summary>
        /// Event for player 1 spawn
        /// </summary>
        public event EventHandler? PlayerSpawn;
        /// <summary>
        /// Event for player 2 spawn
        /// </summary>
        public event EventHandler? Player2Spawn;
        /// <summary>
        /// Event for player 3 spawn
        /// </summary>
        public event EventHandler? Player3Spawn;
        /// <summary>
        /// Event for monster spawn
        /// </summary>
        public event EventHandler? MonsterSpawn;
        /// <summary>
        /// Event for box spawn
        /// </summary>
        public event EventHandler? Box;
        /// <summary>
        /// Event for wall spawn
        /// </summary>
        public event EventHandler? Wall;

        /// <summary>
        /// event for put down mode
        /// </summary>
        public event EventHandler? PutDownMode;
        /// <summary>
        /// event for clear mode. likely unused
        /// </summary>
        public event EventHandler? ClearMode;
        /// <summary>
        /// event for saving the map
        /// </summary>
        public event EventHandler? SaveMap;
        /// <summary>
        /// event for loading the map
        /// </summary>
        public event EventHandler? LoadMap;
        /// <summary>
        /// event for returning to the main menu
        /// </summary>
        public event EventHandler? BackToMenu;

        #endregion

        #region Properties


        //public DelegateCommand ClickCommand { get; private set; }
        /// <summary>
        /// delegate command for player 1 spawn
        /// </summary>
        public DelegateCommand PlayerSpawnCommand { get; private set; }
        /// <summary>
        /// delegate command for player 2 spawn
        /// </summary>
        public DelegateCommand Player2SpawnCommand { get; private set; }
        /// <summary>
        /// delegate command for player 3 spawn
        /// </summary>
        public DelegateCommand Player3SpawnCommand { get; private set; }
        /// <summary>
        /// delegate command for monster spawn
        /// </summary>
        public DelegateCommand MonsterSpawnCommand { get; private set; }
        /// <summary>
        /// delegate command for box spawn
        /// </summary>
        public DelegateCommand BoxCommand { get; private set; }
        /// <summary>
        /// delegate command for wall spawn
        /// </summary>
        public DelegateCommand WallCommand { get; private set; }
        /// <summary>
        /// delegate command for put down mode
        /// </summary>
        public DelegateCommand ClearModeCommand { get; private set; }
        /// <summary>
        /// delegate command for save map
        /// </summary>
        public DelegateCommand SaveMapCommand { get; private set; }
        /// <summary>
        /// delegate command for load map
        /// </summary>
        public DelegateCommand LoadMapCommand { get; private set; }
        /// <summary>
        /// delegate command for returning to the main menu
        /// </summary>
        public DelegateCommand BackToMenuCommand { get; private set; }

        #endregion
        /// <summary>
        /// constructor for the editor window viewmodel
        /// </summary>
        /// <param name="editorModel"></param>

        public EditorWindowVM(Model.Model editorModel)
        {

            _editorModel = editorModel;
            Width = 10;//_editorModel.size;
            Height = 10;// _editorModel.size;
            EditorFields = null!;


            PlayerSpawnCommand = new DelegateCommand(param => OnPlayerSpawn());
            Player2SpawnCommand = new DelegateCommand(param => OnPlayer2Spawn());
            Player3SpawnCommand = new DelegateCommand(param => OnPlayer3Spawn());
            MonsterSpawnCommand = new DelegateCommand(param => OnMonsterSpawn());
            BoxCommand = new DelegateCommand(param => OnBox());
            WallCommand = new DelegateCommand(param => OnWall());
            ClearModeCommand = new DelegateCommand(param => OnClearMode());
            SaveMapCommand = new DelegateCommand(param => OnSaveMap());
            LoadMapCommand = new DelegateCommand(param => OnLoadMap());
            BackToMenuCommand = new DelegateCommand(param => OnBackToMenu());


            GenerateTable(10, 10);
        }


        #region Field Generation
        /// <summary>
        /// generates the table for the editor
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public void GenerateTable(int w, int h) //üres játéktábla legenerálása
        {
            EditorFields = new ObservableCollection<GameParts>();

            for (Int32 i = 0; i < w; i++)
            {
                for (Int32 j = 0; j < h; j++)
                {
                    EditorFields.Add(new GameParts(i, j, i*_editorModel.fields.GetFieldMatrix().GetLength(0) + j ,GameParts.FieldValue.Clear, new DelegateCommand(param => VMClick(Convert.ToInt32(param)))));
                    
                }
            }
        }
        /// <summary>
        /// updates the table
        /// </summary>
        public void RefreshTable()
        {

            foreach (GameParts part in EditorFields)
            {
                /*
                foreach (var elem in _model.fields[part.X, part.Y])
                {
                    if (elem == Wall.GetInstance())
                    {
                        part.Value = "W";
                    }
                    else if (elem == _model.players[0])
                    {
                        part.Value = "P1";
                    }
                }
                */
                int elemCount = _editorModel.fields.GetFieldMatrix()[part.X, part.Y].Count;
                if (elemCount == 0)
                {
                    part.Value = "Clear";
                    continue;
                }
                List<GameParts.FieldValueSingle> values = new List<GameParts.FieldValueSingle>();
                for (int i = 0; i < elemCount; i++)
                {
                    if (_editorModel.fields.GetFieldMatrix()[part.X, part.Y][i] is Player/*== _editorModel.players[0]*/)
                    {
                        Player player = (Player)_editorModel.fields.GetFieldMatrix()[part.X, part.Y][i];
                        if (player.id == 0) { values.Add(GameParts.FieldValueSingle.Player1); }
                        if (player.id == 1) { values.Add(GameParts.FieldValueSingle.Player2); }
                        if (player.id == 2) { values.Add(GameParts.FieldValueSingle.Player3); }
                        
                    }
                    /*
                    else if (_editorModel.fields.GetFieldMatrix()[part.X, part.Y][i] == _editorModel.players[1])
                    {
                        values.Add(GameParts.FieldValueSingle.Player2);
                    }
                    else if (_editorModel.fields.GetFieldMatrix()[part.X, part.Y][i] == _editorModel.players[2])
                    {
                        values.Add(GameParts.FieldValueSingle.Player3);
                    }
                    */
                    else if (_editorModel.fields.GetFieldMatrix()[part.X, part.Y][i] is Wall)
                    {
                        values.Add(GameParts.FieldValueSingle.Wall);
                    }
                    else if (_editorModel.fields.GetFieldMatrix()[part.X, part.Y][i] is Chest)
                    {
                        values.Add(GameParts.FieldValueSingle.Chest);
                    }
                    else if (_editorModel.fields.GetFieldMatrix()[part.X, part.Y][i] is Bomb)
                    {
                        values.Add(GameParts.FieldValueSingle.Bomb);
                    }
                    //TODO:explosion TBA
                    else if (_editorModel.fields.GetFieldMatrix()[part.X, part.Y][i] is Creature)
                    {
                        values.Add(GameParts.FieldValueSingle.Monster);
                    }
                    else if (_editorModel.fields.GetFieldMatrix()[part.X, part.Y][i].GetType() is Boost)
                    {
                        values.Add(GameParts.FieldValueSingle.Bonus);
                    }
                    //TODO: negative bonus TBA

                }
                values.Sort();

                string value = values.Aggregate("", (current, val) => current + Enum.GetName(typeof(GameParts.FieldValueSingle), val));
                Debug.WriteLine(value);
                part.Value = value;
                continue;


                /*
                if(_model.fields[part.X, part.Y].Count == 0)
                {
                    part.Value = "Clear";
                    continue;
                }
                if (_model.fields[part.X, part.Y][0] == Wall.GetInstance())
                {
                    part.Value = "Wall";
                    continue;
                }
                else if (_model.fields[part.X, part.Y][0] == ChestPlant.GetInstance())
                {
                    part.Value = "Chest";
                    continue;
                }
                else if (_model.fields[part.X, part.Y][0] == _model.players[0])
                {
                    part.Value = "Player1";
                    continue;
                }
                else if (_model.fields[part.X, part.Y][0] == _model.players[1])
                {
                    part.Value = "Player2";
                    continue;
                }
                else if (_model.fields[part.X, part.Y][0].GetType() == typeof(Bomb))
                {
                    part.Value = "B";
                    continue;
                }
                else if (_model.fields[part.X, part.Y][0] == _model.creatures[0])
                {
                    part.Value = "Monster";
                    continue;
                }
                */


            }


            OnPropertyChanged(nameof(EditorFields));

            //OnPropertyChanged(nameof(GameTime));
        }


        #endregion
        /// <summary>
        /// relays the click event to the model
        /// </summary>
        /// <param name="index"></param>
        public void VMClick(Int32 index)
        {
            GameParts field = EditorFields[index];

            _editorModel.Click(field.X, field.Y);

            RefreshTable();
        }
        /// <summary>
        /// event invoker for player 1 spawn
        /// </summary>
        public void OnPlayerSpawn()
        {
            PlayerSpawn?.Invoke(this, EventArgs.Empty);
            Debug.WriteLine("player beaéllitva");
        }
        /// <summary>
        /// event invoker for player 2 spawn
        /// </summary>
        public void OnPlayer2Spawn()
        {
            Player2Spawn?.Invoke(this, EventArgs.Empty);
            Debug.WriteLine("player2 beaéllitva");
        }
        /// <summary>
        /// event invoker for player 3 spawn
        /// </summary>
        public void OnPlayer3Spawn()
        {
            Player3Spawn?.Invoke(this, EventArgs.Empty);
            Debug.WriteLine("player3 beaéllitva");
        }
        /// <summary>
        /// event invoker for monster spawn
        /// </summary>
        public void OnMonsterSpawn()
        {
            MonsterSpawn?.Invoke(this, EventArgs.Empty);
            Debug.WriteLine("monster beaéllitva");
        }
        /// <summary>
        /// event invoker for box spawn
        /// </summary>
        public void OnBox()
        {
            Box?.Invoke(this, EventArgs.Empty);
            Debug.WriteLine("box beaéllitva");
        }
        /// <summary>
        /// event invoker for wall spawn
        /// </summary>
        public void OnWall()
        {
            Wall?.Invoke(this, EventArgs.Empty);
            Debug.WriteLine("fal beaéllitva");
        }
        /// <summary>
        /// event invoker for put down mode
        /// </summary>
        public void OnPutDownMode()
        {
            PutDownMode?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// event invoker for clear mode
        /// </summary>
        public void OnClearMode()
        {
            ClearMode?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// event invoker for saving the map
        /// </summary>
        public void OnSaveMap()
        {
            if (_editorModel.players.Count == 3) { 
            SaveMap?.Invoke(this, EventArgs.Empty);
            RefreshTable();
            }
        }
        /// <summary>
        /// event invoker for loading the map
        /// </summary>
        public void OnLoadMap()
        {
            if (_editorModel.players.Count == 3)
            {
                LoadMap?.Invoke(this, EventArgs.Empty);
                RefreshTable();
            }
        }
        /// <summary>
        /// event invoker for returning to the main menu
        /// </summary>
        public void OnBackToMenu()
        {
            BackToMenu?.Invoke(this, EventArgs.Empty);
        }
    }
}
