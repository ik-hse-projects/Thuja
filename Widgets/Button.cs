using System;
using System.Collections.Generic;

namespace Thuja.Widgets
{
    public readonly struct KeySelector
    {
        public readonly ConsoleModifiers Modifiers;
        public readonly ConsoleKey Key;

        public KeySelector(ConsoleKey key, ConsoleModifiers modifiers = 0)
        {
            Modifiers = modifiers;
            Key = key;
        }

        public bool Match(in ConsoleKeyInfo key)
        {
            return key.Modifiers.HasFlag(Modifiers) && key.Key == Key;
        }
    }

    public class Button : Label, IKeyHandler
    {
        private bool isFocused;

        public Button(string text, int maxWidth = int.MaxValue) : base(text, maxWidth)
        {
        }

        public override Style CurrentStyle => isFocused ? Style.Active : Style.Inactive;

        public Style FocusedStyle { get; set; } = Style.Active;
        public Style UnfocusedStyle { get; set; } = Style.Inactive;

        public Dictionary<KeySelector, Action> Actions { get; } = new Dictionary<KeySelector, Action>();


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