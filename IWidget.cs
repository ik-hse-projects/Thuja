using System;

namespace Thuja
{
    public interface IFocusable : IWidget
    {
        public void FocusChange(bool isFocused);
    }
    
    public interface IWidget
    {
        public void OnRegistered(MainLoop loop)
        {
        }

        void Render(RenderContext context);
        
        public int Fps => 0;

        public void Update(Tick tick)
        {
        }

        public bool BubbleDown(ConsoleKeyInfo key) => false;
    }
}