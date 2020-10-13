using System;
using System.Linq;

namespace Thuja.Widgets
{
    public sealed class Container: BaseContainer
    {
        public override void Render(RenderContext context)
        {
            foreach (var widget in widgets)
            {
                widget.Render(context);
            }
        }
    }
    
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

    public class RelativePosition : BaseContainer
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Layer { get; set; }
        
        public void Add(IWidget widget)
        {
            base.Add(widget);
            if (widget is IFocusable focusable)
            {
                Focused = focusable;
            }
        }
        
        public RelativePosition(int x, int y, int layer=0)
        {
            X = x;
            Y = y;
            Layer = layer;
        }

        public override void Render(RenderContext context)
        {
            foreach (var widget in widgets)
            {
                var newPosition = (X, Y, Layer);
                widget.Render(context.Derive(newPosition));
            }
        }
    }
}