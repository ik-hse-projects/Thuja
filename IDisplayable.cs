namespace Thuja
{
    public interface IDisplayable
    {
        public (int x, int y, int layer) Position { get; }
        ColoredChar[,] Render();
    }
}