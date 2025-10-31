using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using Game;

namespace ChessPieces
{


  public class Knight : ChessPiece
  {


    public Knight(int x, int y, string team) : base(x, y, team)
    {
      display = team == "white" ? "♞" : "♘";
    }


    public override bool canMove(ChessPiece?[][] board, ChessGame game, bool simulatedMove, int x, int y)
    {
      if (!validUniversalBaseMove(board, x, y))
      {
        return false;
      }

      if (!validKnightMove(x, y))
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
    
    
    private bool validKnightMove(int x, int y)
    {
      int absXChange = Math.Abs(x - xPosition);
      int absYChange = Math.Abs(y - yPosition);
      bool knightLike = (absXChange == 1 && absYChange == 2) || (absXChange == 2 && absYChange == 1);

      if (!knightLike)
      {
        return false;
      }

      return true;
    }
  }
}