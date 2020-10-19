namespace Thuja.Widgets
{
    /// <summary>
    /// Рамка и фон вокруг всего содержимого.
    /// </summary>
    public class Frame : BaseContainer
    {
        /// <summary>
        /// Создаёт рамку с опциональным цветом фона.
        /// По-умолчанию в качестве стиля самой рамки используется <see cref="Thuja.Style.Decoration"/>. 
        /// </summary>
        public Frame(MyColor background = MyColor.Default) : this(Style.Decoration, background)
        {
        }

        /// <summary>
        /// Создаёт рамку со стилем символов и опциональным цветом фона.
        /// </summary>
        public Frame(Style style, MyColor background = MyColor.Default)
        {
            Style = style;
            Background = background;
        }

        /// <summary>
        /// Стиль символов рамки.
        /// </summary>
        public Style Style { get; set; }
        
        /// <summary>
        /// Цвет фона под содержимым рамки.
        /// </summary>
        public MyColor Background { get; set; }

        /// <inheritdoc />
        public override void Render(RenderContext context)
        {
            var ctx = context.Derive((1, 1, 1));
            foreach (var widget in Widgets)
            {
                widget.Render(ctx);
            }

            var (width, height) = ctx.Size;

            if (width == 0 && height == 0)
            {
                return;
            }

            var right = width + 1;
            var bottom = height + 1;

            var above = context.Derive((0, 0, 1));
            for (var x = 1; x <= width; x++)
            {
                above[x, 0] = new ColoredChar(Style, '═');
                above[x, bottom] = new ColoredChar(Style, '═');
            }

            for (var y = 1; y <= height; y++)
            {
                above[0, y] = new ColoredChar(Style, '║');
                above[right, y] = new ColoredChar(Style, '║');
            }

            above[0, 0] = new ColoredChar(Style, '╔');
            above[right, 0] = new ColoredChar(Style, '╗');
            above[0, bottom] = new ColoredChar(Style, '╚');
            above[right, bottom] = new ColoredChar(Style, '╝');

            var bgStyle = new Style(Background, Background);
            for (var x = 0; x <= right; x++)
            for (var y = 0; y <= bottom; y++)
            {
                context[x, y] = new ColoredChar(bgStyle, ' ');
            }
        }
    }
}