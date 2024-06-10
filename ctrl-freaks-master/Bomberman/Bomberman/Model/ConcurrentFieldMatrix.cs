using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.Model
{
    /// <summary>
    /// Provides a thread-safe way to access the field matrix.
    /// </summary>
    public class ConcurrentFieldMatrix
    {
        private readonly List<Field>[,] fields;
        private readonly object lockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentFieldMatrix"/> class.
        /// </summary>
        /// <param name="fields">The field matrix.</param>
        public ConcurrentFieldMatrix(List<Field>[,] fields)
        {
            this.fields = fields;
        }

        /// <summary>
        /// Adds a field to the specified position in the field matrix.
        /// </summary>
        /// <param name="x">The x-coordinate of the position.</param>
        /// <param name="y">The y-coordinate of the position.</param>
        /// <param name="value">The field to add.</param>
        public void AddFields(int x, int y, Field value)
        {
            lock (lockObject)
            {
                fields[x, y].Add(value);
            }
        }

        /// <summary>
        /// Removes a field from the specified position in the field matrix.
        /// </summary>
        /// <param name="x">The x-coordinate of the position.</param>
        /// <param name="y">The y-coordinate of the position.</param>
        /// <param name="value">The field to remove.</param>
        public void RemoveField(int x, int y, Field value)
        {
            lock (lockObject)
            {
                fields[x, y].Remove(value);
            }
        }

        /// <summary>
        /// Clears the field at the specified position in the field matrix.
        /// </summary>
        /// <param name="x">The x-coordinate of the position.</param>
        /// <param name="y">The y-coordinate of the position.</param>
        public void Clear(int x, int y)
        {
            lock (lockObject)
            {
                fields[x, y].Clear();
            }
        }

        /// <summary>
        /// Gets the field matrix.
        /// </summary>
        /// <returns>The field matrix.</returns>
        public List<Field>[,] GetFieldMatrix()
        {
            return fields;
        }
    }

   
}
