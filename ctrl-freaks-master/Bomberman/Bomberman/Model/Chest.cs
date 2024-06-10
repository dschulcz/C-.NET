using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bomberman.Model
{
    /// <summary>
    /// Represents a chest in the Bomberman game. Fields and attributes are meshed together in this class (wrongly).
    /// </summary>
    public class Chest : Field
    {
        /// <summary>
        /// Gets or sets a value indicating whether the chest can drop loot.
        /// </summary>
        public bool canDropLoot = true;

        /// <summary>
        /// Gets or sets the x position of the chest.
        /// </summary>
        public int xPosition;

        /// <summary>
        /// Gets or sets the y position of the chest.
        /// </summary>
        public int yPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="Chest"/> class.
        /// </summary>
        /// <param name="canDropLoot">A value indicating whether the chest can drop loot.</param>
        /// <param name="xp">The x position of the chest.</param>
        /// <param name="yp">The y position of the chest.</param>
        public Chest(bool canDropLoot, int xp, int yp)
        {
            this.canDropLoot = canDropLoot;
            this.xPosition = xp;
            this.yPosition = yp;
        }

        /// <summary>
        /// Destroys the chest. Empty method.
        /// </summary>
        public void Destroy()
        {

        }

        /// <summary>
        /// Handles the death of the chest. It removes the chest from the field matrix and maybe drops any kind of boost
        /// </summary>
        /// <param name="fields">The concurrent field matrix.</param>
        public void chestDie(ConcurrentFieldMatrix fields)
        {
            fields.GetFieldMatrix()[xPosition, yPosition].Remove(this);
            Random random = new Random();

            int randomNumber = random.Next(1);
            if (randomNumber == 0 && canDropLoot)
            {
                randomNumber = random.Next(0, 11);
                switch (randomNumber)
                {
                    case 0:
                        fields.AddFields(xPosition, yPosition, Detonator.GetInstance());
                        break;
                    case 1:
                        fields.AddFields(xPosition, yPosition, Skate.GetInstance());
                        break;
                    case 2:
                        fields.AddFields(xPosition, yPosition, Invulnerable.GetInstance());
                        break;
                    case 3:
                        fields.AddFields(xPosition, yPosition, Ghost.GetInstance());
                        break;
                    case 4:
                        fields.AddFields(xPosition, yPosition, ChestPlant.GetInstance());
                        break;
                    case 5:
                        fields.AddFields(xPosition, yPosition, BombSizeUp.GetInstance());
                        break;
                    case 6:
                        fields.AddFields(xPosition, yPosition, BombCounterUp.GetInstance());
                        break;
                    case 7:
                        fields.AddFields(xPosition, yPosition, Slow.GetInstance());
                        break;
                    case 8:
                        fields.AddFields(xPosition, yPosition, BombSizeDown.GetInstance());
                        break;
                    case 9:
                        fields.AddFields(xPosition, yPosition, BombPlantProblem.GetInstance());
                        break;
                    case 10:
                        fields.AddFields(xPosition, yPosition, InstantPlant.GetInstance());
                        break;
                }
            }
        }
    }
}
