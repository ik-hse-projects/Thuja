using System;

namespace Thuja
{
    public class RenderContext
    {
        private readonly (int x, int y, int layer) absoluteOffset;
        private readonly Canvas canvas;

        private readonly ParentRef? parentRef;
        private (int x, int y) size;

        private RenderContext((int x, int y, int layer) absoluteOffset, Canvas canvas, ParentRef parentRef) : this(
            absoluteOffset, canvas)
        {
            this.parentRef = parentRef;
        }

        public RenderContext((int x, int y, int layer) absoluteOffset, Canvas canvas)
        {
            this.absoluteOffset = absoluteOffset;
            this.canvas = canvas;
            size = (-1, -1);
        }

        public (int x, int y) Size => (size.x + 1, size.y + 1);

        public ColoredChar this[int x, int y]
        {
            get => Get(x, y) ?? throw new IndexOutOfRangeException();
            set => Set(x, y, value);
        }

        public (int x, int y) CursorPosition
        {
            get => (
                Console.CursorLeft - absoluteOffset.x,
                Console.CursorTop - absoluteOffset.y
            );
            set
            {
                Console.CursorLeft = absoluteOffset.x + value.x;
                Console.CursorTop = absoluteOffset.y + value.y;
            }
        }

        public ColoredChar? Get(int x, int y)
        {
            return canvas.Get(x + absoluteOffset.x, y + absoluteOffset.y);
        }

        public void Set(int x, int y, ColoredChar ch)
        {
            if (x < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            if (y < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            UpdateSize(x, y);
            if (absoluteOffset.layer != 0)
            {
                ch = new ColoredChar(ch.Style, ch.Char, ch.Layer + absoluteOffset.layer);
            }

            canvas.TrySet(x + absoluteOffset.x, y + absoluteOffset.y, ch);
        }

        private void UpdateSize(int x, int y)
        {
            if (x > size.x)
            {
                size.x = x;
            }

            if (y > size.y)
            {
                size.y = y;
            }

            if (parentRef != null)
            {
                var p = parentRef.Value;
                p.Parent.UpdateSize(x + p.RelativeOffset.x, y + p.RelativeOffset.y);
            }
        }

        public RenderContext Derive((int x, int y, int layer) offset)
        {
            var newOffset = (
                absoluteOffset.x + offset.x,
                absoluteOffset.y + offset.y,
                absoluteOffset.layer + offset.layer
            );
            return new RenderContext(newOffset, canvas, new ParentRef(this, offset));
        }

        public void PlaceString(string str, Style style, int layer = 0)
        {
            for (var i = 0; i < str.Length; i++)
            {
                this[i, 0] = new ColoredChar(style, str[i], layer);
            }
        }

        private readonly struct ParentRef
        {
            public readonly RenderContext Parent;
            public readonly (int x, int y, int layer) RelativeOffset;

            public ParentRef(RenderContext parent, (int x, int y, int layer) relativeOffset)
            {
                Parent = parent;
                RelativeOffset = relativeOffset;
            }
        }
    }
}