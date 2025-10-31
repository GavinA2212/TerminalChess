

using System.Runtime.CompilerServices;
using Game;

namespace ChessPieces

{
  public abstract class ChessPiece
  {
    public int xPosition { get; set; }
    public int yPosition { get; set; }
    public string display { get; set; } = "";
    public string team { get; set; }
    public bool firstMove = true;


    public ChessPiece(int xposition, int yposition, string team)
    {
      xPosition = xposition;
      yPosition = yposition;
      this.team = team;
    }


    abstract public bool canMove(ChessPiece?[][] board, ChessGame game, bool simulatedMove, int x, int y);


    protected bool validUniversalBaseMove(ChessPiece?[][] board, int x, int y)
    {
      ChessPiece? moveLocation = board[x][y];

      bool outsideBoard = x < 0 || x > 7 || y < 0 || y > 7;

      bool sameSquareAsSelf = x == xPosition && y == yPosition;

      bool alliedPieceOnMove = moveLocation != null && moveLocation.team == team;

      if (outsideBoard)
      {
        return false;
      }

      if (sameSquareAsSelf)
      {
        return false;
      }

      if (alliedPieceOnMove)
      {
        return false;
      }

      return true;
    }


    protected bool pieceBetweenPieceAndMove(ChessPiece?[][] board, int x, int y)
    {
      foreach (var (squareX, squareY) in squaresBetween(xPosition, yPosition, x, y))
      {
        bool pieceExists = board[squareX][squareY] != null;

        if (pieceExists && board[squareX][squareY] != null)
        {
          return true;
        }
      }
      return false;
    }


    private IEnumerable<(int x, int y)> squaresBetween(int startX, int startY, int endX, int endY)
    {
      int xStep = Math.Sign(endX - startX);
      int yStep = Math.Sign(endY - startY);

      int xVal = startX + xStep;
      int yVal = startY + yStep;

      while (xVal != endX || yVal != endY)
      {
        yield return (xVal, yVal);
        xVal += xStep; yVal += yStep;
      }
    }


    protected bool moveResultsInCheckOnSelf(ChessGame game, int x, int y)
    {
      bool resultsInCheckOnSelf = game.resultsInCheck(xPosition, yPosition, x, y, team);

      if (resultsInCheckOnSelf)
      {
        return true;
      }

      return false;
    }

    
    protected void removeCheck(ChessGame game){
      bool playerinCheck = team == "white" ? game.whiteInCheck : game.blackInCheck;
      if (playerinCheck)
      {
        if (team == "white")
        {
          game.whiteInCheck = false;
        }
        else
        {
          game.blackInCheck = false;
        }
      }
    }
  }
}