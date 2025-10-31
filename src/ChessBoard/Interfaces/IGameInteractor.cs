namespace GameInteractor;

public interface IGameInteractor
{
  public void promptPawnPromotion(int currentX, int currentY, int x, int y, string team);
}