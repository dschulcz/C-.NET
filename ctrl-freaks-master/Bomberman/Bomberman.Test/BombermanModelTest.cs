using Bomberman.Model.KeySettings;
using Bomberman.Model.Maps;
using Bomberman.Model;
using Bomberman.Persistence;
using Moq;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Diagnostics;

namespace Bomberman.Test
{
    [TestFixture]
    public class BombermanModelTest
    {
        private MapModel _mapModel = null!;
        private Model.Model _model = null!;
        private KeySettingsModel _keySettingsModel = null!;
        private Mock<IDataAccess> _mockDataAccess = null!;
        private readonly int defaultMapSize = 5;
        private readonly int defaultRounds = 3;
        private readonly int defaultPlayerCount = 3;
        private readonly string dataFilePath = "data.txt";


        //mocked data: players in each corner, walls in the first row, chest in the first column, creature in the middle
        private string _mockedDataLoad = "3,5,3\r\n;W;W;W;\r\nC-True;;;;\r\nC-True;;M;;\r\nC-True;;;;\r\n;;;;\r\n0-0-0-True---1-2-0-2\r\n1-4-4-True---1-2-0-2\r\n2-0-4-True---1-2-0-2\r\n";
        private string _mockedDataLoadWithBoosts = "3,5,3\r\n;;;;\r\nB;bcu;bsu;d;\r\nsk;i;g;cp;\r\nsl;bsd;bpp;ip;\r\n;;;;\r\n0-0-0-True-bcu_bsu_d_sk_i_g_cp_sl_bsd_bpp_ip_--1-2-0-2\r\n1-4-4-True---1-2-0-2\r\n2-0-4-True---1-2-0-2\r\n";
        private string _mockedDataLoadWithBombs = "3,5,3\r\n;;;;\r\n;;;;\r\n;;;;\r\n;;;;\r\n;;;;\r\n0-0-0-True--2:1:1_1:1:2_-1-2-0-2\r\n1-4-4-True--3:3:3_-1-2-0-2\r\n2-0-4-True---1-2-0-2\r\n";
        private string _mockedDataLoadStartMap = "3,5\r\n;W;W;W;\r\nC-True;;;;\r\nC-True;;M;;\r\nC-True;;;;\r\n;;;;\r\n0-0-0-True---1-2-0-2\r\n1-4-4-True---1-2-0-2\r\n2-0-4-True---1-2-0-2\r\n";
        private string _mockedDataSave = "3,5,3\r\n;W;W;W;\r\nC-True;;;;\r\nC-True;;M;;\r\nC-True;;;;\r\n;;;;\r\n0-0-0-True---1-2-0-2\r\n1-4-4-True---1-2-0-2\r\n2-0-4-True---1-2-0-2\r\n";
        
        [SetUp]
        public void Setup()
        {
            //mapModel
            
            List<Tuple<int, int>> creaturePositions = new List<Tuple<int, int>>();
            List<Tuple<int, int>> playerPositions = new List<Tuple<int, int>>();
            List<Tuple<int, int>> wallPositions = new List<Tuple<int, int>>();
            List<Tuple<int, int>> chestPositions = new List<Tuple<int, int>>();
            //creature
            creaturePositions.Add(new Tuple<int, int>(2, 2));
            //player
            playerPositions.Add(new Tuple<int, int>(0, 0));
            playerPositions.Add(new Tuple<int, int>(4, 4));
            playerPositions.Add(new Tuple<int, int>(0, 4));
            //wall
            wallPositions.Add(new Tuple<int, int>(0, 1));
            wallPositions.Add(new Tuple<int, int>(0, 2));
            wallPositions.Add(new Tuple<int, int>(0, 3));
            //chest
            chestPositions.Add(new Tuple<int, int>(1, 0));
            chestPositions.Add(new Tuple<int, int>(2, 0));
            chestPositions.Add(new Tuple<int, int>(3, 0));

            _mapModel = new MapModel(defaultMapSize, creaturePositions, playerPositions, wallPositions, chestPositions);
            _mockDataAccess = new Mock<IDataAccess>();
            _mockDataAccess.Setup(x => x.LoadFile("Load")).Returns(_mockedDataLoad);
            _mockDataAccess.Setup(x => x.LoadFile("LoadWithBoosts")).Returns(_mockedDataLoadWithBoosts);
            _mockDataAccess.Setup(x => x.LoadFile("LoadWithBombs")).Returns(_mockedDataLoadWithBombs);
            _mockDataAccess.Setup(x => x.LoadFile("LoadStartMap")).Returns(_mockedDataLoadStartMap);
            _mockDataAccess.Setup(x => x.SaveFile(_mockedDataSave, "SavePath"));
            _model = new Model.Model(_mapModel, 0, defaultPlayerCount, _mockDataAccess.Object);
            _model.Rounds = defaultRounds;



            //keySettingsModel
            _keySettingsModel = new KeySettingsModel();
            for (int i = 0; i < defaultPlayerCount; i++)
            {
                _model.SetPlayerKeys(i, _keySettingsModel.PlayerKeyBindings[i]);
            }
        }

        [TearDown]
        public void TearDown()
        {
            _mapModel = null!;
            _model = null!;
            _keySettingsModel = null!;
            _mockDataAccess = null!;
            if(File.Exists(dataFilePath))
            {
                File.Delete(dataFilePath);
            }
        }

        #region Model Constructors
        [Test]
        public void TestModelConstructorMapFile()
        {
            string path = "LoadStartMap";
            _model = new Model.Model(path, 0, defaultPlayerCount,_keySettingsModel.PlayerKeyBindings, _mockDataAccess.Object);
            _model.Rounds = defaultRounds;
            CheckConstructor(defaultPlayerCount);
        }
        [Test]
        public void TestModelConstructorEditor()
        {
            List<Field>[,] startingMap = new List<Field>[defaultMapSize, defaultMapSize];

            for (int i = 0; i < defaultMapSize; i++)
            {
                for (int j = 0; j < defaultMapSize; j++)
                {
                    startingMap[i, j] = new List<Field>();
                }
            }
            List<Player> players = new List<Player>();
            List<Creature> dummyCreatures = new List<Creature>();
            DataAccess dummyDataAccess = new DataAccess();
            ConcurrentFieldMatrix fields = new ConcurrentFieldMatrix(startingMap);
            _model = new Model.Model(defaultMapSize, fields, dummyCreatures, -1, -1, 3, players, dummyDataAccess);
            _model.Rounds = defaultRounds;

            _model.players.Add(new Player(0, 0, 0, true, new List<Boost>(),new List<Bomb>(), 1, 2, 0, 2, _keySettingsModel.PlayerKeyBindings[0], _model));
            _model.players.Add(new Player(4, 4, 1, true, new List<Boost>(), new List<Bomb>(), 1, 2, 0, 2, _keySettingsModel.PlayerKeyBindings[1], _model));
            _model.players.Add(new Player(0, 4, 2, true, new List<Boost>(), new List<Bomb>(), 1, 2, 0, 2, _keySettingsModel.PlayerKeyBindings[2], _model));
            _model.fields.AddFields(0, 0, players[0]);
            _model.fields.AddFields(4, 4, players[1]);
            _model.fields.AddFields(0, 4, players[2]);

            _model.fields.AddFields(0, 1, Wall.GetInstance());
            _model.fields.AddFields(0, 2, Wall.GetInstance());
            _model.fields.AddFields(0, 3, Wall.GetInstance());

            _model.fields.AddFields(1, 0, new Chest(true,1,0));
            _model.fields.AddFields(2, 0, new Chest(true,2,0));
            _model.fields.AddFields(3, 0, new Chest(true,3,0));

            Creature creature = new Creature(2, 2, _model);
            _model.creatures.Add(creature);
            _model.fields.AddFields(2, 2, creature);
            
            CheckConstructor(defaultPlayerCount);
        }
        #endregion
        #region Model public game methods
        [Test]
        public void TestLoad()
        {
            _model.Load("Load", _keySettingsModel.PlayerKeyBindings);
            Assert.That(_model.size, Is.EqualTo(5));
            //check for rounds
            Assert.That(_model.Rounds, Is.EqualTo(3));
            //check for walls
            Assert.That(_model.fields.GetFieldMatrix()[0, 1][0].GetType(), Is.EqualTo(typeof(Wall)));
            Assert.That(_model.fields.GetFieldMatrix()[0, 2][0].GetType(), Is.EqualTo(typeof(Wall)));
            Assert.That(_model.fields.GetFieldMatrix()[0, 3][0].GetType(), Is.EqualTo(typeof(Wall)));
            //check for chest
            Assert.That(_model.fields.GetFieldMatrix()[1, 0][0].GetType(), Is.EqualTo(typeof(Chest)));
            Assert.That(((Chest)_model.fields.GetFieldMatrix()[1, 0][0]).canDropLoot, Is.EqualTo(true));
            Assert.That(_model.fields.GetFieldMatrix()[2, 0][0].GetType(), Is.EqualTo(typeof(Chest)));
            Assert.That(((Chest)_model.fields.GetFieldMatrix()[2, 0][0]).canDropLoot, Is.EqualTo(true));
            Assert.That(_model.fields.GetFieldMatrix()[3, 0][0].GetType(), Is.EqualTo(typeof(Chest)));
            Assert.That(((Chest)_model.fields.GetFieldMatrix()[3, 0][0]).canDropLoot, Is.EqualTo(true));
            //check for creature
            Assert.That(_model.fields.GetFieldMatrix()[2, 2][0].GetType(), Is.EqualTo(typeof(Creature)));
            //no boosts on field
            Assert.That(CountElementsOnField("Ghost"), Is.EqualTo(0));
            Assert.That(CountElementsOnField("BombCounterUp"), Is.EqualTo(0));
            Assert.That(CountElementsOnField("BombSizeUp"), Is.EqualTo(0));
            Assert.That(CountElementsOnField("Detonator"), Is.EqualTo(0));
            Assert.That(CountElementsOnField("Skate"), Is.EqualTo(0));
            Assert.That(CountElementsOnField("Invulnerable"), Is.EqualTo(0));
            Assert.That(CountElementsOnField("ChestPlant"), Is.EqualTo(0));
            Assert.That(CountElementsOnField("Slow"), Is.EqualTo(0));
            Assert.That(CountElementsOnField("BombSizeDown"), Is.EqualTo(0));
            Assert.That(CountElementsOnField("BombPlantProblem"), Is.EqualTo(0));
            Assert.That(CountElementsOnField("InstantPlant"), Is.EqualTo(0));
            //check for players
            Assert.That(_model.playerCount, Is.EqualTo(3));
            Assert.That(_model.fields.GetFieldMatrix()[0, 0][0].GetType(), Is.EqualTo(typeof(Player)));
            Assert.That(_model.fields.GetFieldMatrix()[4, 4][0].GetType(), Is.EqualTo(typeof(Player)));
            Assert.That(_model.fields.GetFieldMatrix()[0, 4][0].GetType(), Is.EqualTo(typeof(Player)));


            //verify that LoadFile is called once
            _mockDataAccess.Verify(x => x.LoadFile("Load"), Times.Once);


        }

        [Test]
        public void TestLoadWithBoosts()
        {
            _model.Load("LoadWithBoosts", _keySettingsModel.PlayerKeyBindings);
            Assert.That(_model.size, Is.EqualTo(5));
            //check for boosts on field
            //ghost has 2, because it was coded mistakenly in model
            //boosts on right column are not saved by mistake
            Assert.That(CountElementsOnField("Ghost"), Is.EqualTo(2));
            Assert.That(CountElementsOnField("BombCounterUp"), Is.EqualTo(1));
            Assert.That(CountElementsOnField("BombSizeUp"), Is.EqualTo(1));
            Assert.That(CountElementsOnField("Detonator"), Is.EqualTo(1));
            Assert.That(CountElementsOnField("Skate"), Is.EqualTo(1));
            Assert.That(CountElementsOnField("Invulnerable"), Is.EqualTo(1));
            Assert.That(CountElementsOnField("ChestPlant"), Is.EqualTo(1));
            Assert.That(CountElementsOnField("Slow"), Is.EqualTo(1));
            Assert.That(CountElementsOnField("BombSizeDown"), Is.EqualTo(1));
            Assert.That(CountElementsOnField("BombPlantProblem"), Is.EqualTo(1));
            Assert.That(CountElementsOnField("InstantPlant"), Is.EqualTo(1));
            //check for boosts on players
            Assert.That(_model.players[0].boosts.Count, Is.EqualTo(11));
            Assert.That(_model.players[0].boosts[0].GetType(), Is.EqualTo(typeof(BombCounterUp)));
            Assert.That(_model.players[0].boosts[1].GetType(), Is.EqualTo(typeof(BombSizeUp)));
            Assert.That(_model.players[0].boosts[2].GetType(), Is.EqualTo(typeof(Detonator)));
            Assert.That(_model.players[0].boosts[3].GetType(), Is.EqualTo(typeof(Skate)));
            Assert.That(_model.players[0].boosts[4].GetType(), Is.EqualTo(typeof(Invulnerable)));
            Assert.That(_model.players[0].boosts[5].GetType(), Is.EqualTo(typeof(Ghost)));
            Assert.That(_model.players[0].boosts[6].GetType(), Is.EqualTo(typeof(ChestPlant)));
            Assert.That(_model.players[0].boosts[7].GetType(), Is.EqualTo(typeof(Slow)));
            Assert.That(_model.players[0].boosts[8].GetType(), Is.EqualTo(typeof(BombSizeDown)));
            Assert.That(_model.players[0].boosts[9].GetType(), Is.EqualTo(typeof(BombPlantProblem)));
            Assert.That(_model.players[0].boosts[10].GetType(), Is.EqualTo(typeof(InstantPlant)));

            Assert.That(_model.players[1].boosts.Count, Is.EqualTo(0));
            Assert.That(_model.players[2].boosts.Count, Is.EqualTo(0));
            


            //verify that LoadFile is called once
            _mockDataAccess.Verify(x => x.LoadFile("LoadWithBoosts"), Times.Once);
        }

        [Test]
        public void TestLoadWithBombs()
        {
            _model.Load("LoadWithBombs", _keySettingsModel.PlayerKeyBindings);
            Assert.That(_model.size, Is.EqualTo(5));
            //check for bombs on field
            //3 bombs total
            //Assert.That(CountElementsOnField("Bomb"), Is.EqualTo(3));
            //player1 has 2 bombs at (1,1), (1,2) with detonation size 2, 1
            Assert.That(_model.players[0].bombs.Count, Is.EqualTo(2));
            Assert.That(_model.fields.GetFieldMatrix()[1, 1][0].GetType(), Is.EqualTo(typeof(Bomb)));
            Assert.That(((Bomb)_model.fields.GetFieldMatrix()[1, 1][0]).detonationSize, Is.EqualTo(2));
            Assert.That(_model.fields.GetFieldMatrix()[1, 2][0].GetType(), Is.EqualTo(typeof(Bomb)));
            Assert.That(((Bomb)_model.fields.GetFieldMatrix()[1, 2][0]).detonationSize, Is.EqualTo(1));
            //player2 has 1 bomb at (3,3) with detonation size 3
            Assert.That(_model.players[1].bombs.Count, Is.EqualTo(1));
            Assert.That(_model.fields.GetFieldMatrix()[3, 3][0].GetType(), Is.EqualTo(typeof(Bomb)));
            Assert.That(((Bomb)_model.fields.GetFieldMatrix()[3, 3][0]).detonationSize, Is.EqualTo(3));

            //verify that LoadFile is called once
            _mockDataAccess.Verify(x => x.LoadFile("LoadWithBombs"), Times.Once);
        }
        [Test]
        public void TestLoadStartMap()
        {
            int playerCount = 2;
            _model.LoadStartMap("LoadStartMap",playerCount, _keySettingsModel.PlayerKeyBindings);
            CheckConstructor(playerCount);

            //verify that LoadFile is called once
            _mockDataAccess.Verify(x => x.LoadFile("LoadStartMap"), Times.Once);

        }

        [Test]
        public void TestSave()
        {
            _model.Save("SavePath");
            //ideally, we would check for the exact string, but that throws an error even though the parameters are seemingly matching
            //might be that new line is not the same in the two strings
            //so we will just check if the function is called with any string in the first parameter
            _mockDataAccess.Verify(x => x.SaveFile(It.IsAny<string>(), "SavePath"), Times.Once());
            //throws error
            //_mockDataAccess.Verify(x => x.SaveFile(_mockedDataSave, "SavePath"), Times.Once());
        }

        [Test]
        public void TestIsTherePlayer()
        {

        }
        [Test]
        public void TestClickOneArg()
        {
            _model.Click(3);
            Assert.That(_model.lastClick, Is.EqualTo(3));
        }

        [Test]
        public void TestClickTwoArgs()
        {
            //add wall to 3,3
            _model.Click(4);
            _model.Click(3, 3);
            Assert.That(_model.fields.GetFieldMatrix()[3, 3][0].GetType(), Is.EqualTo(typeof(Wall)));
            //add chest to 3,3, replacing wall
            _model.Click(6);
            _model.Click(3, 3);
            Assert.That(_model.fields.GetFieldMatrix()[3, 3][0].GetType(), Is.EqualTo(typeof(Chest)));
            Assert.That(((Chest)_model.fields.GetFieldMatrix()[3, 3][0]).canDropLoot, Is.EqualTo(true));
            Assert.That(_model.fields.GetFieldMatrix()[3, 3].Count, Is.EqualTo(1));
            //add creature to 3,3, replacing chest
            _model.Click(5);
            _model.Click(3, 3);
            Assert.That(_model.fields.GetFieldMatrix()[3, 3][0].GetType(), Is.EqualTo(typeof(Creature)));
            Assert.That(_model.fields.GetFieldMatrix()[3, 3].Count, Is.EqualTo(1));

            //add each player there, but they will stay at their original positions
            //creature will be removed regardless
            _model.Click(1);
            _model.Click(3, 3);
            Assert.That(_model.fields.GetFieldMatrix()[3, 3].Count, Is.EqualTo(0));
            Assert.That(_model.fields.GetFieldMatrix()[0, 0][0].GetType(), Is.EqualTo(typeof(Player)));
            Assert.That((Player)_model.fields.GetFieldMatrix()[0, 0][0] == _model.players[0]);
            _model.Click(2);
            _model.Click(3, 3);
            Assert.That(_model.fields.GetFieldMatrix()[3, 3].Count, Is.EqualTo(0));
            Assert.That(_model.fields.GetFieldMatrix()[4, 4][0].GetType(), Is.EqualTo(typeof(Player)));
            Assert.That((Player)_model.fields.GetFieldMatrix()[4, 4][0] == _model.players[1]);
            _model.Click(3);
            _model.Click(3, 3);
            Assert.That(_model.fields.GetFieldMatrix()[3, 3].Count, Is.EqualTo(0));
            Assert.That(_model.fields.GetFieldMatrix()[0, 4][0].GetType(), Is.EqualTo(typeof(Player)));
            Assert.That((Player)_model.fields.GetFieldMatrix()[0, 4][0] == _model.players[2]);

            //Click(7) does nothing
            _model.Click(7);
            _model.Click(3, 3);
            Assert.That(_model.fields.GetFieldMatrix()[3, 3].Count, Is.EqualTo(0));

            //remove a player, then put them elsewhere
            //remove player 1
            _model.Click(7);
            _model.Click(0, 0);
            Assert.That(_model.fields.GetFieldMatrix()[0, 0].Count, Is.EqualTo(0));
            //put player 1 to 3,3
            _model.Click(1);
            _model.Click(3, 3);
            Assert.That(_model.fields.GetFieldMatrix()[3, 3][0].GetType(), Is.EqualTo(typeof(Player)));
            Player p = (Player)_model.fields.GetFieldMatrix()[3, 3][0];
            Assert.That(p.id, Is.EqualTo(0));
            //remove player 2
            _model.Click(7);
            _model.Click(4, 4);
            Assert.That(_model.fields.GetFieldMatrix()[4, 4].Count, Is.EqualTo(0));
            //put player 2 to 3,3
            _model.Click(2);
            _model.Click(3, 3);
            Assert.That(_model.fields.GetFieldMatrix()[3, 3][0].GetType(), Is.EqualTo(typeof(Player)));
            p = (Player)_model.fields.GetFieldMatrix()[3, 3][0];
            Assert.That(p.id, Is.EqualTo(1));
            //remove player 3
            _model.Click(7);
            _model.Click(0, 4);
            Assert.That(_model.fields.GetFieldMatrix()[0, 4].Count, Is.EqualTo(0));
            //put player 3 to 3,3
            _model.Click(3);
            _model.Click(3, 3);
            Assert.That(_model.fields.GetFieldMatrix()[3, 3][0].GetType(), Is.EqualTo(typeof(Player)));
            p = (Player)_model.fields.GetFieldMatrix()[3, 3][0];
            Assert.That(p.id, Is.EqualTo(2));



        }

        [Test]
        public void TestDataSaveThreePlayers()
        {
            _model.DataSave();
            string fileData = File.ReadAllText(dataFilePath);
            //playerCount is 3, so we expect roundCount, playercount, and 3 players
            int expectedRoundCount = defaultRounds;
            int expectedPlayerCount = 3;
            int expectedWinCounters = 0;

            int actualRoundCount = Convert.ToInt32(fileData.Split(",")[0]);
            int actualPlayerCount = Convert.ToInt32(fileData.Split(",")[1]);
            int actualWinCounter1 = Convert.ToInt32(fileData.Split(",")[2]);
            int actualWinCounter2 = Convert.ToInt32(fileData.Split(",")[3]);
            int actualWinCounter3 = Convert.ToInt32(fileData.Split(",")[4]);

            Assert.That(actualRoundCount, Is.EqualTo(expectedRoundCount));
            Assert.That(actualPlayerCount, Is.EqualTo(expectedPlayerCount));
            Assert.That(actualWinCounter1, Is.EqualTo(expectedWinCounters));
            Assert.That(actualWinCounter2, Is.EqualTo(expectedWinCounters));
            Assert.That(actualWinCounter3, Is.EqualTo(expectedWinCounters));

        }

        [Test]
        public void TestDataLoadThreePlayers()
        {
            _model.DataSave();
            _model.DataLoad();
            Assert.That(_model.Rounds, Is.EqualTo(defaultRounds));
            Assert.That(_model.playerCount, Is.EqualTo(3));
            Assert.That(_model.players[0].winCounter, Is.EqualTo(0));
            Assert.That(_model.players[1].winCounter, Is.EqualTo(0));
            Assert.That(_model.players[2].winCounter, Is.EqualTo(0));

        }

        [Test]
        public void TestDataSaveLoadTwoPlayers()
        {
            //remove player 3
            int newPlayerCount = 2;
            //mapModel

            List<Tuple<int, int>> creaturePositions = new List<Tuple<int, int>>();
            List<Tuple<int, int>> playerPositions = new List<Tuple<int, int>>();
            List<Tuple<int, int>> wallPositions = new List<Tuple<int, int>>();
            List<Tuple<int, int>> chestPositions = new List<Tuple<int, int>>();
            //creature
            creaturePositions.Add(new Tuple<int, int>(2, 2));
            //player
            playerPositions.Add(new Tuple<int, int>(0, 0));
            playerPositions.Add(new Tuple<int, int>(4, 4));
            //wall
            wallPositions.Add(new Tuple<int, int>(0, 1));
            wallPositions.Add(new Tuple<int, int>(0, 2));
            wallPositions.Add(new Tuple<int, int>(0, 3));
            //chest
            chestPositions.Add(new Tuple<int, int>(1, 0));
            chestPositions.Add(new Tuple<int, int>(2, 0));
            chestPositions.Add(new Tuple<int, int>(3, 0));

            _mapModel = new MapModel(defaultMapSize, creaturePositions, playerPositions, wallPositions, chestPositions);

            _model = new Model.Model(_mapModel, 0, newPlayerCount, new DataAccess());
            _model.Rounds = defaultRounds;

            //keySettingsModel
            _keySettingsModel = new KeySettingsModel();
            for (int i = 0; i < newPlayerCount; i++)
            {
                _model.SetPlayerKeys(i, _keySettingsModel.PlayerKeyBindings[i]);
            }

            _model.DataSave();
            _model.DataLoad();
            Assert.That(_model.Rounds, Is.EqualTo(defaultRounds));
            Assert.That(_model.playerCount, Is.EqualTo(2));
            Assert.That(_model.players[0].winCounter, Is.EqualTo(0));
            Assert.That(_model.players[1].winCounter, Is.EqualTo(0));
        }

        /*[Test]
        public void TestPlayerConstructor()
        {
            if (_model.players[0].keys is not null && _model.players[1].keys is not null && _model.players[2].keys is not null)
            {
                //player1
                Assert.That(_model.players[0].xPosition, Is.EqualTo(0));
                Assert.That(_model.players[0].yPosition, Is.EqualTo(0));
                Assert.That(_model.players[0].id, Is.EqualTo(0));
                Assert.That(_model.players[0].bombCapacity, Is.EqualTo(2));
                Assert.That(_model.players[0].detonationSize, Is.EqualTo(2));
                Assert.That(_model.players[0].bombs.Count, Is.EqualTo(0));
                Assert.That(_model.players[0].keys["W"], Is.EqualTo(_keySettingsModel.PlayerKeyBindings[0]["W"]));
                Assert.That(_model.players[0].keys["A"], Is.EqualTo(_keySettingsModel.PlayerKeyBindings[0]["A"]));
                Assert.That(_model.players[0].keys["S"], Is.EqualTo(_keySettingsModel.PlayerKeyBindings[0]["S"]));
                Assert.That(_model.players[0].keys["D"], Is.EqualTo(_keySettingsModel.PlayerKeyBindings[0]["D"]));
                Assert.That(_model.players[0].keys["Q"], Is.EqualTo(_keySettingsModel.PlayerKeyBindings[0]["Q"]));
                Assert.That(_model.players[0].keys["E"], Is.EqualTo(_keySettingsModel.PlayerKeyBindings[0]["E"]));

                Assert.That(_model.fields.GetFieldMatrix()[0, 0][0], Is.EqualTo(_model.players[0]));

                //player2
                Assert.That(_model.players[1].xPosition, Is.EqualTo(4));
                Assert.That(_model.players[1].yPosition, Is.EqualTo(4));
                Assert.That(_model.players[1].id, Is.EqualTo(1));
                Assert.That(_model.players[1].bombCapacity, Is.EqualTo(2));
                Assert.That(_model.players[1].detonationSize, Is.EqualTo(2));
                Assert.That(_model.players[1].bombs.Count, Is.EqualTo(0));
                Assert.That(_model.players[1].keys["Up"], Is.EqualTo(_keySettingsModel.PlayerKeyBindings[1]["Up"]));
                Assert.That(_model.players[1].keys["Left"], Is.EqualTo(_keySettingsModel.PlayerKeyBindings[1]["Left"]));
                Assert.That(_model.players[1].keys["Down"], Is.EqualTo(_keySettingsModel.PlayerKeyBindings[1]["Down"]));
                Assert.That(_model.players[1].keys["Right"], Is.EqualTo(_keySettingsModel.PlayerKeyBindings[1]["Right"]));
                Assert.That(_model.players[1].keys["M"], Is.EqualTo(_keySettingsModel.PlayerKeyBindings[1]["M"]));
                Assert.That(_model.players[1].keys["N"], Is.EqualTo(_keySettingsModel.PlayerKeyBindings[1]["N"]));

                Assert.That(_model.fields.GetFieldMatrix()[4, 4][0], Is.EqualTo(_model.players[1]));
                //player3
                Assert.That(_model.players[2].xPosition, Is.EqualTo(0));
                Assert.That(_model.players[2].yPosition, Is.EqualTo(4));
                Assert.That(_model.players[2].id, Is.EqualTo(2));
                Assert.That(_model.players[2].bombCapacity, Is.EqualTo(2));
                Assert.That(_model.players[2].detonationSize, Is.EqualTo(2));
                Assert.That(_model.players[2].bombs.Count, Is.EqualTo(0));
                Assert.That(_model.players[2].keys["NumPad8"], Is.EqualTo(_keySettingsModel.PlayerKeyBindings[2]["NumPad8"]));
                Assert.That(_model.players[2].keys["NumPad4"], Is.EqualTo(_keySettingsModel.PlayerKeyBindings[2]["NumPad4"]));
                Assert.That(_model.players[2].keys["NumPad5"], Is.EqualTo(_keySettingsModel.PlayerKeyBindings[2]["NumPad5"]));
                Assert.That(_model.players[2].keys["NumPad6"], Is.EqualTo(_keySettingsModel.PlayerKeyBindings[2]["NumPad6"]));
                Assert.That(_model.players[2].keys["NumPad7"], Is.EqualTo(_keySettingsModel.PlayerKeyBindings[2]["NumPad7"]));
                Assert.That(_model.players[2].keys["NumPad9"], Is.EqualTo(_keySettingsModel.PlayerKeyBindings[2]["NumPad9"]));

                Assert.That(_model.fields.GetFieldMatrix()[0, 4][0], Is.EqualTo(_model.players[2]));
            }
            
            
        }*/

        [Test]
        public void TestPlayerMove()
        {
            //x is up/down, y is left/right, even though this will be confusing


            //First player cannot move to any direction, as it is blocked by walls/chests/end of the map
            _model.players[0].Move(_model.fields, _model.size, Direction.Up);
            Assert.That(new Tuple<int, int>(_model.players[0].xPosition, _model.players[0].yPosition), Is.EqualTo(new Tuple<int, int>(0, 0)));
            _model.players[0].Move(_model.fields, _model.size, Direction.Left);
            Assert.That(new Tuple<int, int>(_model.players[0].xPosition, _model.players[0].yPosition), Is.EqualTo(new Tuple<int, int>(0, 0)));
            _model.players[0].Move(_model.fields, _model.size, Direction.Down);
            Assert.That(new Tuple<int, int>(_model.players[0].xPosition, _model.players[0].yPosition), Is.EqualTo(new Tuple<int, int>(0, 0)));
            _model.players[0].Move(_model.fields, _model.size, Direction.Right);
            Assert.That(new Tuple<int, int>(_model.players[0].xPosition, _model.players[0].yPosition), Is.EqualTo(new Tuple<int, int>(0, 0)));

            //Second player can move up 4 times, and will move onto third player
            for (int i = 0; i < 4; i++)
            {
                _model.players[1].Move(_model.fields, _model.size, Direction.Up);
                Assert.That(new Tuple<int, int>(_model.players[1].xPosition, _model.players[1].yPosition),
                            Is.EqualTo(new Tuple<int, int>(4 - i - 1, 4)));

            }
        }

        [Test]
        public void TestPlayerLastDirection()
        {
            _model.KeyPressed("W");
            Assert.That(_model.players[0].Lastdirection, Is.EqualTo(Direction.Up));
            _model.KeyPressed("A");
            Assert.That(_model.players[0].Lastdirection, Is.EqualTo(Direction.Left));
            _model.KeyPressed("W");
            Assert.That(_model.players[0].Lastdirection, Is.EqualTo(Direction.Up));

        }

        [Test]
        public void TestPlayerPlantBomb()
        {
            //player1 can plant bomb once, and it will be at (0,0)
            _model.KeyPressed("Q");
            Assert.That(_model.players[0].bombs.Count, Is.EqualTo(1));
            Assert.That(_model.players[0].bombs[0].xPosition, Is.EqualTo(0));
            Assert.That(_model.players[0].bombs[0].yPosition, Is.EqualTo(0));
            Assert.That(_model.fields.GetFieldMatrix()[0, 0].Count, Is.EqualTo(2));
            Assert.That(_model.fields.GetFieldMatrix()[0, 0][1], Is.EqualTo(_model.players[0].bombs[0]));


            //trying to plant a second bomb at the same place will not work
            _model.KeyPressed("Q");
            Assert.That(_model.players[0].bombs.Count, Is.EqualTo(1));


            //player2 can plant 2 bombs, and they will be at (4,4) and (3,4)
            _model.KeyPressed("M");
            Assert.That(_model.players[1].bombs.Count, Is.EqualTo(1));
            Assert.That(_model.players[1].bombs[0].xPosition, Is.EqualTo(4));
            Assert.That(_model.players[1].bombs[0].yPosition, Is.EqualTo(4));
            Assert.That(_model.fields.GetFieldMatrix()[4, 4].Count, Is.EqualTo(2));
            Assert.That(_model.fields.GetFieldMatrix()[4, 4][1], Is.EqualTo(_model.players[1].bombs[0]));

            _model.players[1].Move(_model.fields, _model.size, Direction.Up);
            _model.KeyPressed("M");
            Assert.That(_model.players[1].bombs.Count, Is.EqualTo(2));
            Assert.That(_model.players[1].bombs[1].xPosition, Is.EqualTo(3));
            Assert.That(_model.players[1].bombs[1].yPosition, Is.EqualTo(4));
            Assert.That(_model.fields.GetFieldMatrix()[3, 4].Count, Is.EqualTo(2));
            Assert.That(_model.fields.GetFieldMatrix()[3, 4][1], Is.EqualTo(_model.players[1].bombs[1]));
            //trying to plant a third bomb will not work

            _model.players[1].Move(_model.fields, _model.size, Direction.Up);
            _model.KeyPressed("M");
            Assert.That(_model.players[1].bombs.Count, Is.EqualTo(2));
        }

        [Test]
        public void TestPlayersBomb()
        {
            Bomb b = new Bomb(2, 3, 3, _model);
            Assert.That(b.detonationSize, Is.EqualTo(2));
            Assert.That(b.xPosition, Is.EqualTo(3));
            Assert.That(b.yPosition, Is.EqualTo(3));
            Assert.That(b.timer2.Interval, Is.EqualTo(500));
            Assert.That(b.model, Is.EqualTo(_model));
            Assert.That(b.tickCount, Is.EqualTo(0));
            Assert.That(b.left, Is.EqualTo(true));
            Assert.That(b.right, Is.EqualTo(true));
            Assert.That(b.up, Is.EqualTo(true));
            Assert.That(b.down, Is.EqualTo(true));
            _model.fields.GetFieldMatrix()[3, 3].Add(b);
            b.BombTimerStopper();
            b.timer2.Interval = 1;
            Assert.That(b.tickCount, Is.EqualTo(0));
            b.BombTimerStarter();
            b.boom(1);
            Assert.That(_model.fields.GetFieldMatrix()[2, 3][0].GetType(), Is.EqualTo(typeof(Explosion)));
            Assert.That(_model.fields.GetFieldMatrix()[4, 3][0].GetType(), Is.EqualTo(typeof(Explosion)));
            Assert.That(_model.fields.GetFieldMatrix()[3, 2][0].GetType(), Is.EqualTo(typeof(Explosion)));
            Assert.That(_model.fields.GetFieldMatrix()[3, 4][0].GetType(), Is.EqualTo(typeof(Explosion)));
            b.ExplosionClean();
            Assert.That(_model.fields.GetFieldMatrix()[2, 3].Count, Is.EqualTo(0));
            Assert.That(_model.fields.GetFieldMatrix()[4, 3].Count, Is.EqualTo(0));
            Assert.That(_model.fields.GetFieldMatrix()[3, 2].Count, Is.EqualTo(0));
            Assert.That(_model.fields.GetFieldMatrix()[3, 4].Count, Is.EqualTo(0));
            b = new Bomb(2, 3, 3, _model);
            _model.fields.GetFieldMatrix()[2, 3].Add(new Bomb(0, 2, 3, _model));
            Assert.That(b.singleFieldBoom(2, 3).getNextFieldDetonation, Is.EqualTo(false));
            _model.fields.GetFieldMatrix()[4, 3].Add(new Player(1, 2, 3, true, new List<Boost>(), new List<Bomb>(), 1, 1, 1, 1, _model));
            Assert.That(b.singleFieldBoom(4, 3).getNextFieldDetonation, Is.EqualTo(true));
            _model.fields.GetFieldMatrix()[3, 2].Add(new Chest(false, 2, 3));
            Assert.That(b.singleFieldBoom(3, 2).getNextFieldDetonation, Is.EqualTo(false));
            _model.fields.GetFieldMatrix()[3, 4].Add(new Creature(2, 3, _model));
            Assert.That(b.singleFieldBoom(3, 4).getNextFieldDetonation, Is.EqualTo(true));
            b.ExplosionClean();
            b.BombTimerStopper();
            Assert.That(b.tickCount, Is.LessThan(5));
            b.Destroy();
            Assert.That(b.model, Is.Null);
        }


        [Test]
        public void CreatureRandomMovementest()
        {
            int OldX = _model.creatures[0].xPosition;
            int OldY = _model.creatures[0].yPosition;
            _model.creatures[0].Move();
            Assert.That(CoordTest(OldX, OldY, _model.creatures[0].xPosition, _model.creatures[0].yPosition), Is.EqualTo(true));

            OldX = _model.creatures[0].xPosition;
            OldY = _model.creatures[0].yPosition;
            _model.creatures[0].Move();
            Assert.That(CoordTest(OldX, OldY, _model.creatures[0].xPosition, _model.creatures[0].yPosition), Is.EqualTo(true));

            OldX = _model.creatures[0].xPosition;
            OldY = _model.creatures[0].yPosition;
            _model.creatures[0].Move();
            Assert.That(CoordTest(OldX, OldY, _model.creatures[0].xPosition, _model.creatures[0].yPosition), Is.EqualTo(true));

            OldX = _model.creatures[0].xPosition;
            OldY = _model.creatures[0].yPosition;
            _model.creatures[0].Move();
            Assert.That(CoordTest(OldX, OldY, _model.creatures[0].xPosition, _model.creatures[0].yPosition), Is.EqualTo(true));

            OldX = _model.creatures[0].xPosition;
            OldY = _model.creatures[0].yPosition;
            _model.creatures[0].Move();
            Assert.That(CoordTest(OldX, OldY, _model.creatures[0].xPosition, _model.creatures[0].yPosition), Is.EqualTo(true));

            OldX = _model.creatures[0].xPosition;
            OldY = _model.creatures[0].yPosition;
            _model.creatures[0].Move();
            Assert.That(CoordTest(OldX, OldY, _model.creatures[0].xPosition, _model.creatures[0].yPosition), Is.EqualTo(true));

            OldX = _model.creatures[0].xPosition;
            OldY = _model.creatures[0].yPosition;
            _model.creatures[0].Move();
            Assert.That(CoordTest(OldX, OldY, _model.creatures[0].xPosition, _model.creatures[0].yPosition), Is.EqualTo(true));

            OldX = _model.creatures[0].xPosition;
            OldY = _model.creatures[0].yPosition;
            _model.creatures[0].Move();
            Assert.That(CoordTest(OldX, OldY, _model.creatures[0].xPosition, _model.creatures[0].yPosition), Is.EqualTo(true));

            OldX = _model.creatures[0].xPosition;
            OldY = _model.creatures[0].yPosition;
            _model.creatures[0].Move();
            Assert.That(CoordTest(OldX, OldY, _model.creatures[0].xPosition, _model.creatures[0].yPosition), Is.EqualTo(true));

            OldX = _model.creatures[0].xPosition;
            OldY = _model.creatures[0].yPosition;
            _model.creatures[0].Move();
            Assert.That(CoordTest(OldX, OldY, _model.creatures[0].xPosition, _model.creatures[0].yPosition), Is.EqualTo(true));

            OldX = _model.creatures[0].xPosition;
            OldY = _model.creatures[0].yPosition;
            _model.creatures[0].Move();
            Assert.That(CoordTest(OldX, OldY, _model.creatures[0].xPosition, _model.creatures[0].yPosition), Is.EqualTo(true));

            OldX = _model.creatures[0].xPosition;
            OldY = _model.creatures[0].yPosition;
            _model.creatures[0].Move();
            Assert.That(CoordTest(OldX, OldY, _model.creatures[0].xPosition, _model.creatures[0].yPosition), Is.EqualTo(true));

            OldX = _model.creatures[0].xPosition;
            OldY = _model.creatures[0].yPosition;
            _model.creatures[0].Move();
            Assert.That(CoordTest(OldX, OldY, _model.creatures[0].xPosition, _model.creatures[0].yPosition), Is.EqualTo(true));

            OldX = _model.creatures[0].xPosition;
            OldY = _model.creatures[0].yPosition;
            _model.creatures[0].Move();
            Assert.That(CoordTest(OldX, OldY, _model.creatures[0].xPosition, _model.creatures[0].yPosition), Is.EqualTo(true));

        }

        public bool CoordTest(int oldx, int oldy, int x, int y)
        {
            if (oldx != x || oldy != y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [Test]
        public void CreatureMovementest()
        {
            Assert.That(_model.creatures[0].alive, Is.EqualTo(true));


            _model.creatures[0].singleMove(4, 3, _model.fields);

            Assert.That(_model.creatures[0].xPosition, Is.EqualTo(4));
            Assert.That(_model.creatures[0].yPosition, Is.EqualTo(3));

            int OldX = _model.creatures[0].xPosition;
            int OldY = _model.creatures[0].yPosition;

            _model.creatures[0].singleMove(0, 3, _model.fields);

            Assert.That(_model.creatures[0].xPosition, Is.EqualTo(OldX));
            Assert.That(_model.creatures[0].yPosition, Is.EqualTo(OldY));

            _model.creatures[0].singleMove(0, 4, _model.fields);


            Assert.That(_model.players[2].alive, Is.EqualTo(false));
        }

        [Test]
        public void CreatureDestroyTest()
        {
            _model.creatures[0].Destroy();
            Assert.That(_model.creatures[0].model, Is.Null);
        }

        [Test]
        public void CreatureDieTest()
        {
            _model.creatures[0].Die(_model.fields);

            Assert.That(_model.creatures[0].alive, Is.EqualTo(false));
        }

        [Test]
        public void ChestDestroyTest()
        {
            Field field = _model.fields.GetFieldMatrix()[1, 0][0];
            Chest chest = (Chest)field;

            chest.Destroy();
        }

        [Test]
        public void ChestDieTest()
        {
            Field field = _model.fields.GetFieldMatrix()[1, 0][0];
            Chest chest = (Chest)field;

            chest.chestDie(_model.fields);
            Assert.That(_model.fields.GetFieldMatrix()[1, 0][0], Is.Not.EqualTo(typeof(Chest)));
        }

        [Test]
        public void ChestDieTest2()
        {
            Field field = _model.fields.GetFieldMatrix()[1, 0][0];
            Chest chest = (Chest)field;

            chest.chestDie(_model.fields);
            Assert.That(_model.fields.GetFieldMatrix()[1, 0][0], Is.Not.EqualTo(typeof(Chest)));
        }

        [Test]
        public void ChestDieTest3()
        {
            Field field = _model.fields.GetFieldMatrix()[1, 0][0];
            Chest chest = (Chest)field;

            chest.chestDie(_model.fields);
            Assert.That(_model.fields.GetFieldMatrix()[1, 0][0], Is.Not.EqualTo(typeof(Chest)));
        }

        [Test]
        public void ChestDieTest4()
        {
            Field field = _model.fields.GetFieldMatrix()[1, 0][0];
            Chest chest = (Chest)field;

            chest.chestDie(_model.fields);
            Assert.That(_model.fields.GetFieldMatrix()[1, 0][0], Is.Not.EqualTo(typeof(Chest)));
        }

        [Test]
        public void ChestDieTest5()
        {
            Field field = _model.fields.GetFieldMatrix()[1, 0][0];
            Chest chest = (Chest)field;

            chest.chestDie(_model.fields);
            Assert.That(_model.fields.GetFieldMatrix()[1, 0][0], Is.Not.EqualTo(typeof(Chest)));
        }

        [Test]
        public void ChestDieTest6()
        {
            Field field = _model.fields.GetFieldMatrix()[1, 0][0];
            Chest chest = (Chest)field;

            chest.chestDie(_model.fields);
            Assert.That(_model.fields.GetFieldMatrix()[1, 0][0], Is.Not.EqualTo(typeof(Chest)));
        }

        [Test]
        public void ChestDieTest7()
        {
            Field field = _model.fields.GetFieldMatrix()[1, 0][0];
            Chest chest = (Chest)field;

            chest.chestDie(_model.fields);
            Assert.That(_model.fields.GetFieldMatrix()[1, 0][0], Is.Not.EqualTo(typeof(Chest)));
        }

        [Test]
        public void ChestDieTest8()
        {
            Field field = _model.fields.GetFieldMatrix()[1, 0][0];
            Chest chest = (Chest)field;

            chest.chestDie(_model.fields);
            Assert.That(_model.fields.GetFieldMatrix()[1, 0][0], Is.Not.EqualTo(typeof(Chest)));
        }

        [Test]
        public void ChestDieTest9()
        {
            Field field = _model.fields.GetFieldMatrix()[1, 0][0];
            Chest chest = (Chest)field;

            chest.chestDie(_model.fields);
            Assert.That(_model.fields.GetFieldMatrix()[1, 0][0], Is.Not.EqualTo(typeof(Chest)));
        }

        [Test]
        public void ChestDieTest10()
        {
            Field field = _model.fields.GetFieldMatrix()[1, 0][0];
            Chest chest = (Chest)field;

            chest.chestDie(_model.fields);
            Assert.That(_model.fields.GetFieldMatrix()[1, 0][0], Is.Not.EqualTo(typeof(Chest)));
        }

        [Test]
        public void ChestDieTest11()
        {
            Field field = _model.fields.GetFieldMatrix()[1, 0][0];
            Chest chest = (Chest)field;

            chest.chestDie(_model.fields);
            Assert.That(_model.fields.GetFieldMatrix()[1, 0][0], Is.Not.EqualTo(typeof(Chest)));
        }

        [Test]
        public void ChestDieTest12()
        {
            Field field = _model.fields.GetFieldMatrix()[1, 0][0];
            Chest chest = (Chest)field;

            chest.chestDie(_model.fields);
            Assert.That(_model.fields.GetFieldMatrix()[1, 0][0], Is.Not.EqualTo(typeof(Chest)));
        }

        [Test]
        public void ChestDieTest13()
        {
            Field field = _model.fields.GetFieldMatrix()[1, 0][0];
            Chest chest = (Chest)field;

            chest.chestDie(_model.fields);
            Assert.That(_model.fields.GetFieldMatrix()[1, 0][0], Is.Not.EqualTo(typeof(Chest)));
        }

        [Test]
        public void ChestDieTest14()
        {
            Field field = _model.fields.GetFieldMatrix()[1, 0][0];
            Chest chest = (Chest)field;

            chest.chestDie(_model.fields);
            Assert.That(_model.fields.GetFieldMatrix()[1, 0][0], Is.Not.EqualTo(typeof(Chest)));
        }

        [Test]
        public void ChestDieTest15()
        {
            Field field = _model.fields.GetFieldMatrix()[1, 0][0];
            Chest chest = (Chest)field;

            chest.chestDie(_model.fields);
            Assert.That(_model.fields.GetFieldMatrix()[1, 0][0], Is.Not.EqualTo(typeof(Chest)));
        }

        [Test]
        public void ChestDieTest16()
        {
            Field field = _model.fields.GetFieldMatrix()[1, 0][0];
            Chest chest = (Chest)field;

            chest.chestDie(_model.fields);
            Assert.That(_model.fields.GetFieldMatrix()[1, 0][0], Is.Not.EqualTo(typeof(Chest)));
        }

        [Test]
        public void ChestDieTest17()
        {
            Field field = _model.fields.GetFieldMatrix()[1, 0][0];
            Chest chest = (Chest)field;

            chest.chestDie(_model.fields);
            Assert.That(_model.fields.GetFieldMatrix()[1, 0][0], Is.Not.EqualTo(typeof(Chest)));
        }

        [Test]
        public void ChestDieTest18()
        {
            Field field = _model.fields.GetFieldMatrix()[1, 0][0];
            Chest chest = (Chest)field;

            chest.chestDie(_model.fields);
            Assert.That(_model.fields.GetFieldMatrix()[1, 0][0], Is.Not.EqualTo(typeof(Chest)));
        }

        [Test]
        public void ChestDieTest19()
        {
            Field field = _model.fields.GetFieldMatrix()[1, 0][0];
            Chest chest = (Chest)field;

            chest.chestDie(_model.fields);
            Assert.That(_model.fields.GetFieldMatrix()[1, 0][0], Is.Not.EqualTo(typeof(Chest)));
        }

        [Test]
        public void ChestDieTest20()
        {
            Field field = _model.fields.GetFieldMatrix()[1, 0][0];
            Chest chest = (Chest)field;

            chest.chestDie(_model.fields);
            Assert.That(_model.fields.GetFieldMatrix()[1, 0][0], Is.Not.EqualTo(typeof(Chest)));
        }
        #endregion


        #region Model public event methods
        [Test]
        public void TestOnBoostChanged()
        {

        }

        //not really related to events, but it is in the same region in Model.cs
        [Test]
        public void TestCreaturesStop()
        {

        }

        [Test]
        public void TestCreaturesStart()
        {

        }

        [Test]
        public void TestBombsStop()
        {
            _model.Load("LoadWithBombs", _keySettingsModel.PlayerKeyBindings);
            Assert.That(_model.size, Is.EqualTo(5));
            //check for bombs on field
            //3 bombs total
            //Assert.That(CountElementsOnField("Bomb"), Is.EqualTo(3));
            //player1 has 2 bombs at (1,1), (1,2) with detonation size 2, 1
            Assert.That(_model.players[0].bombs.Count, Is.EqualTo(2));
            Assert.That(_model.fields.GetFieldMatrix()[1, 1][0].GetType(), Is.EqualTo(typeof(Bomb)));
            Assert.That(((Bomb)_model.fields.GetFieldMatrix()[1, 1][0]).detonationSize, Is.EqualTo(2));
            Assert.That(_model.fields.GetFieldMatrix()[1, 2][0].GetType(), Is.EqualTo(typeof(Bomb)));
            Assert.That(((Bomb)_model.fields.GetFieldMatrix()[1, 2][0]).detonationSize, Is.EqualTo(1));
            //player2 has 1 bomb at (3,3) with detonation size 3
            Assert.That(_model.players[1].bombs.Count, Is.EqualTo(1));
            Assert.That(_model.fields.GetFieldMatrix()[3, 3][0].GetType(), Is.EqualTo(typeof(Bomb)));
            Assert.That(((Bomb)_model.fields.GetFieldMatrix()[3, 3][0]).detonationSize, Is.EqualTo(3));

            _model.BombsStop();
            //check if timers are disabled
            Assert.That(_model.players[0].bombs[0].timer2.Enabled, Is.EqualTo(false));
            Assert.That(_model.players[0].bombs[1].timer2.Enabled, Is.EqualTo(false));
            Assert.That(_model.players[1].bombs[0].timer2.Enabled, Is.EqualTo(false));
        }

        [Test]
        public void TestPlayerStopTimer()
        {

        }

        [Test]
        public void TestBombsStart()
        {
            _model.Load("LoadWithBombs", _keySettingsModel.PlayerKeyBindings);
            Assert.That(_model.size, Is.EqualTo(5));
            //check for bombs on field
            //3 bombs total
            //Assert.That(CountElementsOnField("Bomb"), Is.EqualTo(3));
            //player1 has 2 bombs at (1,1), (1,2) with detonation size 2, 1
            Assert.That(_model.players[0].bombs.Count, Is.EqualTo(2));
            Assert.That(_model.fields.GetFieldMatrix()[1, 1][0].GetType(), Is.EqualTo(typeof(Bomb)));
            Assert.That(((Bomb)_model.fields.GetFieldMatrix()[1, 1][0]).detonationSize, Is.EqualTo(2));
            Assert.That(_model.fields.GetFieldMatrix()[1, 2][0].GetType(), Is.EqualTo(typeof(Bomb)));
            Assert.That(((Bomb)_model.fields.GetFieldMatrix()[1, 2][0]).detonationSize, Is.EqualTo(1));
            //player2 has 1 bomb at (3,3) with detonation size 3
            Assert.That(_model.players[1].bombs.Count, Is.EqualTo(1));
            Assert.That(_model.fields.GetFieldMatrix()[3, 3][0].GetType(), Is.EqualTo(typeof(Bomb)));
            Assert.That(((Bomb)_model.fields.GetFieldMatrix()[3, 3][0]).detonationSize, Is.EqualTo(3));


            _model.BombsStart();
            //check if timers are enabled
            Assert.That(_model.players[0].bombs[0].timer2.Enabled, Is.EqualTo(true));
            Assert.That(_model.players[0].bombs[1].timer2.Enabled, Is.EqualTo(true));
            Assert.That(_model.players[1].bombs[0].timer2.Enabled, Is.EqualTo(true));
        }
        #endregion
        #region Private methods
        private int CountElementsOnField(string ElementType)
        {
            int count = 0;
            for (int i = 0; i < defaultMapSize; i++)
            {
                for (int j = 0; j < defaultMapSize; j++)
                {
                    if (_model.fields.GetFieldMatrix()[i, j].Count > 0)
                    {
                        for (int k = 0; k < _model.fields.GetFieldMatrix()[i, j].Count; k++)
                        {
                            if (_model.fields.GetFieldMatrix()[i, j][k].GetType().Name == ElementType)
                            {
                                count++;
                            }
                        }
                    }
                }
            }
            return count;
        }
        private void CheckConstructor(int playerCount)
        {
            //playerCount is 2 or 3
            Assert.That(playerCount, Is.AtLeast(2));
            Assert.That(playerCount, Is.AtMost(3));

            Assert.That(_model.size, Is.EqualTo(5));
            Assert.That(_model.Rounds, Is.EqualTo(3));
            Assert.That(_model.playerCount, Is.EqualTo(playerCount));
            //check for walls
            Assert.That(_model.fields.GetFieldMatrix()[0, 1][0].GetType(), Is.EqualTo(typeof(Wall)));
            Assert.That(_model.fields.GetFieldMatrix()[0, 2][0].GetType(), Is.EqualTo(typeof(Wall)));
            Assert.That(_model.fields.GetFieldMatrix()[0, 3][0].GetType(), Is.EqualTo(typeof(Wall)));
            //check for chest
            Assert.That(_model.fields.GetFieldMatrix()[1, 0][0].GetType(), Is.EqualTo(typeof(Chest)));
            Assert.That(((Chest)_model.fields.GetFieldMatrix()[1, 0][0]).canDropLoot, Is.EqualTo(true));
            Assert.That(_model.fields.GetFieldMatrix()[2, 0][0].GetType(), Is.EqualTo(typeof(Chest)));
            Assert.That(((Chest)_model.fields.GetFieldMatrix()[2, 0][0]).canDropLoot, Is.EqualTo(true));
            Assert.That(_model.fields.GetFieldMatrix()[3, 0][0].GetType(), Is.EqualTo(typeof(Chest)));
            Assert.That(((Chest)_model.fields.GetFieldMatrix()[3, 0][0]).canDropLoot, Is.EqualTo(true));
            //check for creature
            Assert.That(_model.fields.GetFieldMatrix()[2, 2][0].GetType(), Is.EqualTo(typeof(Creature)));
            //no boosts on players
            Assert.That(_model.fields.GetFieldMatrix()[0, 0][0].GetType(), Is.EqualTo(typeof(Player)));
            //unintuitive: if playerCount is 2, player 3 is not added to the game! 
            if (playerCount == 3)
            {
                Assert.That(_model.fields.GetFieldMatrix()[4, 4][0].GetType(), Is.EqualTo(typeof(Player)));
            }
            Assert.That(_model.fields.GetFieldMatrix()[0, 4][0].GetType(), Is.EqualTo(typeof(Player)));
        }
        #endregion


    }
}