namespace Game;

using Board;
using ChessPieces;
using NUtils;
using GameInteractor;
using Chessmove;

public class ChessGame
{
  public bool gameRunning { get; set; } = false;
  public string currentPlayer { get; set; } = "white";
  public ChessMove lastMove = new ChessMove(null, 0, 0, 0, 0);

  public bool whiteInCheck { get; set; } = false;
  public bool blackInCheck { get; set; } = false;
 
  public ChessPiece?[][] board { get; set; }
  public IGameInteractor? interactor{ get; set; }


  public ChessGame()
  {
    ChessBoard initializedChessBoard = new ChessBoard();
    board = initializedChessBoard.board;
  }

  public void setInteractor(IGameInteractor interactor)
  {
    this.interactor = interactor;
  }

  public void Start()
  {
    gameRunning = true;
  }


  public (ChessPiece?, int, string) selectPiece(char letter, int number, string player)
  {
    var (x, y) = Utils.convertFromChessNotation(letter, number);

    bool outsideBoard = x < 0 || x > 7 || y < 0 || y > 7;
    if (outsideBoard)
    {
      return (null, 1, "Invalid input " + x + " " + y);
    }

    ChessPiece? selectedSquare = board[x][y];
    if (selectedSquare == null)
    {
      return (null, 1, "Invalid selection: no piece located at (" + letter + " " + number.ToString() + ")");
    }

    if (selectedSquare.team != player)
    {
      return (null, 1, "Invalid selection: this piece is not yours");
    }

    return (board[x][y], 0, "Selected " + selectedSquare.display + " [" + letter + " " + number.ToString() + "]");

  }
  

  public (bool success, string player) tryMove(ChessPiece selectedPiece, int moveX, int moveY)
  {
    bool canMove = selectedPiece.canMove(board, this, false, moveX, moveY);

    if (!canMove)
    {
      return (false, "");
    }

    int selectedYBeforeMoving = selectedPiece.yPosition;
    var (validcastle, rookX, newrookX) = potentialValidCastle(selectedPiece, moveX, moveY);
    var (validenpessant, pawnToDelete) = potentialValidEnPessant(selectedPiece, moveX, moveY);

    // Move piece
    string player = handleMovePiece(selectedPiece.xPosition, selectedPiece.yPosition, moveX, moveY);
    selectedPiece.firstMove = false;

    // Move castling rook if move was castle
    if (validcastle)
    {
      currentPlayer = player;
      handleMovePiece(rookX, selectedYBeforeMoving, newrookX, selectedYBeforeMoving);
    }

    // Remove En pessant captured pawn if En pessant
    if (validenpessant && pawnToDelete != null)
    {
      board[pawnToDelete.xPosition][pawnToDelete.yPosition] = null;
    }

    return (true, player);

  }

  public string handleMovePiece(int selectedX, int selectedY, int moveX, int moveY)
  {
    string playerMakingMove = currentPlayer;
    string oppositePlayer = currentPlayer == "white" ? "black" : "white";
    ChessPiece? selectedPiece = board[selectedX][selectedY];
    if(selectedPiece == null){
      return "";
    }
    moveSelectedPiece(selectedX, selectedY, moveX, moveY);
    lastMove = new ChessMove(selectedPiece, selectedX, selectedY, moveX, moveY);

    bool causesCheckOnEnemy = resultsInCheck(selectedX, selectedY, moveX, moveY, oppositePlayer);
    currentPlayer = oppositePlayer;

    if (!causesCheckOnEnemy)
    {
      return playerMakingMove;
    }
    
    if (inCheckMate(oppositePlayer))
    {
      gameRunning = false;
    }

    if (oppositePlayer == "white")
    {
      whiteInCheck = true;
    }
    else
    {
      blackInCheck = true;
    }
    
    return playerMakingMove;
  }
  

  public bool moveSelectedPiece(int selectedX, int selectedY, int x, int y)
  {
    ChessPiece? piece = board[selectedX][selectedY];
    if (piece == null)
    {
      return false;
    }
    board[selectedX][selectedY] = null;
    board[x][y] = piece;
    piece.xPosition = x;
    piece.yPosition = y;

    return true;
  }


  public bool inCheckMate(string player)
  {
    for (int i = 0; i < 8; i++)
    {
      for (int j = 0; j < 8; j++)
      {
        ChessPiece? piece = board[i][j];
        if (piece == null || piece.team != player)
        {
          continue;
        }

        if (hasCheckmateStoppingMove(piece, player))
        { 
          return false;
        }
      }
    }
    return true;
  }
  

  public bool hasCheckmateStoppingMove(ChessPiece piece, string player)
  {
    for (int i = 0; i < 8; i++)
    {
      for (int j = 0; j < 8; j++)
      {
        if (piece.canMove(board, this, true, i, j) && !resultsInCheck(piece.xPosition, piece.yPosition, i, j, player))
        {
          return true;
        }
      }
    }
    return false;
  }


  public bool resultsInCheck(int currentX, int currentY, int moveX, int moveY, string player)
  {
    ChessPiece? currentPiece = board[currentX][currentY];
    ChessPiece? moveSquarePiece = board[moveX][moveY];

    // Simulate move if move hasnt been made yet 
    bool moveMade = currentX == moveX && currentY == moveY;
    if(!moveMade)
    {
      moveSelectedPiece(currentX, currentY, moveX, moveY);
    }

    var (kingX, kingY) = findKing(player);

    // Check if any enemy piece can attack the king
    for (int i = 0; i < 8; i++)
    {
      for (int j = 0; j < 8; j++)
      {
        ChessPiece? piece = board[i][j];
        if (piece == null || piece.team == player)
        {
          continue;
        }
        if (piece.canMove(board, this, true, kingX, kingY))
        {
          // End simulation if simulating
          if (!moveMade)
          {
            unMovePiece(currentPiece, moveSquarePiece, currentX, currentY, moveX, moveY);
          }
          return true;
        }
      }
    }
    // End simulation if simulating
    if (!moveMade)
    {
      unMovePiece(currentPiece, moveSquarePiece, currentX, currentY, moveX, moveY);
    }
    
    return false;
  }


  private (int x, int y) findKing(string player)
  {
    for (int i = 0; i < 8; i++)
    {
      for (int j = 0; j < 8; j++)
      {
        ChessPiece? piece = board[i][j];
        bool currentPlayersKing = piece != null && piece.GetType() == typeof(King) && piece.team == player;
        if (currentPlayersKing)
        {
          return (i, j);
        }
      }
    }

    // This should never return
    return (-1, -1);
  }

  // Helper for simulating moves
  private bool unMovePiece(ChessPiece? currentPiece, ChessPiece? moveSquarePiece, int currentX, int currentY, int moveX, int moveY)
  {
    board[currentX][currentY] = currentPiece;
    if (currentPiece == null)
    {
      return false;
    }

    currentPiece.xPosition = currentX;
    currentPiece.yPosition = currentY;
    board[moveX][moveY] = moveSquarePiece;

    return true;
  }


  private (bool valid, int rookX, int newRookX) potentialValidCastle(ChessPiece selectedPiece, int moveX, int moveY)
  {
    bool notKing = selectedPiece.GetType() != typeof(King);
    if (notKing)
    {
      return (false, -1, -1);
    }

    King? king = selectedPiece as King;

    if (king == null)
    {
      return (false, -1, -1); ;
    }

    (bool validcastle, int rookX, int newRookX) = king.validCastle(board, this, false, moveX, moveY);

    return (validcastle, rookX, newRookX);
  }


  private (bool, ChessPiece?) potentialValidEnPessant(ChessPiece selectedPiece, int x, int y)
  {
    bool notPawn = selectedPiece.GetType() != typeof(Pawn);
    if (notPawn)
    {
      return (false, null);
    }

    Pawn? pawn = selectedPiece as Pawn;

    if (pawn == null)
    {
      return (false, null);
    }

    var (validenpessent, piece) = pawn.validEnPessant(this, x, y);
    return (validenpessent, piece);
    
  }
  
  
  public (ChessPiece?, bool) promotePawn(string promotionPiece, int currentX, int currentY, int x, int y, string team)
  {
    ChessPiece newPiece;

    switch (promotionPiece)
    {
      case "queen":
        newPiece = new Queen(x, y, team);
        break;
      case "rook":
        newPiece = new Rook(x, y, team);
        break;
      case "knight":
        newPiece = new Knight(x, y, team);
        break;
      case "bishop":
        newPiece= new Bishop(x, y, team);
        break;
      default:
        return (null, false);
    }
    
    // Remove old pawn
    board[currentX][currentY] = null;
    return (newPiece, true);
  }
    
}