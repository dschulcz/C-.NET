using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bomberman.Model.KeySettings;
using Bomberman.Model;

namespace Bomberman.Test
{
    [TestFixture]
    public class KeySettingsModelTest
    {
        private KeySettingsModel keySettingsModel;

        [SetUp]
        public void Setup()
        {
            keySettingsModel = new KeySettingsModel();
        }

        [Test]
        public void ChangeKeyBinding_ValidKeyAndDirection()
        {
            // Arrange
            int playerIndex = 0;
            string key = "O";
            Direction direction = Direction.Up;

            // old key binding is "W" for direction "Up"
            string oldKey = keySettingsModel.PlayerKeyBindings[playerIndex].FirstOrDefault(x => x.Value == direction).Key;
            Assert.That(oldKey, Is.EqualTo("W"));

            // change key binding should be successful
            bool result = keySettingsModel.ChangeKeyBinding(playerIndex, key, direction);
            Assert.That(result, Is.True);

            // key should be bound to the correct direction
            Assert.That(keySettingsModel.PlayerKeyBindings[playerIndex].Keys.Contains(key), Is.True);
            Assert.That(keySettingsModel.PlayerKeyBindings[playerIndex][key], Is.EqualTo(direction));

            // old key should be unbound
            Assert.That(keySettingsModel.PlayerKeyBindings[playerIndex].Keys.Contains(oldKey), Is.False);
            
        }

        [Test]
        public void ChangeKeyBinding_KeyAlreadyBound()
        {
            // Arrange
            int playerIndex = 0;
            string key = "W";
            Direction direction = Direction.Left;

            // change key binding should be unsuccessful
            bool result = keySettingsModel.ChangeKeyBinding(playerIndex, key, direction);
            Assert.That(result, Is.False);

            // old key binding should remain unchanged: "A" for direction "Left"
            Assert.That(keySettingsModel.PlayerKeyBindings[playerIndex].FirstOrDefault(x => x.Value == direction).Key, Is.EqualTo("A"));

            // key should not be bound to the new direction
            Assert.That(keySettingsModel.PlayerKeyBindings[playerIndex].FirstOrDefault(x => x.Value == direction).Key, Is.Not.EqualTo(key));


        }

        [Test]
        public void SetToDefault_ResetsKeyBindingsToDefaultValues()
        {
            // Arrange
            List<Dictionary<string, Direction>> expectedKeyBindings = new List<Dictionary<string, Direction>>()
            {
                new Dictionary<string, Direction>()
                {
                    { "W", Direction.Up },
                    { "A", Direction.Left },
                    { "S", Direction.Down },
                    { "D", Direction.Right },
                    { "Q", Direction.PlantBomb },
                    { "E", Direction.PlantChest }
                },
                new Dictionary<string, Direction>()
                {
                    { "Up", Direction.Up },
                    { "Left", Direction.Left },
                    { "Down", Direction.Down },
                    { "Right", Direction.Right },
                    { "M", Direction.PlantBomb },
                    { "N", Direction.PlantChest }
                },
                new Dictionary<string, Direction>()
                {
                    { "NumPad8", Direction.Up },
                    { "NumPad4", Direction.Left },
                    { "NumPad5", Direction.Down },
                    { "NumPad6", Direction.Right },
                    { "NumPad7", Direction.PlantBomb },
                    { "NumPad9", Direction.PlantChest }
                }
            };

            // key bindings are changed
            keySettingsModel.ChangeKeyBinding(0, "O", Direction.Up);
            keySettingsModel.ChangeKeyBinding(2, "P", Direction.Left);

            // Act
            keySettingsModel.SetToDefault();
            List<Dictionary<string, Direction>> actualKeyBindings = keySettingsModel.PlayerKeyBindings;

            // Assert
            Assert.That(expectedKeyBindings, Is.EqualTo(actualKeyBindings));
        }
    }
}
