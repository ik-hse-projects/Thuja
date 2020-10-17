using System;
using System.Collections.Generic;

namespace Thuja.Widgets
{
    public class Button : Label, IKeyHandler
    {
        private bool isFocused;

        public Button(string text, int maxWidth = int.MaxValue) : base(text, maxWidth)
        {
        }

        public override Style CurrentStyle => isFocused ? Style.Active : Style.Inactive;

        public Style FocusedStyle { get; set; } = Style.Active;
        public Style UnfocusedStyle { get; set; } = Style.Inactive;

        public Dictionary<HashSet<KeySelector>, Action> Actions { get; } = new Dictionary<HashSet<KeySelector>, Action>();

        public override void Render(RenderContext context)
        {
            if (isFocused) context.CursorPosition = (0, 0);

            base.Render(context);
        }

        public bool BubbleDown(ConsoleKeyInfo key)
        {
            return AsIKeyHandler().TryHandleKey(key);
        }

        public void FocusChange(bool isFocused)
        {
            this.isFocused = isFocused;
        }

        public IKeyHandler AsIKeyHandler() => this;
    }
}