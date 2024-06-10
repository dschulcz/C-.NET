using Bomberman.Model;
using Bomberman.Model.Maps;
using Bomberman.Persistence;

namespace GenerateMaps
{
    class Program
    {
        //TODO: Check if the map is valid
        //valid means that all 3 players are placed
        //size is >= 2
        //in every tile, there is at most 1 element
        static void Main(string[] args)
        {
            int defaultMapSize = 10;
            MapModel _mapModel;
            Model _model;

            List<Tuple<int, int>> creaturePositions = new List<Tuple<int, int>>();
            List<Tuple<int, int>> playerPositions = new List<Tuple<int, int>>();
            List<Tuple<int, int>> wallPositions = new List<Tuple<int, int>>();
            List<Tuple<int, int>> chestPositions = new List<Tuple<int, int>>();

            //creature
            creaturePositions.Add(new Tuple<int, int>(5, 5));

            //player
            playerPositions.Add(new Tuple<int, int>(0, 0));
            playerPositions.Add(new Tuple<int, int>(9, 9));
            playerPositions.Add(new Tuple<int, int>(0, 9));

            //wall
            wallPositions.Add(new Tuple<int, int>(0, 4));
            wallPositions.Add(new Tuple<int, int>(0, 5));
            wallPositions.Add(new Tuple<int, int>(0, 6));

            //chest
            chestPositions.Add(new Tuple<int, int>(7, 7));

            _mapModel = new MapModel(defaultMapSize, creaturePositions, playerPositions, wallPositions, chestPositions);
            _model = new Model(_mapModel,0, 3, new DataAccess());
            

            //Save the map to Persistence/Maps
            _model.Save("../../../../Bomberman/Persistence/Maps/Map3.txt");
        }
    }
}

