namespace Tests;

using Xunit;
using Game;
using ChessPieces;
using NUtils;
using BPrinter;

public class CheckTests
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
    public void checkTests()
    {
        // Setup
        ChessGame game = new ChessGame();
        (string, string)[] moves = [
            ("e 1", "e 3"), // white  king to e 3
            ("e 8", "e 6"), // black rook1 to e 6
            ("a 1", "a 4"), // white1 rook to a 4
            ("h 1", "h 6") // white2 rook to h 6
        ];
        makeMovesFromChessNotation(moves, game);

        ChessPiece? whiterook1 = game.board[0][3];
        ChessPiece? whiterook2 = game.board[7][5];
        ChessPiece? whiteking = game.board[4][2];
        if (whiterook1 == null || whiterook2 == null || whiteking == null)
        {
            Assert.Fail("invalid test piece selection");
        }

        var nonCheckBlockMove = whiterook1.canMove(game.board, game, false, 0, 3);
        var checkBlockMove = whiterook1.canMove(game.board, game,false, 4, 3);
        var captureMove = whiterook2.canMove(game.board, game, false,  4, 5);
        var kingMoveAway = whiteking.canMove(game.board, game, false, 5, 2);

        Assert.False(nonCheckBlockMove, "non check interfering move should be invalid");
        Assert.True(checkBlockMove, "check block move should be valid");
        Assert.True(captureMove, "capture piece causing check should be valid");
        Assert.True(kingMoveAway, "king move away should be valid");
    }

    [Fact]
    public void checkMateTest()
    {
        // Setup
        ChessGame game = new ChessGame();
        (string, string)[] moves = [
            ("e 1", "h 3"), // white  king to h 3
            ("a 8", "g 6"), // black rook1 to g 6
            ("h 8", "e 5")  // black rook2 to e 5
        ];
        makeMovesFromChessNotation(moves, game);

        game.gameRunning = true;
        game.currentPlayer = "black";
        game.handleMovePiece(4, 4, 7, 4); // black rook2 to h 5 (laddercheckmate)

        var checkmate1 = game.gameRunning == false;

        Assert.True(checkmate1, "should cause checkmate");
    }

    [Fact]
    public void falseCheckMateTest()
    {
        // Setup
        ChessGame game = new ChessGame();
        (string, string)[] moves = [
            ("e 1", "h 3"), // white  king to h 3
            ("a 8", "g 6"), // black rook1 to g 6
            ("h 8", "e 5"), // black rook2 to e 5
            ("a 1", "c 4")  // white rook1 to c 4
        ];
        makeMovesFromChessNotation(moves, game);

        game.currentPlayer = "black";
        game.gameRunning = true;
        game.handleMovePiece(4, 4, 7, 4); // black rook2 to h 5 (laddercheckmate attempt)

        bool checkmate1 = game.gameRunning;


        Assert.True(checkmate1, "should not cause checkmate due to rook block");
    }

    [Fact]
    public void piecesBetweenTest()
    {
        // Setup
        ChessGame game = new ChessGame();
        (string, string)[] moves = [
            ("a 8", "g 6"), // black rook1 to g 6
            ("h 8", "g 5"), // black rook2 to e 5
        ];
        makeMovesFromChessNotation(moves, game);

        ChessPiece? piece = game.board[6][5];
        if (piece == null)
        {
            Assert.Fail("failed");
        }

        bool val = piece.canMove(game.board, game, false, 6, 2);

        Assert.False(val, "should not be a valid move");
    
    }
}
