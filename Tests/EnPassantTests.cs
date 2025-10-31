namespace Tests;

using Xunit;
using Game;
using ChessPieces;
using NUtils;

public class EnPassantTests
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
    public void validEnPassantWhiteCaptures()
    {
        Console.WriteLine("Running validEnPassantWhiteCaptures test");
        // Setup - White pawn at e5, black pawn jumps from d7 to d5
        ChessGame game = new ChessGame();
        (string, string)[] moves = [
            ("e 2", "e 4"), // White pawn to e4
            ("a 7", "a 6"), // Black moves (dummy move)
            ("e 4", "e 5"), // White pawn to e5
            ("d 7", "d 5")  // Black pawn jumps to d5 (two squares)
        ];
        makeMovesFromChessNotation(moves, game);

        ChessPiece? whitePawn = game.board[4][4]; // e5
        if (whitePawn == null || whitePawn.GetType() != typeof(Pawn))
        {
            Assert.Fail("White pawn not found at e5");
        }

        // White pawn should be able to capture en passant to d6
        bool canEnPassant = whitePawn.canMove(game.board, game, false, 3, 5);

        Assert.True(canEnPassant, "White pawn should be able to capture en passant");
    }

    [Fact]
    public void validEnPassantBlackCaptures()
    {
        Console.WriteLine("Running validEnPassantBlackCaptures test");
        // Setup - Black pawn at d4, white pawn jumps from e2 to e4
        ChessGame game = new ChessGame();
        (string, string)[] moves = [
            ("a 2", "a 3"), // White moves (dummy move)
            ("d 7", "d 5"), // Black pawn to d5
            ("a 3", "a 4"), // White moves (dummy move)
            ("d 5", "d 4"), // Black pawn to d4
            ("e 2", "e 4")  // White pawn jumps to e4 (two squares)
        ];
        makeMovesFromChessNotation(moves, game);

        ChessPiece? blackPawn = game.board[3][3]; // d4
        ChessPiece? whitePawn = game.board[4][3]; // e4
        if (blackPawn == null || blackPawn.GetType() != typeof(Pawn) || whitePawn == null)
        {
            Assert.Fail("Black pawn not found at d4");
        }

        // Manually set lastMove since handleMovePiece doesn't update it
        game.lastMove = new Chessmove.ChessMove(whitePawn, 4, 1, 4, 3);

        // Black pawn should be able to capture en passant to e3
        bool canEnPassant = blackPawn.canMove(game.board, game, false, 4, 2);

        Assert.True(canEnPassant, "Black pawn should be able to capture en passant");
    }

    [Fact]
    public void invalidEnPassantNotPawnJump()
    {
        Console.WriteLine("Running invalidEnPassantNotPawnJump test");
        // Setup - White pawn at e5, black pawn moves single square from d6 to d5
        ChessGame game = new ChessGame();
        (string, string)[] moves = [
            ("e 2", "e 4"), // White pawn to e4
            ("d 7", "d 6"), // Black pawn to d6 (single square)
            ("e 4", "e 5"), // White pawn to e5
            ("d 6", "d 5")  // Black pawn to d5 (single square, not a jump)
        ];
        makeMovesFromChessNotation(moves, game);

        ChessPiece? whitePawn = game.board[4][4]; // e5
        ChessPiece? blackPawn = game.board[3][4]; // d5
        if (whitePawn == null || whitePawn.GetType() != typeof(Pawn) || blackPawn == null)
        {
            Assert.Fail("White pawn not found at e5");
        }

        // Manually set lastMove for single square move (not a jump)
        game.lastMove = new Chessmove.ChessMove(blackPawn, 3, 5, 3, 4);

        // White pawn should NOT be able to capture en passant (not a jump)
        bool canEnPassant = whitePawn.canMove(game.board, game, false, 3, 5);

        Assert.False(canEnPassant, "En passant should not be valid when enemy pawn didn't jump");
    }

   

    [Fact]
    public void enPassantRemovesCapturedPawn()
    {
        Console.WriteLine("Running enPassantRemovesCapturedPawn test");
        // Setup - White pawn at e5, black pawn jumps from d7 to d5
        ChessGame game = new ChessGame();
        (string, string)[] moves = [
            ("e 2", "e 4"), // White pawn to e4
            ("a 7", "a 6"), // Black moves (dummy move)
            ("e 4", "e 5"), // White pawn to e5
            ("d 7", "d 5")  // Black pawn jumps to d5
        ];
        makeMovesFromChessNotation(moves, game);

        ChessPiece? whitePawn = game.board[4][4]; // e5
        ChessPiece? blackPawn = game.board[3][4]; // d5
        
        if (whitePawn == null || blackPawn == null)
        {
            Assert.Fail("Pawns not found");
        }

        // Manually set lastMove since handleMovePiece doesn't update it
        game.lastMove = new Chessmove.ChessMove(blackPawn, 3, 6, 3, 4);

        // Perform en passant capture
        game.currentPlayer = "white";
        game.tryMove(whitePawn, 3, 5); // White captures to d6

        ChessPiece? capturedPawnSquare = game.board[3][4]; // d5 should be empty
        ChessPiece? newPawnPosition = game.board[3][5]; // d6 should have white pawn

        Assert.Null(capturedPawnSquare);
        Assert.NotNull(newPawnPosition);
        Assert.True(newPawnPosition?.GetType() == typeof(Pawn), "White pawn should be at d6");
        Assert.Equal("white", newPawnPosition?.team);
    }

    [Fact]
    public void validEnPassantLeftSide()
    {
        Console.WriteLine("Running validEnPassantLeftSide test");
        // Setup - White pawn at d5, black pawn jumps from c7 to c5 (left side)
        ChessGame game = new ChessGame();
        (string, string)[] moves = [
            ("d 2", "d 4"), // White pawn to d4
            ("a 7", "a 6"), // Black moves (dummy move)
            ("d 4", "d 5"), // White pawn to d5
            ("c 7", "c 5")  // Black pawn jumps to c5 (left of white pawn)
        ];
        makeMovesFromChessNotation(moves, game);

        ChessPiece? whitePawn = game.board[3][4]; // d5
        ChessPiece? blackPawn = game.board[2][4]; // c5
        if (whitePawn == null || whitePawn.GetType() != typeof(Pawn) || blackPawn == null)
        {
            Assert.Fail("White pawn not found at d5");
        }

        // Manually set lastMove for the c7-c5 jump
        game.lastMove = new Chessmove.ChessMove(blackPawn, 2, 6, 2, 4);

        // White pawn should be able to capture en passant to c6 (left side)
        bool canEnPassant = whitePawn.canMove(game.board, game, false, 2, 5);

        Assert.True(canEnPassant, "White pawn should be able to capture en passant on left side");
    }

    [Fact]
    public void validEnPassantRightSide()
    {
        Console.WriteLine("Running validEnPassantRightSide test");
        // Setup - White pawn at d5, black pawn jumps from e7 to e5 (right side)
        ChessGame game = new ChessGame();
        (string, string)[] moves = [
            ("d 2", "d 4"), // White pawn to d4
            ("a 7", "a 6"), // Black moves (dummy move)
            ("d 4", "d 5"), // White pawn to d5
            ("e 7", "e 5")  // Black pawn jumps to e5 (right of white pawn)
        ];
        makeMovesFromChessNotation(moves, game);

        ChessPiece? whitePawn = game.board[3][4]; // d5
        ChessPiece? blackPawn = game.board[4][4]; // e5
        if (whitePawn == null || whitePawn.GetType() != typeof(Pawn) || blackPawn == null)
        {
            Assert.Fail("White pawn not found at d5");
        }

        // Manually set lastMove for the e7-e5 jump
        game.lastMove = new Chessmove.ChessMove(blackPawn, 4, 6, 4, 4);

        // White pawn should be able to capture en passant to e6 (right side)
        bool canEnPassant = whitePawn.canMove(game.board, game, false, 4, 5);

        Assert.True(canEnPassant, "White pawn should be able to capture en passant on right side");
    }
}
