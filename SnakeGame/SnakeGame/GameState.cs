using Microsoft.Maui.Controls.Shapes;

namespace SnakeGame;

public class GameState
{
    public int Rows{get; set;}
    public int Columns{get; set;}
    public GridValue[,] Grid{get; set;}
    public Direction CurrentDirection{get; protected set;}
    public int Score{get; protected set;}
    public bool GameOver{get; protected set;}
	public DrawGrid drawGrid{get; protected set;}
    public Games curentGame{get; set;}
    protected Grid GameGrid;
    protected Border Overlay;
    protected Label OverlayText;
    protected Label ScoreText;


    protected readonly LinkedList<Direction> directionChanges = new LinkedList<Direction>();

    protected readonly Random random = new Random();


    public GameState(int rows, int cols, Grid gameGrid, Border overlay, Label overlayText, Label scoreText){
        Rows = rows;
        Columns = cols;
        Grid = new GridValue[rows, cols];
        CurrentDirection = Direction.Right;
        GameGrid = gameGrid;
        drawGrid = new DrawGrid(rows, cols, GameGrid, this);
        Overlay = overlay;
        OverlayText = overlayText;
        ScoreText = scoreText;
    }
    
	protected void Draw(){
		drawGrid.Draw();
		ScoreText.Text = $"SCORE: {Score}";
	}

    protected async Task ShowCountDown(){
		for(int i = 3; i >= 1; i--){
			OverlayText.Text = i.ToString();
			await Task.Delay(500);
		}
	}

	protected async Task ShowGameOver(){
		await Task.Delay(1000);
		Overlay.IsVisible = true;
		OverlayText.Text = "Game over";
	}

}