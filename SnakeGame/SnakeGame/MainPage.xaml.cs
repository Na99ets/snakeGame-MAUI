using Microsoft.Maui.Controls.Shapes;
namespace SnakeGame;
using SharpHook.Native;
using SharpHook.Reactive;
using SharpHook;
using System.Reactive.Linq;




public partial class MainPage : ContentPage
{
	

	private readonly Dictionary<GridValue, Color> gridValueToColor = new Dictionary<GridValue, Color>{
		{GridValue.Empty, Color.FromHex("312C40")},
		{GridValue.Snake, Color.FromHex("FF00FF66")},
		{GridValue.Food, Color.FromHex("FFDD270F")}
	};
	private readonly int rows =15, cols =15;
	private GameState gameState;
	private bool gameRunning;

	IReactiveGlobalHook _keyboardHook;


	public MainPage()
	{
		InitializeComponent();
		SetupGrid();
		gameState = new GameState(rows, cols);
	}

	private void UpBtnButtonClicked(object sender, EventArgs e){
		gameState.ChangeDirection(Direction.Up); 
	}
	private void LeftBtnButtonClicked(object sender, EventArgs e){
		gameState.ChangeDirection(Direction.Left); 
	}
	private void RightBtnButtonClicked(object sender, EventArgs e){
		gameState.ChangeDirection(Direction.Right); 
	}
	private void DownBtnButtonClicked(object sender, EventArgs e){
		gameState.ChangeDirection(Direction.Down); 
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		if (_keyboardHook is null)
		{
			_keyboardHook = new SimpleReactiveGlobalHook(GlobalHookType.Keyboard, runAsyncOnBackgroundThread: true);

			// Subscribe to all key presses
			_keyboardHook.KeyPressed.Subscribe(KeyDown);

			await _keyboardHook.RunAsync();
		}
	}

	private void KeyDown(KeyboardHookEventArgs args)
	{
		if (!gameRunning)
		{
			switch (args.Data.KeyCode)
			{
				case KeyCode.VcW:
					OverlayText.Text = "123";
					return;
			}
		}
		if (gameState.GameOver)
		{
			return;
		}
		switch (args.Data.KeyCode)
			{
			case KeyCode.VcW:
				gameState.ChangeDirection(Direction.Up);
				return;
			case KeyCode.VcS:
				gameState.ChangeDirection(Direction.Down);
				return;
			case KeyCode.VcD:
				gameState.ChangeDirection(Direction.Right);
				return;
			case KeyCode.VcA:
				gameState.ChangeDirection(Direction.Left);
				return;
			}
		}


	private async Task RunGame(){
		Draw();
		await ShowCountDown();
		Overlay.IsVisible = false;
		await GameLoop();
		await ShowGameOver();
		gameState = new GameState(rows, cols);
	}

	private async void GameStared(object sender, EventArgs e){
		if(!gameRunning){
			gameRunning = true;
			await RunGame();
			gameRunning = false;
		}
	}

	private async Task GameLoop(){
		while(!gameState.GameOver){
			await Task.Delay(100);
			gameState.MoveSnake();
			Draw();
		}
	}

	private void SetupGrid(){
		for(int r = 0; r < rows; r++){
			GameGrid.RowDefinitions.Add(new RowDefinition{ Height = GridLength.Star });
			for(int c = 0; c < cols; c++){
				GameGrid.ColumnDefinitions.Add(new ColumnDefinition{ Width = 40 });
				var rectangle = new Rectangle{Fill=Color.FromHex("312C40"), 
													Stroke=Color.FromHex("4F4867"), 
													StrokeThickness=3, 
													WidthRequest=39.9, 
													HeightRequest=39.9,
													};
				Grid.SetRow(rectangle, r);
				Grid.SetColumn(rectangle, c);
				GameGrid.Children.Add(rectangle);
			}
		}
	}

	private void Draw(){
		DrowGrid();
		ScoreText.Text = $"SCORE: {gameState.Score}";
	}

	private void DrowGrid(){
		foreach(var child in GameGrid.Children){
			if(child is Rectangle rectangle){
				int row = Grid.GetRow(rectangle);
				int col = Grid.GetColumn(rectangle);
				GridValue gridValue = gameState.Grid[row,col];
				rectangle.Fill = gridValueToColor[gridValue];
			}
		}
	}

	private async Task ShowCountDown(){
		for(int i = 3; i >= 1; i--){
			OverlayText.Text = i.ToString();
			await Task.Delay(500);
		}
	}

	private async Task ShowGameOver(){
		await Task.Delay(1000);
		Overlay.IsVisible = true;
		OverlayText.Text = "Press start to play";
	}

}

