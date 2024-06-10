using Bomberman.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.Persistence
{
    public class DataAccess:IDataAccess
    {
        public void SaveFile(string actualPositions, string path)
        {
            File.WriteAllText(path, actualPositions);
        }

        public string? LoadFile(string path)
        {
            string file = File.ReadAllText(path);
            try
            {
                Convert.ToInt32(file[0]);
            }
            catch (Exception)
            {

                return null;
            }

            return file;
        }
    }
}
