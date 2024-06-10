using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Bomberman.Model;
using Bomberman.Model.EventArguments;

namespace Bomberman.WPF.ViewModel
{
    public class ViewModelMain : ViewModelBase
    {
        #region Fields
        private Model.Model _model = null!;  //modell deklaració helye
        #endregion
        #region Properties
        /// <summary>
        /// width of the game table
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// height of the game table
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// fields of the game table
        /// </summary>
        public ObservableCollection<GameParts> Fields { get; set; }
        /// <summary>
        /// player count
        /// </summary>
        public int PlayerCount { get; set; }
        /// <summary>
        /// players' data
        /// </summary>
        public ObservableCollection<PlayerData> PlayerDatas { get; set; }

        
        // public String GameTime { get { return 0; } }
        #endregion
        
        
        #region Constructors
        /// <summary>
        /// constructor for the viewmodel
        /// </summary>
        /// <param name="model"></param>
        public ViewModelMain(Model.Model model)
        {
            
            _model = model;
            Width = _model.size;
            Height = _model.size;
            Fields = null!;
            PlayerCount = _model.playerCount;
            PlayerDatas = null!;

            _model.PlayerMoved += new EventHandler<MovementEventArgs>(Model_PlayerMoved);
            _model.GameOver += new EventHandler<GameEventArgs>(Model_GameOver);
            _model.FieldChanged += new EventHandler<GameEventArgs>(Model_FieldChanged);
            _model.BoostChanged += new EventHandler<BoostEventArgs>(Model_BoostChanged);


            //_model.State.GameTime = 0;

            GenerateTable(Width, Height);
            GeneratePlayerData();

            RefreshTable();
            RefreshPlayerData();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// updates the table in the viewmodel
        /// </summary>
        public void RefreshTable()
        {
            
            foreach (GameParts part in Fields)
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
                int elemCount = 0;
                if (_model.fields.GetFieldMatrix()[part.X, part.Y] is null)
                {
                    Debug.WriteLine("hát ez itt null");
                    elemCount = 0;
                }
                else 
                {
                    elemCount = _model.fields.GetFieldMatrix()[part.X, part.Y].Count;
                }


                if (elemCount == 0)
                {
                    part.Value = "Clear";
                    continue;
                }

                List<GameParts.FieldValueSingle> values = new List<GameParts.FieldValueSingle>();
                for (int i = 0; i < elemCount; i++)
                {
                    
                    
                    /*if (_model.players[0] is null)
                    {
                        Debug.WriteLine("most meg a player null");
                    }*/

                    if (_model.fields.GetFieldMatrix()[part.X, part.Y][i] == _model.players[0])
                    {
                        values.Add(GameParts.FieldValueSingle.Player1);
                    }
                    else if (_model.fields.GetFieldMatrix()[part.X, part.Y][i] == _model.players[1])
                    {
                        values.Add(GameParts.FieldValueSingle.Player2);
                    }
                    else if (_model.playerCount == 3 && _model.fields.GetFieldMatrix()[part.X, part.Y][i] == _model.players[2])
                    {
                        values.Add(GameParts.FieldValueSingle.Player3);
                    }
                    else if (_model.fields.GetFieldMatrix()[part.X, part.Y][i] == Wall.GetInstance())
                    {
                        values.Add(GameParts.FieldValueSingle.Wall);
                    }
                    else if (_model.fields.GetFieldMatrix()[part.X, part.Y][i] is Chest)
                    {
                        values.Add(GameParts.FieldValueSingle.Chest);
                    }
                    else if (_model.fields.GetFieldMatrix()[part.X, part.Y][i] is Bomb)
                    {
                        values.Add(GameParts.FieldValueSingle.Bomb);
                    }
                    else if (_model.fields.GetFieldMatrix()[part.X, part.Y][i] is Explosion)
                    {
                        //multiple explosions still count as one
                        if (values.Contains(GameParts.FieldValueSingle.Explosion))
                        {
                            continue;
                        }
                        else
                        {
                            values.Add(GameParts.FieldValueSingle.Explosion);
                        }
                        
                    }
                    else if (_model.fields.GetFieldMatrix()[part.X, part.Y][i] is Creature)
                    {
                        values.Add(GameParts.FieldValueSingle.Monster);
                    }
                    else if (_model.fields.GetFieldMatrix()[part.X, part.Y][i] == Detonator.GetInstance() ||
                     _model.fields.GetFieldMatrix()[part.X, part.Y][i] == BombSizeUp.GetInstance() ||
                     _model.fields.GetFieldMatrix()[part.X, part.Y][i] == BombSizeDown.GetInstance() ||
                     _model.fields.GetFieldMatrix()[part.X, part.Y][i] == ChestPlant.GetInstance() ||
                     _model.fields.GetFieldMatrix()[part.X, part.Y][i] == InstantPlant.GetInstance() ||
                     _model.fields.GetFieldMatrix()[part.X, part.Y][i] == Invulnerable.GetInstance() ||
                     _model.fields.GetFieldMatrix()[part.X, part.Y][i] == Skate.GetInstance() ||
                     _model.fields.GetFieldMatrix()[part.X, part.Y][i] == Slow.GetInstance() ||
                     _model.fields.GetFieldMatrix()[part.X, part.Y][i] == BombCounterUp.GetInstance() ||
                     _model.fields.GetFieldMatrix()[part.X, part.Y][i] == BombPlantProblem.GetInstance() ||
                     _model.fields.GetFieldMatrix()[part.X, part.Y][i] == Ghost.GetInstance() /*.GetType() is Boost*/)
                    {
                        values.Add(GameParts.FieldValueSingle.Bonus);
                    }

                    //TODO: negative bonus TBA

                }
                values.Sort();
                
                string value = values.Aggregate("", (current, val) => current + Enum.GetName(typeof(GameParts.FieldValueSingle), val));
                //Debug.WriteLine($"{part.X},{part.Y}: {value}");

                part.Value = value;
                continue;

                
                

                
            }
            

            OnPropertyChanged(nameof(Fields));

            //OnPropertyChanged(nameof(GameTime));
        }

        /// <summary>
        /// generates an empty game table
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public void GenerateTable(int w, int h) //üres játéktábla legenerálása
        {
            Fields = new ObservableCollection<GameParts>();

            for (Int32 i = 0; i < w; i++)
            {
                for (Int32 j = 0; j < h; j++)
                {
                    Fields.Add(new GameParts(i, j, GameParts.FieldValue.Clear));

                }
            }
        }
        /// <summary>
        /// sets the table to empty
        /// </summary>
        public void ClearTable()
        {
            //Field.Clear();
            //GenerateTable();
            //_gameTime = 0;
            //_model.State.GameTime= 0;
            foreach (GameParts f in Fields)  //lehet hatákonyabb megcserélve
            {
                f.Value = "Clear";
            }
        }
        

        /*
        public void Step(object? sender, EventArgs e)
        {
            _model.StepGame();
            RefreshTable();
        }
        */
        #endregion

        /// <summary>
        /// event handler for when the player moves
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Model_PlayerMoved(object? sender, MovementEventArgs e)
        {
            //todo: if performance is bad, here we should only refresh the 2 affected fields
            RefreshTable();
            RefreshPlayerData();
        }
        /// <summary>
        /// event handler for when the boost changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Model_BoostChanged(object? sender, BoostEventArgs e)
        {
            RefreshTable();
            RefreshPlayerData(e);
        }
        /// <summary>
        /// event handler for when the game is over
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Model_GameOver(object? sender, GameEventArgs e)
        { 
            ClearTable();
        }
        /// <summary>
        /// event handler for when the field changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Model_FieldChanged(object? sender, GameEventArgs e)
        {
            RefreshTable();
        }

        private void GeneratePlayerData()
        {
            PlayerDatas = new ObservableCollection<PlayerData>();
            for(int i = 0; i < _model.playerCount; i++)
            {
                
                PlayerDatas.Add(new PlayerData(_model.players[i].xPosition, _model.players[i].yPosition,i, _model.players[i].alive));
                PlayerDatas[i].WinCounter = _model.players[i].winCounter;
                PlayerDatas[i].Speed = _model.players[i].speed;
                PlayerDatas[i].BombCapacity = _model.players[i].bombCapacity;
                PlayerDatas[i].DetonationSize = _model.players[i].detonationSize;
                PlayerDatas[i].DetonatorActive = _model.players[i].boosts.Contains(Detonator.GetInstance());
                PlayerDatas[i].ChestPlantCounter = _model.players[i].chestplantMax;
                PlayerDatas[i].GhostActive = _model.players[i].boosts.Contains(Ghost.GetInstance());
                PlayerDatas[i].InvulnerableActive = _model.players[i].boosts.Contains(Invulnerable.GetInstance());
                PlayerDatas[i].BombPlantProblemActive = _model.players[i].boosts.Contains(BombPlantProblem.GetInstance());
                PlayerDatas[i].SlowActive = _model.players[i].boosts.Contains(Slow.GetInstance());
                PlayerDatas[i].BombSizeDownActive = _model.players[i].boosts.Contains(BombSizeDown.GetInstance());
                PlayerDatas[i].InstantPlantActive = _model.players[i].boosts.Contains(InstantPlant.GetInstance());

                ObservableCollection<Boost> boosts = new ObservableCollection<Boost>();
                foreach (var elem in _model.players[i].boosts)
                {
                    boosts.Add(elem);
                }
            }
        }
        /// <summary>
        /// Update player data in the viewmodel. 
        /// Does not update lastPickedBoost, for that use the other overload
        /// </summary>
        private void RefreshPlayerData()
        {
            if(Application.Current is not null)
            {
                Application.Current.Dispatcher.Invoke(() => {
                    for (int i = 0; i < _model.playerCount; i++)
                    {
                        PlayerDatas[i].X = _model.players[i].xPosition;
                        PlayerDatas[i].Y = _model.players[i].yPosition;
                        PlayerDatas[i].Alive = _model.players[i].alive;
                        PlayerDatas[i].WinCounter = _model.players[i].winCounter;
                        PlayerDatas[i].Speed = _model.players[i].speed;
                        PlayerDatas[i].BombCapacity = _model.players[i].bombCapacity;
                        PlayerDatas[i].DetonationSize = _model.players[i].detonationSize;
                        PlayerDatas[i].DetonatorActive = _model.players[i].boosts.Contains(Detonator.GetInstance());
                        PlayerDatas[i].ChestPlantCounter = _model.players[i].chestplantMax;
                        PlayerDatas[i].GhostActive = _model.players[i].boosts.Contains(Ghost.GetInstance());
                        PlayerDatas[i].InvulnerableActive = _model.players[i].boosts.Contains(Invulnerable.GetInstance());
                        PlayerDatas[i].BombPlantProblemActive = _model.players[i].boosts.Contains(BombPlantProblem.GetInstance());
                        PlayerDatas[i].SlowActive = _model.players[i].boosts.Contains(Slow.GetInstance());
                        PlayerDatas[i].BombSizeDownActive = _model.players[i].boosts.Contains(BombSizeDown.GetInstance());
                        PlayerDatas[i].InstantPlantActive = _model.players[i].boosts.Contains(InstantPlant.GetInstance());
                    }
                    //TODO: if needed for performance, flesh out the properties using BoostEventArgs
                    OnPropertyChanged(nameof(PlayerDatas));
                });
            }
            
            
        }

        private void RefreshPlayerData(BoostEventArgs e)
        {
            int playerID = e.PlayerID;
            string boostType = e.BoostType;
            bool isAdded = e.IsAdded;
            //order corresponds to the order found in Player.cs/NewBoost()
            switch (boostType)
            {
                case "Invulnerable":
                    PlayerDatas[playerID].InvulnerableActive = isAdded;
                    break;
                case "Ghost":
                    PlayerDatas[playerID].GhostActive = isAdded;
                    break;
                case "BombPlantProblem":
                    PlayerDatas[playerID].BombPlantProblemActive = isAdded;
                    break;
                case "BombSizeUp":
                    PlayerDatas[playerID].DetonationSize = _model.players[playerID].detonationSize;
                    break;
                case "BombSizeDown":
                    PlayerDatas[playerID].BombSizeDownActive = isAdded;
                    PlayerDatas[playerID].DetonationSize = _model.players[playerID].detonationSize;
                    break;
                case "Skate":
                    PlayerDatas[playerID].Speed = _model.players[playerID].speed;
                    break;
                case "Slow":
                    PlayerDatas[playerID].SlowActive = isAdded;
                    PlayerDatas[playerID].Speed = _model.players[playerID].speed;
                    break;
                case "InstantPlant":
                    PlayerDatas[playerID].InstantPlantActive = isAdded;
                    break;
                case "BombCounterUp":
                    PlayerDatas[playerID].BombCapacity = _model.players[playerID].bombCapacity;
                    break;
                case "ChestPlant":
                    PlayerDatas[playerID].ChestPlantCounter = _model.players[playerID].chestplantMax;
                    break;
                case "Detonator":
                    PlayerDatas[playerID].DetonatorActive = isAdded;
                    break;
                default:
                    Debug.WriteLine("Boost type not found");
                    break;
            }
            PlayerDatas[playerID].LastPickedBoost = boostType;
            OnPropertyChanged(nameof(PlayerDatas));


        }

    }
}
