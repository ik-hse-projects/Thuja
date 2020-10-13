namespace Thuja.Widgets
{
    public class Frame: BaseContainer
    {
        public Style Style { get; set; } = new Style(MyColor.Gray, MyColor.Black);

        public Frame()
        {
        }
        
        public Frame(Style style)
        {
            Style = style;
        }

        public override void Render(RenderContext context)
        {
            var ctx = context.Derive((1, 1, 0));
            foreach (var widget in widgets)
            {
                widget.Render(ctx);
            }

            var (width, height) = ctx.Size;

            var right = width + 1;
            var bottom = height + 1;
            
            context[0, 0] = new ColoredChar(Style, '+');
            context[right, 0] = new ColoredChar(Style, '+');
            context[0, bottom] = new ColoredChar(Style, '+');
            context[right, bottom] = new ColoredChar(Style, '+');
            for (var x = 1; x <= width; x++)
            {
                context[x, 0] = new ColoredChar(Style, '-');
                context[x, bottom] = new ColoredChar(Style, '-');
            }
            for (var y = 1; y <= height; y++)
            {
                context[0, y] = new ColoredChar(Style, '|');
                context[right, y] = new ColoredChar(Style, '|');
            }
        }
    }
}