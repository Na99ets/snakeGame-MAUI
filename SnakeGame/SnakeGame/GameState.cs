using Microsoft.Maui.Controls.Shapes;

namespace SnakeGame;

public class GameState
{
    public int Rows{get;}
    public int Columns{get;}
    public GridValue[,] Grid{get;}
    public Direction CurrentDirection{get; private set;}
    public int Score{get; private set;}
    public bool GameOver{get; private set;}

    private readonly LinkedList<Direction> directionChanges = new LinkedList<Direction>();

    private readonly LinkedList<Position> snakePosition = new LinkedList<Position>();
    private readonly Random random = new Random();

    public GameState(int rows, int cols)
    {
        Rows = rows;
        Columns = cols;
        Grid = new GridValue[rows, cols];
        CurrentDirection = Direction.Right;

        AddSnake();
        AddFood();
    }

    private void AddSnake(){
        int rowStartPosition = Rows / 2;

        for(int colStartPosition  = 1; colStartPosition <= 3; colStartPosition++){
            Grid[rowStartPosition, colStartPosition] = GridValue.Snake;
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
        Grid[position.Row, position.Column] = GridValue.Snake;
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
        GridValue hit = WillHit(newHeadPosition);

        if(hit == GridValue.Outside || hit == GridValue.Snake){
            GameOver = true;
        }

        else if(hit == GridValue.Empty){
            RemoveTail();
            AddHead(newHeadPosition);
        }

        else if(hit == GridValue.Food){
            AddHead(newHeadPosition);
            Score++;
            AddFood();
        }
    }
}