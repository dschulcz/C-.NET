using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Bomberman.Model;
using Bomberman.Model.EventArguments;
using Bomberman.Model.KeySettings;
using Bomberman.Model.Maps;
using Bomberman.Persistence;
using Moq;

namespace Bomberman.Test
{
    [TestFixture]
    public class EventTest
    {
        private MapModel _mapModel = null!;
        private Model.Model _model = null!;
        private KeySettingsModel _keySettingsModel = null!;
        private Mock<IDataAccess> _mockDataAccess = null!;

        [SetUp]
        public void Initialize()
        {


            //mapModel
            int defaultMapSize = 5;
            List<Tuple<int, int>> creaturePositions = new List<Tuple<int, int>>();
            List<Tuple<int, int>> playerPositions = new List<Tuple<int, int>>();
            List<Tuple<int, int>> wallPositions = new List<Tuple<int, int>>();
            List<Tuple<int, int>> chestPositions = new List<Tuple<int, int>>();
            //creature
            creaturePositions.Add(new Tuple<int, int>(3, 3));
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

            _mapModel = new MapModel(defaultMapSize, creaturePositions, playerPositions, wallPositions, chestPositions);
            
            _model = new Model.Model(_mapModel, 0, 3, new DataAccess());

            //keySettingsModel
            _keySettingsModel = new KeySettingsModel();
            for (int i = 0; i < 3; i++)
            {
                _model.SetPlayerKeys(i, _keySettingsModel.PlayerKeyBindings[i]);
            }

            _model.PlayerMoved += new EventHandler<MovementEventArgs>(Model_PlayerMoved);
            _model.RoundOver += new EventHandler<GameEventArgs>(Model_RoundOver);
            _model.GameOver += new EventHandler<GameEventArgs>(Model_GameOver);
            _model.BoostChanged += new EventHandler<BoostEventArgs>(Model_OnBoostChanged);

            _keySettingsModel.KeySettingsChanged += new EventHandler<KeySettingsEventArgs>(KSModel_OnKeySettingsChanged);
        }

        [Test]
        public void PlayerMoving()
        {
            _model.OnPlayerMoved(0, 0, 1, 0);
        }

        [Test]
        public void RoundOver()
        {
            _model.players[0].StopTimer();
            _model.players[1].StopTimer();
            _model.players[2].StopTimer();
            _model.creatures[0].CreatureTimerStopper();

            _model.players[0].winCounter = 0;
            _model.players[1].winCounter = 0;
            _model.players[2].winCounter = 0;

            Assert.That(_model.Rounds, Is.EqualTo(0));

            _model.players[2].Die(_model.fields);
            _model.players[1].Die(_model.fields);
            
            Assert.That(_model.players[2].alive, Is.EqualTo(false));
            Assert.That(_model.players[1].alive, Is.EqualTo(false));

            _model.IsGameOver();
        }
        
        [Test]
        public void GameOver()
        {
            _model.players[0].StopTimer();
            _model.players[1].StopTimer();
            _model.players[2].StopTimer();
            _model.creatures[0].CreatureTimerStopper();

            _model.players[0].winCounter = 0;
            _model.players[1].winCounter = 0;
            _model.players[2].winCounter = 0;

            _model.players[2].Die(_model.fields);
            _model.players[1].Die(_model.fields);

            Assert.That(_model.players[2].alive, Is.EqualTo(false));
            Assert.That(_model.players[1].alive, Is.EqualTo(false));
            Assert.That(_model.players[0].alive, Is.EqualTo(true));

           

        }

        [Test]
        public void BoostChanged()
        {
            _model.players[0].NewBoost(_model.fields, Ghost.GetInstance());


        }

        [Test]
        public void KeySettingsChanged()
        {
            _keySettingsModel.SetToDefault();
        }

        private void Model_PlayerMoved(Object? sender, MovementEventArgs e)
        {
            
            Assert.That(e.OldX, Is.EqualTo(0));
            Assert.That(e.OldY, Is.EqualTo(0));
            Assert.That(e.NewX, Is.EqualTo(1));
            Assert.That(e.NewY, Is.EqualTo(0));
        }

        private void Model_RoundOver(Object? sender, GameEventArgs e)
        { 
            Assert.That(e.RoundWon, Is.EqualTo(true));
            //winner.id.ToString()
            Assert.That(e.Name, Is.EqualTo(_model.players[0].id.ToString()));
        }

        private void Model_GameOver(Object? sender, GameEventArgs e)
        {
            Assert.That(e.GameWon, Is.EqualTo(true));
            Assert.That(e.Name, Is.EqualTo(_model.players[0].id.ToString())); 
        }

        private void Model_OnBoostChanged(Object? sender, BoostEventArgs e)
        {
            Assert.That(e.PlayerID, Is.EqualTo(_model.players[0].id));
            Assert.That(e.BoostType, Is.EqualTo(nameof(Ghost)));
            Assert.That(e.IsAdded, Is.EqualTo(true));

        }


        private void KSModel_OnKeySettingsChanged(Object? sender, KeySettingsEventArgs e)
        {
            Assert.That(e.PlayerID, Is.Not.EqualTo(null));
            Assert.That(e.NewKey, Is.Not.EqualTo(null));
            Assert.That(e.Direction, Is.Not.EqualTo(null));
        }

    }
}
