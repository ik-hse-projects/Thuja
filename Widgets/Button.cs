using System;

namespace Thuja.Widgets
{
    public class Button: Label, IFocusable
    {
        public Button(string text) : base(text)
        {
        }
        
        public Style FocusedStyle { get; set; } = Style.Active;
        public Style UnfocusedStyle { get; set; } = Style.Inactive;

        public bool CanFocus => OnClick != null;

        public event Action<Button>? OnClick;

        public void Render(RenderContext context)
        {
            context.CursorPosition = (0, 0);
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
            CurrentStyle = isFocused ? FocusedStyle : UnfocusedStyle;
        }
    }
}