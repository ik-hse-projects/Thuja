using System;

namespace Thuja.Widgets
{
    internal class KeyPressed : IWidget
    {
        private ConsoleKeyInfo info;

        public (int x, int y, int layer) RelativePosition { get; set; }

        public void Render(RenderContext context)
        {
            var s = info.KeyChar.ToString();
            context.PlaceString(s, new Style(MyColor.White, MyColor.Red));
        }

        public int Fps => 0;

        public void Update(Tick tick)
        {
        }

        public bool BubbleDown(ConsoleKeyInfo key)
        {
            info = key;
            return true;
        }
    }
}