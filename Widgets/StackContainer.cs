using System;
using System.Collections.Generic;
using System.Linq;

namespace Thuja.Widgets
{
    public enum Orientation
    {
        Vertical,
        Horizontal
    }

    public class StackContainer : BaseContainer
    {
        public StackContainer(Orientation orientation = Orientation.Vertical, int margin = 0,
            int maxVisibleCount = int.MaxValue)
        {
            Orientation = orientation;
            Margin = margin;
            MaxVisibleCount = maxVisibleCount;
        }

        public Orientation Orientation { get; set; }
        public int Margin { get; set; }

        public int MaxVisibleCount { get; set; }
        private int position { get; set; }

        private IEnumerable<IWidget> FindVisible()
        {
            var start = position;
            var minStart = widgets.Count - MaxVisibleCount;
            if (start > minStart)
                start = minStart;
            else
                start -= MaxVisibleCount / 2;

            return widgets.Skip(start).Take(MaxVisibleCount);
        }

        public override void Render(RenderContext context)
        {
            var offsetY = 0;
            var offsetX = 0;
            foreach (var widget in FindVisible())
            {
                var newPosition = Orientation switch
                {
                    Orientation.Vertical => (0, offsetY, 0),
                    Orientation.Horizontal => (offsetX, 0, 0)
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
                .Select((widget, index) => (widget as IFocusable, index))
                .Where(w => w.Item1?.CanFocus ?? default)
                .ToList();
            if (focusable.Count == 0) return false;

            if (Focused == null)
            {
                (Focused, position) = focusable[0];
                return true;
            }

            var currentlySelected = focusable.FindIndex(x => ReferenceEquals(x.Item1, Focused));
            var newSelected = currentlySelected + direction;
            if (newSelected < 0 || newSelected >= focusable.Count) return false;

            (Focused, position) = focusable[newSelected];
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