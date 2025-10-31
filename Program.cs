using Game;

ChessGame basegame = new ChessGame();
TerminalChessInteractor terminalChessGame = new TerminalChessInteractor(basegame);
terminalChessGame.start();