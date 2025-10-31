using BPrinter;
using Game;
using NUtils;
using ChessPieces;
using GameInteractor;

public class TerminalChessInteractor : IGameInteractor
{
  private ChessGame game;
  private BoardPrinter boardPrinter;
  public ChessPiece? selectedPiece{ get; set; }


  public TerminalChessInteractor(ChessGame chessgame)
  {
    game = chessgame;
    boardPrinter = new BoardPrinter(game.board);
    game.setInteractor(this);
  }

  public void start()
  {
    game.Start();

    while (game.gameRunning)
    {
      boardPrinter.printBoard(game.currentPlayer);
      Console.WriteLine("Player " + (game.currentPlayer == "white" ? 1 : 2) + " Move");
      Console.WriteLine("Input format example: a 1");
      promptForPieceSelection();
    }

    string winningPlayer = game.whiteInCheck == false ? "Black" : "White";
    Console.WriteLine("Checkmate!");
    Console.WriteLine(winningPlayer + " wins!");

  }

  private void promptForPieceSelection()
  {
    bool validInput = false;

    while (validInput == false)
    {
      Console.Write("Enter piece coordinates: ");
      string? input = Console.ReadLine();

      if (input == null)
      {
        continue;
      }

      if (!validChessInput(input))
      {
        continue;
      }

      string[] splitInput = input.Split(" ");
      char selectedLetter = splitInput[0][0];
      int selectedNumber = int.Parse(splitInput[1]);

      var (piece, errStatus, message) = game.selectPiece(selectedLetter, selectedNumber, game.currentPlayer);
      if (errStatus == 1)
      {
        Console.WriteLine(message);
        continue;
      }
      validInput = true;
      Console.WriteLine(message + " - (To select a different piece enter 'back')");

      if (piece != null)
      {
        selectedPiece = piece;
      }
    }

    promptForMove();
  }


  private void promptForMove()
  {

    bool validInput = false;

    while (validInput == false)
    {
      Console.Write("Enter square coordinates: ");
      string? input = Console.ReadLine();

      if (input == "back" || selectedPiece == null)
      {
        promptForPieceSelection();
        return;
      }

      if (input == null || !validChessInput(input))
      {
        continue;
      }

      string[] splitInput = input.Split(" ");
      var (moveX, moveY) = Utils.convertFromChessNotation(splitInput[0][0], int.Parse(splitInput[1]));

      var (successfullyMoved, playerMoved) = game.tryMove(selectedPiece, moveX, moveY);

      if (!successfullyMoved)
      {
        Console.WriteLine("Invalid move try again");
        continue;
      }

      // Print board from same players pov with a 1.5 sec delay before board rotates
      boardPrinter.printBoard(playerMoved);
      Thread.Sleep(1500);

      validInput = true;
    }
  }
  

  public void promptPawnPromotion(int currentX, int currentY, int x, int y, string team)
  {
    bool validInput = false;

    string[] validPromotionPieces = { "queen", "rook", "knight", "bishop" };

    while (validInput == false)
    {
      Console.WriteLine("Choose a promotion piece(queen, rook, bishop, knight): ");
      string? promoInput = Console.ReadLine();

      if (!validPromotionPieces.Contains(promoInput) || promoInput == null)
      {
        continue;
      }

      // Valid input
      (ChessPiece? newPiece, _) = game.promotePawn(promoInput, currentX, currentX, x, y, team);
      selectedPiece = newPiece;
      
      validInput = true;
    }
  }


  private bool validChessInput(string? input)
  {
    if (input == "" || input == null)
    {
      return false;
    }

    string[] splitInput = input.Split(" ");
    if (splitInput.Length != 2)
    {
      Console.WriteLine("Invalid input: should be in format (letter number)");
      return false;
    }

    bool moreThanOneChar = splitInput[0].Length != 1 || splitInput[1].Length != 1;

    if (moreThanOneChar)
    {
      Console.WriteLine("Invalid input: values should only be 1 character large");
      return false;
    }

    int val1 = (int)splitInput[0][0];
    if (!validLetter(val1))
    {
      Console.WriteLine("Invalid first letter input: not a-h");
      return false;
    }

    int val2 = (int)splitInput[1][0];
    if (!validNumber(val2))
    {
      Console.WriteLine("Invalid first number input: not 1-8");
      return false;
    }

    return true;
  }
  
  private bool validLetter(int letterVal)
  {
    // a-h ascii
    if (letterVal >= 97 && letterVal <= 104)
    {
      return true;
    }
    else
    {
      return false;
    }
  }

  private bool validNumber(int numberVal)
  {
    // 1-8 ascii
    if (numberVal >= 49 && numberVal <= 56)
    {
      return true;
    }
    else
    {
      return false;
    }
  }
}