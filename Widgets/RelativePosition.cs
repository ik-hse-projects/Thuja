namespace Thuja.Widgets
{
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