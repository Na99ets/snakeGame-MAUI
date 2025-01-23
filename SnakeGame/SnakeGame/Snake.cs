namespace SnakeGame;

public class Snake : GameState
{

    private LinkedList<Position> snakePosition = new LinkedList<Position>();
    public Tetris tetris;
    public Snake(int rows, int cols, Grid gameGrid, Border overlay, Label overlayText, Label scoreText) : base(rows, cols, gameGrid, overlay, overlayText, scoreText)
    {
        AddSnake();
        AddFood();
    }

    private void AddSnake(){
        int rowStartPosition = Rows / 2;

        for(int colStartPosition  = 1; colStartPosition <= 3; colStartPosition++){
            if(colStartPosition == 3){
                Grid[rowStartPosition, colStartPosition] = GridValue.SnakeHead;
            }
            else{
                Grid[rowStartPosition, colStartPosition] = GridValue.Snake;
            }
            snakePosition.AddFirst(new Position(rowStartPosition, colStartPosition));
        }
    }

    private IEnumerable<Position> GetEmptyPositions(){
        for(int r = 0; r < Rows; r++){
            for(int c = 0; c < Columns; c++){
                if (Grid[r,c] == GridValue.Empty){
                    yield return new Position(r,c);
                }
            }
        }
    }

    private void AddFood(){
        List<Position> emptyPositions = new List<Position>(GetEmptyPositions());

        if(emptyPositions.Count == 0){
            return;
        }

        Position position = emptyPositions[random.Next(emptyPositions.Count)];
        Grid[position.Row, position.Column] = GridValue.Food;
        if(Score >= 10){
            position = emptyPositions[random.Next(emptyPositions.Count)];
            Grid[position.Row, position.Column] = GridValue.ToTetrisFood;
        }
    }

    public Position GetHeadPosition(){
        return snakePosition.First.Value;
    }

    public Position GetTailPosition(){
        return snakePosition.Last.Value;
    }

    public IEnumerable<Position> GetSnakePositions(){
        return snakePosition;
    }

    private void AddHead(Position position){
        snakePosition.AddFirst(position);
        Grid[position.Row, position.Column] = GridValue.SnakeHead;
    }

    private void RemoveTail(){
        Position tailPosition = snakePosition.Last.Value;
        Grid[tailPosition.Row, tailPosition.Column] = GridValue.Empty;
        snakePosition.RemoveLast();
    }

    private Direction GetLastDirection(){
        if(directionChanges.Count == 0){
            return CurrentDirection;
        }
        return directionChanges.Last.Value;
    }

    private bool CanChangeDirection(Direction newDirection){
        if(directionChanges.Count == 2){
            return false;
        }
        Direction lastDirection = GetLastDirection();
        return newDirection != lastDirection && newDirection != lastDirection.Opossite();
    }

    public void ChangeDirection(Direction direction){

        if(CanChangeDirection(direction)){
            directionChanges.AddLast(direction);
        }
    }

    private bool OutsideGrid(Position position){
        return position.Row < 0 || position.Row >= Rows ||
               position.Column < 0 || position.Column >= Columns;
    }

    private GridValue WillHit(Position newHeadPosition){
        if(OutsideGrid(newHeadPosition)){
            return GridValue.Outside;
        }

        if(newHeadPosition == GetTailPosition()){
            return GridValue.Empty;
        }

        return Grid[newHeadPosition.Row, newHeadPosition.Column];
    }

    public void MoveSnake(){

        if(directionChanges.Count > 0){
            CurrentDirection = directionChanges.First.Value;
            directionChanges.RemoveFirst();
        }

        Position newHeadPosition = GetHeadPosition().Translate(CurrentDirection);
        Position currentHeadPosition = GetHeadPosition();
        GridValue hit = WillHit(newHeadPosition);

        if(hit == GridValue.Snake){
            GameOver = true;
        }

        else if(hit == GridValue.Outside){
            if(currentHeadPosition.Row == Rows-1){
                newHeadPosition = new Position(0, newHeadPosition.Column);
            }
            if(currentHeadPosition.Row == 0){
                newHeadPosition = new Position(Rows-1, newHeadPosition.Column);
            }
            if(currentHeadPosition.Column == Columns-1){
                newHeadPosition = new Position(newHeadPosition.Row, 0);
            }
            if(currentHeadPosition.Column == 0){
                newHeadPosition = new Position(newHeadPosition.Row, Columns-1);
            }
        }

        if(hit == GridValue.Empty || hit == GridValue.Outside){
            RemoveTail();
            Grid[currentHeadPosition.Row, currentHeadPosition.Column] = GridValue.Snake;
            AddHead(newHeadPosition);
        }

        else if(hit == GridValue.Food){
            Grid[currentHeadPosition.Row, currentHeadPosition.Column] = GridValue.Snake;
            AddHead(newHeadPosition);
            Score++;
            AddFood();
        }
        else if(hit == GridValue.ToTetrisFood){
            RemoveTail();
            Grid[currentHeadPosition.Row, currentHeadPosition.Column] = GridValue.Snake;
            AddHead(newHeadPosition);
            foreach(var position in snakePosition){
                Grid[position.Row, position.Column] = GridValue.Tetris;
            }
            curentGame = Games.GameTetris;
        }
    }

    public async Task RunGame(){
		drawGrid = new DrawGrid(Rows, Columns, GameGrid, this);
		Draw();
		await ShowCountDown();
		Overlay.IsVisible = false;
		await GameLoop();
		await ShowGameOver();
	}

    private async Task GameLoop(){
		while(!GameOver){
			await Task.Delay(100);
            if(curentGame == Games.GameSnake){
			    MoveSnake();
			    Draw();
            }
            else{
                tetris = new Tetris(Rows, Columns, GameGrid, Overlay, OverlayText, ScoreText, Grid, Score);
                await tetris.RunGame();
                GameOver = true;
            }
		}
	}
}