using System;

namespace Thuja
{
    public interface IDisplayable
    {
        public (int x, int y, int layer) Position { get; }
        ColoredChar[,] Render();
    }

    public interface IFocusable : IWidget
    {
        public void FocusChange(bool focused);
    }
    
    public interface IWidget : IDisplayable
    {
        public int Fps => 0;

        public void Update(Tick tick)
        {
        }

        public bool BubbleDown(ConsoleKeyInfo key) => false;
    }
}