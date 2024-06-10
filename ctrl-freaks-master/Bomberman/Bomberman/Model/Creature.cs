using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bomberman.Model
{
    /// <summary>
    /// Represents a creature in the Bomberman game. Fields and attributes are meshed together in this class (wrongly).
    /// </summary>
    public class Creature : Field
    {
        /// <summary>
        /// Gets or sets the x position of the creature.
        /// </summary>
        public int xPosition;
        /// <summary>
        /// Gets or sets the y position of the creature.
        /// </summary>
        public int yPosition;
        /// <summary>
        /// Gets or sets a value indicating whether the creature is alive.
        /// </summary>
        public bool alive = true;
        /// <summary>
        /// timer for the creature needed for the movement.
        /// </summary>
        private System.Timers.Timer timer;
        /// <summary>
        /// here because spaghetti code.
        /// </summary>
        public Model model;
        /// <summary>
        /// constructor for the creature. parameter names are self-explanatory.
        /// </summary>
        /// <param name="xPosition"></param>
        /// <param name="yPosition"></param>
        /// <param name="model"></param>
        public Creature(int xPosition, int yPosition, Model model)
        {
            this.xPosition = xPosition;
            this.yPosition = yPosition;
            timer = new();
            timer.Interval = 3000;
            timer.Elapsed += Tick;
            timer.Start();
            this.model = model;
        }
        /// <summary>
        /// removes the creature from the field matrix and stops the timer.
        /// </summary>
        public void Destroy()
        {
            timer.Elapsed -= Tick;
            model = null!;
        }
        /// <summary>
        /// Moves the creature to a random direction. No need for this to be public.
        /// </summary>
        /// <returns>true if movement successful. not used anywhere though</returns>
        public bool Move() {
            Random random = new Random();
            ConcurrentFieldMatrix fields = model.fields;
            int randomNumber = random.Next(4); 
            for (int i = 0; i < 4; i++) {
                if (randomNumber == 4) {
                    randomNumber = 0;
                }

                switch (randomNumber)
                {
                    case 0:// up
                        if (singleMove(xPosition-1, yPosition, fields))
                        {
                            return true;
                        }
                        else
                        {
                            randomNumber++;
                            break;
                        }
                    case 1: // down
                        if (singleMove(xPosition+1, yPosition, fields))
                        {
                            return true;
                        }
                        else
                        {
                            randomNumber++;
                            break;
                        }
                    case 2: // left
                        if (singleMove(xPosition, yPosition-1, fields))
                        {
                            return true;
                        }
                        else
                        {
                            randomNumber++;
                            break;
                        }
                    case 3: // right
                        if (singleMove(xPosition, yPosition+1, fields))
                        {
                            return true;
                        }
                        else {
                            randomNumber++;
                            break;
                        }
                }
            }
            return false;
        }
        /// <summary>
        /// Kills the creature. Removes it from the field matrix and stops the timer.
        /// </summary>
        /// <param name="fields">field from which it removes itself</param>
        public void Die(ConcurrentFieldMatrix fields)
        {
            fields.RemoveField(xPosition,yPosition,this);
            this.timer.Stop();
            alive = false;
        
        }
        /// <summary>
        /// moves the creature to the specified position. if the position is not valid, it returns false. Assumes that this is only used for adjacent movement, even though teleportation is entirely possible.
        /// </summary>
        /// <param name="x">new x</param>
        /// <param name="y">new y</param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public bool singleMove(int x, int y, ConcurrentFieldMatrix fields) {
            if (x < fields.GetFieldMatrix().GetLength(0) && x >= 0 && y < fields.GetFieldMatrix().GetLength(0) && y >= 0)
            {
                foreach (Field elem in fields.GetFieldMatrix()[x, y].ToList())
                {
                    if ((elem is Wall) || (elem is Chest))
                    {
                        return false;
                    }

                }
            }
            else
            {
                return false;
            }


            fields.RemoveField(xPosition, yPosition, this);

            foreach (Field elem in fields.GetFieldMatrix()[x, y].ToList())
            {
                if (elem.GetType() == typeof(Player))
                {
                    Player player = (Player)elem;
                    player.Die(fields);
                }
            }

            fields.AddFields(x,y, this);
            this.xPosition = x;
            this.yPosition = y;
            return true;
        
        }
        /// <summary>
        /// timer event handler. moves the creature.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArg"></param>
        private void Tick(object? sender, EventArgs eventArg)
        {
            this.Move();
            model.OnPlayerMoved(0, 0, 0, 0);
            //Debug.WriteLine("megy a szorny.");
        }
        /// <summary>
        /// public method to stop the timer.
        /// </summary>
        public void CreatureTimerStopper()
        { 
            timer.Stop();
        }
        /// <summary>
        /// public method to start the timer.
        /// </summary>
        public void CreatureTimerStarter()
        {
            timer.Start();
        }
    }
}
