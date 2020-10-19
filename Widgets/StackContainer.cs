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

    public class StackContainer : BaseContainer, IWidget
    {
        private IFocusable? lastFocused;
        private int position;

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

        public void Update()
        {
            if (Focused != lastFocused)
            {
                lastFocused = Focused;
                MoveSelection(0);
            }
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

        private IEnumerable<IWidget> FindVisible()
        {
            if (Widgets.Count <= MaxVisibleCount)
            {
                return Widgets;
            }

            var offsetBefore = MaxVisibleCount / 2;
            var offsetAfter = MaxVisibleCount / 2;
            if (MaxVisibleCount % 2 != 0)
            {
                offsetAfter++;
            }

            var start = position - offsetBefore;
            var end = position + offsetAfter;

            if (start < 0)
            {
                return Widgets.GetRange(0, MaxVisibleCount);
            }

            if (end > Widgets.Count)
            {
                return Widgets.GetRange(Widgets.Count - MaxVisibleCount, MaxVisibleCount);
            }

            return Widgets.GetRange(start, end - start);
        }

        private List<(IFocusable?, int index)> FindFocusable() =>
            Widgets
                .Select((widget, index) => (widget as IFocusable, index))
                .Where(w => w.Item1?.CanFocus ?? false)
                .ToList();

        private bool MoveSelection(int direction)
        {
            var focusable = FindFocusable();
            if (focusable.Count == 0)
            {
                return false;
            }

            var currentlySelected = focusable.FindIndex(x => ReferenceEquals(x.Item1, Focused));
            if (currentlySelected == -1)
            {
                var nearest = focusable.MinBy(i => Math.Abs(i.index - position));
                if (nearest == null)
                {
                    (Focused, position) = (null, 0);
                    return false;
                }

                currentlySelected = nearest.Value.index;
            }

            var newSelected = currentlySelected + direction;
            if (newSelected < 0 || newSelected >= focusable.Count)
            {
                return false;
            }

            (Focused, position) = focusable[newSelected];
            return true;
        }

        protected override bool BubbleUp(ConsoleKeyInfo key)
        {
            if (base.BubbleUp(key))
            {
                return true;
            }

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
                case ConsoleKey.End:
                case ConsoleKey.PageDown:
                {
                    var focusable = FindFocusable();
                    if (focusable.Count == 0)
                    {
                        return false;
                    }

                    (Focused, position) = focusable.Last();
                    return true;
                }
                case ConsoleKey.Home:
                case ConsoleKey.PageUp:
                {
                    var focusable = FindFocusable();
                    if (focusable.Count == 0)
                    {
                        return false;
                    }

                    (Focused, position) = focusable.First();
                    return true;
                }
                default:
                    return false;
            }
        }
    }
}