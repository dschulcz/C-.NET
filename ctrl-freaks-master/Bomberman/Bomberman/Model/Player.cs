using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.Model
{
  
    public class Player : Field
    {
        /// <summary>
        /// id of the player
        /// </summary>
        public int id;
        /// <summary>
        /// x position of the player
        /// </summary>
        public int xPosition;
        /// <summary>
        /// y position of the player
        /// </summary>
        public int yPosition;
        /// <summary>
        /// status of the player: alive or dead
        /// </summary>
        public bool alive;
        /// <summary>
        /// list of boosts the player has
        /// </summary>
        public List<Boost> boosts;
        /// <summary>
        /// list of bombs the player has
        /// </summary>
        public List<Bomb> bombs;
        /// <summary>
        /// move interval! of player
        /// </summary>
        public int speed;
        /// <summary>
        /// size of the detonation of the bombs
        /// </summary>
        public int detonationSize;
        /// <summary>
        /// keybindings of the player
        /// </summary>
        public Dictionary<string, Direction>? keys;
        /// <summary>
        /// bomb capacity of the player
        /// </summary>
        public int bombCapacity;
        /// <summary>
        /// number of wins of the player
        /// </summary>
        public int winCounter;
        /// <summary>
        /// needed because of spaghetti code
        /// </summary>
        public Model model;
        private System.Timers.Timer timer = new System.Timers.Timer();
        /// <summary>
        /// Last direction from keypress
        /// </summary>
        public Direction Lastdirection;
        /// <summary>
        /// tick counter for movement
        /// </summary>
        public int tickCounter = 0;
        private System.Timers.Timer ghostTimer = new System.Timers.Timer();
        private System.Timers.Timer invulnerableTimer = new System.Timers.Timer();
        private System.Timers.Timer bombPlantProblemTimer = new System.Timers.Timer();
        private System.Timers.Timer bombSizeDownTimer = new System.Timers.Timer();
        private System.Timers.Timer instantPlantTimer = new System.Timers.Timer();
        private System.Timers.Timer slowTimer = new System.Timers.Timer();


        //-----------------------
        /// <summary>
        /// max number of chests that can be planted
        /// </summary>
        public int chestplantMax = 0;
        /// <summary>
        /// list of chests planted by the player
        /// </summary>
        public List<Chest> chests;

        /// <summary>
        /// Constructor for the player class. keybindings are not set
        /// </summary>
        /// <param name="id"></param>
        /// <param name="xPosition"></param>
        /// <param name="yPosition"></param>
        /// <param name="alive"></param>
        /// <param name="boosts"></param>
        /// <param name="bombs"></param>
        /// <param name="speed"></param>
        /// <param name="bombCapacity"></param>
        /// <param name="winCounter"></param>
        /// <param name="detonationSize"></param>
        /// <param name="model"></param>
        public Player(int id, int xPosition, int yPosition, bool alive, List<Boost> boosts, List<Bomb> bombs, int speed, int bombCapacity, int winCounter, int detonationSize, Model model)
        {
            this.id = id;
            this.xPosition = xPosition;
            this.yPosition = yPosition;
            this.alive = alive;
            this.boosts = boosts;
            this.bombs = bombs;
            this.speed = speed;
            this.bombCapacity = bombCapacity;
            this.winCounter = winCounter;
            this.detonationSize = detonationSize;
            this.keys = null!;
            
            this.model = model;
            timer = new();
            timer.Interval = 250;
            timer.Elapsed += Tick;
            timer.Start();
            Lastdirection = Direction.empty;
            //ghost timer
            ghostTimer = new();
            ghostTimer.Interval = 8000;
            ghostTimer.Elapsed += GhostTimer;
            //invulnerable timer
            invulnerableTimer = new();
            invulnerableTimer.Interval = 8000;
            invulnerableTimer.Elapsed += InvulnerableTimer;
            //bombplatproblem timer
            bombPlantProblemTimer = new();
            bombPlantProblemTimer.Interval = 8000;
            bombPlantProblemTimer.Elapsed += BombPlantProblemTimer;
            //bombSizeDown timer
            bombSizeDownTimer = new();
            bombSizeDownTimer.Interval = 8000;
            bombSizeDownTimer.Elapsed += BombSizeDownTimer;
            //instantPlant timer
            instantPlantTimer = new();
            instantPlantTimer.Interval = 8000;
            instantPlantTimer.Elapsed += InstantPlantTimer;
            slowTimer = new();
            slowTimer.Interval = 8000;
            slowTimer.Elapsed += SlowTimer;
            chests = new List<Chest>();
        }
        /// <summary>
        /// constructor for the player class. keybindings are set
        /// </summary>
        /// <param name="id"></param>
        /// <param name="xPosition"></param>
        /// <param name="yPosition"></param>
        /// <param name="alive"></param>
        /// <param name="boosts"></param>
        /// <param name="bombs"></param>
        /// <param name="speed"></param>
        /// <param name="bombCapacity"></param>
        /// <param name="winCounter"></param>
        /// <param name="detonationSize"></param>
        /// <param name="keys"></param>
        /// <param name="model"></param>
        public Player(int id, int xPosition, int yPosition,  bool alive, List<Boost> boosts, List<Bomb> bombs, int speed, int bombCapacity, int winCounter, int detonationSize, Dictionary<string, Direction>? keys, Model model)
        {
            this.id = id;
            this.xPosition = xPosition;
            this.yPosition = yPosition;
            this.alive = alive;
            this.boosts = boosts;
            this.bombs = bombs;
            this.speed = speed;
            this.bombCapacity = bombCapacity;
            this.winCounter = winCounter;
            this.detonationSize = detonationSize;
            if (keys != null ) 
            this.keys = keys;
        
            this.model = model;
            timer = new();
            timer.Interval = 250;
            timer.Elapsed += Tick;
            timer.Start();
            Lastdirection = Direction.empty;
            //ghost timer
            ghostTimer = new();
            ghostTimer.Interval = 8000;
            ghostTimer.Elapsed += GhostTimer;
            //invulnerable timer
            invulnerableTimer = new();
            invulnerableTimer.Interval = 8000;
            invulnerableTimer.Elapsed += InvulnerableTimer;
            //bombplatproblem timer
            bombPlantProblemTimer = new();
            bombPlantProblemTimer.Interval = 8000;
            bombPlantProblemTimer.Elapsed += BombPlantProblemTimer;
            //bombSizeDown timer
            bombSizeDownTimer = new();
            bombSizeDownTimer.Interval = 8000;
            bombSizeDownTimer.Elapsed += BombSizeDownTimer;
            //instantPlant timer
            instantPlantTimer = new();
            instantPlantTimer.Interval = 8000;
            instantPlantTimer.Elapsed += InstantPlantTimer;
            slowTimer = new();
            slowTimer.Interval = 8000;
            slowTimer.Elapsed += SlowTimer;
            chests = new List<Chest>();
        }
        /// <summary>
        /// removes the player from the field matrix and stops the timer.
        /// </summary>
        public void Destroy()
        {
            timer.Elapsed -= Tick;
            ghostTimer.Elapsed -= GhostTimer;
            invulnerableTimer.Elapsed -= InvulnerableTimer;
            bombPlantProblemTimer.Elapsed -= BombPlantProblemTimer;
            bombSizeDownTimer.Elapsed -= BombSizeDownTimer;
            boosts = new List<Boost>();
            for (int i = 0; i < bombs.Count; i++)
            {
                bombs[i].Destroy();
            }
            bombs = new List<Bomb>();
            model = null!;
        }
        /// <summary>
        /// eventhandler for the timer. moves the player in the last direction
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArg"></param>
        private void Tick(object? sender, EventArgs eventArg)
        {
            if (speed <= tickCounter)
            {
                if (Lastdirection != Direction.empty)
                {
                    this.Move(model.fields, (int)model.fields.GetFieldMatrix().GetLength(0), Lastdirection);
                }
                Lastdirection = Direction.empty;
                tickCounter = 0;
                if (alive) {model.OnPlayerMoved(0, 0, 0, 0); }
                
            }
            else { 
                tickCounter++;
            }
            
        }
        /// <summary>
        /// stops the timer of the player
        /// </summary>
        public void StopTimer() { 
            timer.Stop();
        
        }
        private void GhostTimer(object? sender, EventArgs eventArg)
        {
            Ghost? g = boosts.OfType<Ghost>().FirstOrDefault();
            if (g is not null) {
            boosts.Remove(g);
            model.OnBoostChanged(id,nameof(Ghost),false);
                if (model.fields.GetFieldMatrix()[xPosition, yPosition].OfType<Wall>().Any() || model.fields.GetFieldMatrix()[xPosition, yPosition].OfType<Chest>().Any() || model.fields.GetFieldMatrix()[xPosition, yPosition].OfType<Bomb>().Any())
                {
                    Debug.WriteLine("asd1");
                    Die(model.fields);

                }
            }
            ghostTimer.Stop();
        }

        private void InvulnerableTimer(object? sender, EventArgs eventArg)
        {
            Invulnerable? i = boosts.OfType<Invulnerable>().FirstOrDefault();
            if (i is not null)
            {
                boosts.Remove(i);
                model.OnBoostChanged(id, nameof(Invulnerable), false);
            }
            invulnerableTimer.Stop();
        }

        private void BombPlantProblemTimer(object? sender, EventArgs eventArg)
        {
            //likely incorrect: why would bombCapacity be reset to 2?
            //a separate bool could be used to check if bombs can be planted
            //also removal of the boost is missing
            bombCapacity = 2;
            bombPlantProblemTimer.Stop();
           
                BombPlantProblem? i = boosts.OfType<BombPlantProblem>().FirstOrDefault();
            if (i is not null)
            {
                boosts.Remove(i);
                model.OnBoostChanged(id, nameof(BombPlantProblem), false);
            }
        }
        private void SlowTimer(object? sender, EventArgs eventArg)
        {
            //Debug.WriteLine(bombCapacity); // i don't know why but this isn't run down 

            //likely incorrect: why would bombCapacity be reset to 2?
            //a separate bool could be used to check if bombs can be planted
            //also removal of the boost is missing
            speed = 1;
            slowTimer.Stop();
            model.OnBoostChanged(id, nameof(Slow), false);
        }

        private void BombSizeDownTimer(object? sender, EventArgs eventArg)
        {
            BombSizeDown? i = boosts.OfType<BombSizeDown>().FirstOrDefault();
            if (i is not null) 
            boosts.Remove(i);
            detonationSize = 2;
            bombSizeDownTimer.Stop();
            model.OnBoostChanged(id, nameof(BombSizeDown), false);
        }

        private void InstantPlantTimer(object? sender, EventArgs eventArg)
        {
            InstantPlant? i = boosts.OfType<InstantPlant>().FirstOrDefault();
            if (i is not null)
            boosts.Remove(i);
            model.OnBoostChanged(id, nameof(InstantPlant), false);
            detonationSize = 2;
            instantPlantTimer.Stop();
        }
        /// <summary>
        /// sets the keybindings of the player
        /// </summary>
        /// <param name="keys"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void SetKeys(Dictionary<string, Direction> keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException("keys");
            }
            if(this.keys == null)
            {
                this.keys = keys;
            }
            
        }
        /// <summary>
        /// based on the keypress, the player might take an action
        /// </summary>
        /// <param name="pressed"></param>
        /// <param name="fields">needed in case bomb is planted, so that field is updated with the new bomb</param>
        public void Keypressed( string pressed, ConcurrentFieldMatrix fields) {
            Direction value = Direction.empty;
            bool hasValue = false;
            if (keys != null)
            {
                hasValue = keys.TryGetValue(pressed, out value);
            }
            if (hasValue)
            {
                if (value == Direction.PlantBomb)
                {
                    PlantBomb(fields);
                }
                else if (value == Direction.PlantChest)
                {
                    if(boosts.OfType<ChestPlant>().Any()){ chestPlant(fields);
                    }
                }
                else
                {
                    Lastdirection = value;
                }
            }
        }
      /// <summary>
      /// moves the player in the given direction
      /// </summary>
      /// <param name="fields"></param>
      /// <param name="size"></param>
      /// <param name="direction"></param>
        public void Move(ConcurrentFieldMatrix fields, int size, Direction direction)
        {
            int oldX = xPosition;
            int oldY = yPosition;
            int newX = oldX;
            int newY = oldY;
            switch (direction)
            {
                case Direction.Up:
                    newX--;
                    break;
                case Direction.Down:
                    newX++;
                    break;
                case Direction.Right:
                    newY++;
                    break;
                case Direction.Left:
                    newY--;
                    break;
            }
            //cantStep accounts for all conditions affecting movement
            bool cantStep = false;
            if (alive)
            {
                if (newX < size && newX >= 0 && newY < size && newY >= 0)
                {
                    if (fields.GetFieldMatrix()[newX, newY].OfType<Wall>().Any())
                    {
                        if (!ghostTimer.Enabled) { 
                            cantStep = true;
                        }
                    }

                    if (fields.GetFieldMatrix()[newX, newY].OfType<Chest>().Any())
                    {
                        if (!ghostTimer.Enabled) { 
                            cantStep = true;
                        }
                    }
                    if (fields.GetFieldMatrix()[newX, newY].OfType<Bomb>().Any())
                    {
                        if(!ghostTimer.Enabled) {
                            cantStep = true;
                        }
                    }
                }
                else
                {
                    cantStep = true;
                }
            }
            else

            {
                cantStep = true;
            }
            if (!cantStep) {
                xPosition = newX;
                yPosition = newY;
                if (fields.GetFieldMatrix()[newX, newY].OfType<Boost>().Any())
                {
                    Boost pickUp = fields.GetFieldMatrix()[newX, newY].OfType<Boost>().First<Boost>();
                    fields.RemoveField(newX, newY, pickUp);
                    NewBoost(fields, pickUp);
                }
                fields.GetFieldMatrix()[oldX, oldY].Remove(this);
                fields.GetFieldMatrix()[newX, newY].Add(this);
                foreach (Field field in fields.GetFieldMatrix()[newX,newY].ToList()) {
                    if (field is Creature) {
                        this.Die(fields);
                        //fields.RemoveField(newX,newY, this);
                    }
                   
                }

                if (instantPlantTimer.Enabled) {
                    PlantBomb(fields);
                }
                //adjust own values
               
            }
                //replace player in fields
               
        }
        /// <summary>
        /// plants a chest at the player's position
        /// </summary>
        /// <param name="fields"></param>
        public void chestPlant(ConcurrentFieldMatrix fields)
        {
            if (chestplantMax >= chests.Count+1)
            {
                if (!fields.GetFieldMatrix()[xPosition, yPosition].OfType<Chest>().Any() && !fields.GetFieldMatrix()[xPosition, yPosition].OfType<Wall>().Any())
                {
                    Chest chest = new Chest(false, xPosition, yPosition);
                    fields.GetFieldMatrix()[xPosition, yPosition].Add(chest);
                    chests.Add(chest);
                }             
            }
        }
        /// <summary>
        /// if the player is not invulnerable, it dies
        /// </summary>
        /// <param name="fields">needed to remove itself from fields</param>
        public void Die(ConcurrentFieldMatrix fields)
        {
            if (!invulnerableTimer.Enabled)
            {

                fields.RemoveField(xPosition, yPosition, this);
                alive = false;
                model.IsGameOver();  // ha egyszerre halnak meg, előfordul, hogy a megmaradt játékos is meghal majd egy null modelre hívódik meg a method
            }
        }
        /// <summary>
        /// plants a bomb at the player's position
        /// </summary>
        /// <param name="fields"></param>
        public void PlantBomb(ConcurrentFieldMatrix fields)
        {
            //if player is alive
            if (alive)
            {
                //if player has at least 1 unused bomb
                if (bombCapacity >= bombs.Count + 1)
                {
                    //if player is not standing on a bomb
                    if (!fields.GetFieldMatrix()[xPosition, yPosition].OfType<Bomb>().Any())
                    {
                        Bomb bomb = new Bomb(detonationSize, xPosition, yPosition, model);
                        if (boosts.OfType<Detonator>().Any())
                        {
                            bomb.timer2.Stop();
                        }
                        fields.GetFieldMatrix()[xPosition, yPosition].Add(bomb);
                        bombs.Add(bomb);
                    }
                    else
                    {
                        Debug.WriteLine($"Bomb plant at (x:{xPosition},y:{yPosition}) failed as there is already a bomb there");
                    }
                }
                else
                {
                    if (boosts.OfType<Detonator>().Any())
                    {
                        foreach (var bomb in bombs)
                        {
                            bomb.boom(detonationSize);
                        }
                        bombs.Clear();
                    }
                    else
                    {
                        Debug.WriteLine($"Bomb plant at (x:{xPosition},y:{yPosition}) failed as player has no bombs left:");
                        Debug.WriteLine($"Bomb capacity: {bombCapacity}, current bombs: {bombs.Count}");
                    }
                }
                
            }
           
        }
        /// <summary>
        /// picks up a boost from the field
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="boost"></param>
        public void NewBoost(ConcurrentFieldMatrix fields, Boost boost)
        {
            switch (boost)
            {
                case Invulnerable instance1 when boost == Invulnerable.GetInstance():
                    if (!(boosts.OfType<Invulnerable>().Any()))
                    { 
                        boosts.Add(boost);
                        model.OnBoostChanged(id, nameof(Invulnerable), true);
                        invulnerableTimer.Start();
                    }
                        break;
                case Ghost instance2 when boost == Ghost.GetInstance():
                    if (!(boosts.OfType<Ghost>().Any()))
                    { 
                        boosts.Add(boost);
                        model.OnBoostChanged(id, nameof(Ghost), true);
                        ghostTimer.Start();
                    }
                        break;
                case BombPlantProblem instance3 when boost == BombPlantProblem.GetInstance():
                    if (!(boosts.OfType<BombPlantProblem>().Any()))
                    {
                        bombCapacity = 0;
                        boosts.Add(boost);
                        model.OnBoostChanged(id, nameof(BombPlantProblem), true);
                        bombPlantProblemTimer.Start();
                    }
                    break;
                case BombSizeUp instance4 when boost == BombSizeUp.GetInstance():
                    detonationSize++;
                    model.OnBoostChanged(id, nameof(BombSizeUp), true);
                    break;
                case BombSizeDown instance5 when boost == BombSizeDown.GetInstance():
                    //if bombsizedown is not already active
                    if (!(boosts.OfType<BombSizeDown>().Any()))
                    {
                        boosts.Add(boost);
                        detonationSize = 1;
                        model.OnBoostChanged(id, nameof(BombSizeDown), true);
                    }
                    
                    break;
                case Skate instance6 when boost == Skate.GetInstance():
                    speed--;
                    if (speed < 0) speed = 0;
                    model.OnBoostChanged(id, nameof(Skate), true);
                    break;
                case Slow instance7 when boost == Slow.GetInstance():
                    speed++;
                    slowTimer.Start();
                    model.OnBoostChanged(id, nameof(Slow), true);
                    
                    break;
                case InstantPlant instance8 when boost == InstantPlant.GetInstance():
                    //if instantPlant is not already active
                    if (!(boosts.OfType<InstantPlant>().Any())) 
                    {
                        boosts.Add(boost);
                        model.OnBoostChanged(id, nameof(InstantPlant), true);
                        instantPlantTimer.Start();
                    }
                    break;
                case BombCounterUp instance9 when boost == BombCounterUp.GetInstance():
                    bombCapacity++; 
                    model.OnBoostChanged(id, nameof(BombCounterUp), true);
                    break;
                case ChestPlant instance10 when boost == ChestPlant.GetInstance():
                    chestplantMax += 3;
                    boosts.Add(boost);
                    model.OnBoostChanged(id, nameof(ChestPlant), true);
                    break;
                case Detonator instance11 when boost == Detonator.GetInstance():
                    //if detonator is not already active
                    if (!(boosts.OfType<Detonator>().Any()))
                    {
                        foreach (var bomb in bombs)
                        {
                            bomb.timer2.Stop();
                        }
                        boosts.Add(boost);
                        model.OnBoostChanged(id, nameof(Detonator), true);
                    }
                    break;
                default:
                    boosts[boosts.Count] = boost;
                    break;
            }
            
            
        }
    }


    
}
