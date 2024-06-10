using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.Model.Maps
{
    /// <summary>
    /// Represents a map in the Bomberman game.
    /// </summary>
    public class MapModel
    {
        private int size;

        private List<Tuple<int, int>> creaturePositions;
        private List<Tuple<int, int>> playerPositions;
        private List<Tuple<int, int>> wallPositions;
        private List<Tuple<int, int>> chestPositions;

        /// <summary>
        /// Gets the size of the map.
        /// </summary>
        public int Size
        {
            get { return size; }
        }

        /// <summary>
        /// Gets the positions of the creatures on the map.
        /// </summary>
        public List<Tuple<int, int>> CreaturePositions
        {
            get { return creaturePositions; }
        }

        /// <summary>
        /// Gets the positions of the players on the map.
        /// </summary>
        public List<Tuple<int, int>> PlayerPositions
        {
            get { return playerPositions; }
        }

        /// <summary>
        /// Gets the positions of the walls on the map.
        /// </summary>
        public List<Tuple<int, int>> WallPositions
        {
            get { return wallPositions; }
        }

        /// <summary>
        /// Gets the positions of the chests on the map.
        /// </summary>
        public List<Tuple<int, int>> ChestPositions
        {
            get { return chestPositions; }
        }

        /// <summary>
        /// Initializes a new instance of the MapModel class.
        /// </summary>
        /// <param name="size">The size of the map.</param>
        /// <param name="creaturePositions">The positions of the creatures on the map.</param>
        /// <param name="playerPositions">The positions of the players on the map.</param>
        /// <param name="wallPositions">The positions of the walls on the map.</param>
        /// <param name="chestPositions">The positions of the chests on the map.</param>
        public MapModel(int size, List<Tuple<int, int>> creaturePositions, List<Tuple<int, int>> playerPositions, List<Tuple<int, int>> wallPositions, List<Tuple<int, int>> chestPositions)
        {
            this.size = size;
            this.creaturePositions = creaturePositions;
            this.playerPositions = playerPositions;
            this.wallPositions = wallPositions;
            this.chestPositions = chestPositions;
        }
    }
}
