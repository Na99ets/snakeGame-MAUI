using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnakeGame;
using System.Collections.Generic;

namespace SnakeGameTests1
{
    [TestClass]
    public class GameStateTests
    {
        [TestMethod]
        public void ChangeDirection_ValidDirectionChange()
        {
            // Arrange
            GameState gameState = new GameState(10, 10);

            // Act
            gameState.ChangeDirection(Direction.Down);

            // Assert
            gameState.MoveSnake(); // Process direction change
            Assert.AreEqual(Direction.Down, gameState.CurrentDirection);
        }

        [TestMethod]
        public void ChangeDirection_InvalidDirectionChange()
        {
            // Arrange
            GameState gameState = new GameState(10, 10);

            // Act
            gameState.ChangeDirection(Direction.Left);

            // Assert
            gameState.MoveSnake(); // Process direction change
            Assert.AreEqual(Direction.Right, gameState.CurrentDirection); // Snake cannot move back into itself
        }

        [TestMethod]
        public void MoveSnake_HitsWall_GameOver()
        {
            // Arrange
            GameState gameState = new GameState(3, 3);

            // Act
            gameState.MoveSnake(); // Move right
            gameState.MoveSnake(); // Move right again (into the wall)

            // Assert
            Assert.IsTrue(gameState.GameOver);
        }

        [TestMethod]
        public void MoveSnake_HitsSelf_GameOver()
        {
            // Arrange
            GameState gameState = new GameState(10, 10);
            gameState.ChangeDirection(Direction.Down);
            gameState.MoveSnake();
            gameState.ChangeDirection(Direction.Left);
            gameState.MoveSnake();
            gameState.ChangeDirection(Direction.Up);

            // Act
            gameState.MoveSnake(); // Move into itself

            // Assert
            Assert.IsTrue(gameState.GameOver);
        }

        [TestMethod]
        public void MoveSnake_FoodConsumed_AddsSegment()
        {
            // Arrange
            GameState gameState = new GameState(10, 10);
            Position foodPosition = new Position(5, 4);
            gameState.Grid[foodPosition.Row, foodPosition.Column] = GridValue.Food;

            // Act
            gameState.MoveSnake();

            // Assert
            Assert.AreEqual(1, gameState.Score);
            Assert.AreEqual(GridValue.Snake, gameState.Grid[foodPosition.Row, foodPosition.Column]);
            Assert.AreEqual(4, new List<Position>(gameState.GetSnakePositions()).Count); // Snake length increases
        }
    }
}
