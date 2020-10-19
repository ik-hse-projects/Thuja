namespace Thuja.Widgets
{
    public class RelativePosition : BaseContainer
    {
        public RelativePosition(int x, int y, int layer = 0)
        {
            X = x;
            Y = y;
            Layer = layer;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Layer { get; set; }

        public override void Render(RenderContext context)
        {
            foreach (var widget in Widgets)
            {
                var newPosition = (X, Y, Layer);
                widget.Render(context.Derive(newPosition));
            }
        }
    }
}