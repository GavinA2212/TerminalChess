using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using Game;

namespace ChessPieces
{


  public class King : ChessPiece
  {

    public King(int x, int y, string team) : base(x, y, team)
    {
      display = team == "white" ? "♚" : "♔";
    }


    public override bool canMove(ChessPiece?[][] board, ChessGame game, bool simulatedMove, int x, int y)
    {

      if (!validUniversalBaseMove(board, x, y))
      {
        return false;
      }

      (bool validcastle, _, _) = validCastle(board, game, simulatedMove, x, y);

      if(!validcastle && !validKingMove(x, y))
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


    private bool validKingMove(int x, int y)
    {
      int absXChange = Math.Abs(x - xPosition);
      int absYChange = Math.Abs(y - yPosition);

      if (absXChange > 1 || absYChange > 1)
      {
        return false;
      }

      return true;
    }

    public (bool, int rookX, int newRookX) validCastle(ChessPiece?[][] board, ChessGame game, Boolean simulatedMove, int x, int y)
    {
      if (firstMove == false || y != yPosition)
      {
        return (false, -1, -1);
      }

      int absXChange = Math.Abs(xPosition - x);
      bool validCastleXPosition = absXChange == 2;

      if (!validCastleXPosition)
      {
        return (false, -1, -1);
      }

      int distanceLeftRook = x;
      int distanceRightRook = 7 - x;

      bool useLeft = distanceLeftRook < distanceRightRook;
      int rookX = useLeft == true ? 0 : 7;

      ChessPiece? rook = board[rookX][y];
      bool invalidRook = rook == null || rook.GetType() != typeof(Rook) || rook.firstMove == false;

      if (invalidRook)
      {
        return (false, -1, -1);
      }

      if (pieceBetweenPieceAndMove(board, rookX, y))
      {
        return (false, -1, -1);
      }

      int direction = Math.Sign(x - xPosition);
      // Check on first square movement
      if (moveResultsInCheckOnSelf(game, xPosition + direction, y))
      {
        return (false, -1, -1);
      }

      int newRookX;
      if(rookX == 0)
      {
        newRookX = 3;
      }
      else
      {
        newRookX = 5;
      }

      return (true, rookX, newRookX);
    }
  }
}