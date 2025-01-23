namespace SnakeGame;

public class Tetris : GameState
{
    private LinkedList<LinkedList<Position>> currentTetrisPosition = new LinkedList<LinkedList<Position>>();
    private LinkedList<LinkedList<Position>> fallenTetrisPosition = new LinkedList<LinkedList<Position>>();
    private bool isStrike = false;
    public Tetris(int rows, int cols, Grid gameGrid, Border overlay, Label overlayText, Label scoreText, GridValue[,] grid, int score) : base(rows, cols, gameGrid, overlay, overlayText, scoreText)
    {
        Score = score;
        Grid = grid;
        for(int col = 0; col < Columns; col++){
            var verticalList = new LinkedList<Position>();
            for (int row = 0; row < Rows; row++){
                if (Grid[row, col] == GridValue.Tetris){
                    verticalList.AddFirst(new Position(row, col));
                    Grid[row, col] = GridValue.Tetris;
                }
                else{
                    Grid[row, col] = GridValue.Empty;
                }
            }
            if (verticalList.Count > 0) currentTetrisPosition.AddFirst(verticalList);
        }
    }

    private void AddTetris(){
        switch(random.Next(5)){
            case 0:
                AddStripe();
                return;
            case 1:
                AddSquare();
                return;
            case 2:
                AddPanties();
                return;
            case 3:
                AddHookLeft();
                return;
            case 4:
                AddHookRight();
                return;
        }
    }

    private void AddStripe(){
        for (int i = 3; i < 7; i++){
            var verticalList = new LinkedList<Position>();
            verticalList.AddFirst(new Position(0, i));
            currentTetrisPosition.AddFirst(verticalList);
            Grid[0, i] = GridValue.Tetris;
        }
    }
    private void AddSquare(){ 
        for(int i = 4; i < 6; i++){
            var verticalList = new LinkedList<Position>();
            for (int c = 0; c < 2; c++){
                verticalList.AddFirst(new Position(c, i));
                Grid[c, i] = GridValue.Tetris;
            }
            currentTetrisPosition.AddFirst(verticalList);
        }
    }
    private void AddPanties(){ 
        for (int i = 3; i < 6; i++){
            var verticalList = new LinkedList<Position>();
            verticalList.AddFirst(new Position(0, i));
            if(i == 4){
                verticalList.AddFirst(new Position(1, 4));
                Grid[1, 4] = GridValue.Tetris;
            }
            currentTetrisPosition.AddFirst(verticalList);
            Grid[0, i] = GridValue.Tetris;
        }
    }
    private void AddHookLeft(){ 
        for (int i = 3; i < 6; i++){
            var verticalList = new LinkedList<Position>();
            verticalList.AddFirst(new Position(0, i));
            if(i == 3){
                verticalList.AddFirst(new Position(1, 3));
                Grid[1, 3] = GridValue.Tetris;
            }
            currentTetrisPosition.AddFirst(verticalList);
            Grid[0, i] = GridValue.Tetris;
        }
    }
    private void AddHookRight(){ 
        for (int i = 3; i < 6; i++){
            var verticalList = new LinkedList<Position>();
            verticalList.AddFirst(new Position(0, i));
            if(i == 5){
                verticalList.AddFirst(new Position(1, 5));
                Grid[1, 5] = GridValue.Tetris;
            }
            currentTetrisPosition.AddFirst(verticalList);
            Grid[0, i] = GridValue.Tetris;
        }
    }


    private bool OutsideGrid(Position position){
        return position.Row < 0 || position.Row >= Rows ||
               position.Column < 0 || position.Column >= Columns;
    }

    private GridValue WillHit(Position newBlockPosition){
        if(OutsideGrid(newBlockPosition)){
            return GridValue.Outside;
        }
        return Grid[newBlockPosition.Row, newBlockPosition.Column];
    }

    private bool CanMoveTetris(Direction direction){
        foreach (var linkedList in currentTetrisPosition){
            foreach (var position in linkedList){
                Position newPosition = position.Translate(direction);
                if(WillHit(newPosition) == GridValue.Outside || WillHit(newPosition) == GridValue.FallenTetris){
                    return false;
                }
            }
        }
        return true;
    }
    private bool HasReachedTop(){
        for (int col = 0; col < Columns; col++){
            if (Grid[0, col] == GridValue.FallenTetris){ 
                return true;
            }
        }
        return false;
    }

    private Position GetLowerBlock(LinkedList<Position> currentStripe){
        return currentStripe.First.Value;
    }

    private void AddLowBlock(Position position, LinkedList<Position> currentStripe){
        currentStripe.AddFirst(position);
        Grid[position.Row, position.Column] = GridValue.Tetris;
    }

    private void RemoveTopBlock(LinkedList<Position> currentStripe){
        Position topBlockPosition = currentStripe.Last.Value;
        Grid[topBlockPosition.Row, topBlockPosition.Column] = GridValue.Empty;
        currentStripe.RemoveLast();
    }

    private void ClearRow(int row){
        for (int col = 0; col < Columns; col++){
            Grid[row, col] = GridValue.Empty;
        }
    }

    private void ClearOldBlocks() {
        foreach (var verticalList in currentTetrisPosition) {
            foreach (var block in verticalList) {
                Grid[block.Row, block.Column] = GridValue.Empty;
            }
        }
    }

    private void PlaceNewBlocks() {
        foreach (var verticalList in currentTetrisPosition) {
            foreach (var block in verticalList) {
                Grid[block.Row, block.Column] = GridValue.Tetris;
            }
        }
    }

    private int[,] ConvertToMatrix(int maxRow, int maxCol, int minRow, int minCol){
        int sizeR = maxRow - minRow + 1;
        int sizeC = maxCol - minCol + 1;
        int[,] matrix = new int[sizeR, sizeC];

        foreach (var verticalList in currentTetrisPosition) {
            foreach (var block in verticalList) {
                int row = block.Row - minRow;
                int col = block.Column - minCol;
                matrix[row, col] = 1;
            }
        }

        return matrix;
    }


    private LinkedList<LinkedList<Position>> ConvertToBlocks(int[,] matrix, int minRow, int minCol, int maxRow, int maxCol) {
        LinkedList<LinkedList<Position>> newFigure = new LinkedList<LinkedList<Position>>();
        int sizeR = matrix.GetLength(0);
        int sizeC = matrix.GetLength(1);
        for (int col = 0; col < sizeC; col++) {
            LinkedList<Position> verticalList = new LinkedList<Position>();
            for (int row = 0; row < sizeR; row++) {
                if (matrix[row, col] == 1) {
                    verticalList.AddFirst(new Position(minRow + row, minCol + col));
                }
            }
            if (verticalList.Count > 0) newFigure.AddFirst(verticalList);
        }

        return newFigure;
    }


    private int[,] RotateMatrix(int[,] matrix) {
        int sizeR = matrix.GetLength(0);
        int sizeC = matrix.GetLength(1);
        int[,] newMatrix = new int[sizeC, sizeR];

        for (int i = 0; i < sizeR; i++) {
            for (int j = 0; j < sizeC; j++) {
                newMatrix[j, sizeR - 1 - i] = matrix[i, j];
            }
        }

        return newMatrix;
    }



    public void RotateTetris() {
        int maxRow = int.MinValue, maxCol = int.MinValue;
        int minRow = int.MaxValue, minCol = int.MaxValue;

        foreach (var stripe in currentTetrisPosition) {
            foreach (var block in stripe) {
                maxRow = Math.Max(maxRow, block.Row);
                minRow = Math.Min(minRow, block.Row);
                maxCol = Math.Max(maxCol, block.Column);
                minCol = Math.Min(minCol, block.Column);
            }
        }

        int[,] matrix = ConvertToMatrix(maxRow, maxCol, minRow, minCol);

        matrix = RotateMatrix(matrix);

        var newCurrentTetrisPosition = ConvertToBlocks(matrix, minRow, minCol, maxRow, maxCol);

        ClearOldBlocks();

        currentTetrisPosition = newCurrentTetrisPosition;

        PlaceNewBlocks();
    }


    public void MoveTetris(Direction direction){
        if(CanMoveTetris(direction)){
            if(direction == Direction.Down){
                foreach (var linkedList in currentTetrisPosition){
                    Position newPosition = GetLowerBlock(linkedList).Translate(direction);
                    RemoveTopBlock(linkedList);
                    AddLowBlock(newPosition, linkedList);
                }
            }
            else if(!isStrike){
                foreach (var linkedList in currentTetrisPosition){
                    foreach (var position in linkedList){
                        Grid[position.Row, position.Column] = GridValue.Empty;
                    }
                }
                foreach (var linkedList in currentTetrisPosition){
                    var verticalList = linkedList.First;
                    while (verticalList != null){
                        verticalList.Value = verticalList.Value.Translate(direction);
                        Grid[verticalList.Value.Row, verticalList.Value.Column] = GridValue.Tetris;
                        verticalList = verticalList.Next;
                    }
                }
            }
        }
        else if(direction == Direction.Down){
            isStrike = false;
            int strikeRow = Rows;
            foreach (var linkedList in currentTetrisPosition){
                foreach (var position in linkedList){
                    Grid[position.Row, position.Column] = GridValue.FallenTetris;
                }
            }
            for(int row = Rows - 1; row >= 0; row--){
                bool rowIsFull = true;
                for (int col = 0; col < Columns; col++){
                    if (Grid[row, col] != GridValue.FallenTetris){ 
                        rowIsFull = false;
                        break;
                    }
                }
                if(rowIsFull){
                    ClearRow(row);
                    isStrike = true;
                    strikeRow = row;
                    Score++;
                }
            }
            currentTetrisPosition.Clear();
            if(isStrike){
                for(int col = 0; col < Columns; col++){
                    var verticalList = new LinkedList<Position>();
                    for (int row = 0; row < strikeRow; row++){
                        if (Grid[row, col] == GridValue.FallenTetris){
                            verticalList.AddFirst(new Position(row, col));
                            Grid[row, col] = GridValue.Tetris;
                        }
                    }
                    if (verticalList.Count > 0) currentTetrisPosition.AddFirst(verticalList);
                }
            }
            if(HasReachedTop()){
                GameOver = true;
            }
            if(currentTetrisPosition.Count == 0){
                isStrike = false;
                AddTetris();
            } 
                
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
			MoveTetris(Direction.Down);
			Draw();
		}
	}

}