using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bomberman.Model;
using Bomberman.Persistence;
using Newtonsoft.Json.Linq;

namespace Bomberman.Test
{
    [TestFixture]
    public class TestPlayer
    {
        private ConcurrentFieldMatrix fields = null!;
        private List<Field>[,] matrix = null!;
        private int Size = 10;
        private Player player1 = null!;
        private Player player2 = null!;
        private Model.Model model = null!;
        private Dictionary<string, Direction>? dic;

        [SetUp]
        public void Setup()
        {
            dic = new Dictionary<string, Direction>();
            dic.Add("W", Direction.Up);
            dic.Add("A", Direction.Left);
            dic.Add("S", Direction.Down);
            dic.Add("D", Direction.Right);
            dic.Add("Q", Direction.PlantBomb);
            dic.Add("E", Direction.PlantChest);
            matrix = new List<Field>[Size, Size];
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    matrix[i, j] = new List<Field>();
                }
            }
            fields = new ConcurrentFieldMatrix(matrix);
            List<Player> players = new List<Player>();
            model = new Model.Model(10, fields, new List<Creature>(), 0, 0, 2, players, new DataAccess());
            player1 = new Player(0, 0, 0, true, new List<Boost>(), new List<Bomb>(), 2, 2, 0, 2, dic, model);
            player1.StopTimer();
            players.Add(player1);
            fields.AddFields(0, 0, player1);
            dic = new Dictionary<string, Direction>();
            dic.Add("Up", Direction.Up);
            dic.Add("Left", Direction.Left);
            dic.Add("Down", Direction.Down);
            dic.Add("Right", Direction.Right);
            dic.Add("M", Direction.PlantBomb);
            dic.Add("N", Direction.PlantChest);
            player2 = new Player(1, 2, 2, true, new List<Boost>(), new List<Bomb>(), 2, 2, 0, 2, dic, model);
            player2.StopTimer();
            players.Add(player2);
            fields.AddFields(2, 2, player2);


        }
        [Test]
        public void TestSetUp()
        {
            Assert.That(fields.GetFieldMatrix()[0, 0][0], Is.EqualTo(player1));
            Assert.That(fields.GetFieldMatrix()[2, 2][0], Is.EqualTo(player2));
            Assert.That(model.players.Count, Is.EqualTo(2));
            Player player3 = new Player(2, 2, 2, true, new List<Boost>(), new List<Bomb>(), 2, 2, 0, 2, model);
            player3.StopTimer();
            Assert.That(player3.model, Is.EqualTo(model));
            Assert.That(player3.xPosition, Is.EqualTo(2));
            Assert.That(player3.yPosition, Is.EqualTo(2));
            Assert.That(player3.id, Is.EqualTo(2));
            Assert.That(player3.speed, Is.EqualTo(2));
            Assert.That(player3.bombCapacity, Is.EqualTo(2));
            Assert.That(player3.detonationSize, Is.EqualTo(2));
            Assert.That(player3.winCounter, Is.EqualTo(0));
            Assert.That(player3.boosts.Count, Is.EqualTo(0));
            Assert.That(player3.bombs.Count, Is.EqualTo(0));

        
        }
        [Test]
        public void TestConstructor()
        {
            
            Assert.That(player2.model, Is.EqualTo(model));
            Assert.That(player2.xPosition, Is.EqualTo(2));
            Assert.That(player2.yPosition, Is.EqualTo(2));
            Assert.That(player2.id, Is.EqualTo(1));
            Assert.That(player2.speed, Is.EqualTo(2));
            Assert.That(player2.bombCapacity, Is.EqualTo(2));
            Assert.That(player2.detonationSize, Is.EqualTo(2));
            Assert.That(player2.winCounter, Is.EqualTo(0));
            Assert.That(player2.boosts.Count, Is.EqualTo(0));
            Assert.That(player2.bombs.Count, Is.EqualTo(0));

        }

        [Test]
        public void TestKeypressed()
        {
            Assert.That(fields.GetFieldMatrix()[0, 0].Count, Is.EqualTo(1));
            player1.Keypressed("Q", fields);
            Assert.That(fields.GetFieldMatrix()[0, 0].Count, Is.EqualTo(2));
            
        }
        [Test]
        public void TestPlantBomb()
        {
            Assert.That(fields.GetFieldMatrix()[0, 0].Count, Is.EqualTo(1));
            player1.PlantBomb(fields);
            Assert.That(fields.GetFieldMatrix()[0, 0].Count, Is.EqualTo(2));
            player1.PlantBomb(fields);
            Assert.That(fields.GetFieldMatrix()[0, 0].Count, Is.EqualTo(2));

        }

        [Test]
        public void TestNewBoostSkate()
        {
            Boost boost = Skate.GetInstance();
            Assert.That(boost, Is.EqualTo(Skate.GetInstance()));
            player1.NewBoost(fields, Skate.GetInstance());
            Assert.That(player1.speed, Is.EqualTo(1));
            Assert.That(player1.boosts.Count, Is.EqualTo(0));
        }
        [Test]
        public void TestNewBoostSlow()
        {
            Boost boost = Slow.GetInstance();
            Assert.That(boost, Is.EqualTo(Slow.GetInstance()));
            player1.NewBoost(fields, Slow.GetInstance());
            Assert.That(player1.speed, Is.EqualTo(3));
            Assert.That(player1.boosts.Count, Is.EqualTo(0));
        }
        [Test]
        public void TestNewBoostBombSizeUP()
        {
            Boost boost = BombSizeUp.GetInstance();
            Assert.That(boost, Is.EqualTo(BombSizeUp.GetInstance()));
            player1.NewBoost(fields, BombSizeUp.GetInstance());
            Assert.That(player1.detonationSize, Is.EqualTo(3));
            Assert.That(player1.boosts.Count, Is.EqualTo(0));
        }
        [Test]
        public void TestNewBoostBombSizeDown()
        {
            Boost boost = BombSizeDown.GetInstance();
            Assert.That(boost, Is.EqualTo(BombSizeDown.GetInstance()));
            player1.NewBoost(fields, BombSizeDown.GetInstance());
            Assert.That(player1.detonationSize, Is.EqualTo(1));
            Assert.That(player1.boosts.Count, Is.EqualTo(1));
        
            player1.NewBoost(fields, BombSizeDown.GetInstance());
            Assert.That(player1.boosts.Count, Is.EqualTo(1));
        }
        [Test]
        public void TestNewBoostInvulnerable()
        {
            Boost boost = Invulnerable.GetInstance();
            Assert.That(boost, Is.EqualTo(Invulnerable.GetInstance()));
            player1.NewBoost(fields, Invulnerable.GetInstance());
            Assert.That(player1.boosts.Count, Is.EqualTo(1));
            player1.NewBoost(fields, Invulnerable.GetInstance());
            Assert.That(player1.boosts.Count, Is.EqualTo(1));

        }
        [Test]
        public void TestNewBoostGhost()
        {
            Boost boost = Ghost.GetInstance();
            Assert.That(boost, Is.EqualTo(Ghost.GetInstance()));
            player1.NewBoost(fields, Ghost.GetInstance());
            Assert.That(player1.boosts.Count, Is.EqualTo(1));
            player1.NewBoost(fields, Ghost.GetInstance());
            Assert.That(player1.boosts.Count, Is.EqualTo(1));

        }
        [Test]
        public void TestNewBoostBombPlantProblem()
        {
            Boost boost = BombPlantProblem.GetInstance();
            Assert.That(boost, Is.EqualTo(BombPlantProblem.GetInstance()));
            player1.NewBoost(fields, BombPlantProblem.GetInstance());
            Assert.That(player1.boosts.Count, Is.EqualTo(1));
            player1.NewBoost(fields, BombPlantProblem.GetInstance());
            Assert.That(player1.boosts.Count, Is.EqualTo(1));

        }
        [Test]
        public void TestNewBoostInstantPlant()
        {
            Boost boost = InstantPlant.GetInstance();
            Assert.That(boost, Is.EqualTo(InstantPlant.GetInstance()));
            player1.NewBoost(fields, InstantPlant.GetInstance());
            Assert.That(player1.boosts.Count, Is.EqualTo(1));
            player1.NewBoost(fields, InstantPlant.GetInstance());
            Assert.That(player1.boosts.Count, Is.EqualTo(1));

        }
        [Test]
        public void TestNewBoostBombCounterUp()
        {
            Boost boost = BombCounterUp.GetInstance();
            Assert.That(boost, Is.EqualTo(BombCounterUp.GetInstance()));
            player1.NewBoost(fields, BombCounterUp.GetInstance());
            Assert.That(player1.boosts.Count, Is.EqualTo(0));
            Assert.That(player1.bombCapacity, Is.EqualTo(3));

        }
        [Test]
        public void TestNewBoostChestPlant()
        {
            Boost boost = ChestPlant.GetInstance();
            Assert.That(boost, Is.EqualTo(ChestPlant.GetInstance()));
            player1.NewBoost(fields, ChestPlant.GetInstance());
            Assert.That(player1.boosts.Count, Is.EqualTo(1));
            Assert.That(player1.chestplantMax, Is.EqualTo(3));
            player1.NewBoost(fields, ChestPlant.GetInstance());
            Assert.That(player1.boosts.Count, Is.EqualTo(2));
            Assert.That(player1.chestplantMax, Is.EqualTo(6));

        }
        [Test]
        public void TestNewBoostDetonator()
        {
            Boost boost = Detonator.GetInstance();
            Assert.That(boost, Is.EqualTo(Detonator.GetInstance()));
            player1.NewBoost(fields, Detonator.GetInstance());
            Assert.That(player1.boosts.Count, Is.EqualTo(1));
            player1.NewBoost(fields, Detonator.GetInstance());
            Assert.That(player1.boosts.Count, Is.EqualTo(1));

        }
        [Test]
        public void TestDie()
        {
            player1.Die(fields);
            Assert.That(player1.alive, Is.EqualTo(false));
            player1.PlantBomb(fields);
            Assert.That(fields.GetFieldMatrix()[0,0].Count, Is.EqualTo(0));
            player2.NewBoost(fields, Invulnerable.GetInstance());
            Assert.That(player2.alive, Is.EqualTo(true));
            player2.PlantBomb(fields);
            Assert.That(fields.GetFieldMatrix()[2, 2].Count, Is.EqualTo(2));

        }
        [Test]
        public void TestChestPlant()
        {
            player1.NewBoost(fields, ChestPlant.GetInstance());
            player1.chestPlant(fields);
            Assert.That(fields.GetFieldMatrix()[0, 0].Count, Is.EqualTo(2));

        }
    } 
}
