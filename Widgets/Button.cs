using System;

namespace Thuja.Widgets
{
    public class Button: Label, IFocusable
    {
        public Button(string text) : base(text)
        {
        }

        public override Style CurrentStyle => isFocused ? Style.Active : Style.Inactive;

        public Style FocusedStyle { get; set; } = Style.Active;
        public Style UnfocusedStyle { get; set; } = Style.Inactive;

        public event Action<Button>? OnClick;

        private bool isFocused;

        public void Render(RenderContext context)
        {
            if (isFocused)
            {
                context.CursorPosition = (0, 0);
            }

            base.Render(context);
        }

        public bool BubbleDown(ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.Spacebar)
            {
                OnClick?.Invoke(this);
                return true;
            }

            return false;
        }

        public void FocusChange(bool isFocused)
        {
            this.isFocused = isFocused;
        }
    }
}