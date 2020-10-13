using System;
using System.Linq;

namespace Thuja.Widgets
{
    public enum Orientation
    {
        Vertical,
        Horizontal,
    }
    
    public class StackContainer: BaseContainer
    {
        public Orientation Orientation { get; set; }
        public int Margin { get; set; }

        public StackContainer(Orientation orientation = Orientation.Vertical, int margin = 0)
        {
            Orientation = orientation;
            Margin = margin;
        }

        public override void Render(RenderContext context)
        {
            var offsetY = 0;
            var offsetX = 0;
            foreach (var widget in widgets)
            {
                var newPosition = Orientation switch
                {
                    Orientation.Vertical => (0, offsetY, 0),
                    Orientation.Horizontal => (offsetX, 0, 0),
                };
                var ctx = context.Derive(newPosition);
                widget.Render(ctx);
                offsetX += ctx.Size.x + Margin;
                offsetY += ctx.Size.y + Margin;
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
                case ConsoleKey.UpArrow when Orientation == Orientation.Vertical:
                case ConsoleKey.LeftArrow when Orientation == Orientation.Horizontal:
                    return MoveSelection(-1);
                case ConsoleKey.Enter:
                case ConsoleKey.Tab:
                case ConsoleKey.DownArrow when Orientation == Orientation.Vertical:
                case ConsoleKey.RightArrow when Orientation == Orientation.Horizontal:
                    return MoveSelection(+1);
                default:
                    return false;
            }
        }
        
    }
}