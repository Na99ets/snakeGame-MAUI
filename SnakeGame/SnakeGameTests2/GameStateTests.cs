https://github.com/Na99ets/snakeGame-MAUI/tree/mainusing Microsoft.VisualStudio.TestTools.UnitTesting;
using SnakeGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGameTests2
{
    [TestClass]
    public class TetrisTest
    {
        [TestMethod]
        public void RoteteTatris(){
            Grid GameGrid = new Grid();
            Border Overlay = new Border();
            Label OverlayText = new Label();
            GridValue[,] Grid = new GridValue[20, 10];
            Label Score = new Label();
            Tetris tetris = new Tetris(20, 10, GameGrid, Overlay, OverlayText, Grid, Score);
            tetris.AddStripe();
            for(int i; i < 3; i++){
                tetris.MoveTetris(Direction.Down);
            }
            var startTetrisposition = tetris.currentTetrisPosition;
            for(int i; i < 4; i++){
                tetris.RotateTetris();
            }
            Assert.AreEqual(startTetrisposition, tetris.currentTetrisPosition);
        }
    }
}