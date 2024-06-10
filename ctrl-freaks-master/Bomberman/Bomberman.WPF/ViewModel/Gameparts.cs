


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bomberman.WPF.ViewModel
{
    public class GameParts : ViewModelBase
    { 
        /// <summary>
        /// X coordinate of the field.
        /// </summary>
        public Int32 X { get; set; }
        /// <summary>
        /// Y coordinate of the field.
        /// </summary>
        public Int32 Y { get; set; }
        #region FieldValue
        /// <summary>
        /// what can be on a field by element
        /// </summary>
        public enum FieldValueSingle
        {
            Player1,
            Player2,
            Player3,
            Wall,
            Chest,
            Bomb,
            Explosion,
            Monster,
            Bonus,
            NegativeBonus,
        }
        /// <summary>
        /// what can be on a field by combination of elements
        /// </summary>
        public enum FieldValue
        {
            //TODO: Do players die if they are within a wall when an explosion reaches the wall?
            //single values
            Player1,
            Player2,
            Player3,
            Wall,
            Chest,
            Bomb,
            Explosion,
            Monster,
            Bonus,
            NegativeBonus,
            Clear,

            //2 values
            //players on each other
            Player1Player2,
            Player1Player3,
            Player2Player3,

            //1 player with other objects
            //player cannot overlap with bonus or negative bonus
            Player1Wall,
            Player1Chest,
            Player1Bomb,
            Player1Explosion,
            Player1Monster,

  
            Player2Wall,
            Player2Chest,
            Player2Bomb,
            Player2Explosion,
            Player2Monster,

            Player3Wall,
            Player3Chest,
            Player3Bomb,
            Player3Explosion,
            Player3Monster,

            //wall and chest are done, because only players can overlap with them
            
            BombExplosion,
            BombMonster,
            BombBonus,
            BombNegativeBonus,

            ExplosionMonster,
            ExplosionBonus,
            ExplosionNegativeBonus,

            MonsterMonster,
            MonsterBonus,
            MonsterNegativeBonus,

            ExplosionExplosion,
            ExplosionExplosionExplosion,
            ExplosionExplosionExplosionExplosion,

            //assume the only 1 bonus can be at a time on a field

            //here is hoping no chest can be placed on a bonus or negative bonus

            //3 values
            Player1Player2Player3,

            //2 players with other objects
            Player1Player2Wall,
            Player1Player2Chest,
            Player1Player2Bomb,
            Player1Player2Explosion,
            Player1Player2Monster,

            Player1Player3Wall,
            Player1Player3Chest,
            Player1Player3Bomb,
            Player1Player3Explosion,
            Player1Player3Monster,

            Player2Player3Wall,
            Player2Player3Chest,
            Player2Player3Bomb,
            Player2Player3Explosion,
            Player2Player3Monster,

            //1 player with 2 other objects
            Player1BombExplosion,
            Player1BombMonster, //can monster step on bomb?
            Player1ExplosionMonster,

            Player2BombExplosion,
            Player2BombMonster,
            Player2ExplosionMonster,

            Player3BombExplosion,
            Player3BombMonster,
            Player3ExplosionMonster,
            
            //3 other objects
            BombExplosionMonster,
            BombExplosionBonus,
            BombExplosionNegativeBonus,
            BombMonsterBonus,
            BombMonsterNegativeBonus,

            ExplosionMonsterBonus,
            ExplosionMonsterNegativeBonus,
            MonsterMonsterMonster,

            //4 values
            //3 player with 1 other objects
            Player1Player2Player3Wall,
            Player1Player2Player3Chest,
            Player1Player2Player3Bomb,
            Player1Player2Player3Explosion,
            Player1Player2Player3Monster,

            //2 players with 2 other objects
            Player1Player2BombExplosion,
            Player1Player2BombMonster,
            Player1Player2ExplosionMonster,

            Player1Player3BombExplosion,
            Player1Player3BombMonster,
            Player1Player3ExplosionMonster,

            Player2Player3BombExplosion,
            Player2Player3BombMonster,
            Player2Player3ExplosionMonster,


            //1 player with 3 other objects
            Player1BombExplosionMonster,
            Player2BombExplosionMonster,
            Player3BombExplosionMonster,

            //4 other objects
            BombExplosionMonsterBonus,
            BombExplosionMonsterNegativeBonus,
            MonsterMonsterMonsterMonster,
            //5 values
            //3 players with 2 other object
            Player1Player2Player3BombExplosion,
            Player1Player2Player3BombMonster,
            Player1Player2Player3ExplosionMonster,

            //2 players with 3 other objects
            Player1Player2BombExplosionMonster,
            Player1Player3BombExplosionMonster,
            Player2Player3BombExplosionMonster,

            //that's it for now
            Player1MonsterMonster,
            Player1MonsterMonsterMonster,
            Player1MonsterMonsterMonsterMonster,

            Player2MonsterMonster,
            Player2MonsterMonsterMonster,
            Player2MonsterMonsterMonsterMonster,

            Player3MonsterMonster,
            Player3MonsterMonsterMonster,
            Player3MonsterMonsterMonsterMonster,

            Player1Player2MonsterMonster,
            Player1Player2MonsterMonsterMonster,

            Player1Player3MonsterMonster,
            Player1Player3MonsterMonsterMonster,

            Player2Player3MonsterMonster,
            Player2Player3MonsterMonsterMonster,

        }
        #endregion
        /// <summary>
        /// FieldValue of the field. used to determine which image to display.
        /// </summary>
        public FieldValue CurrentFieldValue { get; set; }
        /// <summary>
        /// needed for editor mode. combines x and y coordinates to a single number, so it is easier to use for databinding
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// delegate command for clicking on the field
        /// </summary>
        public DelegateCommand? ClickCommand { get; set; }
        /// <summary>
        /// constructor for the game parts for main game
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="fv"></param>
        public GameParts(int x, int y, FieldValue fv)
        {
            this.X = x; this.Y = y;
            this.CurrentFieldValue = fv;
        }
        /// <summary>
        /// constructor for the game parts for editor mode
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="num"></param>
        /// <param name="fv"></param>
        /// <param name="stepCommand"></param>
        public GameParts(int x, int y, int num, FieldValue fv, DelegateCommand? stepCommand)
        {
            this.X = x;
            this.Y = y;
            this.CurrentFieldValue = fv;
            this.Number = num;
            ClickCommand = stepCommand;
        }
        /// <summary>
        /// string version of the FieldValue
        /// </summary>
        
        public String Value
        {
            get
            {
                string? value = Enum.GetName(typeof(FieldValue), CurrentFieldValue);
                if(value is null)
                {
                    throw new Exception("FieldValue is not a valid enum");
                }
                return value;

            }
            set
            {
                FieldValue fv;
                if(Enum.TryParse<FieldValue>(value, out fv))
                {
                    CurrentFieldValue = fv;
                }
                else
                {
                    throw new Exception($"Value is not a valid FieldValue: {value}");
                }
                OnPropertyChanged();
            }
        }
        
    }
}
