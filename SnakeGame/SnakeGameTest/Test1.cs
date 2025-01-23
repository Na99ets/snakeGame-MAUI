using SnakeGame;
namespace SnakeGameTest;


[TestClass]
public sealed class Test1
{
    [TestMethod]
    public void RoteteTatris(){
        Tetris tetris = new Tetris(20, 10, new Grid GameGrid(), new Border Overlay(), new Label OverlayText(), new GridValue[20, 10], new Label Score());
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
