using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.Persistence
{
    public interface IDataAccess
    {
        public void SaveFile(string actualPositions, string path);

        public string? LoadFile(string path);
    }
}
