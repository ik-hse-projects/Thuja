using System;

namespace Thuja
{
    public class RenderContext
    {
        private readonly RenderContext? parent;
        private readonly (int x, int y, int layer) offset;
        private readonly Canvas canvas;
        private (int x, int y) size;
        public (int x, int y) Size => size;

        public RenderContext((int x, int y, int layer) offset, Canvas canvas, RenderContext? parent = null)
        {
            this.offset = offset;
            this.canvas = canvas;
            this.parent = parent;
            this.size = (0, 0);
        }

        public ColoredChar this[int x, int y]
        {
            get => Get(x, y) ?? throw new IndexOutOfRangeException();
            set => Set(x, y, value);
        }

        public ColoredChar? Get(int x, int y) => canvas.Get(x + offset.x, y + offset.y);

        public void Set(int x, int y, ColoredChar ch)
        {
            if (x < 0) throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0) throw new ArgumentOutOfRangeException(nameof(x));

            UpdateSize(x, y);
            if (offset.layer != 0)
            {
                ch = new ColoredChar(ch.Style, ch.Char, ch.Layer + offset.layer);
            }

            canvas.TrySet(x + offset.x, y + offset.y, ch);
        }

        private void UpdateSize(int x, int y)
        {
            if (x > size.x) size.x = x;
            if (y > size.y) size.y = y;
            parent?.UpdateSize(x + offset.x, y + offset.y);
        }

        public RenderContext Derive((int x, int y, int layer) offset)
        {
            var newOffset = (
                this.offset.x + offset.x,
                this.offset.y + offset.y,
                this.offset.layer + offset.y
            );
            return new RenderContext(newOffset, canvas, this);
        }

        public void PlaceString(string str, Style style, int layer = 0)
        {
            for (var i = 0; i < str.Length; i++)
            {
                this[i, 0] = new ColoredChar(style, str[i], layer);
            }
        }

        public (int x, int y) CursorPosition
        {
            get => (
                Console.CursorLeft - offset.x,
                Console.CursorTop - offset.y
            );
            set
            {
                Console.CursorLeft = offset.x + value.x;
                Console.CursorTop = offset.y + value.y;
            }
        }
    }
}