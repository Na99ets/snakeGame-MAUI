namespace SnakeGame;
using Microsoft.Maui.Controls.Shapes;
using SharpHook.Native;
using SharpHook.Reactive;
using SharpHook;
using System.Reactive.Linq;




public partial class MainPage : ContentPage
{	
	


	private int rows = 20, cols = 10;
	private Tetris tetris;
	private Snake snake;
	private bool gameRunning;
	private List<string> gameNamesList = new List<string>{"Start Snake", "Start Tetres"};

	IReactiveGlobalHook _keyboardHook;


	public MainPage()
	{
		InitializeComponent();
		snake = new Snake(rows, cols, GameGrid, Overlay, OverlayText, ScoreText);
		tetris = snake.tetris;
		snake.drawGrid.SetupGrid();
	}

	protected override async void OnAppearing(){
		base.OnAppearing();
		if (_keyboardHook is null)
			{
				_keyboardHook = new SimpleReactiveGlobalHook(GlobalHookType.Keyboard, runAsyncOnBackgroundThread: true);

				// Subscribe to all key presses
				_keyboardHook.KeyPressed.Subscribe(KeyDown);

				await _keyboardHook.RunAsync();
			}
	}

	private async void KeyDown(KeyboardHookEventArgs args){
        MainThread.InvokeOnMainThreadAsync(() =>
        {
            if (snake.curentGame == Games.GameTetris)
            {
                if (snake.tetris.GameOver)
                {
                    return;
                }

                switch (args.Data.KeyCode)
                {
					case KeyCode.VcSpace:
						snake.tetris.RotateTetris();
						return;
                    case KeyCode.VcD:
                    case KeyCode.VcRight:
                        snake.tetris.MoveTetris(Direction.Right);
                        return;
                    case KeyCode.VcA:
                    case KeyCode.VcLeft:
                        snake.tetris.MoveTetris(Direction.Left);
                        return;
                }
            }
        });

		if(snake.curentGame == Games.GameSnake){
			switch(args.Data.KeyCode){
				case KeyCode.VcW:
				case KeyCode.VcUp:
					snake.ChangeDirection(Direction.Up);
					return;
				case KeyCode.VcS:
				case KeyCode.VcDown:
					snake.ChangeDirection(Direction.Down); 
					return;
				case KeyCode.VcD:
				case KeyCode.VcRight:
					snake.ChangeDirection(Direction.Right); 
					return;
				case KeyCode.VcA:
				case KeyCode.VcLeft:
					snake.ChangeDirection(Direction.Left); 
					return;
			}
		}
	}




	private async void GameStared(object sender, EventArgs e){
		snake = new Snake(rows, cols, GameGrid, Overlay, OverlayText, ScoreText);
		tetris = snake.tetris;
		if(!gameRunning){
			gameRunning = true;
			await snake.RunGame();
			gameRunning = false;
			OverlayText.Text = "Press start";
		}
	}


}

