namespace BPrinter;

using ChessPieces;

public class BoardPrinter
{
  private ChessPiece?[][] board;

  public BoardPrinter(ChessPiece?[][] board)
  {
    this.board = board;
  }

  public void printBoard(string pov)
  {
    int start, end, step;
    string letterRow;

    // Initialize values depending on pov
    if (pov == "white") {
      start = 7;
      end = -1;
      step = -1;
      letterRow = "     a     b     c     d     e     f     g     h   ";
    } else {
      start = 0;
      end = 8;
      step = 1;
      letterRow = "     h     g     f     e     d     c     b     a   ";
    }

    Console.WriteLine("\n" + letterRow);
    Console.WriteLine("\n   _______________________________________________ ");

    for (int i = start; i != end; i += step) {
      Console.WriteLine("  |     |     |     |     |     |     |     |     |");
      Console.Write((i + 1).ToString() + " |  ");
      for (int j = start; j != end; j += step) {
        // subtract from 7 to mirror horizontally
        var piece = board[7 - j][i];
        if (piece != null) {
          Console.Write(piece.display + "  |  ");
        }
        else {
          Console.Write("   |  ");
        }
      }

      Console.WriteLine("\n  |_____|_____|_____|_____|_____|_____|_____|_____|");
    }
    Console.WriteLine("\n" + letterRow + "\n");
  }
}