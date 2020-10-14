namespace Thuja.Widgets
{
    public class Frame : BaseContainer
    {
        public Frame(MyColor background = MyColor.Default) : this(Style.Decoration, background)
        {
        }

        public Frame(Style style, MyColor background = MyColor.Default)
        {
            Style = style;
            Background = background;
        }

        public Style Style { get; set; }
        public MyColor Background { get; set; }

        public override void Render(RenderContext context)
        {
            var ctx = context.Derive((1, 1, 1));
            foreach (var widget in widgets) widget.Render(ctx);

            var (width, height) = ctx.Size;

            var right = width + 1;
            var bottom = height + 1;

            var above = context.Derive((0, 0, 1));
            above[0, 0] = new ColoredChar(Style, '╔');
            above[right, 0] = new ColoredChar(Style, '╗');
            above[0, bottom] = new ColoredChar(Style, '╚');
            above[right, bottom] = new ColoredChar(Style, '╝');
            for (var x = 1; x <= width; x++)
            {
                above[x, 0] = new ColoredChar(Style, '═');
                above[x, bottom] = new ColoredChar(Style, '═');
            }

            for (var y = 1; y <= height; y++)
            {
                context[0, y] = new ColoredChar(Style, '║');
                context[right, y] = new ColoredChar(Style, '║');
            }

            var bgStyle = new Style(MyColor.Transparent, Background);
            for (var x = 0; x <= right; x++)
            for (var y = 0; y <= bottom; y++)
                context[x, y] = new ColoredChar(bgStyle, ' ');
        }
    }
}