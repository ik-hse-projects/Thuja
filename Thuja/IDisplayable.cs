namespace Thuja
{
    public interface IDisplayable
    {
        public (int x, int y) Position { get; }
        ColoredChar?[,] Render();
    }
}