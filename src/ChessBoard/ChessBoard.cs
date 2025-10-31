using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using ChessPieces;

namespace Board
{
  public class ChessBoard
  {
    public ChessPiece?[][] board = new ChessPiece?[8][];

    public ChessBoard(){
      // Initialize array of arrays
      for (int i = 0; i < 8; i++)
      {
        board[i] = new ChessPiece?[8];
      }
      Initialize();
    }

    public void Initialize(){
      // Initialize white ranks
      board[0][0] = new Rook(0, 0, "white");
      board[1][0] = new Knight(1, 0, "white");
      board[2][0] = new Bishop(2, 0, "white");
      board[3][0] = new Queen(3, 0, "white");
      board[4][0] = new King(4, 0, "white");
      board[5][0] = new Bishop( 5, 0, "white");
      board[6][0] = new Knight(6, 0, "white");
      board[7][0] = new Rook(7, 0, "white");

      // Initialize white pawns
      for (int i = 0; i < 8; i++){
        board[i][1] = new Pawn(i, 1, "white");
      }

      //Initialize black ranks
      board[0][7] = new Rook(0, 7, "black");
      board[1][7] = new Knight(1, 7, "black");
      board[2][7] = new Bishop(2, 7, "black");
      board[3][7] = new Queen(3, 7, "black");
      board[4][7] = new King(4, 7, "black");
      board[5][7] = new Bishop(5, 7, "black");
      board[6][7] = new Knight(6, 7, "black");
      board[7][7] = new Rook(7, 7, "black");

      // Initialize black pawns
      for (int i = 0; i < 8; i++){
        board[i][6] = new Pawn(i, 6, "black");
      }
    }    
  }
}