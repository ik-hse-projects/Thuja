using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    
    public class Button: Label, IFocusable, IEnumerable<(KeySelector, Action)>
    {
        public Button(string text) : base(text)
        {
        }

        public override Style CurrentStyle => isFocused ? Style.Active : Style.Inactive;

        public Style FocusedStyle { get; set; } = Style.Active;
        public Style UnfocusedStyle { get; set; } = Style.Inactive;

        public Dictionary<KeySelector, Action> Actions { get; } = new Dictionary<KeySelector, Action>();

        private bool isFocused;

        public void Render(RenderContext context)
        {
            if (isFocused)
            {
                context.CursorPosition = (0, 0);
            }

            base.Render(context);
        }

        public void Add((KeySelector selector, Action action) handler)
        {
            Actions.Add(handler.selector, handler.action);
        }

        public bool BubbleDown(ConsoleKeyInfo key)
        {
            var result = false;
            foreach (var (selector, handler) in Actions)
            {
                if (selector.Match(key))
                {
                    handler();
                    result = true;
                }
            }

            return result;
        }

        public void FocusChange(bool isFocused)
        {
            this.isFocused = isFocused;
        }

        public IEnumerator<(KeySelector, Action)> GetEnumerator()
        {
            return Actions
                .Select(kv => (kv.Key, kv.Value))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}