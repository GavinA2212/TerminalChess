namespace Tests;

using Xunit;
using Game;
using ChessPieces;
using NUtils;
using BPrinter;

public class CastleTests
{
    // Helper to make moves on a board
    private void makeMovesFromChessNotation((string piece, string move)[] moves, ChessGame game)
    {
        foreach(var (piece, move) in moves)
        {
            string[] splitPieceInput = piece.Split(" ");
            char pieceSelectedLetter = splitPieceInput[0][0];
            int pieceSelectedNumber = int.Parse(splitPieceInput[1]);
            
            string[] splitMoveInput = move.Split(" ");
            char moveSelectedLetter = splitMoveInput[0][0];
            int moveSelectedNumber = int.Parse(splitMoveInput[1]);

            var (parsedPieceLetter, parsedPieceNumber) = Utils.convertFromChessNotation(pieceSelectedLetter, pieceSelectedNumber);
            var (parsedMoveLetter, parsedMoveNumber) = Utils.convertFromChessNotation(moveSelectedLetter, moveSelectedNumber);

            game.handleMovePiece(parsedPieceLetter, parsedPieceNumber, parsedMoveLetter, parsedMoveNumber);
        }
    }

    [Fact]
    public void validKingSideCastleWhite()
  { 
        Console.WriteLine("Running validKingSideCastleWhite test");
        // Setup - Clear path between king and kingside rook
        ChessGame game = new ChessGame();
        (string, string)[] moves = [
            ("f 1", "f 3"), // Move white bishop out
            ("g 1", "g 3")  // Move white knight out
        ];
        makeMovesFromChessNotation(moves, game);

        ChessPiece? whiteKing = game.board[4][0];
        if (whiteKing == null || whiteKing.GetType() != typeof(King))
        {
            Assert.Fail("White king not found");
        }

        // King should be able to castle kingside (move to g1)
        bool canCastle = whiteKing.canMove(game.board, game, false, 6, 0);

        Assert.True(canCastle, "White should be able to castle kingside");
    }

    [Fact]
    public void validQueenSideCastleWhite()
  {
        Console.WriteLine("Running validQueenSideCastleWhite test");
        // Setup - Clear path between king and queenside rook
        ChessGame game = new ChessGame();
        (string, string)[] moves = [
            ("b 1", "b 3"), // Move white knight out
            ("c 1", "c 3"), // Move white bishop out
            ("d 1", "d 3")  // Move white queen out
        ];
        makeMovesFromChessNotation(moves, game);

        ChessPiece? whiteKing = game.board[4][0];
        if (whiteKing == null || whiteKing.GetType() != typeof(King))
        {
            Assert.Fail("White king not found");
        }

        // King should be able to castle queenside (move to c1)
        bool canCastle = whiteKing.canMove(game.board, game, false, 2, 0);

        Assert.True(canCastle, "White should be able to castle queenside");
    }

    [Fact]
    public void validKingSideCastleBlack()
  {
        Console.WriteLine("Running validKingSideCastleBlack test");
        // Setup - Clear path between king and kingside rook
        ChessGame game = new ChessGame();
        (string, string)[] moves = [
            ("f 8", "f 6"), // Move black bishop out
            ("g 8", "g 6")  // Move black knight out
        ];
        makeMovesFromChessNotation(moves, game);

        ChessPiece? blackKing = game.board[4][7];
        if (blackKing == null || blackKing.GetType() != typeof(King))
        {
            Assert.Fail("Black king not found");
        }

        // King should be able to castle kingside (move to g8)
        bool canCastle = blackKing.canMove(game.board, game, false, 6, 7);

        Assert.True(canCastle, "Black should be able to castle kingside");
    }

    [Fact]
    public void invalidCastleKingMoved()
    {
        // Setup - King has moved
        ChessGame game = new ChessGame();
        (string, string)[] moves = [
            ("f 1", "f 3"), // Move white bishop out
            ("g 1", "g 3"), // Move white knight out
        ];
        makeMovesFromChessNotation(moves, game);

        ChessPiece? whiteKing = game.board[4][0];
        
        if (whiteKing == null || whiteKing.GetType() != typeof(King))
        {
          Assert.Fail("White king not found");
        }else{
          whiteKing.firstMove = false;
        }

        // King should NOT be able to castle after moving
        bool canCastle = whiteKing.canMove(game.board, game, false, 6, 0);

        Assert.False(canCastle, "King should not be able to castle after moving");
    }

    [Fact]
    public void invalidCastleRookMoved()
    {
        // Setup - Rook has moved
        ChessGame game = new ChessGame();
        (string, string)[] moves = [
            ("f 1", "f 3"), // Move white bishop out
            ("g 1", "g 3") // Move white knight out
        ];
        makeMovesFromChessNotation(moves, game);

        ChessPiece? whiteKing = game.board[4][0];
        ChessPiece? whiteRook = game.board[7][0];
        if (whiteKing == null || whiteKing.GetType() != typeof(King) || whiteRook == null || whiteRook.GetType() != typeof(Rook))
        {
            Assert.Fail("White king not found");
        }else{
          whiteRook.firstMove = false;
        }

        // King should NOT be able to castle after rook moved
        bool canCastle = whiteKing.canMove(game.board, game, false, 6, 0);

        Assert.False(canCastle, "King should not be able to castle after rook moved");
    }

    [Fact]
    public void invalidCastlePieceInWay()
    {
        // Setup - Piece blocking castle path
        ChessGame game = new ChessGame();
        (string, string)[] moves = [
            ("f 1", "f 3") // Move white bishop out, but leave knight
        ];
        makeMovesFromChessNotation(moves, game);

        ChessPiece? whiteKing = game.board[4][0];
        if (whiteKing == null || whiteKing.GetType() != typeof(King))
        {
            Assert.Fail("White king not found");
        }

        // King should NOT be able to castle with piece in the way
        bool canCastle = whiteKing.canMove(game.board, game, false, 6, 0);

        Assert.False(canCastle, "King should not be able to castle with piece blocking");
    }

    [Fact]
    public void invalidCastleThroughCheck()
    {
        // Setup - Enemy piece attacking square king must pass through
        ChessGame game = new ChessGame();
        (string, string)[] moves = [
            ("f 1", "a 4"), // Move white bishop out
            ("g 1", "a 5"), // Move white knight out
            ("f 2", "a 3"), // Move f pawn to open f file
            ("a 8", "f 5")  // Black rook attacks f file
        ];
        makeMovesFromChessNotation(moves, game);

        ChessPiece? whiteKing = game.board[4][0];
    if (whiteKing == null || whiteKing.GetType() != typeof(King))
    {
      Assert.Fail("White king not found");
    }

    BoardPrinter printer = new BoardPrinter(game.board);
    printer.printBoard("white");

        // King should NOT be able to castle through check
        bool canCastle = whiteKing.canMove(game.board, game, false, 6, 0);

        Assert.False(canCastle, "King should not be able to castle through check");
    }

  [Fact]
  public void castleMovesRookCorrectly()

  {
    Console.WriteLine("Running castleMovesRookCorrectly test");

    // Setup - Valid castle scenario
    ChessGame game = new ChessGame();
        
    (string, string)[] moves = [
        ("f 1", "f 3"), // Move white bishop out
        ("g 1", "g 3")  // Move white knight out
    ];
    makeMovesFromChessNotation(moves, game);

    // Perform castle
    game.currentPlayer = "white";

    ChessPiece? whiteKing = game.board[4][0];
    if (whiteKing == null){
      Assert.Fail("null piece selected");
    }

    game.tryMove(whiteKing, 6, 0); // King castles kingside

    ChessPiece? kingAfterCastle = game.board[6][0];
    ChessPiece? rookAfterCastle = game.board[5][0];
    ChessPiece? oldKingPosition = game.board[4][0];
    ChessPiece? oldRookPosition = game.board[7][0];

    Assert.NotNull(kingAfterCastle);
    Assert.NotNull(rookAfterCastle);
    Assert.Null(oldKingPosition);
    Assert.Null(oldRookPosition);
    Assert.True(kingAfterCastle?.GetType() == typeof(King), "King should be at g1");
    Assert.True(rookAfterCastle?.GetType() == typeof(Rook), "Rook should be at f1");
  }
}
