using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.Swift;
using System.Security.Cryptography.X509Certificates;
using Game;

namespace ChessPieces
{


  public class Pawn : ChessPiece
  {
    


    public Pawn(int x, int y, string team) : base(x, y, team)
    {
      display = team == "white" ? "◉" : "○";
    }


    public override bool canMove(ChessPiece?[][] board, ChessGame game, bool simulatedMove, int x, int y)
    {
      if (!validUniversalBaseMove(board, x, y))
      {
        return false;
      }

      if (!validPawnMove(board, game, x, y))
      {
        return false;
      }

      if (moveResultsInCheckOnSelf(game, x, y))
      {
        return false;
      }

      int endY = team == "white" ? 7 : 0;
      bool promotionMove = y == endY;

      if (promotionMove && !simulatedMove)
      {
        if (game.interactor == null)
        {
          return false;
        }

        game.interactor.promptPawnPromotion(xPosition, yPosition, x, y, team);
      }

      // Remove check if it exists
      removeCheck(game);

      return true;
    }


    private bool validPawnMove(ChessPiece?[][] board, ChessGame game, int x, int y)
    {
      int direction = team == "white" ? 1 : -1;
      int xMovement = (x - xPosition) * direction;
      int yMovement = (y - yPosition) * direction;

      // Not 1 or 2 spaces up and within 1 space horizontally
      if (yMovement < 1 || yMovement > 2 || xMovement > 1 || xMovement < -1)
      {
        return false;
      }

      // 2 spaces up x not same
      if (yMovement == 2 && xMovement != 0)
      {
        return false;
      }

      // Move 2 without first move
      if (yMovement == 2 && firstMove == false)
      {
        return false;
      }

      // Allied or enemy piece on or between move (same x rank)
      if (xMovement == 0)
      {
        int i = this.yPosition;
        do
        {
          i += direction;
          ChessPiece? piece = board[x][i];
          if (piece != null)
          {

            return false;
          }

        } while (i != y);
      }

      ChessPiece? moveLocation = board[x][y];

      // En pessant
      var (validenpessant, _) = validEnPessant(game, x, y);

      bool validCapture = xMovement != 0 && moveLocation != null && moveLocation.team != team;

      if (xMovement != 0 && !validCapture && !validenpessant)
      {
        return false;
      }

      return true;
    }

    public (bool, ChessPiece?) validEnPessant(ChessGame game, int x, int y)
    {
      int direction = team == "white" ? 1 : -1;
      int xMovement = (x - xPosition) * direction;

      bool validEnPessant = xMovement != 0 && game.lastMove.wasPawnJump() && game.lastMove.moveToX == x && game.lastMove.moveToY == y - direction;
      if (validEnPessant)
      {
        return (true, game.lastMove.piece);
      }
      return (false, null);
    }
  }
}