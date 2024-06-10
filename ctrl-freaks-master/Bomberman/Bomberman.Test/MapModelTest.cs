using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bomberman.Model.Maps;

namespace Bomberman.Test
{
    [TestFixture]
    public class MapModelTest
    {
        private MapModel mapModel = null!;
        private int Size = 10;
        private List<Tuple<int, int>> creaturePositions;
        private List<Tuple<int, int>> playerPositions;
        private List<Tuple<int, int>> wallPositions;
        private List<Tuple<int, int>> chestPositions;
        [SetUp]
        public void Setup()
        {

            creaturePositions = new List<Tuple<int, int>>();
            playerPositions = new List<Tuple<int, int>>();
            wallPositions = new List<Tuple<int, int>>();
            chestPositions = new List<Tuple<int, int>>();
            mapModel = new MapModel(Size, creaturePositions, playerPositions, wallPositions, chestPositions);
        }
        [Test]
        public void TestMapModelSize()
        {
            int expectedSize = 10;
            Assert.That(mapModel.Size, Is.EqualTo(expectedSize));
        }

        [Test]
        public void TestMapModelCreaturePositions()
        {
            List<Tuple<int, int>> expectedCreaturePositions = new List<Tuple<int, int>>()
                {
                    new Tuple<int, int>(1, 2),
                    new Tuple<int, int>(3, 4)
                };
            mapModel = new MapModel(Size, expectedCreaturePositions, playerPositions, wallPositions, chestPositions);

            // Act
            List<Tuple<int, int>> actualCreaturePositions = mapModel.CreaturePositions;

            // Assert
            Assert.That(actualCreaturePositions, Is.EquivalentTo(expectedCreaturePositions));
        }

        [Test]
        public void TestMapModelPlayerPositions()
        {
            // Arrange
            List<Tuple<int, int>> expectedPlayerPositions = new List<Tuple<int, int>>()
                {
                    new Tuple<int, int>(5, 6),
                    new Tuple<int, int>(7, 8)
                };
            mapModel = new MapModel(Size, creaturePositions, expectedPlayerPositions, wallPositions, chestPositions);

            // Act
            List<Tuple<int, int>> actualPlayerPositions = mapModel.PlayerPositions;

            // Assert
            Assert.That(actualPlayerPositions, Is.EquivalentTo(expectedPlayerPositions));
        }

        [Test]
        public void TestMapModelWallPositions()
        {
            // Arrange
            List<Tuple<int, int>> expectedWallPositions = new List<Tuple<int, int>>()
                {
                    new Tuple<int, int>(9, 10),
                    new Tuple<int, int>(11, 12)
                };
            mapModel = new MapModel(Size, creaturePositions, playerPositions, expectedWallPositions, chestPositions);

            // Act
            List<Tuple<int, int>> actualWallPositions = mapModel.WallPositions;

            // Assert
            Assert.That(actualWallPositions, Is.EquivalentTo(expectedWallPositions));
        }

        [Test]
        public void TestMapModelChestPositions()
        {
            // Arrange
            List<Tuple<int, int>> expectedChestPositions = new List<Tuple<int, int>>()
                {
                    new Tuple<int, int>(13, 14),
                    new Tuple<int, int>(15, 16)
                };
            mapModel = new MapModel(Size, creaturePositions, playerPositions, wallPositions, expectedChestPositions);

            // Act
            List<Tuple<int, int>> actualChestPositions = mapModel.ChestPositions;

            // Assert
            Assert.That(actualChestPositions, Is.EquivalentTo(expectedChestPositions));
        }
    }
}
