using Bomberman.Model.EventArguments;
using Bomberman.Model.KeySettings;
using Bomberman.Model.Maps;
using Bomberman.Persistence;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;


namespace Bomberman.Model
{
    public enum Direction
    {
        Up, Down, Left, Right, PlantBomb, PlantChest, empty
    }

    /// <summary>
    /// Represents the model of the Bomberman game.
    /// </summary>
    public class Model
    {
        
        #region fields
        /// <summary>
        /// size of the map
        /// </summary>
        public int size;
        /// <summary>
        /// fields of the map, thread safe
        /// </summary>
        public ConcurrentFieldMatrix fields;
        /// <summary>
        /// creatures on the map
        /// </summary>
        public List<Creature> creatures;
        // timer!
        /// <summary>
        /// player 1's win counter
        /// </summary>
        public int winNum1 = 0;
        /// <summary>
        /// player 2's win counter
        /// </summary>
        public int winNum2 = 0;
        /// <summary>
        /// player 3's win counter
        /// </summary>
        public int winNum3 = 0;
        /// <summary>
        /// unused
        /// </summary>
        public int WinCounter;
        /// <summary>
        /// number of rounds any player needs to win to win the game
        /// </summary>
        public int Rounds = 0;
        /// <summary>
        /// unused
        /// </summary>
        public int BattleRoyalPhase = -1; // If Battleroyal isn't aktiv then -1;
        /// <summary>
        /// number of players
        /// </summary>
        public int playerCount;
        /// <summary>
        /// players on the map
        /// </summary>
        public List<Player> players;
        /// <summary>
        /// last clicked button in editor. spaghetti code
        /// </summary>
        public int lastClick = 0;
        /// <summary>
        /// persistence object
        /// </summary>
        Persistence.IDataAccess persistence = new Persistence.DataAccess();


        //public int rounds { get { return Rounds; }  set { } }

        #endregion
        #region Events
        /// <summary>
        /// Event handler for player movement
        /// </summary>
        public event EventHandler<MovementEventArgs>? PlayerMoved;
        /// <summary>
        /// Event handler for round over
        /// </summary>
        public event EventHandler<GameEventArgs>? RoundOver;
        /// <summary>
        /// Event handler for game over
        /// </summary>
        public event EventHandler<GameEventArgs>? GameOver;
        /// <summary>
        /// Event handler for field change
        /// </summary>
        public event EventHandler<GameEventArgs>? FieldChanged;
        /// <summary>
        /// Event handler for boost change
        /// </summary>
        public event EventHandler<BoostEventArgs>? BoostChanged;

        #endregion
        #region Constructors
        /// <summary>
        /// constructor for the model in editor mode
        /// </summary>
        /// <param name="size"></param>
        /// <param name="fields"></param>
        /// <param name="creatures"></param>
        /// <param name="winCounter"></param>
        /// <param name="battleRoyalPhase"></param>
        /// <param name="playerCount"></param>
        /// <param name="players"></param>
        /// <param name="persistence"></param>
        public Model(int size, ConcurrentFieldMatrix fields, List<Creature> creatures, int winCounter, int battleRoyalPhase,int playerCount, List<Player> players, IDataAccess persistence)
        {
            
            this.size = size;
            this.fields = fields;
            this.creatures = creatures;
            WinCounter = winCounter;
            BattleRoyalPhase = battleRoyalPhase;
            this.playerCount = playerCount;
            this.players = players;
            this.persistence = persistence;
            DataLoad();
            ClearExplosion();
        }
        /// <summary>
        /// Helper constructor for generating maps. Helpful for testing and creating maps.
        /// </summary>
        /// <param name="mapModel">contains most info on what is on field</param>
        /// <param name="winCounter"></param>
        /// <param name="playerCount"></param>
        /// <param name="persistence"></param>
        public Model(MapModel mapModel, int winCounter, int playerCount, IDataAccess persistence)
        {
            this.size = mapModel.Size;
            this.WinCounter = winCounter;
            this.playerCount = playerCount;
            this.persistence = persistence;

            //lists
            List<Field>[,] tmpfields = new List<Field>[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    tmpfields[i, j] = new List<Field>();
                }
            }
            creatures = new List<Creature>();
            foreach (var item in mapModel.CreaturePositions)
            {
                Creature creature = new Creature(item.Item1, item.Item2, this);
                tmpfields[item.Item1, item.Item2].Add(creature);
                creatures.Add(creature);            
            }
            players = new List<Player>();
            for(int i=0; i<playerCount; i++)
            {
                Player player = new Player(i, mapModel.PlayerPositions[i].Item1, mapModel.PlayerPositions[i].Item2, true, new List<Boost>(), new List<Bomb>(), 1, 2, 0, 2, this);
                tmpfields[mapModel.PlayerPositions[i].Item1, mapModel.PlayerPositions[i].Item2].Add(player);
                players.Add(player);
            }
            foreach (var item in mapModel.WallPositions)
            {
                tmpfields[item.Item1, item.Item2].Add(Wall.GetInstance());
            }
            foreach (var item in mapModel.ChestPositions)
            {
                tmpfields[item.Item1, item.Item2].Add(new Chest(true, item.Item1, item.Item2));
            }
            fields = new ConcurrentFieldMatrix(tmpfields);
            DataLoad();
            ClearExplosion();
        }
        /// <summary>
        /// Model constructor for main game
        /// </summary>
        /// <param name="path">path of the map file</param>
        /// <param name="playerCount"></param>
        /// <param name="winCounter"></param>
        /// <param name="keys"></param>
        /// <param name="persistence"></param>
        public Model(string path, int winCounter, int playerCount, List<Dictionary<string, Direction>> keys, IDataAccess persistence)
        {
            //TODO: extract data from file in path
            this.persistence = persistence;
            this.playerCount = playerCount;
            this.WinCounter = winCounter;
            this.creatures = new List<Creature>();
            this.players = new List<Player>();

            //just to remove warning will be overwritten in LoadStartMap
            fields = new ConcurrentFieldMatrix(new List<Field>[10, 10]);
            LoadStartMap(path, playerCount, keys);
            //fields = new ConcurrentFieldMatrix(new List<Field>[10,10]);

            //stop creature movement
            CreaturesStop();
            DataLoad();
            ClearExplosion();
        }
        #endregion
        #region Public game methods

        /// <summary>
        /// Loads a previously saved game from a file.
        /// </summary>
        /// <param name="path"></param>
        public void Load(string path, List<Dictionary<string, Direction>> playerKeyBindings)
        {
            ResetArrays();
            string? file = persistence.LoadFile(path);
            if (file != null)
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        try
                        {
                            fields.GetFieldMatrix()[i, j].ForEach(o => o.Destroy());
                        }
                        catch(Exception) { }
                    }
                }
                creatures = new List<Creature>();
                int playersCount = Convert.ToInt32(file.Split('\n')[0].Split(',')[0]);
                size = Convert.ToInt32(file.Split('\n')[0].Split(',')[1]);
                Rounds = Convert.ToInt32(file.Split('\n')[0].Split(',')[2]);
                List<Field>[,]localfields = new List<Field>[size, size];
                fields =new ConcurrentFieldMatrix (new List<Field>[size,size]);
                for (int i = 0; i < size; i++) {
                    for (int j = 0; j < size; j++)
                    {
                        localfields[i,j] = new List<Field>();
                        for (int x = 0; x < file.Split('\n')[i+1].Split(';')[j].Split(',').Length; x++)
                        {
                            if (file.Split('\n')[i + 1].Split(';')[j].Split(',')[x].Split('-')[0] == "W")
                            {
                                localfields[i, j].Add(Wall.GetInstance());
                            }
                            if (file.Split('\n')[i + 1].Split(';')[j].Split(',')[x].Split('-')[0] == "C")
                            {
                                localfields[i, j].Add(new Chest(Convert.ToBoolean(file.Split('\n')[i+1].Split(';')[j].Split(',')[x].Split('-')[1]), i,j));
                            }
                            if (file.Split('\n')[i + 1].Split(';')[j].Split(',')[x].Split('-')[0] == "M")
                            {
                                Creature szorny = new Creature(i,j,this);
                                localfields[i , j].Add(szorny);
                                creatures.Add(szorny);
                            }
                            if (file.Split('\n')[i+1].Split(';')[j].Split(',')[x].Split('-')[0] == "B")
                            {
                                localfields[i, j].Add(Ghost.GetInstance());
                            }
                            if (file.Split('\n')[i + 1].Split(';')[j].Split(',')[x].Split('-')[0] == "bcu")
                            {
                                localfields[i, j].Add(BombCounterUp.GetInstance());
                            }
                            if (file.Split('\n')[i + 1].Split(';')[j].Split(',')[x].Split('-')[0] == "bsu")
                            {
                                localfields[i, j].Add(BombSizeUp.GetInstance());
                            }
                            if (file.Split('\n')[i + 1].Split(';')[j].Split(',')[x].Split('-')[0] == "d")
                            {
                                localfields[i, j].Add(Detonator.GetInstance());
                            }
                            if (file.Split('\n')[i + 1].Split(';')[j].Split(',')[x].Split('-')[0] == "sk")
                            {
                                localfields[i, j].Add(Skate.GetInstance());
                            }
                            if (file.Split('\n')[i + 1].Split(';')[j].Split(',')[x].Split('-')[0] == "i")
                            {
                                localfields[i, j].Add(Invulnerable.GetInstance());
                            }
                            if (file.Split('\n')[i + 1].Split(';')[j].Split(',')[x].Split('-')[0] == "g")
                            {
                                localfields[i, j].Add(Ghost.GetInstance());
                            }
                            if (file.Split('\n')[i + 1].Split(';')[j].Split(',')[x].Split('-')[0] == "cp")
                            {
                                localfields[i, j].Add(ChestPlant.GetInstance());
                            }
                            if (file.Split('\n')[i + 1].Split(';')[j].Split(',')[x].Split('-')[0] == "sl")
                            {
                                localfields[i, j].Add(Slow.GetInstance());
                            }
                            if (file.Split('\n')[i + 1].Split(';')[j].Split(',')[x].Split('-')[0] == "bsd")
                            {
                                localfields[i, j].Add(BombSizeDown.GetInstance());
                            }
                            if (file.Split('\n')[i + 1].Split(';')[j].Split(',')[x].Split('-')[0] == "bpp")
                            {
                                localfields[i, j].Add(BombPlantProblem.GetInstance());
                            }
                            if (file.Split('\n')[i + 1].Split(';')[j].Split(',')[x].Split('-')[0] == "ip")
                            {
                                localfields[i, j].Add(InstantPlant.GetInstance());
                            }
                        }
                    }
                    fields = new ConcurrentFieldMatrix(localfields);
                }

                //players data
                //player 0
                players = new List<Player>();
                string[] playerConstuctoreArray = file.Split('\n');
                Debug.WriteLine($"egesz: {file.Split('\n').Length}, jatekosok: {size+playersCount}");
                
                for (int i = size+1; i <= size+playersCount; i++)
                {
                    //Debug.WriteLine(i);
                    List<Boost> playerBoostsInFile = new List<Boost>(); //4
                    for (int pb = 0; pb < playerConstuctoreArray[i].Split('-')[4].Split('_').Length - 1; pb++)
                    {
                        switch (playerConstuctoreArray[i].Split('-')[4].Split('_')[pb])
                        {
                            case "bcu":playerBoostsInFile.Add(BombCounterUp.GetInstance()); break;
                            case "bsu":playerBoostsInFile.Add(BombSizeUp.GetInstance()); break;
                            case "d":playerBoostsInFile.Add(Detonator.GetInstance()); break;
                            case "sk":playerBoostsInFile.Add(Skate.GetInstance()); break;
                            case "i":playerBoostsInFile.Add(Invulnerable.GetInstance()); break;
                            case "g":playerBoostsInFile.Add(Ghost.GetInstance()); break;
                            case "cp":playerBoostsInFile.Add(ChestPlant.GetInstance()); break;
                            case "sl":playerBoostsInFile.Add(Slow.GetInstance()); break;
                            case "bsd": playerBoostsInFile.Add(BombSizeDown.GetInstance()); break;
                            case "bpp":playerBoostsInFile.Add(BombPlantProblem.GetInstance()); break;
                            case "ip":playerBoostsInFile.Add(InstantPlant.GetInstance()); break;
                            default: ;
                                break;
                        }
                    }
                    List<Bomb> playerBombsInFile = new List<Bomb>(); //5
                    for (int pb = 0; pb < playerConstuctoreArray[i].Split('-')[5].Split('_').Length-1; pb++)
                    {
                        playerBombsInFile.Add(new Bomb(Convert.ToInt32(playerConstuctoreArray[i].Split('-')[5].Split('_')[pb].Split(':')[0]), Convert.ToInt32(playerConstuctoreArray[i].Split('-')[5].Split('_')[pb].Split(':')[1]), Convert.ToInt32(playerConstuctoreArray[i].Split('-')[5].Split('_')[pb].Split(':')[2]), this));
                    }
                    //also add bombs to the field
                    for (int pb = 0; pb < playerBombsInFile.Count; pb++)
                    {
                        fields.AddFields(playerBombsInFile[pb].xPosition, playerBombsInFile[pb].yPosition, playerBombsInFile[pb]);
                    }

                    players.Add(new Player(Convert.ToInt32(playerConstuctoreArray[i].Split('-')[0]), Convert.ToInt32(playerConstuctoreArray[i].Split('-')[1]), Convert.ToInt32(playerConstuctoreArray[i].Split('-')[2]), Convert.ToBoolean(playerConstuctoreArray[i].Split('-')[3]), playerBoostsInFile, playerBombsInFile, Convert.ToInt32(playerConstuctoreArray[i].Split('-')[6]), Convert.ToInt32(playerConstuctoreArray[i].Split('-')[7]), Convert.ToInt32(playerConstuctoreArray[i].Split('-')[8]), Convert.ToInt32(playerConstuctoreArray[i].Split('-')[9]),this));
                    SetPlayerKeys(i-size-1, playerKeyBindings[i-size-1]);
                }

                for (int i = 0; i < players.Count; i++)
                {
                    localfields[players[i].xPosition, players[i].yPosition].Add(players[i]);
                }

                fields = new ConcurrentFieldMatrix(localfields);
            }
        }
        /// <summary>
        /// loads a map for the start of the round
        /// </summary>
        /// <param name="path"></param>
        /// <param name="activePlayerCount">number of players actually playing</param>
        /// <param name="playerKeyBindings"></param>
        /// <exception cref="Exception">thrown when map file does not contain 3 player data</exception>
        public void LoadStartMap(string path, int activePlayerCount, List<Dictionary<string, Direction>> playerKeyBindings)
        {
            
            Debug.WriteLine("load elkezd�dik.");
            ResetArrays();
            string? file = persistence.LoadFile(path);
            
            if (file != null)
            {
                //maps are ready for at most 3 players
                //save file includes the number of players, but that is for loading an ongoing game, not for a starting map
                //not used
                int playersCount = Convert.ToInt32(file.Split('\n')[0].Split(',')[0]); //so as not to mess up file reading
                if(playersCount != 3)
                {
                    throw new Exception($"Starting map can only have 3 players. In loaded file {path}, player count is {playersCount}");
                }

                size = Convert.ToInt32(file.Split('\n')[0].Split(',')[1]);
                List<Field>[,] localfields = new List<Field>[size, size];
                fields = new ConcurrentFieldMatrix(new List<Field>[size, size]);
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        localfields[i, j] = new List<Field>();
                        //only walls, chests, creatures can be on the starting map
                        for (int x = 0; x < file.Split('\n')[i + 1].Split(';')[j].Split(',').Length; x++)
                        {
                            if (file.Split('\n')[i + 1].Split(';')[j].Split(',')[x].Split('-')[0] == "W")
                            {
                                localfields[i, j].Add(Wall.GetInstance());
                            }
                            if (file.Split('\n')[i + 1].Split(';')[j].Split(',')[x].Split('-')[0] == "C")
                            {
                                localfields[i, j].Add(new Chest(Convert.ToBoolean(file.Split('\n')[i + 1].Split(';')[j].Split(',')[x].Split('-')[1]), i, j));
                            }
                            if (file.Split('\n')[i + 1].Split(';')[j].Split(',')[x].Split('-')[0] == "M")
                            {
                                Creature creature = new Creature(i, j, this);
                                localfields[i, j].Add(creature);
                                creatures.Add(creature);
                            }
                        }
                    }
                    fields = new ConcurrentFieldMatrix(localfields);
                }

                //players data
                players = new List<Player>();
                string[] playerConstuctoreArray = file.Split('\n');
                Debug.WriteLine($"egesz: {file.Split('\n').Length}, jatekosok: {size + activePlayerCount}");
                for (int i = size + 1; i <= size + playersCount; i++)
                {
                    //read 3rd player coordinates, but ignore it if playerCount is not 3
                    if(activePlayerCount != 3 && i == size + 2)
                    {
                        Debug.WriteLine("Player 3's starting position is ignored");
                        continue;
                    }
                    players.Add(new Player(Convert.ToInt32(playerConstuctoreArray[i].Split('-')[0]), Convert.ToInt32(playerConstuctoreArray[i].Split('-')[1]), Convert.ToInt32(playerConstuctoreArray[i].Split('-')[2]), Convert.ToBoolean(playerConstuctoreArray[i].Split('-')[3]), new List<Boost>(), new List<Bomb>(), Convert.ToInt32(playerConstuctoreArray[i].Split('-')[6]), Convert.ToInt32(playerConstuctoreArray[i].Split('-')[7]), Convert.ToInt32(playerConstuctoreArray[i].Split('-')[8]), Convert.ToInt32(playerConstuctoreArray[i].Split('-')[9]), this));
                }
                Debug.WriteLine("LoadStartMap - player count: " + players.Count);

                for (int i = 0; i < activePlayerCount; i++) 
                { 
                    localfields[players[i].xPosition, players[i].yPosition].Add(players[i]);
                }
                fields = new ConcurrentFieldMatrix(localfields);

            }
            playerCount = players.Count;
            for (int i = 0; i < playerCount; i++)
            {
                SetPlayerKeys(i, playerKeyBindings[i]);
            }
            ClearExplosion();
            OnFieldChanged();
        }
        /// <summary>
        /// Saves the current game state to a file.
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{players.Count},{size},{Rounds}\n");
            //fields data
            List<Field>[,] localfields = fields.GetFieldMatrix();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    for (int x = 0; x < localfields[i, j].Count; x++)
                    {
                        if (x > 0)
                            sb.Append(",");

                        if (localfields[i, j][x].GetType() == typeof(Wall))
                        {
                            sb.Append($"W");
                        }
                        if (localfields[i, j][x].GetType() == typeof(Creature))
                        {
                            sb.Append($"M");
                        }
                        if (localfields[i, j][x].GetType() == typeof(Chest))
                        {
                            Chest c = (Chest)localfields[i, j][x];
                            sb.Append($"C-{c.canDropLoot}");
                        }
                        if (localfields[i, j][x].GetType() == typeof(BombCounterUp))
                            sb.Append("bcu");
                        if (localfields[i, j][x].GetType() == typeof(BombSizeUp))
                            sb.Append("bsu");
                        if (localfields[i, j][x].GetType() == typeof(Detonator))
                            sb.Append("d");
                        if (localfields[i, j][x].GetType() == typeof(Skate))
                            sb.Append("sk");
                        if (localfields[i, j][x].GetType() == typeof(Invulnerable))
                            sb.Append("i");
                        if (localfields[i, j][x].GetType() == typeof(Ghost))
                            sb.Append("g");
                        if (localfields[i, j][x].GetType() == typeof(ChestPlant))
                            sb.Append("cp");
                        if (localfields[i, j][x].GetType() == typeof(Slow))
                            sb.Append("sl");
                        if (localfields[i, j][x].GetType() == typeof(BombSizeDown))
                            sb.Append("bsd");
                        if (localfields[i, j][x].GetType() == typeof(BombPlantProblem))
                            sb.Append("bpp");
                        if (localfields[i, j][x].GetType() == typeof(InstantPlant))
                            sb.Append("ip");
                    }
                    if(j != size - 1)
                        sb.Append(';');
                }
                sb.Append('\n');
            }
            //players data
            string player = "";
            for (int i = 0; i<players.Count;i++)
            {
                string boost = "";
                string bombs = "";
                for (int b = 0; b < players[i].boosts.Count; b++)
                {
                    if (players[i].boosts[b].GetType() == typeof(BombCounterUp))
                        boost += "bcu_";
                    if (players[i].boosts[b].GetType() == typeof(BombSizeUp))
                        boost += "bsu_";
                    if (players[i].boosts[b].GetType() == typeof(Detonator))
                        boost += "d_";
                    if (players[i].boosts[b].GetType() == typeof(Skate))
                        boost += "sk_";
                    if (players[i].boosts[b].GetType() == typeof(Invulnerable))
                        boost += "i_";
                    if (players[i].boosts[b].GetType() == typeof(Ghost))
                        boost += "g_";
                    if (players[i].boosts[b].GetType() == typeof(ChestPlant))
                        boost += "cp_";
                    if (players[i].boosts[b].GetType() == typeof(Slow))
                        boost += "sl_";
                    if (players[i].boosts[b].GetType() == typeof(BombSizeDown))
                        boost += "bsd_";
                    if (players[i].boosts[b].GetType() == typeof(BombPlantProblem))
                        boost += "bpp_";
                    if (players[i].boosts[b].GetType() == typeof(InstantPlant))
                        boost += "ip_";
                }
                for (int b = 0; b < players[i].bombs.Count; b++)
                {
                    bombs += players[i].bombs[b].detonationSize + ":" + players[i].bombs[b].xPosition + ":" + players[i].bombs[b].yPosition+"_";
                }
                
                player += $"{players[i].id}-{players[i].xPosition}-{players[i].yPosition}-{players[i].alive}-{boost}-{bombs}-{players[i].speed}-{players[i].bombCapacity}-{players[i].winCounter}-{players[i].detonationSize}\n";
            }
            sb.Append(player);

            persistence.SaveFile(sb.ToString(), path);
        }
        /// <summary>
        /// based on the player's keybindings, one of the players might take an action
        /// </summary>
        /// <param name="pressed">pressed key e.g. "K"</param>
        public void KeyPressed(String pressed) {
            foreach (Player player in players) {
                player.Keypressed(pressed, fields);
            }
            OnPlayerMoved(0,0,1,1);
        }
        /// <summary>
        /// sets a player's keybindings
        /// </summary>     
        /// <param name="playerID">player's id</param>
        /// <param name="keys">keybindings</param>
        public void SetPlayerKeys(int playerID, Dictionary<string, Direction> keys)
        {
            players[playerID].SetKeys(keys);
            
        }
        /// <summary>
        /// checks if the game/round is over
        /// </summary>
        public void IsGameOver() {
             int alivePlayers = 0;
            Player? winner = null;
            foreach (Player player in players)
            {
                if (player.alive) {
                    alivePlayers++;
                    winner = player;
                }
            }
            if (alivePlayers <= 1) { 
                if (winner != null) {
                    winner.winCounter++;
                    if(players.Any(p => p.winCounter == Rounds))
                    {
                        System.Threading.Thread.Sleep(2000);
                        OnGameOver(winner.id.ToString());
                        winNum1 = 0; winNum2 = 0; winNum3 = 0;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(2000);
                        Debug.WriteLine("j�t�kos: " + winner.winCounter);
                        OnRoundOver(winner.id.ToString());
                    }
                    Debug.WriteLine(WinCounter);
                    Debug.WriteLine("GameOVer");
                }
                //WinCounter++;

            }

            if (winner != null)
            {
               
            }
            DataLoad();
        }
        /// <summary>
        /// replaces an element on the field in editor mode
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        public void Click(int x, int y)
        {
            if (lastClick != 0)
            {
                if (fields.GetFieldMatrix()[x, y] is not null)
                {
                    foreach (Field field in fields.GetFieldMatrix()[x, y])
                    {
                        if (field is Player)
                        {
                            Player player = (Player)field;

                            players.Remove(player);
                        }
                        if (field is Creature)
                        {
                            Creature creature = (Creature)field;

                            creatures.Remove(creature);
                        }

                    }

                   

                    fields.Clear(x, y);
                }

                switch (lastClick)
                {
                    case 1:
                        bool shouldbreak = false;
                        foreach (Player player in players)
                        {
                            if (player.id == 0) { 
                               shouldbreak = true;
                            }
                        }
                        if (shouldbreak) break; 
                        Player player1 = new Player(0, x, y, true, new List<Boost>(), new List<Bomb>(), 1, 2, 0, 2, null, this);
                        players.Add(player1);
                        fields.AddFields(x,y, player1);

                        break;
                    case 2:
                        shouldbreak = false;
                        foreach (Player player in players)
                        {
                            if (player.id == 1)
                            {
                                shouldbreak = true;
                            }
                        }
                        if (shouldbreak) break;
                        player1 = new Player(1, x, y, true, new List<Boost>(), new List<Bomb>(), 1, 2, 0, 2, null, this);
                        players.Add(player1);
                        fields.AddFields(x, y, player1);
                        break;
                    case 3:
                        shouldbreak = false;
                        foreach (Player player in players)
                        {
                            if (player.id == 2)
                            {
                                shouldbreak = true;
                            }
                        }
                        if (shouldbreak) break;
                        player1 = new Player(2, x, y, true, new List<Boost>(), new List<Bomb>(), 1, 2, 0, 2, null, this);
                        players.Add(player1);
                        fields.AddFields(x, y, player1);
                        break;
                    case 4:
                        fields.AddFields(x, y, Wall.GetInstance());
                        break;
                    case 5:
                        Creature creature = new Creature(x, y, this);
                        creatures.Add(creature);
                        creature.CreatureTimerStopper();
                        fields.AddFields(x, y, creature);
                        break;
                    case 6:
                        fields.AddFields(x, y, new Chest(true, x, y));
                        break;
                    case 7:


                        
                        break;


                }
                
            }
            Debug.WriteLine(x +", "+y);
        }
        /// <summary>
        /// sets the type of element to be placed on the field in editor mode
        /// </summary>
        /// <param name="x">1-7, see the version with 2 arguments for what each does (spaghetti code)</param>
        public void Click(int x)
        {
            lastClick = x;
        }
        /// <summary>
        /// resets the fields and other arrays to be empty
        /// </summary>
        private void ResetArrays()
        {
            List<Field>[,] tmpfields = new List<Field>[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    tmpfields[i, j] = new List<Field>();
                }
            }
            fields = new ConcurrentFieldMatrix(tmpfields);
            
            players = new List<Player>();
            creatures = new List<Creature>();
        }
        /// <summary>
        /// saves rounds, playercount and win counters to a file
        /// </summary>
        public void DataSave()
        {
            string file;
            if (playerCount == 2)
            {
                file = $"{Rounds},{playerCount},{players[0].winCounter},{players[1].winCounter}";
            }
            else
            {
                file = $"{Rounds},{playerCount},{players[0].winCounter},{players[1].winCounter},{players[2].winCounter}";
            }

            File.WriteAllText("data.txt", file);
        }
        /// <summary>
        /// loads round, maybe playercount and win counters from a file
        /// </summary>
        public void DataLoad()
        {
            if (File.Exists("data.txt"))
            {
                Debug.WriteLine("L�tezik a f�jl");
                string file = File.ReadAllText("data.txt");
                if (file.Split(',').Length > 1)
                {
                    Rounds = Convert.ToInt32(file.Split(',')[0]);
                    playerCount = Convert.ToInt32(file.Split(',')[1]);
                    if(playerCount == 2)
                    {
                        players[0].winCounter = Convert.ToInt32(file.Split(',')[2]);
                        players[1].winCounter = Convert.ToInt32(file.Split(',')[3]);
                    }
                    else
                    {
                        players[0].winCounter = Convert.ToInt32(file.Split(',')[2]);
                        players[1].winCounter = Convert.ToInt32(file.Split(',')[3]);
                        players[2].winCounter = Convert.ToInt32(file.Split(',')[4]);
                    }
                }
                else
                {
                    Rounds = Convert.ToInt32(file);
                }
            }
        }
        
        private void ClearExplosion()
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (fields.GetFieldMatrix()[i, j].OfType<Explosion>().Any() || fields.GetFieldMatrix()[i, j].OfType<Bomb>().Any())
                    {
                        for (int l = 0; l < fields.GetFieldMatrix()[i, j].Count; l++)
                        {
                            if (fields.GetFieldMatrix()[i, j][l].GetType() == typeof(Explosion) || fields.GetFieldMatrix()[i, j][l].GetType() == typeof(Bomb))
                            {
                                fields.GetFieldMatrix()[i, j].Remove(fields.GetFieldMatrix()[i, j][l]);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Public event methods
        /// <summary>
        /// Event method for player movement
        /// </summary>
        /// <param name="oldX">original x position</param>
        /// <param name="oldY">original y position</param>
        /// <param name="newX">new x position</param>
        /// <param name="newY">new y position</param>
        public void OnPlayerMoved(int oldX, int oldY, int newX, int newY)
        {
            PlayerMoved?.Invoke(this, new MovementEventArgs(oldX, oldY, newX, newY));
        }
        /// <summary>
        /// Event method for boost change
        /// </summary>
        /// <param name="playerID">id of player affected</param>
        /// <param name="boostType">type of boost</param>
        /// <param name="isAdded">true/false</param>
        public void OnBoostChanged(int playerID, string boostType, bool isAdded)
        {
            BoostChanged?.Invoke(this, new BoostEventArgs(playerID,boostType, isAdded));
        }
        
        /// <summary>
        /// stops all creatures' movement
        /// </summary>
        public void CreaturesStop()
        {
            for (int i = 0; i < creatures.Count(); i++)
            {
                creatures[i].CreatureTimerStopper();
            }
        }
        /// <summary>
        /// starts all creatures' movement
        /// </summary>
        public void CreaturesStart()
        {
            for (int i = 0; i < creatures.Count(); i++)
            {
                creatures[i].CreatureTimerStarter();
            }
        }
        /// <summary>
        /// stops all bombs' timers
        /// </summary>
        public void BombsStop()
        {
            for (int i = 0; i < players[0].bombs.Count(); i++)
            {
                players[0].bombs[i].BombTimerStopper();
            }

            for (int i = 0; i < players[1].bombs.Count(); i++)
            {
                players[1].bombs[i].BombTimerStopper();
            }

            if(playerCount == 3)
            { 
                for (int i = 0; i < players[2].bombs.Count(); i++)
                {
                    players[2].bombs[i].BombTimerStopper();
                }
            }
        }
        /// <summary>
        /// starts all players' timers
        /// </summary>
        public void PlayerStopTimer()
        {
            foreach (Player player in players)
            {
                player.StopTimer();
            }
        }
        /// <summary>
        /// starts all bombs' timers
        /// </summary>
        public void BombsStart()
        {
            for (int i = 0; i < players[0].bombs.Count(); i++)
            {
                players[0].bombs[i].BombTimerStarter();
            }

            for (int i = 0; i < players[1].bombs.Count(); i++)
            {
                players[1].bombs[i].BombTimerStarter();
            }

            if (playerCount == 3)
            {
                for (int i = 0; i < players[2].bombs.Count(); i++)
                {
                    players[2].bombs[i].BombTimerStarter();
                }
            }
        }

        private void OnFieldChanged()
        {
            FieldChanged?.Invoke(this, new GameEventArgs(false, false, ""));
        }
        private void OnRoundOver(String playerName)
        {
            DataSave();
            ClearExplosion();
            RoundOver?.Invoke(this, new GameEventArgs(true, false, playerName));
        }

        private void OnGameOver(String playerName)
        {
            if (File.Exists("data.txt"))
            {
                File.Delete("data.txt");
            }
            GameOver?.Invoke(this, new GameEventArgs(false, true, playerName));
        }

        #endregion
    }
}
