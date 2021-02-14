using System;

namespace Thuja.Widgets
{
    public class Expandable : AssertRegistered, IFocusable
    {
        public IWidget Collapsed { get; set; }
        public IWidget Expanded { get; set; }
        public IWidget Current => isFocused ? Expanded : Collapsed;

        private bool isFocused;
        private MainLoop? mainLoop;

        public Expandable(IWidget collapsed, IWidget expanded)
        {
            Collapsed = collapsed;
            Expanded = expanded;
        }

        public override void OnRegistered(MainLoop loop)
        {
            loop.Register(Collapsed);
            loop.Register(Expanded);
            mainLoop = loop;
        }

        public override void OnUnregistered()
        {
            mainLoop?.Unregister(Collapsed);
            mainLoop?.Unregister(Expanded);
        }

        public override void Render(RenderContext context)
        {
            Current.Render(context);
        }

        public void FocusChange(bool isFocused)
        {
            if (this.isFocused == isFocused)
            {
                return;
            }

            this.isFocused = isFocused;

            if (Expanded is IFocusable focusable)
            {
                focusable.FocusChange(isFocused);
            }
        }

        public bool BubbleDown(ConsoleKeyInfo key)
        {
            if (Current is IFocusable focusable)
            {
                return focusable.BubbleDown(key);
            }

            return false;
        }
    }
}