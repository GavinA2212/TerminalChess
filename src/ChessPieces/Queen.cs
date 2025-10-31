using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using Game;
using NUtils;

namespace ChessPieces
{


  public class Queen : ChessPiece
  {

    public Queen(int x, int y, string team) : base(x, y, team)
    {
      display = team == "white" ? "♛" : "♕";
    }
    

    public override bool canMove(ChessPiece?[][] board, ChessGame game, bool simulatedMove, int x, int y)
    {

      if (!validUniversalBaseMove(board, x, y))
      {
        return false;
      }

      if (!validQueenMove(board, x, y))
      {
        return false;
      }

      // - Valid standard move at this point - 

      if (moveResultsInCheckOnSelf(game, x, y))
      {
        return false;
      }

      // Remove check if it exists
      removeCheck(game);

      return true;
    }


    private bool validQueenMove(ChessPiece?[][] board, int x, int y)
    {
      int xChange = x - xPosition;
      int yChange = y - yPosition;

      bool diagonal = Math.Abs(xChange) == Math.Abs(yChange);
      bool straight = xChange == 0 || yChange == 0;
    
      bool queenLike = diagonal || straight;

      if (!queenLike)
      {
        return false;
      }

      if (pieceBetweenPieceAndMove(board, x, y))
      {
        return false;
      }

      return true;
    }
  }
}