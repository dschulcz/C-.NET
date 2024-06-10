using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;


namespace Bomberman.Model
{
    /// <summary>
    /// Bomb class, which is a field that explodes after a certain time
    /// </summary>
    public class Bomb : Field
    {
        /// <summary>
        /// size of the detonation
        /// </summary>
        public int detonationSize = 2;
        /// <summary>
        /// x position of the bomb
        /// </summary>
        public int xPosition;
        /// <summary>
        /// y position of the bomb
        /// </summary>
        public int yPosition;
        /// <summary>
        /// timer for the bomb, not sure why this is public
        /// </summary>
        public System.Timers.Timer timer2 = new System.Timers.Timer();
        public System.Timers.Timer timer = new System.Timers.Timer();
        public Model model;
        /// <summary>
        /// tick count for the bomb
        /// </summary>
        public int tickCount;
        /// <summary>
        /// bool for the bomb, only set in constructor and only used in test
        /// </summary>
        public bool left;
        /// <summary>
        /// bool for the bomb, only set in constructor and only used in test
        /// </summary>
        public bool right;
        /// <summary>
        /// bool for the bomb, only set in constructor and only used in test
        /// </summary>
        public bool up;
        /// <summary>
        /// bool for the bomb, only set in constructor and only used in test
        /// </summary>
        public bool down;
        /// <summary>
        /// custom struct for bomb explosion
        /// </summary>
        public struct boomRec
        {
            bool nextFieldDetonation;

            public boomRec(bool nextFieldDetonation)
            {
                this.nextFieldDetonation = nextFieldDetonation;
            }

            public bool getNextFieldDetonation()
            {

                return nextFieldDetonation;
            }
            public void setNextFieldDetonation(bool next)
            {

                this.nextFieldDetonation = next;
            }
        }
        /// <summary>
        /// constructor for bomb
        /// </summary>
        /// <param name="detonationSize">size of detonation</param>
        /// <param name="xp">bomb x position</param>
        /// <param name="yp">bomb y position</param>
        /// <param name="model">needs model because spaghetti code</param>
        public Bomb(int detonationSize, int xp, int yp, Model model)
        {
            this.detonationSize = detonationSize;
            this.xPosition = xp;
            this.yPosition = yp;
            timer2 = new();
            timer2.Interval = 500;
            timer2.Elapsed += Tick2;
            timer2.Start();
            timer = new();
            timer.Interval = 750;
            timer.Elapsed += Tick;
            this.model = model;
            tickCount = 0;

            left = true;
            right = true;
            up = true;
            down = true;
        }
        /// <summary>
        /// removes the bomb from the field
        /// </summary>
        public void Destroy()
        {
            timer2.Stop();
            timer2.Elapsed -= Tick2;
            timer.Elapsed -= Tick;
            ExplosionClean();
            model = null!;
        }
        /// <summary>
        /// detonates the bomb
        /// </summary>
        /// <param name="detSize">range of explosion</param>
        public void boom(int detSize)
        {
            int fieldSize = model.fields.GetFieldMatrix().GetLength(0);
            boomRec returnvalue;
            model.fields.GetFieldMatrix()[xPosition, yPosition].Remove(this);
            for (int i = 0; i <= detSize; i++)
            {
                if (xPosition + i < fieldSize && xPosition + i >= 0 && yPosition < fieldSize && yPosition >= 0)
                {
                    ;
                    if (singleFieldBoom(xPosition + i, yPosition).getNextFieldDetonation() == false)
                    {
                        break;
                    }
                    else
                    {
                        model.fields.GetFieldMatrix()[xPosition + i, yPosition].Add(Explosion.GetInstance());
                    }
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; i <= detSize; i++)
            {
                if (xPosition - i < fieldSize && xPosition - i >= 0 && yPosition < fieldSize && yPosition >= 0 )
                {
                    returnvalue = singleFieldBoom(xPosition - i, yPosition);
                    if (returnvalue.getNextFieldDetonation() == false)
                    {
                        break;
                    }
                    else
                    {
                        model.fields.GetFieldMatrix()[xPosition-i, yPosition].Add(Explosion.GetInstance());
                    }
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; i <= detSize; i++)
            {
                if (xPosition < fieldSize && xPosition >= 0 && yPosition + i < fieldSize && yPosition + i >= 0)
                {
                    returnvalue = singleFieldBoom(xPosition, yPosition + i);
                    if (returnvalue.getNextFieldDetonation() == false)
                    {
                        break;
                    }
                    else
                    {
                        model.fields.GetFieldMatrix()[xPosition, yPosition + i].Add(Explosion.GetInstance());
                    }
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; i <= detSize; i++)
            {
                if (xPosition < fieldSize && xPosition >= 0 && yPosition - i < fieldSize && yPosition - i >= 0)
                {
                    returnvalue = singleFieldBoom(xPosition, yPosition - i);
                    if (returnvalue.getNextFieldDetonation() == false)
                    {
                        break;
                    }
                    else
                    {
                        model.fields.GetFieldMatrix()[xPosition, yPosition - i].Add(Explosion.GetInstance());
                    }
                }
                else
                {
                    break;
                }
            }
            timer.Start();
        }
        /// <summary>
        /// detonates a single field, returns if the next field should be detonated
        /// </summary>
        /// <param name="xp">detonation x position</param>
        /// <param name="yp">detonation y position</param>
        /// <returns>whether next field should detonate. No idea why this is not just a bool</returns>
        public boomRec singleFieldBoom(int xp, int yp)
        {
            boomRec value = new boomRec();
            value.setNextFieldDetonation(true);
            foreach (Field field in model.fields.GetFieldMatrix()[xp, yp].ToList())
            {
                if (field.GetType() == typeof(Player))
                {
                    Player player = (Player)field;
                    player.Die(model.fields);
                }
                else
                if (field.GetType() == typeof(Creature))
                {
                    Creature creature = (Creature)field;
                    creature.Die(model.fields);
                }
                else
                if (field.GetType() == typeof(Player))
                {
                    Player player = (Player)field;
                    player.Die(model.fields);
                }
                else
                    if (field.GetType() == typeof(Chest))
                {
                    if (tickCount != 5)
                    {
                        Chest chest = (Chest)field;
                        chest.chestDie(model.fields);
                    }
                    value.setNextFieldDetonation(false);
                    //model.OnPlayerMoved(0, 0, 0, 0);
                }
                else if (field.GetType() == typeof(Bomb))
                {
                    Bomb bomb = (Bomb)field;
                    if (bomb != this)
                    {
                        bomb.tickCount = 5;
                        //bomb.boom(bomb.detonationSize);
                    }
                    value.setNextFieldDetonation(false);
                }
                else if (field.GetType() == typeof(Wall))
                {
                    value.setNextFieldDetonation(false);
                }
            }

            return value;
        }
        /// <summary>
        /// Removes explosions from the field. Not necessarily thread safe :(
        /// </summary>
        public void ExplosionClean()
        {
            int fieldSize = model.fields.GetFieldMatrix().GetLength(0);
            for (int i = 0; i <= detonationSize; i++)
            {
                if (xPosition + i < fieldSize && xPosition + i >= 0 && yPosition < fieldSize && yPosition >= 0 )
                {
                    if(model.fields.GetFieldMatrix()[xPosition + i, yPosition].OfType<Explosion>().Any()) {
                        Explosion e = model.fields.GetFieldMatrix()[xPosition + i, yPosition].OfType<Explosion>().First();
                        model.fields.GetFieldMatrix()[xPosition + i, yPosition].Remove(e);
                    }
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; i <= detonationSize; i++)
            {
                if (xPosition - i < fieldSize && xPosition - i >= 0 && yPosition < fieldSize && yPosition >= 0)
                {
                    if (model.fields.GetFieldMatrix()[xPosition - i, yPosition].OfType<Explosion>().Any())
                    {
                        Explosion e = model.fields.GetFieldMatrix()[xPosition - i, yPosition].OfType<Explosion>().First();
                        model.fields.GetFieldMatrix()[xPosition - i, yPosition].Remove(e);
                    }
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; i <= detonationSize; i++)
            {
                if (xPosition < fieldSize && xPosition >= 0 && yPosition + i < fieldSize && yPosition + i >= 0)
                {
                    if (model.fields.GetFieldMatrix()[xPosition, yPosition + i].OfType<Explosion>().Any())
                    {
                        Explosion e = model.fields.GetFieldMatrix()[xPosition, yPosition + i].OfType<Explosion>().First();
                        model.fields.GetFieldMatrix()[xPosition, yPosition + i].Remove(e);
                    }
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; i <= detonationSize; i++)
            {
                if (xPosition < fieldSize && xPosition >= 0 && yPosition - i < fieldSize && yPosition - i >= 0)
                {
                    if (model.fields.GetFieldMatrix()[xPosition, yPosition - i].OfType<Explosion>().Any())
                    {
                        Explosion e = model.fields.GetFieldMatrix()[xPosition, yPosition - i].OfType<Explosion>().First();
                        model.fields.GetFieldMatrix()[xPosition, yPosition - i].Remove(e);
                    }
                }
                else
                {
                    break;
                }
            }
        }
        /// <summary>
        /// Timer tick event handler
        /// </summary>
        /// <param name="sender">irrelevant</param>
        /// <param name="eventArg">irrelevant</param>
        private void Tick2(object? sender, EventArgs eventArg)
        {
            tickCount++;
            if(tickCount == 5)
            {
                boom(1);
            }
            if (tickCount == 6)
            {
                ExplosionClean();
                boom(detonationSize);
                foreach (Player player in model.players)
                {
                    if (player.bombs.Contains(this))
                    {
                        player.bombs.Remove(this);
                    }
                }
            }
            if(tickCount == 7)
            {
                timer2.Stop();
                ExplosionClean();
            }
        }

        private void Tick(object? sender, EventArgs eventArg)
        {
            ExplosionClean();
            timer.Stop();
        }

        public void BombTimerStopper()
        {
            timer2.Stop();
        }
        /// <summary>
        /// public method to start the bomb timer, even though the timer is already public in the first place
        /// </summary>
        public void BombTimerStarter()
        {
            timer2.Start();
        }
    }
}
