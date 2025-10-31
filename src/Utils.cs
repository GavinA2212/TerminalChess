namespace NUtils;

public static class Utils
{
  public static (int, int) convertFromChessNotation(char letter, int number)
  {
    int letterConverted = (int)letter - 97;
    int numberConverted = number - 1;
    return (letterConverted, numberConverted);
  }

  public static (char, int) convertToChessNotation(int letter, int number)
  {
    char letterConverted = (char)(letter + 97);
    int numberConverted = number + 1;
    return (letterConverted, numberConverted);
  }

  // Iteratively returns the next square between two squares
  public static IEnumerable<(int x, int y)> squaresBetween(int startX, int startY, int endX, int endY)
  {
    int xStep = Math.Sign(endX - startX);
    int yStep = Math.Sign(endY - startY);

    int xVal = startX + xStep;
    int yVal = startY + yStep;

    while (xVal != endX || yVal != endY)
    {
      yield return (xVal, yVal);
      xVal += xStep; yVal += yStep;
    }
  }
}