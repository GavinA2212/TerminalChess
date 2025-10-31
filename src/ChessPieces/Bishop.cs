using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using Game;
using NUtils;

namespace ChessPieces
{


  public class Bishop : ChessPiece
  {


    public Bishop(int x, int y, string team) : base(x, y, team)
    {
      this.display = team == "white" ? "♝" : "♗";
    }


    public override bool canMove(ChessPiece?[][] board, ChessGame game, bool simulatedMove, int x, int y)
    {

      if (!validUniversalBaseMove(board, x, y))
      {
        return false;
      }
      
      if(!validBishopMove(board, x, y))
      {
        return false;
      }

      // - Valid standard move at this point - 

      if(moveResultsInCheckOnSelf(game, x, y))
      {
        return false;
      }

      removeCheck(game);

      return true;
    }
    

    private bool validBishopMove(ChessPiece?[][] board, int x, int y)
    {
      int xChange = x - this.xPosition;
      int yChange = y - this.yPosition;

      bool diagonal = Math.Abs(xChange) == Math.Abs(yChange);

      if (!diagonal)
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