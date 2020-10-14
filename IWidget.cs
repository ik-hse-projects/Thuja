using System;

namespace Thuja
{
    public interface IFocusable : IWidget
    {
        public bool CanFocus => true;
        public void FocusChange(bool isFocused);
    }

    public interface IWidget
    {
        public int Fps => 0;

        public void OnRegistered(MainLoop loop)
        {
        }

        void Render(RenderContext context);

        public void Update()
        {
        }

        public bool BubbleDown(ConsoleKeyInfo key)
        {
            return false;
        }
    }
}