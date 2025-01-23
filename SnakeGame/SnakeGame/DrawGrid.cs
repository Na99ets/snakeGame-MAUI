namespace SnakeGame;
using Microsoft.Maui.Controls.Shapes;


public class DrawGrid
{
	public int Rows{get;}
    public int Columns{get;}
    private Grid GameGrid;

	private GameState gameState;

	private readonly Dictionary<GridValue, Color> gridValueToColor = new Dictionary<GridValue, Color>{
		{GridValue.Empty, Color.FromHex("312C40")},
		{GridValue.Snake, Color.FromHex("FF00FF66")},
		{GridValue.Food, Color.FromHex("FFDD270F")},
		{GridValue.ToTetrisFood, Color.FromHex("#FF35A4C3")},
		{GridValue.Tetris, Color.FromHex("FFDD270F")},
		{GridValue.FallenTetris, Color.FromHex("FFDD270F")},
		{GridValue.SnakeHead, Color.FromHex("FF1B5924")},
	};

    public DrawGrid(int rows, int cols, Grid gameGrid, GameState _gameState){
        Rows = rows;
        Columns = cols;
		GameGrid = gameGrid;
		gameState = _gameState;
    }

    public void SetupGrid(){
		GameGrid.Children.Clear();
		for(int r = 0; r < Rows; r++){
			GameGrid.RowDefinitions.Add(new RowDefinition{ Height = 26.7});
			for(int c = 0; c < Columns; c++){
				GameGrid.ColumnDefinitions.Add(new ColumnDefinition{ Width = 26.7 });
				var rectangle = new Rectangle{Fill=Color.FromHex("312C40"), 
													Stroke=Color.FromHex("4F4867"), 
													StrokeThickness=3, 
													WidthRequest=26.6, 
													HeightRequest=26.6,
													};
				Grid.SetRow(rectangle, r);
				Grid.SetColumn(rectangle, c);
				GameGrid.Children.Add(rectangle);
			}
		}
	}
    
    public void Draw(){
		Brush brush = Color.FromHex("4F4867");
		foreach(var child in GameGrid.Children){
			if(child is Rectangle rectangle){
				int row = Grid.GetRow(rectangle);
				int col = Grid.GetColumn(rectangle);
				GridValue gridValue = gameState.Grid[row,col];
				rectangle.Fill = gridValueToColor[gridValue];
				// if(gridValue == GridValue.Snake || gridValue == GridValue.SnakeHead){
				// 	rectangle.Stroke = gridValueToColor[gridValue];	
				// }
				// else if(rectangle.Stroke != brush){
				// 	rectangle.Stroke = brush;
				// }
			}
		}
	}
}