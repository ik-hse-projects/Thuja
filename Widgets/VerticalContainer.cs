using System;
using System.Linq;

namespace Thuja.Widgets
{
    public class VerticalContainer: BaseContainer
    {
        public override void Render(RenderContext context)
        {
            var offsetY = 0;
            foreach (var widget in widgets)
            {
                var newPosition = (0, offsetY, 0);
                var ctx = context.Derive(newPosition);
                widget.Render(ctx);
                offsetY += ctx.Size.y;
            }
        }
        
        private bool MoveSelection(int direction)
        {
            var focusable = widgets
                .OfType<IFocusable>()
                .Where(w => w.CanFocus)
                .ToList();
            if (focusable.Count == 0)
            {
                return false;
            }

            if (Focused == null)
            {
                Focused = focusable[0];
                return true;
            }

            var currentlySelected = focusable.FindIndex(x => ReferenceEquals(x, Focused));
            var newIndex = currentlySelected + direction;
            if (newIndex < 0 || newIndex >= focusable.Count)
            {
                return false;
            }

            Focused = focusable[newIndex];
            return true;
        }

        public override bool BubbleUp(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.Tab when key.Modifiers.HasFlag(ConsoleModifiers.Shift):
                case ConsoleKey.UpArrow:
                    return MoveSelection(-1);
                case ConsoleKey.Enter:
                case ConsoleKey.Tab:
                case ConsoleKey.DownArrow:
                    return MoveSelection(+1);
                default:
                    return false;
            }
        }
        
    }
}