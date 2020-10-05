using System;

namespace Thuja
{
    public interface IFocusable : IWidget
    {
        public void FocusChange(bool focused);
    }
    
    public interface IWidget
    {
        public (int x, int y, int layer) RelativePosition { get; }
        void Render(RenderContext context);
        
        public int Fps => 0;

        public void Update(Tick tick)
        {
        }

        public bool BubbleDown(ConsoleKeyInfo key) => false;
    }
}