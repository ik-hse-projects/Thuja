namespace Thuja.Widgets
{
    /// <summary>
    /// Контейнер, который сдвигает всё его содержимое на указанное расстояние.
    /// </summary>
    public class RelativePosition : BaseContainer
    {
        /// <param name="x">Отступ слева.</param>
        /// <param name="y">Отступ справа.</param>
        /// <param name="layer">Число слоёв, на которое нужно сдвинуть.</param>
        public RelativePosition(int x, int y, int layer = 0)
        {
            X = x;
            Y = y;
            Layer = layer;
        }

        /// <summary>
        /// Отступ слева.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Отступ сверха.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// На сколько слоёв сдвинуть.
        /// </summary>
        public int Layer { get; set; }

        /// <inheritdoc />
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