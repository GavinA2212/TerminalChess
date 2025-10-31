namespace Chessmove;

using ChessPieces;

public class ChessMove
{
  public ChessPiece? piece { get; set; }
  public int initialX { get; set; }
  public int initialY { get; set; }
  public int moveToX { get; set; }
  public int moveToY { get; set; }

  public ChessMove(ChessPiece? piece, int initialX, int initialY, int moveToX, int moveToY)
  {
    this.piece = piece;
    this.initialX = initialX;
    this.initialY = initialY;
    this.moveToX = moveToX;
    this.moveToY = moveToY;
  }

  public void update(int initialX, int initialY, int moveToX, int moveToY)
  {
    this.initialX = initialX;
    this.initialY = initialY;
    this.moveToX = moveToX;
    this.moveToY = moveToY;
  }


  // En passant helper
  public bool wasPawnJump()
  {
    if (piece == null || piece.GetType() != typeof(Pawn))
    {
      return false;
    }

    if (Math.Abs(moveToY - initialY) != 2)
    {
      return false;
    }

    return true;
  }

}